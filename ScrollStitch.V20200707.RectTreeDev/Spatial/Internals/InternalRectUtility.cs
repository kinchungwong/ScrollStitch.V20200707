using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    public static class InternalRectUtility
    {
        public static class Inline
        {
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
            /// <exception cref="ArgumentException">
            /// One of more input rectangles have non-positive width and/or height.
            /// </exception>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool HasIntersect(Rect a, Rect b)
            {
                if (a.Width <= 0 || a.Height <= 0 ||
                    b.Width <= 0 || b.Height <= 0)
                {
                    return NoInline.Throw<bool>();
                }
                int maxLeft = Math.Max(a.Left, b.Left);
                int minRight = Math.Min(a.Right, b.Right);
                int maxTop = Math.Max(a.Top, b.Top);
                int minBottom = Math.Min(a.Bottom, b.Bottom);
                return (maxLeft < minRight) && (maxTop < minBottom);
            }

            /// <summary>
            /// Checks whether the second rectangle fits completely inside the first rectangle.
            /// </summary>
            /// 
            /// <param name="checkOuter">
            /// The supposedly larger rectangle to be tested.
            /// </param>
            /// 
            /// <param name="checkInner">
            /// The supposedly smaller rectangle to be tested.
            /// </param>
            /// 
            /// <returns>
            /// True if the supposedly smaller rectangle fits completely inside the supposedly 
            /// larger rectangle.
            /// </returns>
            /// 
            /// <exception cref="ArgumentException">
            /// One of more input rectangles have non-positive width and/or height.
            /// </exception>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool ContainsWithin(Rect checkOuter, Rect checkInner)
            {
                if (checkOuter.Width <= 0 || checkOuter.Height <= 0 ||
                    checkInner.Width <= 0 || checkInner.Height <= 0)
                {
                    return NoInline.Throw<bool>();
                }
                return (checkOuter.Left <= checkInner.Left) &&
                    (checkOuter.Top <= checkInner.Top) &&
                    (checkOuter.Right >= checkInner.Right) &&
                    (checkOuter.Bottom >= checkInner.Bottom);
            }
        }

        public static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static bool HasIntersect(Rect a, Rect b)
            {
                return Inline.HasIntersect(a, b);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static bool ContainsWithin(Rect checkOuter, Rect checkInner)
            {
                return Inline.ContainsWithin(checkOuter, checkInner);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static PseudoReturnType Throw<PseudoReturnType>()
            {
                throw new ArgumentException();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void Throw()
            {
                throw new ArgumentException();
            }
        }
    }
}
