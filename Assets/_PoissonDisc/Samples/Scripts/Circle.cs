using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc.Sample
{
    public class Circle : MonoBehaviour
    {
        private Vector3 offset;
        private IRoot root;

        private Transform tf;

        public Transform Transform
        {
            get
            {
                if (tf == null)
                    tf = transform;
                return tf;
            }
        }

        public void Init(Vector3 offset, IRoot root)
        {
            this.offset = offset;
            this.root = root;

            Transform.localPosition = offset;
        }

        public void UpdateStep(float deltaTime)
        {
            Vector3 targetPos = root.Transform.localPosition + offset;

            Transform.position = Vector3.MoveTowards(
                Transform.position,
                targetPos,
                root.MoveSpeed * deltaTime
            );
        }
    }
}
