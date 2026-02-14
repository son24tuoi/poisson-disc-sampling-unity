using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace One.Utilities.PoissonDisc.Sample
{
    public class TestPoissonDisc : MonoBehaviour
    {
        public float radius = 1f;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1f;

        private PoissonSampler sampler;

        private Coroutine coroutine;

        private void Setup()
        {
            Stop();

            sampler = new PoissonSampler(regionSize, radius, rejectionSamples);

            coroutine = StartCoroutine(sampler.IEGeneratePoints());
        }

        private void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private void OnDrawGizmos()
        {
            if (sampler != null)
            {
                sampler.OnDrawGizmos(transform.position, displayRadius);
            }
        }




#if UNITY_EDITOR
        [CustomEditor(typeof(TestPoissonDisc))]
        public class TestPoissonDisc_Editor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                TestPoissonDisc target = (TestPoissonDisc)this.target;

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
#endif
    }
}