using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            int rectWidth = rectToEncode.Width;
            int rectHeight = rectToEncode.Height;
            if (SpatialBitMaskUtility.FastNegativeTest(boundWidth, boundHeight, stepSize, rectWidth, rectHeight))
            {
                return false;
            }
            //
            // Check for intersection. 
            //
            // This is a precondition for the correct use of IRectMaskArith. 
            // If the rectToEncode does not intersect the bounding rectangle, 
            // none of the bits will be set, and the RectMask conveys no useful 
            // information with regard to accelerated rectangle intersection check.
            //
            int boundMinX = boundingRect.X;
            int boundMinY = boundingRect.Y;
            int boundMaxX = boundMinX + boundWidth - 1;
            int boundMaxY = boundMinY + boundHeight - 1;
            int rectMinX = rectToEncode.X;
            int rectMinY = rectToEncode.Y;
            int rectMaxX = rectMinX + rectWidth - 1;
            int rectMaxY = rectMinY + rectHeight - 1;
            if (SpatialBitMaskUtility.FastNegativeTest(
                rectMaxX - boundMinX,
                boundMaxX - rectMinX,
                rectMaxY - boundMinY,
                boundMaxY - rectMinY))
            {
                // The rectangle to encode does not intersect the bounding rectangle.
                return false;
            }
            //
            // Intersection, that is, the clipping of rectToEncode to the boundingRectangle.
            //
            int interMinX = Math.Max(boundMinX, rectMinX);
            int interMinY = Math.Max(boundMinY, rectMinY);
            int interMaxX = Math.Min(boundMaxX, rectMaxX);
            int interMaxY = Math.Min(boundMaxY, rectMaxY);
            //
            // Converts the minimum and maximum X and Y values (all inclusive) into their cell index.
            //
#if false
            CellIndex ciLT = Internal_TryFindCell(boundingRect, stepSize, new Point(interMinX, interMinY));
            CellIndex ciRB = Internal_TryFindCell(boundingRect, stepSize, new Point(interMaxX, interMaxY));
            int cellMinX = ciLT.CellX;
            int cellMinY = ciLT.CellY;
            int cellMaxX = ciRB.CellX;
            int cellMaxY = ciRB.CellY;
#else
            // (PERF OPT) Not using Point and CellIndex made the code run much faster based on benchmark.
            // 
            // (NOTE) The use of Internal_TryFindCell_Deconstructed() requires all arguments to be
            // pre-validated. Refer to validation source code above.
            //
            Internal_TryFindCell_Deconstructed(
                boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height, 
                stepSize, interMinX, interMinY, 
                out int cellMinX, out int cellMinY);
            Internal_TryFindCell_Deconstructed(
                boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height,
                stepSize, interMaxX, interMaxY,
                out int cellMaxX, out int cellMaxY);
#endif
            //
            // Generates the X and Y bit masks by converting the cell index into bit positions,
            // of which there is a first bit and a last bit, and then also fills up all the bits 
            // in between.
            //
            ulong maskMinX = SpatialBitMaskUtility.SetAllBitsAbove(1uL << cellMinX);
            ulong maskMaxX = SpatialBitMaskUtility.SetAllBitsBelow(1uL << cellMaxX);
            ulong maskMinY = SpatialBitMaskUtility.SetAllBitsAbove(1uL << cellMinY);
            ulong maskMaxY = SpatialBitMaskUtility.SetAllBitsBelow(1uL << cellMaxY);
            xmask = maskMinX & maskMaxX;
            ymask = maskMinY & maskMaxY;
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
            int cellX = DivideIfNonNegativeLessThan.Compute_Unchecked(offsetX, stepSize, boundWidth);
            int cellY = DivideIfNonNegativeLessThan.Compute_Unchecked(offsetY, stepSize, boundHeight);
            return new CellIndex(cellX, cellY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Internal_TryFindCell_Deconstructed(
            int boundX, int boundY, int boundW, int boundH, 
            int stepSize, 
            int pointX, int pointY, 
            out int cellX, out int cellY)
        {
            int offsetX = pointX - boundX;
            int offsetY = pointY - boundY;
            cellX = DivideIfNonNegativeLessThan.Compute_Unchecked(offsetX, stepSize, boundW);
            cellY = DivideIfNonNegativeLessThan.Compute_Unchecked(offsetY, stepSize, boundH);
        }

        /// <summary>
        /// Returns the quotient if the the value satisfies: <br/>
        /// <c>(0 &lt;= value &lt; limit)</c>.
        /// 
        /// <para>
        /// This is a static class containing three methods: <br/>
        /// <c><see cref="Compute_ElseThrow(int, int, int)"/></c> (throws if any parameter is invalid)<br/>
        /// <c><see cref="Compute_Clamped(int, int, int)"/></c> (clamps the value)<br/>
        /// <c><see cref="Compute_Unchecked(int, int, int)"/></c> (does not perform any check)
        /// </para>
        /// </summary>
        /// 
        public static class DivideIfNonNegativeLessThan
        {
            /// <summary>
            /// Returns the quotient if the the value satisfies: <br/>
            /// <c>(0 &lt;= value &lt; limit)</c>.
            /// 
            /// <para>
            /// Exception is thrown if any arguments are invalid.
            /// </para>
            /// </summary>
            /// 
            /// <param name="value">
            /// The value to be divided. Must be non-negative and less than <c>limit</c>.
            /// </param>
            /// 
            /// <param name="divisor">
            /// The divisor. Must be positive.
            /// </param>
            /// 
            /// <param name="limit">
            /// The exclusive upper limit for <c>value</c>. Must be positive.
            /// </param>
            /// 
            /// <returns>
            /// The quotient given by <c>(value / divisor).</c>
            /// </returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">
            /// One or more arguments are negative or outside the range.
            /// </exception>
            /// 
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static int Compute_ElseThrow(int value, int divisor, int limit)
            {
                if (limit <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(limit));
                }
                if (divisor <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(divisor));
                }
                if (value < 0 || 
                    value >= limit)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                return value / divisor;
            }

            /// <summary>
            /// Returns the quotient if the the value satisfies: <br/>
            /// <c>(0 &lt;= value &lt; limit)</c>.
            /// 
            /// <para>
            /// This function does not throw any exception.
            /// </para>
            /// </summary>
            /// 
            /// <param name="value">
            /// The value to be divided. Normally non-negative and less than <c>limit</c>. 
            /// <br/>
            /// If the value is zero or negative, this function returns zero. 
            /// <br/>
            /// If the <c>limit</c> is valid but the value is equal to or greater than limit, 
            /// the quotient is computed as <c>((limit - 1) / divisor)</c>.
            /// </param>
            /// 
            /// <param name="divisor">
            /// The divisor. Normally positive. 
            /// <br/>
            /// If the divisor is zero or negative, this function returns zero.
            /// </param>
            /// 
            /// <param name="limit">
            /// The exclusive upper limit for <c>value</c>. Normally positive. 
            /// <br/>
            /// If the limit is zero or negative, this function returns zero.
            /// </param>
            /// 
            /// <returns>
            /// The quotient given by <c>(value / divisor)</c> if all arguments are within their 
            /// normal range. 
            /// <br/>
            /// Returns <c>(0 / divisor)</c> or <c>((limit - 1) / divisor)</c> if both <c>limit</c>
            /// and <c>divisor</c> are valid but <c>value</c> is outside the range.
            /// <br/>
            /// Returns zero if <c>limit</c> or <c>divisor </c> is outside their normal range.
            /// </returns>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int Compute_Clamped(int value, int divisor, int limit)
            {
                // ======
                // Tests that divisor >= 1 and limit >= 1.
                // The need to test both (divisor) and (divisor - 1) is to catch the case 
                // where (divisor == int.MinValue).
                // ======
                // If value is negative, zero is returned.
                // ======
                if (SpatialBitMaskUtility.FastNegativeTest(value, divisor, limit, divisor - 1, limit - 1))
                {
                    return 0;
                }
                if (value >= limit)
                {
                    value = limit - 1;
                }
                return value / divisor;
            }

            /// <summary>
            /// Returns the quotient if the the value satisfies: <br/>
            /// <c>(0 &lt;= value &lt; limit)</c>.
            /// 
            /// <para>
            /// This function does not perform any argument validation. It is intended to generate
            /// the smallest code for the purpose, with the precondition that all arguments are
            /// within their normal range.
            /// </para>
            /// 
            /// <para>
            /// Invalid argument may cause unspecified return value, runtime exception, program 
            /// termination, or other behavior. The outcome may be non-deterministic, and may depend
            /// on the .NET Runtime on which the program is running.
            /// </para>
            /// </summary>
            /// 
            /// <param name="value">
            /// The value to be divided. Must be non-negative and less than <c>limit</c>.
            /// </param>
            /// 
            /// <param name="divisor">
            /// The divisor. Must be positive.
            /// </param>
            /// 
            /// <param name="limit">
            /// The exclusive upper limit for <c>value</c>. Must be positive. 
            /// <br/>
            /// Since validation is not performed, the function does not make actual use of this argument.
            /// </param>
            /// 
            /// <returns>
            /// The quotient given by <c>(value / divisor).</c>
            /// </returns>
            /// 
            [SuppressMessage("Style", "IDE0060:Remove unused parameter", 
                Justification = "Unused parameter is kept for consistency with its sibling functions.")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int Compute_Unchecked(int value, int divisor, int limit)
            {
                unchecked
                {
                    uint uv = (uint)value;
                    uint ud = (uint)divisor;
                    return (int)(uv / ud);
                }
            }
        }
    }
}
