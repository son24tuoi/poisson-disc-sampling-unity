using System;
using System.Collections;
using System.Collections.Generic;
using One.Utilities.Easing;
using UnityEngine;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class RadialRadiusProvider : IRadiusProvider
    {
        [SerializeField] private Vector2 center;
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 10f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private EaseType easeType = EaseType.Linear;

        public float MinRadius => minRadius;
        public float MaxRadius => maxRadius;

        public RadialRadiusProvider(Vector2 center, float minRadius, float maxRadius, float maxDistance, EaseType easeType = EaseType.Linear)
        {
            this.center = center;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.maxDistance = maxDistance;
            this.easeType = easeType;
        }

        public float GetRadius(Vector2 position)
        {
            float dist = Vector2.Distance(position, center);
            // float t = Mathf.Clamp01(dist / maxDistance);
            float t = EaseManager.Evaluate(easeType, dist, maxDistance);
            return Mathf.Lerp(minRadius, maxRadius, t);
        }
    }
}
