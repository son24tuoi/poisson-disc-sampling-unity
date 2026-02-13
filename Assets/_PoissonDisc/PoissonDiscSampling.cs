using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace One.Utilities.PoissonDisc
{
    public static class PoissonDiscSampling
    {
        public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
        {
            float cellSize = radius / Mathf.Sqrt(2);

            int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];

            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();

            spawnPoints.Add(sampleRegionSize / 2);

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
            }

            return points;
        }

        private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            if (candidate.x < 0 || candidate.y < 0 || candidate.x >= sampleRegionSize.x || candidate.y >= sampleRegionSize.y)
                return false;

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int startX = Mathf.Max(0, cellX - 2);
            int endX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int startY = Mathf.Max(0, cellY - 2);
            int endY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

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

        public static List<Vector2> GeneratePoints(
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

            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();

            spawnPoints.Add(sampleRegionSize / 2);

            while (spawnPoints.Count > 0)
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
            }

            return points;
        }

        private static bool IsValid(
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
            float sqrRadius = candidateRadius * candidateRadius;

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int startX = Mathf.Max(0, cellX - 2);
            int endX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int startY = Mathf.Max(0, cellY - 2);
            int endY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

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
    }
}
