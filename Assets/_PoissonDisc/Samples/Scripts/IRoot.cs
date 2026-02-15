using UnityEngine;

namespace One.Utilities.PoissonDisc.Sample
{
    public interface IRoot
    {
        public Transform Transform { get; }
        
        public float MoveSpeed { get; }
    }
}