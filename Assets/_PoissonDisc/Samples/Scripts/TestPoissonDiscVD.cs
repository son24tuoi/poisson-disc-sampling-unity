using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using One.Utilities.Easing;



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
        public EaseType easeType = EaseType.Linear;

        private VariableDensityPoissonSampler sampler;
        // private TextureRadiusProvider textureRadiusProvider;
        private RadialRadiusProvider radialRadiusProvider;

        private Coroutine coroutine;

        private void Setup()
        {
            Stop();

            sampler = new VariableDensityPoissonSampler(regionSize, radius, rejectionSamples, maxCount);
            // textureRadiusProvider = new TextureRadiusProvider(texture2D, regionSize, rangeRadius.x, rangeRadius.y, 0f);
            radialRadiusProvider = new RadialRadiusProvider(regionSize /2f, rangeRadius.x, rangeRadius.y, regionSize.magnitude / 2f, easeType);

            // coroutine = StartCoroutine(sampler.IEGeneratePoints(textureRadiusProvider));
            coroutine = StartCoroutine(sampler.IEGeneratePoints(radialRadiusProvider));
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

        private void OnDrawGizmos()
        {
            if (sampler != null)
            {
                sampler.OnDrawGizmos(transform.position, displayRadius);
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
