using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="RectBoundBitEncoder"/> accelerates rectangular overlap testing inside a searchable rectangle 
    /// collection.
    /// 
    /// <para>
    /// This is an internal component, typically used in collection classes that implement <see cref="IRectQuery{T}"/>.
    /// </para>
    /// 
    /// <para>
    /// The X-axis and Y-axis is subdivided into some bands of interest. Each band on each axis is assigned a bit. 
    /// This allows fast overlap checking for two rectangles via bit testing. The <see cref="Test(uint, uint)"/> 
    /// function tests whether overlapping bits are set on at least one horizontal band and at least one vertical band.
    /// </para>
    /// </summary>
    /// 
    [Obsolete]
    public struct RectBoundBitEncoder
    {
        public int Left { get; }

        public int Top { get; }

        public int Step { get; }

        public int Count { get; }

        public Point TopLeft => new Point(Left, Top);

        public Size Size => new Size(Step * Count, Step * Count);

        public Rect Rect => new Rect(TopLeft, Size);

        public const int UsableBitCount = 62;

        public RectBoundBitEncoder(Rect rect, int step)
        {
            if (rect.Width <= 0 || rect.Height <= 0 ||
                rect.Width != rect.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(rect));
            }
            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(step));
            }
            int count = rect.Width / step;
            if (rect.Width != count * step)
            {
                throw new ArgumentOutOfRangeException(nameof(step));
            }
            if (count * 2 > UsableBitCount)
            { 
                throw new ArgumentOutOfRangeException(nameof(step));
            }
            Left = rect.Left;
            Top = rect.Top;
            Step = step;
            Count = count;
        }

        /// <summary>
        /// Encodes the rectangle's bounds into a bit mask.
        /// </summary>
        /// 
        /// <param name="rect">
        /// A rectangle. <br/>
        /// This rectangle must have positive width and height, and must be completely contained 
        /// within the rectangular bounds of this <see cref="RectBoundBitEncoder"/> instance.
        /// </param>
        /// 
        /// <returns>
        /// The encoded bit mask for the specified rectangle.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.NoInlining)]
        public ulong Encode(Rect rect)
        {
            if (!_ContainsWithin(Rect, rect))
            {
                _Throw<ulong>();
            }
            return _InternalEncode(rect);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong _InternalEncode(Rect rect)
        {
            int offsetMinX = rect.Left - Left;
            int offsetMaxX = offsetMinX + rect.Width - 1;
            int offsetMinY = rect.Top - Top;
            int offsetMaxY = offsetMinY + rect.Height - 1;
            ulong flagMinX = _ComputeFlag(offsetMinX);
            ulong flagMaxX = _ComputeFlag(offsetMaxX);
            ulong flagMinY = _ComputeFlag(offsetMinY);
            ulong flagMaxY = _ComputeFlag(offsetMaxY);
            ulong oobCheck = (flagMinX | flagMaxX | flagMinY | flagMaxY) & _OobMask;
            ulong flagX = _SetAllBitsAbove(flagMinX) & _SetAllBitsBelow(flagMaxX);
            ulong flagY = _SetAllBitsAbove(flagMinY) & _SetAllBitsBelow(flagMaxY);
            return (flagY << Count) | flagX | oobCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool Test_NoInline(ulong flag1, ulong flag2)
        {
            return Test(flag1, flag2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(ulong flag1, ulong flag2)
        {
            if (_IsOob(flag1 | flag2))
            {
                _Throw<bool>();
            }
            ulong mask = (1uL << Count) - 1uL;
            ulong horzMask = mask;
            ulong vertMask = mask << Count;
            return _HasIntersect(flag1, flag2, horzMask, vertMask);
        }

        private const ulong _OobBelow = 0x4000_0000_0000_0000uL;
        private const ulong _OobAbove = 0x8000_0000_0000_0000uL;
        private const ulong _OobMask = 0xC000_0000_0000_0000uL;

        /// <summary>
        /// Divides the offset by the <see cref="Step"/>, and converts the result into a bit flag.
        /// 
        /// <para>
        /// The input to this function, "offset", is the coordinate value (either X-axis or Y-axis)
        /// minus the <see cref="Left"/> or <see cref="Top"/> of that axis.
        /// </para>
        /// 
        /// <para>
        /// The within-range is defined as: <c>Range(0, Step * Count)</c>.
        /// </para>
        /// </summary>
        /// 
        /// <param name="offset">
        /// The input to this function, "offset", is the coordinate value (either X-axis or Y-axis)
        /// minus the <see cref="Left"/> or <see cref="Top"/> of that axis.
        /// </param>
        /// 
        /// <returns>
        /// A bit flag representing the quotient, computed as:<br/>
        /// <code>
        /// quotient = offset / Step; <br/>
        /// flag = 1u &lt;&lt; quotient;
        /// </code>
        /// If the offset if outside the range, this function returns either <see cref="_OobBelow"/>
        /// or <see cref="_OobAbove"/>.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong _ComputeFlag(int offset)
        {
            if (offset < 0)
            {
                return _OobBelow;
            }
            int quotient = offset / Step;
            if (quotient >= Count)
            {
                return _OobAbove;
            }
            return 1uL << quotient;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool _IsOob(ulong flag)
        {
            return (flag & _OobMask) != 0uL;
        }

        /// <summary>
        /// Given a bit mask with exactly one bit set, this function returns a
        /// 32-bit mask where the input bit and all bits above are set.
        /// 
        /// <para>
        /// The function's output is meaningless if the precondition is not met.
        /// </para>
        /// 
        /// <para>
        /// This function does not perform any other masking. Thus, the bits reserved
        /// for the out-of-bound indicators will also be set. It is the caller's 
        /// responsibility to apply masking on the output.
        /// </para>
        /// </summary>
        /// 
        /// <param name="singleFlag">
        /// A 32-bit mask where exactly one bit is set.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit mask where the input bit and all bits above are set.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong _SetAllBitsAbove(ulong singleFlag)
        {
            return unchecked(singleFlag - 1uL) ^ 0xFFFF_FFFF_FFFF_FFFFuL;
        }

        /// <summary>
        /// Given a bit mask with exactly one bit set, this function returns a
        /// 32-bit mask where the input bit and all bits below are set.
        /// 
        /// <para>
        /// The function's output is meaningless if the precondition is not met.
        /// </para>
        /// </summary>
        /// 
        /// <param name="singleFlag">
        /// A 32-bit mask where exactly one bit is set.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit mask where the input bit and all bits below are set.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong _SetAllBitsBelow(ulong singleFlag)
        {
            return singleFlag | unchecked(singleFlag - 1uL);
        }

        /// <summary>
        /// Tests two bit-encoded rectangle flags for overlap.
        /// 
        /// <para>
        /// The X-axis and Y-axis is subdivided into some bands of interest. Each band
        /// on each axis is assigned a bit. This function checks whether overlapping bits
        /// are set on at least one horizontal band and at least one vertical band.
        /// </para>
        /// </summary>
        /// 
        /// <param name="flag1">
        /// The first bit-encoded rectangle flag to be tested.
        /// </param>
        /// 
        /// <param name="flag2">
        /// The second bit-encoded rectangle flag to be tested.
        /// </param>
        /// 
        /// <param name="horzMask">
        /// The range of bits belonging to bands on the horizontal axis.
        /// </param>
        /// 
        /// <param name="vertMask">
        /// The range of bits belonging to bands on the vertical axis.
        /// </param>
        /// 
        /// <returns>
        /// <para>
        /// True if there are overlaps in their horizontal and vertical bands. <br/>
        /// The caller should proceed to test the actual rectangle coordinates to check for
        /// intersection.
        /// </para>
        /// <para>
        /// False if there are no overlaps in their horizontal and vertical bands. <br/>
        /// The caller will not need to perform tests on the actual rectangle coordinates
        /// because they cannot possibly intersect.
        /// </para>
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool _HasIntersect(ulong flag1, ulong flag2, ulong horzMask, ulong vertMask)
        {
            ulong flag12 = flag1 & flag2;
            return ((flag12 & horzMask) != 0uL) &&
                ((flag12 & vertMask) != 0uL);
        }

        /// <summary>
        /// Internal implementation of checking existence of intersection between two rectangles.
        /// 
        /// <para>
        /// RectTree requires its own implementation for the following reasons:
        /// <br/>
        /// Firstly, RectTree requires an implementation that validates both rectangle arguments 
        /// to have positive width and height. 
        /// <br/>
        /// Also, a result of true (the existence of intersection) requires the intersection width 
        /// and height to be positive.
        /// </para>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool _InternalHasIntersect(Rect a, Rect b)
        {
            return InternalRectUtility.Inline.HasIntersect(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool _ContainsWithin(Rect checkOuter, Rect checkInner)
        {
            return InternalRectUtility.Inline.ContainsWithin(checkOuter, checkInner);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static PseudoReturnType _Throw<PseudoReturnType>()
        {
            return InternalRectUtility.NoInline.Throw<PseudoReturnType>();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _Throw()
        {
            InternalRectUtility.NoInline.Throw();
        }
    }
}
