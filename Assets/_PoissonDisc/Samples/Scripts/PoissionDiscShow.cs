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

        private PoissonSampler sampler;
        private Coroutine coroutine;
        private WaitForSeconds wait;

        private IEnumerator Start()
        {
            Stop();

            wait = new WaitForSeconds(intervalTime);
            transform.localScale = new Vector3(regionSize.x, regionSize.y, regionSize.y);
            yield return null;
            sampler = new PoissonSampler(regionSize, radius, rejectionSamples);
            coroutine = StartCoroutine(sampler.IEGeneratePoints());

            for (int i = 0; i < sampler.Count; i++)
            {
                Spawn(sampler[i]);
                yield return wait;
            }
        }

        private void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
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
            if (sampler != null)
            {
                sampler.OnDrawGizmos(transform.position, displayRadius);
            }
        }
    }
}