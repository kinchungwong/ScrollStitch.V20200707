using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Memory;
    using Data;

    public class FloatBitmap
        : IArrayBitmap<float>
    {
        private static IArrayPoolClient<float> DefaultArrayPool { get; } = ExactLengthArrayPool<float>.DefaultInstance;

        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        public float[] Data { get; private set; }
        public Type DataType => typeof(float);
        public Type ArrayType => typeof(float[]);

        public float this[Point p]
        {
            get => Data[_PtoI(p)];
            set => Data[_PtoI(p)] = value;
        }

        public FloatBitmap(Size size)
            : this(size.Width, size.Height)
        {
        }

        public FloatBitmap(Size size, float[] dataArray)
            : this(size.Width, size.Height, dataArray)
        {
        }

        public FloatBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Data = DefaultArrayPool.Rent(width * height);
        }

        public FloatBitmap(int width, int height, float[] dataArray)
        {
            Width = width;
            Height = height;
            if ((dataArray?.Length ?? 0) != width * height)
            {
                throw new ArgumentException(nameof(dataArray));
            }
            Data = dataArray;
        }

        public void Dispose()
        {
            if (!(Data is null))
            {
                DefaultArrayPool.Return(Data);
                Data = null;
            }
        }

        private int _PtoI(Point p)
        {
            return p.Y * Width + p.X;
        }
    }
}
