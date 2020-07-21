using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public struct Point 
        : IEquatable<Point>
    {
        public static Point Origin { get; } = new Point(0, 0);

        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

#if false
        public Point(HashPoint hashPoint)
        {
            X = hashPoint.X;
            Y = hashPoint.Y;
        }

        public Point(ImageHashPoint hashPoint)
        {
            X = hashPoint.X;
            Y = hashPoint.Y;
        }
#endif

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Point other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(Point other)
        {
            return X == other.X &&
                Y == other.Y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }

        public static implicit operator System.Drawing.Point(Point p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator Point(System.Drawing.Point p)
        {
            return new Point(p.X, p.Y);
        }

#if false
        ///// <summary>
        ///// Computes the movement from the first point to the second point.
        ///// Mathematically, this is denoted as (p2 - p1), where p1 and p2 are two-dimensional vectors.
        ///// </summary>
        ///// <param name="p1">The first point, or the original location.</param>
        ///// <param name="p2">The second point, or the moved location.</param>
        ///// <returns>
        ///// The vector of movement from the first point to the second point.
        ///// </returns>
#endif

        /// <summary>
        /// Computes the vector difference between the two points.
        /// 
        /// <para>
        /// Mathematically, the result is the vector subtraction of the two points.
        /// </para>
        /// 
        /// <example>
        /// <para>
        /// In typical usage, image content that has moved from a previous location (on the previous screenshot)
        /// to the current location (on the current screenshot) will be represented by a <see cref="Movement"/> 
        /// calculated as follows:
        /// </para>
        /// <para><code>
        /// L001    int prevX, prevY; <br/>
        /// L002    var prev = new Point(prevX, prevY); <br/>
        /// L003    int currX, currY; <br/>
        /// L004    var curr = new Point(currX, currY); <br/>
        /// L005    <br/>
        /// L006    // Calculates the movement by invoking the minus operator: <br/>
        /// L007    Movement movement = (curr - prev); <br/>
        /// L008    <br/>
        /// L009    // An equivalent way of calculating the movement: <br/>
        /// L010    int deltaX = currX - prevX; <br/>
        /// L011    int deltaY = currY - prevY; <br/>
        /// L012    Movement movementEquiv = new Movement(deltaX, deltaY); <br/>
        /// </code></para>
        /// </example>
        /// </summary>
        /// 
        /// <param name="argLeft">
        /// The <see cref="Point"/> on the left side of the minus operator.
        /// </param>
        /// 
        /// <param name="argRight">
        /// The <see cref="Point"/> on the right side of the minus operator.
        /// </param>
        /// 
        /// <returns>
        /// The vector difference between the two arguments.
        /// </returns>
        /// 
        public static Movement operator -(Point argLeft, Point argRight)
        {
            return new Movement(argLeft.X - argRight.X, argLeft.Y - argRight.Y);
        }

        public static Point operator +(Point p, Movement m)
        {
            return new Point(p.X + m.DeltaX, p.Y + m.DeltaY);
        }

        public static Point operator -(Point p, Movement m)
        {
            return new Point(p.X - m.DeltaX, p.Y - m.DeltaY);
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<Point>().Ingest(X, Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
