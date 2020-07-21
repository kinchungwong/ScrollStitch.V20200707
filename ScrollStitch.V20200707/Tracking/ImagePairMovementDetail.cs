using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using Data;
    using Collections;
    using Collections.Specialized;
    using Imaging.Hash2D;

    public class ImagePairMovementDetail
    {
        public IReadOnlyList<HashPoint> FirstPoints { get; set; }

        public IReadOnlyList<HashPoint> SecondPoints { get; set; }

        public IReadOnlyList<ValueTuple<Point, Point>> CommonPoints { get; set; }

        public IReadOnlyList<Point> UnmatchedFirstPoints { get; set; }

        public IReadOnlyList<Point> UnmatchedSecondPoints { get; set; }

        public IHistogram<Movement, int> MovementHistogram { get; set; }

        public IHistogram<Movement, int> GetFilteredHistogram(double filterFrac)
        {
            int filterCount = _GetFilterCount(filterFrac);
            bool filterFunc(Movement movement, int vote) => (vote >= filterCount);
            return MovementHistogram.GetFiltered(filterFunc);
        }

        public List<KeyValuePair<Movement, int>> GetSortedHistogram(double? filterFracOrNull = null)
        {
            var list = MovementHistogram.ToList();
            int sortFunc(KeyValuePair<Movement, int> kvp1, KeyValuePair<Movement, int> kvp2)
            {
                int v1 = kvp1.Value;
                int v2 = kvp2.Value;
                // reverse order
                return (v1 > v2) ? (-1) : (v1 < v2) ? (1) : (0);
            }
            list.Sort(sortFunc);
            if (filterFracOrNull.HasValue)
            {
                int filterCount = _GetFilterCount(filterFracOrNull.Value);
                bool removePred(KeyValuePair<Movement, int> kvp)
                {
                    int votes = kvp.Value;
                    return votes < filterCount;
                }
                _RemoveFromEndWhile(list, removePred);
            }
            return list;
        }

        public void ToString(StringBuilder sb, string indent = null)
        {
            if (indent is null)
            {
                indent = string.Empty;
            }
            sb.AppendLine($"{indent}CommonPointCount: {CommonPoints.Count}");
            sb.AppendLine($"{indent}UnmatchedFirstPointCount: {UnmatchedFirstPoints.Count}");
            sb.AppendLine($"{indent}UnmatchedSecondPointCount: {UnmatchedSecondPoints.Count}");
            var list = GetSortedHistogram(0.05);
            void printFunc(KeyValuePair<Movement, int> kvp)
            {
                sb.AppendLine($"{indent}Movement={kvp.Key}, Votes={kvp.Value}");
            }
            foreach (var kvp in list)
            {
                printFunc(kvp);
            }
        }

        private int _GetFilterCount(double filterFrac)
        {
            int sharedPointCount = CommonPoints.Count;
            int filterCount = (int)Math.Floor(sharedPointCount * filterFrac);
            return filterCount;
        }

        private static void _RemoveFromEndWhile<T>(List<T> list, Func<T, bool> removePred)
        {
            while (list.Count > 0 &&
                removePred(list[list.Count - 1]))
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
