using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class TextureRadiusProvider : IRadiusProvider
    {
        [SerializeField] private Texture2D densityTexture;
        [SerializeField] private Vector2 regionSize;
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 10f;
        [SerializeField] private float alphaThreshold = 0.1f;

        public float MinRadius => minRadius;
        public float MaxRadius => maxRadius;

        public TextureRadiusProvider() { }

        public TextureRadiusProvider(Texture2D densityTexture, Vector2 regionSize, float minRadius, float maxRadius, float alphaThreshold = 0.1f)
        {
            this.densityTexture = densityTexture;
            this.regionSize = regionSize;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.alphaThreshold = alphaThreshold;
        }

        public float GetRadius(Vector2 position)
        {
            float u = position.x / regionSize.x;
            float v = position.y / regionSize.y;

            Color pixel = densityTexture.GetPixelBilinear(u, v);

            if (pixel.a <= alphaThreshold)
                return -1f;

            float density = pixel.grayscale;

            return Mathf.Lerp(maxRadius, minRadius, density);
        }
    }
}
