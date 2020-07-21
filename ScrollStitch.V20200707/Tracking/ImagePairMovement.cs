using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using Data;
    using Collections.Specialized;
    using Imaging.Hash2D;

    /// <summary>
    /// Tracks movement of hash points between two images.
    /// </summary>
    public class ImagePairMovement
    {
        #region private
        /// <summary>
        /// (Remark) this field is cleared when processing is complete.
        /// </summary>
        private Dictionary<int, PointIndexPair> _hashToPointIndex;
        #endregion

        public ImagePairMovementDetail Detail { get; private set; }
        public ImagePairMovementHeuristic Heuristic { get; private set; }
        public IReadOnlyList<HashPoint> FirstPoints => Detail.FirstPoints;
        public IReadOnlyList<HashPoint> SecondPoints => Detail.SecondPoints;
        public IReadOnlyList<ValueTuple<Point, Point>> CommonPoints => Detail.CommonPoints;
        public IReadOnlyList<Point> UnmatchedFirstPoints => Detail.UnmatchedFirstPoints;
        public IReadOnlyList<Point> UnmatchedSecondPoints => Detail.UnmatchedSecondPoints;
        public IHistogram<Movement, int> MovementHistogram => Detail.MovementHistogram;

        public ImagePairMovement(
            IEnumerable<HashPoint> firstPoints,
            IEnumerable<HashPoint> secondPoints)
        {
            Detail = new ImagePairMovementDetail();
            Detail.FirstPoints = _ToReadOnlyPoints(firstPoints);
            Detail.SecondPoints = _ToReadOnlyPoints(secondPoints);
        }

        private static IReadOnlyList<HashPoint> _ToReadOnlyPoints(IEnumerable<HashPoint> points)
        {
            switch (points)
            {
                case List<HashPoint> list:
                    return list.AsReadOnly();
                case IReadOnlyList<HashPoint> rol:
                    return rol;
                default:
                    return new List<HashPoint>(points).AsReadOnly();
            }
        }

        public void Process()
        {
            _PopulateHashToPointIndex(Which.First, FirstPoints);
            _PopulateHashToPointIndex(Which.Second, SecondPoints);
            _FindCommonPoints();
            _ComputeMovements();
            _ComputeHeuristic();
            _Cleanup();
        }

        private void _PopulateHashToPointIndex(Which which, IReadOnlyList<HashPoint> whichList)
        {
            if (_hashToPointIndex is null)
            {
                _hashToPointIndex = new Dictionary<int, PointIndexPair>();
            }
            int count = whichList.Count;
            for (int pointIndex = 0; pointIndex < count; ++pointIndex)
            {
                HashPoint hashPoint = whichList[pointIndex];
                int hashValue = hashPoint.HashValue;
                if (!_hashToPointIndex.TryGetValue(hashValue, out PointIndexPair pip))
                {
                    pip = new PointIndexPair();
                    _hashToPointIndex.Add(hashValue, pip);
                }
                pip[which] = pointIndex;
            }
        }

        private void _FindCommonPoints()
        {
            var commonPoints = new List<ValueTuple<Point, Point>>();
            var unmatchedFirst = new List<Point>();
            var unmatchedSecond = new List<Point>();
            foreach (var firstAndSecondIndex in _hashToPointIndex.Values)
            {
                int firstIndex = firstAndSecondIndex.FirstIndex;
                int secondIndex = firstAndSecondIndex.SecondIndex;
                if (firstIndex < 0 ||
                    secondIndex < 0)
                {
                    if (firstIndex >= 0)
                    {
                        unmatchedFirst.Add(FirstPoints[firstIndex].Point);
                    }
                    if (secondIndex >= 0)
                    {
                        unmatchedSecond.Add(SecondPoints[secondIndex].Point);
                    }
                    continue;
                }
                HashPoint point1 = FirstPoints[firstIndex];
                HashPoint point2 = SecondPoints[secondIndex];
                commonPoints.Add(new ValueTuple<Point, Point>(point1.Point, point2.Point));
            }
            Detail.CommonPoints = commonPoints.AsReadOnly();
            Detail.UnmatchedFirstPoints = unmatchedFirst.AsReadOnly();
            Detail.UnmatchedSecondPoints = unmatchedSecond.AsReadOnly();
        }

        private void _ComputeMovements()
        {
            var hist = HistogramUtility.CreateIntHistogram<Movement>();
            foreach ((Point point1, Point point2) in CommonPoints)
            {
                hist.Add(point2 - point1);
            }
            Detail.MovementHistogram = hist;
        }

        private void _ComputeHeuristic()
        {
            Heuristic = new ImagePairMovementHeuristic(Detail);
        }

        private void _Cleanup()
        {
            _hashToPointIndex = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This type does not provide a meaningful hash function or equality function.
        /// </remarks>
        public class PointIndexPair
        {
            // Implementation note.
            //
            // This is a mutable type because the algorithms that use this type must
            // populate the two fields at different times.
            //
            // This is a reference type because: it is a mutable type, and it has to
            // be the type of the value column in a dictionary.
            //
            // Moreover, its fields need to be initialized with a non-zero value
            // at constructor time.

            public int FirstIndex { get; set; } = -1;
            public int SecondIndex { get; set; } = -1;

            public int this[Which which]
            {
                get
                {
                    switch (which)
                    {
                        case Which.First:
                            return FirstIndex;
                        case Which.Second:
                            return SecondIndex;
                        default:
                            return -1;
                    }
                }
                set
                {
                    switch (which)
                    {
                        case Which.First:
                            FirstIndex = value;
                            return;
                        case Which.Second:
                            SecondIndex = value;
                            return;
                        default:
                            return;
                    }
                }
            }
        }

        public enum Which
        {
            First = 0,
            Second = 1
        }
    }
}
