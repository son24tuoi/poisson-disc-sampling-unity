using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc
{
    [Serializable]
    public class ConstantRadiusProvider : IRadiusProvider
    {
        [SerializeField] private float radius = 5f;

        public float MinRadius => radius;
        public float MaxRadius => radius;

        public ConstantRadiusProvider(float radius)
        {
            this.radius = radius;
        }

        public float GetRadius(Vector2 position) => radius;
    }
}
