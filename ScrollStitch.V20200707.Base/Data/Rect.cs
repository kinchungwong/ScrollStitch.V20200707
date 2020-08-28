using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Data
{
    using HashCode;

    /// <summary>
    /// 
    /// 
    /// Refer to <code>Readme_Hash2D.Data_namespace.md</code>
    /// </summary>
    public struct Rect
        : IEquatable<Rect>
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public Point TopLeft => new Point(Left, Top);
        public Point TopRight => new Point(Right, Top);
        public Point BottomLeft => new Point(Left, Bottom);
        public Point BottomRight => new Point(Right, Bottom);
        public Size Size => new Size(Width, Height);

        public bool IsPositive => Width > 0 && Height > 0;
        public bool IsNonNegative => Width >= 0 && Height >= 0;
        public bool IsNegative => Width < 0 || Height < 0;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect(Point topLeft, Size size)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// <para>
        /// Returns a new rectangle that encompasses the current and the given rectangles.
        /// </para>
        /// <para>
        /// To control the method's handling of rectangles containing zero dimensions 
        /// (width or height), use <seealso cref="Encompass(Rect, bool)"/>.
        /// </para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// A new rectangle that encompasses the current and the given rectangles.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Both rectangles are invalid due to non-positive dimensions (width or height).
        /// </exception>
        public Rect Encompass(Rect other)
        {
            return Encompass(other, respectZeroAreaRects: false, validateBoth: true);
        }

        /// <summary>
        /// <para>
        /// Returns a new rectangle that encompasses the current and the given rectangles.
        /// This method accepts a flag that controls its handling of zero-area rectangles.
        /// </para>
        /// </summary>
        /// <param name="other"></param>
        /// <param name="respectZeroAreaRects">
        /// If true, a rectangle having either zero width or zero height (and neither is 
        /// negative) will be treated as a zero-area yet non-empty rectangle. The coordinates
        /// of this zero-area, non-empty rectangle will participate in the encompassing 
        /// rectangle calculation. As an example,
        /// <code>var myRect = new Rect(10, 10, 50, 30);</code>
        /// <code>var originRect = new Rect(0, 0, 0, 0);</code>
        /// <code>var newRect = myRect.Encompass(originRect, respectZeroAreaRects: true);</code>
        /// will result in <c>newRect</c> encompassing the origin.
        /// Whereas, when <c>respectZeroAreaRects</c> is false, <c>newRect</c> 
        /// will be the same as <c>myRect</c>.
        /// </param>
        /// <param name="validateBoth">
        /// If true, an exception is thrown if either rectangle contains negative dimensions. 
        /// If false, an exception is thrown if both rectangles contain negative dimensions. 
        /// </param>
        /// <returns>
        /// A new rectangle that encompasses the current and the given rectangles.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Both rectangles are invalid due to non-positive dimensions (width or height).
        /// </exception>
        public Rect Encompass(Rect other, bool respectZeroAreaRects, bool validateBoth)
        {
            return _EncompassOrIntersect(_EncompassOrIntersectOp.Encompass, this, other, respectZeroAreaRects, validateBoth).Value;
        }

        public Rect? Intersect(Rect other)
        {
            return _EncompassOrIntersect(_EncompassOrIntersectOp.Intersect, this, other, respectZeroAreaRects: true, validateBoth: true);
        }

        private enum _EncompassOrIntersectOp
        { 
            Encompass,
            Intersect
        }

        private static Rect? _EncompassOrIntersect(_EncompassOrIntersectOp op, Rect self, Rect other, bool respectZeroAreaRects, bool validateBoth)
        {
            bool thisNeg = self.IsNegative;
            bool otherNeg = other.IsNegative;
            if (thisNeg && otherNeg)
            {
                throw new InvalidOperationException("Encompass() is ill-defined when both rectangles contain negative dimensions.");
            }
            if (validateBoth && (thisNeg || otherNeg))
            {
                string msgWhich = thisNeg ? "self" : "other";
                throw new ArgumentException($"{nameof(Rect)}.{nameof(Encompass)}(): rectangle {msgWhich} contains negative dimensions, " +
                    "and validateBoth is specified.");
            }
            if (thisNeg)
            {
                return other;
            }
            if (otherNeg)
            {
                return self;
            }
            if (!respectZeroAreaRects)
            {
                bool thisZero = !self.IsPositive;
                bool otherZero = !other.IsPositive;
                if (thisZero)
                {
                    return other;
                }
                if (otherZero)
                {
                    return self;
                }
            }
            int x1, y1, x2, y2;
            switch (op)
            {
                case _EncompassOrIntersectOp.Encompass:
                    x1 = Math.Min(self.Left, other.Left);
                    y1 = Math.Min(self.Top, other.Top);
                    x2 = Math.Max(self.Right, other.Right);
                    y2 = Math.Max(self.Bottom, other.Bottom);
                    return new Rect(x1, y1, x2 - x1, y2 - y1);
                case _EncompassOrIntersectOp.Intersect:
                    x1 = Math.Max(self.Left, other.Left);
                    y1 = Math.Max(self.Top, other.Top);
                    x2 = Math.Min(self.Right, other.Right);
                    y2 = Math.Min(self.Bottom, other.Bottom);
                    if (x2 >= x1 && y2 >= y1)
                    { 
                        return new Rect(x1, y1, x2 - x1, y2 - y1);
                    }
                    return null;
                default:
                    throw new Exception("Unexpected");
            }
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Rect other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(Rect other)
        {
            return X == other.X &&
                Y == other.Y &&
                Width == other.Width &&
                Height == other.Height;
        }

        public static bool operator ==(Rect p1, Rect p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Rect p1, Rect p2)
        {
            return !p1.Equals(p2);
        }

        public static Rect operator +(Rect r, Movement m)
        {
            return new Rect(r.X + m.DeltaX, r.Y + m.DeltaY, r.Width, r.Height);
        }

        public static Rect operator -(Rect r, Movement m)
        {
            return new Rect(r.X - m.DeltaX, r.Y - m.DeltaY, r.Width, r.Height);
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<Rect>().Ingest(X, Y, Width, Height).GetHashCode();
        }

        public override string ToString()
        {
            return $"(X={X}, Y={Y}, W={Width}, H={Height})";
        }
    }
}
