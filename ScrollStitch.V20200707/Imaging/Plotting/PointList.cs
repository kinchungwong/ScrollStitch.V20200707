using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using HashCode;

    /// <summary>
    /// Represents an immutable list of points and a precomputed hash code.
    /// </summary>
    public class PointList 
        : IEquatable<PointList>
        , IReadOnlyList<Point>
    {
        public IReadOnlyList<Point> Points { get; }
        public int Count => Points.Count;
        public bool IsReadOnly => true;
        public Point this[int index] => Points[index];
        private readonly int _hashCode;

        public PointList(ICollection<Point> points)
        {
            Points = new List<Point>(points).AsReadOnly();
            _hashCode = _CtorComputeHash();
        }

        public PointList(IReadOnlyList<Point> points)
        {
            Points = points;
            _hashCode = _CtorComputeHash();
        }

        public PointList(IList<int> coordinates)
        {
            int intCount = coordinates.Count;
            if ((intCount % 2) != 0)
            {
                throw new ArgumentException(nameof(coordinates));
            }
            int pointCount = intCount / 2;
            var points = new List<Point>();
            for (int k = 0; k < pointCount; ++k)
            {
                points.Add(new Point(coordinates[k * 2], coordinates[k * 2 + 1]));
            }
            Points = points.AsReadOnly();
            _hashCode = _CtorComputeHash();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private int _CtorComputeHash()
        {
            var helper = new HashCodeBuilder(GetType());
            foreach (var p in Points)
            {
                helper.Ingest(p.X);
                helper.Ingest(p.Y);
            }
            return helper.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PointList other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(PointList other)
        {
            if (ReferenceEquals(this, other) ||
                ReferenceEquals(Points, other.Points))
            {
                return true;
            }
            if (_hashCode != other._hashCode)
            {
                return false;
            }
            int count = Points.Count;
            if (other.Points.Count != count)
            {
                return false;
            }
            for (int k = 0; k < count; ++k)
            {
                if (Points[k] != other.Points[k])
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return Points.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)Points).GetEnumerator();
        }
    }
}
