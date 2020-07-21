using System;
using System.Collections.Generic;
using System.Linq;
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
    {
        public int CellX { get; }
        public int CellY { get; }

        public CellIndex(int cellX, int cellY)
        {
            CellX = cellX;
            CellY = cellY;
        }

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

        public bool Equals(CellIndex other)
        {
            return CellX == other.CellX &&
                CellY == other.CellY;
        }

        public static bool operator ==(CellIndex p1, CellIndex p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(CellIndex p1, CellIndex p2)
        {
            return !p1.Equals(p2);
        }

        public Point AsPoint()
        {
            return new Point(CellX, CellY);
        }

        public static explicit operator CellIndex(Point p)
        {
            return new CellIndex(p.X, p.Y);
        }

        public static explicit operator Point(CellIndex ci)
        {
            return new Point(ci.CellX, ci.CellY);
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
