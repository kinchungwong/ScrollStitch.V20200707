using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Data
{
    using HashCode;

    /// <summary>
    /// 
    /// Refer to <code>Readme_Hash2D.Data_namespace.md</code>
    /// </summary>
    public struct Movement
        : IEquatable<Movement>
    {
        public static Movement Zero { get; } = new Movement(0, 0);

        public int DeltaX { get; }
        public int DeltaY { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Movement(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Movement other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Movement other)
        {
            return DeltaX == other.DeltaX &&
                DeltaY == other.DeltaY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Movement m1, Movement m2)
        {
            return m1.Equals(m2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Movement m1, Movement m2)
        {
            return !m1.Equals(m2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Movement operator +(Movement m1, Movement m2)
        {
            return new Movement(m1.DeltaX + m2.DeltaX, m1.DeltaY + m2.DeltaY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Movement operator -(Movement m1, Movement m2)
        {
            return new Movement(m1.DeltaX - m2.DeltaX, m1.DeltaY - m2.DeltaY);
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<Movement>().Ingest(DeltaX, DeltaY).GetHashCode();
        }

        public override string ToString()
        {
            return $"(dx={DeltaX}, dy={DeltaY})";
        }
    }
}
