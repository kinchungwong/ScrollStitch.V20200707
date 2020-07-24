using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;
    using HashCode;

    /// <summary>
    /// Represents the X and Y index of a cell in a grid.
    /// 
    /// <para>
    /// <c>CellIndex</c> and <c>Point</c> are designed to be distinct, in order to reduce programming 
    /// mistakes in algorithms that operate on both grid cell coordinates and image coordinates.
    /// </para>
    /// 
    /// <para>
    /// See also:
    /// <seealso cref="Grid"/>,
    /// <seealso cref="Point"/>
    /// </para>
    /// </summary>
    public struct CellIndex
        : IEquatable<CellIndex>
        , IComparable<CellIndex>
    {
        public int CellX { get; }
        public int CellY { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellIndex(int cellX, int cellY)
        {
            CellX = cellX;
            CellY = cellY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellIndex((int cellX, int cellY) cellXY)
        {
            CellX = cellXY.cellX;
            CellY = cellXY.cellY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellIndex(Point point)
        {
            CellX = point.X;
            CellY = point.Y;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case CellIndex other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CellIndex other)
        {
            return CellX == other.CellX &&
                CellY == other.CellY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(CellIndex other)
        {
            if (CellY > other.CellY) return 1;
            if (CellY < other.CellY) return -1;
            if (CellX > other.CellX) return 1;
            if (CellX < other.CellX) return -1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CellIndex p1, CellIndex p2)
        {
            return p1.Equals(p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CellIndex p1, CellIndex p2)
        {
            return !p1.Equals(p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point AsPoint()
        {
            return new Point(CellX, CellY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator CellIndex(Point p)
        {
            return new CellIndex(p.X, p.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Point(CellIndex ci)
        {
            return new Point(ci.CellX, ci.CellY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator CellIndex((int cellX, int cellY) cellXY)
        {
            return new CellIndex(cellXY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator (int, int)(CellIndex ci)
        {
            return (ci.CellX, ci.CellY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out int cellX, out int cellY)
        {
            cellX = CellX;
            cellY = CellY;
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<CellIndex>().Ingest(CellX, CellY).GetHashCode();
        }

        public override string ToString()
        {
            return $"(cell: {CellX}, {CellY})";
        }
    }
}
