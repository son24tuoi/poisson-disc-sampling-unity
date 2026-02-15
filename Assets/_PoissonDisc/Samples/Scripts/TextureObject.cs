using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.PoissonDisc.Sample
{
    public class TextureObject : MonoBehaviour, IRoot
    {
        [SerializeField] private Circle circlePrefab;
        [SerializeField] private float moveSpeed = 1f;

        public float displayRadius = 1f;
        public bool showGizmos = true;

        [SerializeField] private VariableDensityPoissonSampler sampler = new VariableDensityPoissonSampler();
        [SerializeField] private TextureRadiusProvider textureRadiusProvider = new TextureRadiusProvider();

        private Coroutine generateRoutine;
        private Coroutine circlesRoutine;
        private int index = 0;
        private List<Circle> circles = new List<Circle>();
        private float deltaTime;
        [SerializeField] private int batchCount = 4;

        private int currentBatch;

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

        public float MoveSpeed => moveSpeed;

        public void Update()
        {
            int total = circles.Count;
            int batchSize = Mathf.CeilToInt((float)total / batchCount);

            int start = currentBatch * batchSize;
            int end = Mathf.Min(start + batchSize, total);

            deltaTime = Time.deltaTime;

            for (int i = start; i < end; i++)
            {
                circles[i].UpdateStep(deltaTime);
            }

            currentBatch = (currentBatch + 1) % batchCount;
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (generateRoutine != null)
                StopCoroutine(generateRoutine);

            index = 0;
            circles.Clear();

            if (circlesRoutine != null)
                StopCoroutine(circlesRoutine);

            circlesRoutine = StartCoroutine(IECreateCircles());
        }

        private IEnumerator IECreateCircles()
        {
            yield return sampler.IEGeneratePoints(textureRadiusProvider);

            while (index < sampler.Count)
            {
                CreateCircle(sampler[index++]);
                yield return null;
            }
        }

        private void CreateCircle(Vector2 pos)
        {
            Circle circle = Instantiate(circlePrefab);

            circle.Init(pos, this);
            circles.Add(circle);
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            if (sampler != null)
            {
                sampler.OnDrawGizmos(transform.position, displayRadius);
            }
        }
    }
}
