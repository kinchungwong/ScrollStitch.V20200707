using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;
    using System.Diagnostics;

    public static class RectMaskUtility
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool TryEncodeRect(Rect boundingRect, int stepSize, Rect rectToEncode, out ulong xmask, out ulong ymask)
        {
            xmask = default;
            ymask = default;
            int boundWidth = boundingRect.Width;
            int boundHeight = boundingRect.Height;
            if (boundWidth <= 0 ||
                boundHeight <= 0)
            {
                // Invalid bounding rectangle.
                return false;
            }
            if (stepSize <= 0)
            {
                // Invalid step size.
                return false;
            }
            if (rectToEncode.Width <= 0 ||
                rectToEncode.Height <= 0)
            {
                // Invalid rectangle to encode.
                return false;
            }
            int bl = boundingRect.Left;
            int br = boundingRect.Right;
            int bt = boundingRect.Top;
            int bb = boundingRect.Bottom;
            int rel = rectToEncode.Left;
            int rer = rectToEncode.Right;
            int ret = rectToEncode.Top;
            int reb = rectToEncode.Bottom;
            if (rel >= br || bl >= rer || ret >= bb || bt >= reb)
            {
                // The rectangle to encode does not intersect the bounding rectangle.
                return false;
            }
            int validLeft = Math.Max(bl, rel);
            int validTop = Math.Max(bt, ret);
            int validRight = Math.Min(br, rer);
            int validBottom = Math.Min(bb, reb);
            var ciLT = Internal_TryFindCell(boundingRect, stepSize, new Point(validLeft, validTop));
            var ciRB = Internal_TryFindCell(boundingRect, stepSize, new Point(validRight - 1, validBottom - 1));
            xmask = SpatialBitMaskUtility.SetAllBitsAbove(1uL << ciLT.CellX) &
                SpatialBitMaskUtility.SetAllBitsBelow(1uL << ciRB.CellX);
            ymask = SpatialBitMaskUtility.SetAllBitsAbove(1uL << ciLT.CellY) &
                SpatialBitMaskUtility.SetAllBitsBelow(1uL << ciRB.CellY);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFindCell(Rect boundingRect, int stepSize, Point point, out CellIndex cellIndex)
        {
            cellIndex = default;
            int boundWidth = boundingRect.Width;
            int boundHeight = boundingRect.Height;
            if (boundWidth <= 0 ||
                boundHeight <= 0)
            {
                return false;
            }
            if (stepSize <= 0)
            {
                return false;
            }
            cellIndex = Internal_TryFindCell(boundingRect, stepSize, point);
            return true;
        }

        /// <summary>
        /// Given a grid defined on the bounding rect (with the specified top-left corner, width and height,
        /// and cell width and height), this method computes the cell index for the specified point.
        /// 
        /// <para>
        /// This function assumes all input parameters have been validated.
        /// <br/>
        /// If unvalidated parameters need to be handled, use <see cref="TryFindCell"/> instead.
        /// </para>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CellIndex Internal_TryFindCell(Rect boundingRect, int stepSize, Point point)
        {
            int boundWidth = boundingRect.Width;
            int boundHeight = boundingRect.Height;
            int offsetX = point.X - boundingRect.X;
            int offsetY = point.Y - boundingRect.Y;
            int cellX = Internal_DivideIfNonNegativeLessThan(offsetX, stepSize, boundWidth);
            int cellY = Internal_DivideIfNonNegativeLessThan(offsetY, stepSize, boundHeight);
            return new CellIndex(cellX, cellY);
        }

        /// <summary>
        /// <para>
        /// Returns the quotient if the the value satisfies: <br/>
        /// <c>(0 &lt;= value &lt; limit)</c>.
        /// </para>
        /// 
        /// <para>
        /// This function assumes all input parameters have been validated.
        /// </para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divisor"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Internal_DivideIfNonNegativeLessThan(int value, int divisor, int limit)
        {
#if false
            if (value < 0)
            {
                return int.MinValue;
            }
            else if (value >= limit)
            {
                return int.MaxValue;
            }
            return value / divisor;
#else
            value = (value < 0) ? 0 : (value >= limit) ? (limit - 1) : value;
            return unchecked((int)((uint)value / (uint)divisor));
#endif
        }
    }
}
