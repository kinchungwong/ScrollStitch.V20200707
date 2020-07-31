using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.Internals
{
    using V20200707.Data;
    using V20200707.Functional;

    /// <summary>
    /// <see cref="QuadrantArcPointGenerator"/> generates the sequenece of points for the arc 
    /// corresponding to a quarter-circle.
    /// 
    /// <para>
    /// The generated sequence of points start at <c>(radius, 0)</c>, which lies on the positive
    /// x-axis, and sweeps from the positive x-axis toward the positive y-axis. The end of the 
    /// sequence is reacheed after it has returned the point <c>(0, radius)</c>.
    /// </para>
    /// 
    /// <para>
    /// This class implements <c><see cref="IEnumerator{T}"/> of <see cref="Point"/></c>. <br/>
    /// However, for performance reason, it is always easier for an algorithm to instantiate
    /// this struct and call the <see cref="MoveNext"/> method directly. <br/>
    /// Additionally, accessing the integer properties <c><see cref="X"/> and <see cref="Y"/></c>
    /// has lower overhead then accessing <see cref="Current"/>.
    /// </para>
    /// 
    /// <para>
    /// As a reminder, the model usage of <see cref="IEnumerator{T}"/> is illustrated in the 
    /// folloring sample code:
    /// <code>
    /// L001    while (enumerator.MoveNext()) <br/>
    /// L002    { <br/>
    /// L003    ... use(enumerator.Current); <br/>
    /// L004    }
    /// </code>
    /// Concrete implementations of <see cref="IEnumerator{T}"/> must include the minimal and 
    /// necessary mechanisms to support this model usage.
    /// </para>
    /// </summary>
    public struct QuadrantArcPointGenerator
         : IEnumerator<Point>
    {
        /// <summary>
        /// The maximum supported radius without causing internal integer overflow.
        /// </summary>
        public static readonly int MaxAllowedRadius = 32000;

        /// <summary>
        /// The radius.
        /// </summary>
        public int Radius { get; }

        /// <summary>
        /// The starting X set by the constructor.
        /// </summary>
        public int StartX { get; }

        /// <summary>
        /// The starting X set by the constructor.
        /// </summary>
        public int StartY { get; }

        /// <summary>
        /// The current value of X.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The current value of Y.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The current point, namely <c>new Point(X, Y)</c>.
        /// 
        /// <para>
        /// Performance note. <br/>
        /// The properties <see cref="X"/> and <see cref="Y"/> have lower overhead than 
        /// <see cref="Current"/>.
        /// </para>
        /// </summary>
        public Point Current => new Point(X, Y);

        /// <summary>
        /// Whether the enumerator has been moved into the first item position. Refer to
        /// the CLR documentation for <see cref="IEnumerator{T}"/> for detail.
        /// </summary>
        public bool HasStarted { get; private set; }

        /// <summary>
        /// Whether the enumerator has been moved past the last item position. Refer to
        /// the CLR documentation for <see cref="IEnumerator{T}"/> for detail.
        /// </summary>
        public bool HasFinished { get; private set; }

        /// <summary>
        /// Explicit implementation of non-generic <see cref="IEnumerator.Current"/>.
        /// This property returns the boxed Point.
        /// </summary>
        object IEnumerator.Current => Current;

        #region
        private readonly int _rr;
        private int _xx;
        private int _yy;
        #endregion

        /// <summary>
        /// Initializes the point generator with the specified radius and the starting coordinate
        /// <c>(radius, 0)</c>, which is located on the positive x-axis. The generated points 
        /// sweep toward the positive y-axis.
        /// </summary>
        /// <param name="radius"></param>
        public QuadrantArcPointGenerator(int radius)
        {
            _CtorValidateR(radius);
            Radius = radius;
            StartX = radius;
            StartY = 0;
            X = radius;
            Y = 0;
            _rr = radius * radius;
            _xx = radius * radius;
            _yy = 0;
            HasStarted = false;
            HasFinished = false;
        }

        /// <summary>
        /// Initializes the point generator with the specified radius and a given starting 
        /// coordinate.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public QuadrantArcPointGenerator(int radius, int x, int y)
        {
            _CtorValidateRXY(radius, x, y);
            Radius = radius;
            StartX = x;
            StartY = y;
            X = x;
            Y = y;
            _rr = radius * radius;
            _xx = x * x;
            _yy = y * y;
            HasStarted = false;
            HasFinished = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool MoveNext()
        {
            // ======
            // The following checks must be performed in the specific order, in order to 
            // operate correctly given the model usage of <see cref="IEnumerator{T}"/>.
            // Refer to class comment for details.
            // ======
            if (!HasStarted)
            {
                HasStarted = true;
                return true;
            }
            if (HasFinished)
            {
                return false;
            }
            if (X <= 0)
            {
                HasFinished = true;
                return false;
            }
            // ====== Algorithm ======
            // At each step, the algorithm explores three candidates for the next point:
            // 
            // 1.. Decrease X (horizontally inward), 
            // ... which moves the point closer to the origin,
            // 
            // 2.. Decrease X and increase Y (diagonally), 
            // ... which depend on the current point's location on which part of the arc, 
            //
            // 3.. Increase Y (vertically downward),
            // ... which moves the point further away from the origin.
            // 
            // The algorithm picks the next point that best matches the ideal distance from 
            // ... the origin, maintaining the optimal shape of the circle arc.
            // 
            // As a result of this algorithm, with the exception of Radius == 4,
            // ...
            // ... All invocations of this algorithm (except for Radius == 4) result in 
            // ... a pixel chain that is 8-connected in a non-redundant way.
            // ... 
            // ... That is, if (x, y) and (x - 1, y + 1) are both on the list, then neither
            // ... (x - 1, y) nor (x, y + 1) would be on the list.
            // ======
            int xxMinus = _xx - 2 * X + 1;
            int yyPlus = _yy + 2 * Y + 1;
            int rrDiag = xxMinus + yyPlus;
            int absDiag = Math.Abs(rrDiag - _rr);
            if (rrDiag > _rr)
            {
                int rrInner = xxMinus + _yy;
                int absInner = Math.Abs(rrInner - _rr);
                if (absInner < absDiag)
                {
                    X -= 1;
                    _xx = xxMinus;
                    return true;
                }
            }
            else if (rrDiag < _rr)
            {
                int rrOuter = _xx + yyPlus;
                int absOuter = Math.Abs(rrOuter - _rr);
                if (absOuter < absDiag)
                {
                    Y += 1;
                    _yy = yyPlus;
                    return true;
                }
            }
            X -= 1;
            Y += 1;
            _xx = xxMinus;
            _yy = yyPlus;
            return true;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Resets the <see cref="IEnumerator{T}"/> state to that of the newly constructed state.
        /// 
        /// <para>
        /// This method also sets both <see cref="HasStarted"/> and <see cref="HasFinished"/> to 
        /// false. In other words, the caller will need to call <see cref="MoveNext"/> before 
        /// reading the first element of the generated points.
        /// </para>
        /// 
        /// <para>
        /// If this instance is constructed with a caller-specified starting point, it will be 
        /// used by this method.
        /// </para>
        /// </summary>
        public void Reset()
        {
            X = StartX;
            Y = StartY;
            _xx = X * X;
            _yy = Y * Y;
            HasStarted = false;
            HasFinished = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _CtorValidateR(int radius)
        {
            if (radius < 0 || radius > MaxAllowedRadius)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _CtorValidateRXY(int radius, int x, int y)
        {
            if (radius < 0 || radius > MaxAllowedRadius)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }
            if (x < 0 || x > radius)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y < 0 || y > radius)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
            // ======
            // The starting position's distance from the ideal distance is enforced
            // because the point generation algorithm may not guarantee termination 
            // if the point is too far off.
            // ======
            int r2min = (radius - 1) * (radius - 1);
            int r2max = (radius + 1) * (radius + 1);
            int r2 = x * x + y * y;
            if (r2 < r2min || r2 > r2max)
            {
                throw new InvalidOperationException(
                    "The specified starting point (x, y) is too far from the circle arc.");
            }
        }
    }
}
