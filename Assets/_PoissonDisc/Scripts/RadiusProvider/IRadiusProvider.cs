using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc
{
    public interface IRadiusProvider
    {
        public float MinRadius { get; }

        public float MaxRadius { get; }

        public float GetRadius(Vector2 position);
    }
}
