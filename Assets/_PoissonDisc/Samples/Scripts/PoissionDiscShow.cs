using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc.Sample
{
    public class PoissionDiscShow : MonoBehaviour
    {
        public GameObject spherePrefab;
        public GameObject discPrefab;
        public float radius = 1f;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1f;
        public float intervalTime = 0.5f;

        private List<Vector2> points;
        private WaitForSeconds wait;

        private IEnumerator Start()
        {
            wait = new WaitForSeconds(intervalTime);
            transform.localScale = new Vector3(regionSize.x, regionSize.y, regionSize.y);
            yield return null;
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);

            for (int i = 0; i < points.Count; i++)
            {
                Spawn(points[i]);
                yield return wait;
            }
        }

        private void Spawn(Vector2 pos)
        {
            GameObject sphere = Instantiate(spherePrefab, transform);
            sphere.transform.localScale = new Vector3(displayRadius / regionSize.x, displayRadius / regionSize.y, displayRadius / regionSize.y);
            sphere.transform.SetLocalPositionAndRotation((Vector3)(pos - regionSize / 2) / regionSize, Quaternion.identity);

            GameObject disc = Instantiate(discPrefab, sphere.transform);
            disc.transform.localScale = new Vector3(radius / regionSize.x / sphere.transform.localScale.x, radius / regionSize.y / sphere.transform.localScale.z, disc.transform.localScale.z);
        }

        private void OnDrawGizmosSelected()
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