using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.HashCode;
    using ScrollStitch.V20200707.Data;
    using PercentFromRatio = ScrollStitch.V20200707.Utility.PercentFromRatio;

    public sealed class FastRectNodeChildSpec 
        : IEquatable<FastRectNodeChildSpec>
        , IComparable<FastRectNodeChildSpec>
    {
        /// <summary>
        /// From the parent node's bounding rectangle, the whole horizontal range (width) is subdivided 
        /// into this number of non-overlapping bands.
        /// </summary>
        public int HorzParentCellCount { get; }

        /// <summary>
        /// Each child node shall have this number of horizontal bands as its width.
        /// </summary>
        public int HorzChildCellCount { get; }

        /// <summary>
        /// For each group of child nodes covering different horizontal bands, this is the
        /// amount of shift between their horizontal coverage, measured in number of bands.
        /// </summary>
        public int HorzCellShift { get; }

        /// <summary>
        /// From the parent node's bounding rectangle, the whole vertical range (height) is subdivided 
        /// into this number of non-overlapping bands.
        /// </summary>
        public int VertParentCellCount { get; }

        /// <summary>
        /// Each child node shall have this number of vertical bands as its width.
        /// </summary>
        public int VertChildCellCount { get; }

        /// <summary>
        /// For each group of child nodes covering different vertical bands, this is the
        /// amount of shift between their vertical coverage, measured in number of bands.
        /// </summary>
        public int VertCellShift { get; }

        /// <summary>
        /// This is the ratio of child width to parent width.
        /// 
        /// <para>
        /// This ratio does not take into account the nearest integer rounding that occurs 
        /// in the actual computation of parent and child rectangles.
        /// </para>
        /// </summary>
        public PercentFromRatio HorzRatio => new PercentFromRatio(HorzChildCellCount, HorzParentCellCount);

        /// <summary>
        /// This is the ratio of child height to parent height.
        /// 
        /// <para>
        /// This ratio does not take into account the nearest integer rounding that occurs 
        /// in the actual computation of parent and child rectangles.
        /// </para>
        /// </summary>
        public PercentFromRatio VertRatio => new PercentFromRatio(VertChildCellCount, VertParentCellCount);

        /// <summary>
        /// This is the ratio of child area to parent area.
        /// 
        /// <para>
        /// This ratio does not take into account the nearest integer rounding that occurs 
        /// in the actual computation of parent and child rectangles.
        /// </para>
        /// </summary>
        public PercentFromRatio AreaRatio => new PercentFromRatio(
            HorzChildCellCount * VertChildCellCount, HorzParentCellCount * VertParentCellCount);

        public FastRectNodeChildSpec(
            int horzParentCellCount, int vertParentCellCount, 
            int horzChildCellCount, int vertChildCellCount, 
            int horzCellShift, int vertCellShift)
        {
            if (horzParentCellCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(horzParentCellCount));
            }
            if (vertParentCellCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertParentCellCount));
            }
            if (horzChildCellCount <= 0 || horzChildCellCount > horzParentCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(horzChildCellCount));
            }
            if (vertChildCellCount <= 0 || vertChildCellCount > vertParentCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertChildCellCount));
            }
            if (horzCellShift <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(horzCellShift));
            }
            if (vertCellShift <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertCellShift));
            }
            HorzParentCellCount = horzParentCellCount;
            VertParentCellCount = vertParentCellCount;
            HorzChildCellCount = horzChildCellCount;
            VertChildCellCount = vertChildCellCount;
            HorzCellShift = horzCellShift;
            VertCellShift = vertCellShift;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case FastRectNodeChildSpec other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Tests if this instance is identical to another instance of this class.
        /// </summary>
        /// <param name="other">
        /// Another instance to be tested for equality.
        /// </param>
        /// <returns>
        /// True if this instance is equal to the other instance.
        /// </returns>
        /// 
        public bool Equals(FastRectNodeChildSpec other)
        {
            return HorzParentCellCount == other.HorzParentCellCount &&
                HorzChildCellCount == other.HorzChildCellCount &&
                HorzCellShift == other.HorzCellShift &&
                VertParentCellCount == other.VertParentCellCount &&
                VertChildCellCount == other.VertChildCellCount &&
                VertCellShift == other.VertCellShift;
        }

        /// <summary>
        /// Compares the nominal area ratio to another instance of this class.
        /// 
        /// <para>
        /// Performance remark. <br/>
        /// This comparison function is somewhat slow. It is recommended for sorting to be performed outside 
        /// performance-critical parts of code. For example, it can be performed during configuration 
        /// initialization upon the first use.
        /// </para>
        /// </summary>
        /// 
        /// <param name="other">
        /// Another instance of this class to compare to.
        /// </param>
        /// 
        /// <returns>
        /// A positive integer if this instance has a higher nominal area ratio than the other instance. <br/>
        /// A negative integer if this instance has a lower nominal area ratio than the other instance. <br/>
        /// Zero if this instance has the same nominal area ratio as the other instance.
        /// </returns>
        /// 
        public int CompareTo(FastRectNodeChildSpec other)
        {
            // ====== Order of child rectangle insertion ======
            // During item processing in _ProcessNewData(), each item is sent to the first 
            // child node on the list whose bounding rectangle encompasses the item's rectangle.
            //
            // Thus, to maximize specificity (query efficiency), the child nodes with the most 
            // specific (tiniest) bounding rectangle should be checked upfront so that they are 
            // preferentially selected.
            //
            // The child node's specificity is proportional to its child-to-parent area ratio.
            // Thus, ChildSpec's CompareTo() implementation is based on that ratio.
            // ======
            // Area is compared first.
            double thisAreaRatio = AreaRatio.Ratio;
            double otherAreaRatio = other.AreaRatio.Ratio;
            if (thisAreaRatio > otherAreaRatio) return 1;
            if (thisAreaRatio < otherAreaRatio) return -1;
            // If nominal area ratios are equal, a ChildSpec with a lower 
            // (HorzParentCellCount * VertParentCellCount) will be considered less.
            int thisTotalParentCellCount = HorzParentCellCount * VertParentCellCount;
            int otherTotalParentCellCount = other.HorzParentCellCount * other.VertParentCellCount;
            if (thisTotalParentCellCount > otherTotalParentCellCount) return 1;
            if (thisTotalParentCellCount < otherTotalParentCellCount) return -1;
            return 0;
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<FastRectNodeChildSpec>()
                .Ingest(HorzParentCellCount, HorzChildCellCount, HorzCellShift)
                .Ingest(VertParentCellCount, VertChildCellCount, VertCellShift)
                .GetHashCode();
        }

        /// <summary>
        /// Given the parent node's bounding rectangle, enumerate all child rectangles based on the 
        /// spec parameters.
        /// </summary>
        /// 
        /// <param name="boundingRect">
        /// Parent node's bounding rectangle.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of child node bounding rectangles generated according to the spec.
        /// </returns>
        /// 
        public IEnumerable<Rect> Enumerate(Rect boundingRect)
        {
            int boundWidth = boundingRect.Width;
            int boundHeight = boundingRect.Height;
            if (boundWidth < HorzParentCellCount ||
                boundHeight < VertParentCellCount)
            {
                // If the bounding rectangle is too small, some bands will end up being empty, 
                // which are not valid for use as child rectangles. 
                yield break;
            }
            int boundX = boundingRect.X;
            int boundY = boundingRect.Y;
            Rect ConvertBandToRect(int horzBandStart, int horzBandStop, int vertBandStart, int vertBandStop)
            {
                int startX = boundX + (boundWidth * horzBandStart) / HorzParentCellCount;
                int stopX = boundX + (boundWidth * horzBandStop) / HorzParentCellCount;
                int startY = boundY + (boundHeight * vertBandStart) / VertParentCellCount;
                int stopY = boundY + (boundHeight * vertBandStop) / VertParentCellCount;
                return new Rect(startX, startY, stopX - startX, stopY - startY);
            }
            for (int vertBandStart = 0; 
                vertBandStart + VertChildCellCount <= VertParentCellCount; 
                vertBandStart += VertCellShift)
            {
                int vertBandStop = vertBandStart + VertChildCellCount;
                for (int horzBandStart = 0; 
                    horzBandStart + HorzChildCellCount <= HorzParentCellCount; 
                    horzBandStart += HorzCellShift)
                {
                    int horzBandStop = horzBandStart + HorzChildCellCount;
                    var childRect = ConvertBandToRect(horzBandStart, horzBandStop, vertBandStart, vertBandStop);
                    yield return childRect;
                }
            }
        }
    }
}
