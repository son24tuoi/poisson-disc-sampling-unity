using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace One.Utilities.PoissonDisc.Sample
{
    public class TestPoissonDiscVD : MonoBehaviour
    {
        public Texture2D texture2D;
        public float radius = 1f;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1f;
        public Vector2 rangeRadius = Vector2.one;
        public int maxCount = 1000;

        private Coroutine coroutine;

        public List<Vector2> points = new List<Vector2>();

        private void Setup()
        {
            Stop();

            Vector2 center = regionSize / 2f;

            coroutine = StartCoroutine(IEGeneratePoints(
                points,
                // RadiusFunc,
                CreateRadiusFromTexture,
                regionSize,
                minRadius: radius,
                numSamplesBeforeRejection: rejectionSamples
            ));
        }

        private void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private float RadiusFunc(Vector2 pos)
        {
            //     float dist = Vector2.Distance(pos, center);
            //     float maxDist = regionSize.magnitude / 2f;

            //     float t = dist / maxDist;
            //     return Mathf.Lerp(rangeRadius.x, rangeRadius.y, t); // gần tâm dày, xa tâm thưa

            float noise = Mathf.PerlinNoise(pos.x * 0.05f, pos.y * 0.05f);
            return Mathf.Lerp(rangeRadius.x, rangeRadius.y, noise);
        }

        public float CreateRadiusFromTexture(Vector2 pos)
        {
            float u = pos.x / regionSize.x;
            float v = pos.y / regionSize.y;

            Color pixel = texture2D.GetPixelBilinear(u, v);

            if (pixel.a <= 0f)
                return -1f;

            float density = pixel.grayscale; // 0 → 1

            // density cao -> radius nhỏ
            return Mathf.Lerp(rangeRadius.y, rangeRadius.x, density);
        }

        public IEnumerator IEGeneratePoints(
            List<Vector2> points,
            Func<Vector2, float> radiusFunc,
            Vector2 sampleRegionSize,
            float minRadius,
            int numSamplesBeforeRejection = 30)
        {
            float cellSize = minRadius / Mathf.Sqrt(2);

            int[,] grid = new int[
                Mathf.CeilToInt(sampleRegionSize.x / cellSize),
                Mathf.CeilToInt(sampleRegionSize.y / cellSize)
            ];

            points.Clear();

            List<Vector2> spawnPoints = new List<Vector2>();

            spawnPoints.Add(sampleRegionSize / 2);

            while (spawnPoints.Count > 0 && points.Count <= maxCount)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                float spawnRadius = radiusFunc(spawnCenter);

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    float candidateRadius = radiusFunc(spawnCenter);
                    Vector2 candidate = spawnCenter + dir * Random.Range(candidateRadius, candidateRadius * 2);

                    if (IsValid(candidate, sampleRegionSize, cellSize, radiusFunc, points, grid))
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

        private bool IsValid(
            Vector2 candidate,
            Vector2 sampleRegionSize,
            float cellSize,
            Func<Vector2, float> radiusFunc,
            List<Vector2> points,
            int[,] grid)
        {
            if (candidate.x < 0 || candidate.y < 0 ||
                candidate.x >= sampleRegionSize.x ||
                candidate.y >= sampleRegionSize.y)
                return false;

            float candidateRadius = radiusFunc(candidate);

            if (candidateRadius <= 0f)
                return false;

            float sqrRadius = candidateRadius * candidateRadius;

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
                        float otherRadius = radiusFunc(points[pointIndex]);
                        float minDist = Mathf.Max(candidateRadius, otherRadius);
                        float sqrDist = (candidate - points[pointIndex]).sqrMagnitude;

                        if (sqrDist < minDist * minDist)
                            return false;
                    }
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(regionSize / 2, regionSize);

            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.DrawSphere(points[i], displayRadius);
                }
            }
        }







        [CustomEditor(typeof(TestPoissonDiscVD))]
        public class TestPoissonDiscVD_Editor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                TestPoissonDiscVD target = (TestPoissonDiscVD)this.target;

                if (GUILayout.Button("Generate Points"))
                {
                    target.Setup();
                }

                if (GUILayout.Button("Stop"))
                {
                    target.Stop();
                }
            }
        }
    }
}
