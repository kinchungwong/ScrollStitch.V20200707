using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.HashCode;

    /// <summary>
    /// It is a combination of `Point` and an integer hash value. 
    /// 
    /// Refer to <code>Readme_Hash2D.Data_namespace.md</code>
    /// </summary>
    public struct HashPoint 
        : IEquatable<HashPoint>
    {
        public int HashValue { get; }
        public int X { get; }
        public int Y { get; }

        public Point Point => new Point(X, Y);

        public HashPoint(int hashValue, int x, int y)
        {
            HashValue = hashValue;
            X = x;
            Y = y;
        }

        public HashPoint(int hashValue, Point point)
        {
            HashValue = hashValue;
            X = point.X;
            Y = point.Y;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case HashPoint other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(HashPoint other)
        {
            return HashValue == other.HashValue &&
                X == other.X &&
                Y == other.Y;
        }

        public static bool operator ==(HashPoint p1, HashPoint p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(HashPoint p1, HashPoint p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder().Ingest(HashValue, X, Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"(0x{HashValue:x8}: {X}, {Y})";
        }
    }
}
