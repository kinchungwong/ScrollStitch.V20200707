using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;
    using System.Collections;

    public class AxisSubdiv
        : IReadOnlyList<Range>
    {
        /// <summary>
        /// The input length.
        /// </summary>
        public int InputLength { get; }

        /// <summary>
        /// The input range, which is equivalent to <c>Range(0, InputLength)</c>.
        /// </summary>
        public Range InputRange => new Range(0, InputLength);

        /// <summary>
        /// The collection of start and end positions of each subdivision.
        /// </summary>
        public IReadOnlyList<Range> Ranges { get; private set; }

        /// <summary>
        /// Indexer (getter) for the start and end positions of each subdivision.
        /// </summary>
        /// <param name="rangeIndex">The subdivision index.</param>
        /// <returns></returns>
        public Range this[int rangeIndex] => Ranges[rangeIndex];

        /// <summary>
        /// The number of subdivisions.
        /// </summary>
        public int Count => Ranges.Count;

        /// <summary>
        /// Indicates whether this list of ranges contain any overlapping.
        /// </summary>
        public bool HasOverlap { get; private set; }

        /// <summary>
        /// Indicates whether there are gaps between ranges.
        /// </summary>
        public bool HasGap { get; private set; }

        /// <summary>
        /// Indicates whether the list of ranges fully cover the input range.
        /// </summary>
        public bool IsFullyCovered { get; private set; }

        /// <summary>
        /// Indicates whether all subdivisions have the same size.
        /// </summary>
        public bool IsUniform { get; private set; }

        /// <summary>
        /// Indicates whether the minimum and maximum size of subdivisions 
        /// has a spread (difference) of at most one. 
        /// </summary>
        public bool IsNearlyUniform { get; private set; }

        public int MinLength { get; private set; }

        public int MaxLength { get; private set; }

        public AxisSubdivAlignment Alignment { get; private set; }

        public AxisSubdiv(int inputLength, IEnumerable<Range> rangeList)
        {
            InputLength = inputLength;
            Ranges = new List<Range>(rangeList).AsReadOnly();
            _CtorAnalyzeRanges();
            _CtorAnalyzeAlignment();
        }

        /// <summary>
        /// Finds the subdivision (range) that contains the input value.
        /// 
        /// <para>
        /// There are multiple possible implementations, with significant differences in behavior.
        /// Currently, this function is a placeholder that delegates to one of the already-implemented
        /// versions.
        /// </para>
        /// 
        /// </summary>
        public bool TryFind(int input, out Range range, out int rangeIndex)
        {
            return TryFindAnyContainingRange(input, out range, out rangeIndex);
        }

        /// <summary>
        /// Finds the subdivision (range) that contains the input value.
        /// </summary>
        /// <param name="input">An input value within the input range.</param>
        /// <param name="range"></param>
        /// <param name="rangeIndex"></param>
        /// <returns>True if successful.</returns>
        /// <remarks>
        /// <para>
        /// The function returns false if the input value falls outside the input range.
        /// </para>
        /// <para>
        /// The function returns false if the input value falls into a gap between the nearest ranges.
        /// </para>
        /// <para>
        /// The function returns true if two or more ranges contain the input value due to overlap.
        /// In this case, the range that is returned may be any one of those that contain it.
        /// </para>
        /// </remarks>
        public bool TryFindAnyContainingRange(int input, out Range range, out int rangeIndex)
        {
            int count = Count;
            if (count < 1)
            {
                throw new InvalidOperationException();
            }
            int searchMin = 0;
            int searchMax = count - 1;
            if (input < Ranges[searchMin].Start ||
                input >= Ranges[searchMax].Stop)
            {
                range = default;
                rangeIndex = -1;
                return false;
            }
            while (searchMin >= 0 &&
                searchMax >= searchMin &&
                searchMax < count)
            {
                int searchMid = searchMin + (searchMax - searchMin) / 2;
                Range curr = Ranges[searchMid];
                if (input >= curr.Stop)
                {
                    searchMin = searchMid + 1;
                }
                else if (input < curr.Start)
                {
                    searchMax = searchMid - 1;
                }
                else
                {
                    range = curr;
                    rangeIndex = searchMid;
                    return true;
                }
            }
            range = default;
            rangeIndex = -1;
            return false;
        }

        public IEnumerator<Range> GetEnumerator()
        {
            return Ranges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Ranges).GetEnumerator();
        }

        private void _CtorAnalyzeRanges()
        {
            int count = Count;
            bool hasOverlap = false;
            bool hasGap = false;
            int minStart = int.MaxValue;
            int maxStop = int.MinValue;
            int minLength = int.MaxValue;
            int maxLength = int.MinValue;
            for (int k = 0; k < count; ++k)
            {
                Range curr = Ranges[k];
                if (curr.IsEmpty)
                {
                    throw new ArgumentException(
                        paramName: "rangeList",
                        message: "Range cannot be empty. " +
                        $"Range[{k}]={curr}");
                }
                if (curr.Start < 0 ||
                    curr.Stop > InputLength)
                {
                    throw new ArgumentException(
                        paramName: "rangeList",
                        message: "Range cannot be outside input range. " +
                        $"Input Length={InputLength}. " +
                        $"Range[{k}]={curr}");
                }
                minStart = Math.Min(minStart, curr.Start);
                maxStop = Math.Max(maxStop, curr.Stop);
                minLength = Math.Min(minLength, curr.Count);
                maxLength = Math.Max(maxLength, curr.Count);
                if (k >= 1)
                {
                    Range prev = Ranges[k - 1];
                    if (curr.Start <= prev.Start ||
                        curr.Stop <= prev.Stop)
                    {
                        throw new ArgumentException(
                            paramName: "rangeList",
                            message: "Start and stop must be strictly increasing on the range list. " +
                            $"Range[{k - 1}]={prev}, " +
                            $"Range[{k}]={curr}");
                    }
                    if (curr.Start > prev.Stop)
                    {
                        hasGap = true;
                    }
                    else if (curr.Start < prev.Stop)
                    {
                        hasOverlap = true;
                    }
                }
            }
            HasOverlap = hasOverlap;
            HasGap = hasGap;
            MinLength = minLength;
            MaxLength = maxLength;
            IsFullyCovered = minStart == 0 && maxStop == InputLength && !hasGap;
            IsUniform = minLength == maxLength;
            IsNearlyUniform = (maxLength - minLength) <= 1;
        }

        private void _CtorAnalyzeAlignment()
        {
            // TODO WARNING
            // The following code has not yet been sufficiently analyzed as to correctness or 
            // usefulness.
            //
            int count = Count;
            int minLengthInterior = int.MaxValue;
            int maxLengthInterior = int.MinValue;
            for (int k = 1; k + 1 < count; ++k)
            {
                Range interior = Ranges[k];
                minLengthInterior = Math.Min(minLengthInterior, interior.Count);
                maxLengthInterior = Math.Max(maxLengthInterior, interior.Count);
            }
            if (!IsFullyCovered || HasOverlap)
            {
                Alignment = AxisSubdivAlignment.Unspecified;
            }
            else if (count <= 2)
            {
                Alignment = AxisSubdivAlignment.All;
            }
            else if (minLengthInterior != maxLengthInterior)
            {
                Alignment = AxisSubdivAlignment.Unspecified;
            }
            else
            {
                // claim: (minLengthInterior == maxLengthInterior)
                int interiorLength = minLengthInterior;
                Alignment |= AxisSubdivAlignment.Interior;
                if (count <= 3)
                {
                    Alignment |= AxisSubdivAlignment.Trivial;
                }
                int firstLength = Ranges[0].Count;
                int lastLength = Ranges[count - 1].Count;
                if (firstLength == interiorLength)
                {
                    Alignment |= AxisSubdivAlignment.Left;
                }
                if (lastLength == interiorLength)
                {
                    Alignment |= AxisSubdivAlignment.Right;
                }
            }
        }
    }
}
