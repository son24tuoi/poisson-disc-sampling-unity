using System;
using UnityEngine;

namespace One.Utilities.Array
{
    [Serializable]
    public class Array2D<T>
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private T[] data;

        public int Width => width;
        public int Height => height;
        public int Length => data.Length;

        public T this[int x, int y]
        {
            get => data[y * width + x];
            set => data[y * width + x] = value;
        }

        public T this[Vector2Int pos]
        {
            get => this[pos.x, pos.y];
            set => this[pos.x, pos.y] = value;
        }

        public Array2D(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Array2D size must be greater than 0");

            this.width = width;
            this.height = height;
            data = new T[width * height];
        }

        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public bool Contains(int x, int y) => IsValid(x, y);

        public bool Contains(Vector2Int pos) => IsValid(pos.x, pos.y);

        public void Fill(T value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = value;
            }
        }

        public void Clear() => Fill(default);

        public T[] GetRawData() => data;
    }
}