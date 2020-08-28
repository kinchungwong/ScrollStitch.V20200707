using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Memory;
    using Data;

    public class IntBitmap
        : IArrayBitmap<int>
    {
        private static IArrayPoolClient<int> DefaultArrayPool { get; } = ExactLengthArrayPool<int>.DefaultInstance;

        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        public int[] Data { get; private set; }
        public Type DataType => typeof(int);
        public Type ArrayType => typeof(int[]);
        
        public int this[Point p]
        {
            get => Data[_PtoI(p)];
            set => Data[_PtoI(p)] = value;
        }

        public IntBitmap(Size size)
            : this(size.Width, size.Height)
        {
        }

        public IntBitmap(Size size, int[] dataArray)
            : this(size.Width, size.Height, dataArray)
        {
        }

        public IntBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Data = DefaultArrayPool.Rent(width * height);
        }

        public IntBitmap(int width, int height, int[] dataArray)
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
