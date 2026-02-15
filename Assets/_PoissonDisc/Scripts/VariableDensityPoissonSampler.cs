using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class VariableDensityPoissonSampler
    {
        [SerializeField] private Vector2 sampleRegionSize = Vector2.one;
        [SerializeField] private float minRadius = 1f;
        [SerializeField] private int numSamplesBeforeRejection = 30;
        [SerializeField] private int maxCount = 1000;
        [SerializeField] private List<Vector2> points;

        public int Count => (points == null) ? throw new NullReferenceException() : points.Count;

        public Vector2 this[int index] => (points == null) ? throw new NullReferenceException() : points[index];

        public VariableDensityPoissonSampler()
        {
            points = new List<Vector2>();
        }

        public VariableDensityPoissonSampler(Vector2 sampleRegionSize, float minRadius, int numSamplesBeforeRejection = 30, int maxCount = 1000)
        {
            this.sampleRegionSize = sampleRegionSize;
            this.minRadius = minRadius;
            this.numSamplesBeforeRejection = numSamplesBeforeRejection;
            this.maxCount = maxCount;
            points = new List<Vector2>();
        }

        public IEnumerator IEGeneratePoints(IRadiusProvider radiusProvider)
        {
            float cellSize = minRadius / Mathf.Sqrt(2);

            int[,] grid = new int[
                Mathf.CeilToInt(sampleRegionSize.x / cellSize),
                Mathf.CeilToInt(sampleRegionSize.y / cellSize)
            ];

            points.Clear();

            List<Vector2> spawnPoints = new List<Vector2>
            {
                sampleRegionSize / 2
            };

            while (spawnPoints.Count > 0 && points.Count <= maxCount)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                float spawnRadius = radiusProvider.GetRadius(spawnCenter);

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    float candidateRadius = radiusProvider.GetRadius(spawnCenter);
                    Vector2 candidate = spawnCenter + dir * Random.Range(candidateRadius, candidateRadius * 2);

                    if (IsValid(candidate, sampleRegionSize, cellSize, radiusProvider, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);

                        int cellX = (int)(candidate.x / cellSize);
                        int cellY = (int)(candidate.y / cellSize);
                        grid[cellX, cellY] = points.Count;

                        candidateAccepted = true;
                        break;
                    }
                }

                if (!candidateAccepted)
                    spawnPoints.RemoveAt(spawnIndex);

                yield return null;
            }
        }

        private bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, IRadiusProvider radiusProvider, List<Vector2> points, int[,] grid)
        {
            if (candidate.x < 0 || candidate.y < 0 ||
                candidate.x >= sampleRegionSize.x ||
                candidate.y >= sampleRegionSize.y)
                return false;

            float candidateRadius = radiusProvider.GetRadius(candidate);

            if (candidateRadius <= 0f)
                return false;

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int cellsToCheck = Mathf.CeilToInt(candidateRadius / cellSize);

            int startX = Mathf.Max(0, cellX - cellsToCheck);
            int endX = Mathf.Min(cellX + cellsToCheck, grid.GetLength(0) - 1);
            int startY = Mathf.Max(0, cellY - cellsToCheck);
            int endY = Mathf.Min(cellY + cellsToCheck, grid.GetLength(1) - 1);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float otherRadius = radiusProvider.GetRadius(points[pointIndex]);
                        float minDist = Mathf.Max(candidateRadius, otherRadius);
                        float sqrDist = (candidate - points[pointIndex]).sqrMagnitude;

                        if (sqrDist < minDist * minDist)
                            return false;
                    }
                }
            }

            return true;
        }

        public void OnDrawGizmos(Vector3 position, float displayRadius)
        {
            Gizmos.DrawWireCube(position + (Vector3)sampleRegionSize / 2, sampleRegionSize);

            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.DrawSphere(points[i], displayRadius);
                }
            }
        }
    }
}