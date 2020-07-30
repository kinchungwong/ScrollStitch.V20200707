using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.HashCode;

    public struct ImageHashPoint
        : IEquatable<ImageHashPoint>
    {
        public int ImageIndex { get; }
        public int HashValue { get; }
        public int X { get; }
        public int Y { get; }

        public Point Point => new Point(X, Y);

        public HashPoint HashPoint => new HashPoint(HashValue, X, Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHashPoint(int imageIndex, int hashValue, int x, int y)
        {
            ImageIndex = imageIndex;
            HashValue = hashValue;
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHashPoint(int imageIndex, int hashValue, Point point)
            : this(imageIndex, hashValue, point.X, point.Y)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHashPoint(int imageIndex, HashPoint hp)
            : this(imageIndex, hp.HashValue, hp.X, hp.Y)
        {
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ImageHashPoint other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ImageHashPoint other)
        {
            return ImageIndex == other.ImageIndex &&
                HashValue == other.HashValue &&
                X == other.X &&
                Y == other.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ImageHashPoint p1, ImageHashPoint p2)
        {
            return p1.Equals(p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ImageHashPoint p1, ImageHashPoint p2)
        {
            return !p1.Equals(p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out int imageIndex, out int hashValue, out int x, out int y)
        {
            imageIndex = ImageIndex;
            hashValue = HashValue;
            x = X;
            y = Y;
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<ImageHashPoint>().Ingest(ImageIndex, HashValue, X, Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"(Image[{ImageIndex}] 0x{HashValue:x8}: {X}, {Y})";
        }
    }
}
