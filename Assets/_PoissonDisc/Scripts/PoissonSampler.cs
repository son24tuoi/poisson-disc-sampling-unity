using System;
using System.Collections;
using System.Collections.Generic;
using One.Utilities.Array;
using UnityEngine;

using Random = UnityEngine.Random;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class PoissonSampler
    {
        [SerializeField] private Vector2 sampleRegionSize = Vector2.one;
        [SerializeField] private float radius = 1f;
        [SerializeField] private int numSamplesBeforeRejection = 30;
        [SerializeField] private int maxCount = 1000;
        [SerializeField] private List<Vector2> points;

        public int Count => (points == null) ? throw new NullReferenceException() : points.Count;

        public Vector2 this[int index] => (points == null) ? throw new NullReferenceException() : points[index];

        public PoissonSampler()
        {
            points = new List<Vector2>();
        }

        public PoissonSampler(Vector2 sampleRegionSize, float radius, int numSamplesBeforeRejection = 30, int maxCount = 1000)
        {
            this.sampleRegionSize = sampleRegionSize;
            this.radius = radius;
            this.numSamplesBeforeRejection = numSamplesBeforeRejection;
            this.maxCount = maxCount;
            points = new List<Vector2>();
        }

        public IEnumerator IEGeneratePoints()
        {
            float cellSize = radius / Mathf.Sqrt(2);

            Array2D<int> grid = new Array2D<int>(Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize));

            points.Clear();

            List<Vector2> spawnPoints = new List<Vector2>
            {
                sampleRegionSize / 2
            };

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;

                    Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector2 candidate = spawnCenter + dir * Random.Range(radius, radius * 2);

                    if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                        candidateAccepted = true;
                        break;
                    }
                }

                if (!candidateAccepted)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }

                yield return null;
            }
        }

        private bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, Array2D<int> grid)
        {
            if (candidate.x < 0 || candidate.y < 0 || candidate.x >= sampleRegionSize.x || candidate.y >= sampleRegionSize.y)
                return false;

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int startX = Mathf.Max(0, cellX - 2);
            int endX = Mathf.Min(cellX + 2, grid.Width - 1);
            int startY = Mathf.Max(0, cellY - 2);
            int endY = Mathf.Min(cellY + 2, grid.Height - 1);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
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
