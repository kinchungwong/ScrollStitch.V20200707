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
            /// Computes the intersecting rectangle between the two input rectangles.
            /// 
            /// <para>
            /// This method does not throw any exception. <br/>
            /// It may return null, namely a <see cref="Nullable{T}"/> of <see cref="Rect"/> with no value.
            /// </para>
            /// </summary>
            /// 
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// 
            /// <returns>
            /// A nullable <see cref="Rect"/>, which may be: <br/>
            /// The intersection of the two rectangles, if it exists (as a positive rectangle). <br/>
            /// Null, if the two rectangles do not intersect, if their intersection is empty 
            /// (zero width or height), or if any of the rectangles are non-positive.
            /// </returns>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Rect? TryComputeIntersection(Rect a, Rect b)
            {
                if (a.Width <= 0 || a.Height <= 0 ||
                    b.Width <= 0 || b.Height <= 0)
                {
                    return null;
                }
                int maxLeft = Math.Max(a.Left, b.Left);
                int minRight = Math.Min(a.Right, b.Right);
                int maxTop = Math.Max(a.Top, b.Top);
                int minBottom = Math.Min(a.Bottom, b.Bottom);
                if ((maxLeft < minRight) && (maxTop < minBottom))
                {
                    return new Rect(maxLeft, maxTop, minRight - maxLeft, minBottom - maxTop);
                }
                return null;
            }

            /// <summary>
            /// Checks whether the second rectangle fits completely inside the first rectangle.
            /// </summary>
            /// 
            /// <param name="rectOuter">
            /// The supposedly larger rectangle to be tested.
            /// </param>
            /// 
            /// <param name="rectInner">
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
            public static bool ContainsWithin(Rect rectOuter, Rect rectInner)
            {
                if (rectOuter.Width <= 0 || rectOuter.Height <= 0 ||
                    rectInner.Width <= 0 || rectInner.Height <= 0)
                {
                    return NoInline.Throw<bool>();
                }
                return (rectOuter.Left <= rectInner.Left) &&
                    (rectOuter.Top <= rectInner.Top) &&
                    (rectOuter.Right >= rectInner.Right) &&
                    (rectOuter.Bottom >= rectInner.Bottom);
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
            public static Rect? TryComputeIntersection(Rect a, Rect b)
            {
                return Inline.TryComputeIntersection(a, b);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static bool ContainsWithin(Rect rectOuter, Rect rectInner)
            {
                return Inline.ContainsWithin(rectOuter: rectOuter, rectInner: rectInner);
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
