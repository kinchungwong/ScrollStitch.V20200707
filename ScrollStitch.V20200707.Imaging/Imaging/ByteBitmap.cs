using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Memory;
    using Data;

    public class ByteBitmap
        : IArrayBitmap<byte>
    {
        private static IArrayPoolClient<byte> DefaultArrayPool { get; } = ExactLengthArrayPool<byte>.DefaultInstance;

        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        public byte[] Data { get; private set; }
        public Type DataType => typeof(byte);
        public Type ArrayType => typeof(byte[]);

        public byte this[Point p]
        {
            get => Data[_PtoI(p)];
            set => Data[_PtoI(p)] = value;
        }

        public ByteBitmap(Size size)
            : this(size.Width, size.Height)
        { 
        }

        public ByteBitmap(Size size, byte[] dataArray)
            : this(size.Width, size.Height, dataArray)
        {
        }

        public ByteBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Data = DefaultArrayPool.Rent(width * height);
        }

        public ByteBitmap(int width, int height, byte[] dataArray)
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
