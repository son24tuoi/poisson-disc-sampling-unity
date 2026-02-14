using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class NoiseRadiusProvider : IRadiusProvider
    {
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 8f;
        [SerializeField] private float scale = 0.05f;

        public float MinRadius => minRadius;
        public float MaxRadius => maxRadius;

        public NoiseRadiusProvider(float minRadius, float maxRadius, float scale = 0.05f)
        {
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.scale = scale;
        }

        public float GetRadius(Vector2 position)
        {
            float noise = Mathf.PerlinNoise(position.x * scale, position.y * scale);
            return Mathf.Lerp(maxRadius, minRadius, noise);
        }
    }
}
