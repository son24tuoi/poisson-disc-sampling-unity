using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc.Sample
{
    public class TestPoissonDisc : MonoBehaviour
    {
        public float radius = 1f;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1f;

        private List<Vector2> points;

        private void OnValidate()
        {
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
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
    }
}