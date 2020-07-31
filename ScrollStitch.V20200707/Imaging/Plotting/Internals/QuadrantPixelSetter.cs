using System;
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
    /// A static factory class that provides convenience methods for creating <see cref="QuadrantPixelSetter{TFunc}"/>.
    /// </summary>
    public static class QuadrantPixelSetterFactory
    {
        /// <summary>
        /// Creates an instance with the specified wrapped function and the center point.
        /// </summary>
        /// <typeparam name="TFunc">
        /// The wrapped function type, which is required to be a struct.
        /// </typeparam>
        /// <param name="center">
        /// The center point.
        /// </param>
        /// <param name="func">
        /// An instance of the wrapped function. If the wrapped function is stateful, the 
        /// state will be copied. <br/>
        /// (Remark: the wrapped function type is required to be a struct, for which CLR 
        /// implements bitwise copying.)
        /// </param>
        /// <returns>
        /// The newly created instance of <see cref="QuadrantPixelSetter{TFunc}"/>.
        /// </returns>
        /// 
        public static QuadrantPixelSetter<TFunc> Create<TFunc>(Point center, TFunc func)
            where TFunc : struct, IFuncInline<TFunc, int, int, int>
        {
            return new QuadrantPixelSetter<TFunc>(center, func);
        }
    }

    /// <summary>
    /// <para>
    /// Given a center point, this functor wraps another functor and modifies the <c>Invoke(int, int)</c>
    /// method as follows: <br/>
    /// ... The constructor accepts and stores the center point; <br/>
    /// ... The <c>Invoke</c> method on this instance accepts <c>(dx, dy)</c>, which are offsets from the center point; <br/>
    /// ... The <c>Invoke</c> method on this instance then generates four absolute points: <br/>
    /// ... ... <c>(Center.X &#177; dx, Center.Y &#177; dy)</c> <br/>
    /// ... And then calls the wrapped functor with these four points.
    /// </para>
    /// </summary>
    public struct QuadrantPixelSetter<TFunc>
        : IFuncInline<QuadrantPixelSetter<TFunc>, int, int, int>
        where TFunc : struct, IFuncInline<TFunc, int, int, int>
    {
        private readonly TFunc _func;
        private readonly int _centerX;
        private readonly int _centerY;

        /// <summary>
        /// Creates an instance with the specified wrapped function and the center point.
        /// 
        /// <para>
        /// Suggestion. It is more convenient to use the static creation methods provided by 
        /// <see cref="QuadrantPixelSetterFactory"/>, because these methods can infer the generic 
        /// type parameters, reducing code clutter.
        /// </para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="func"></param>
        public QuadrantPixelSetter(Point center, TFunc func)
        {
            _func = func;
            _centerX = center.X;
            _centerY = center.Y;
        }

        /// <summary>
        /// <para>
        /// Calls the wrapped functor with four generated points defined as: <br/>
        /// ... <c>(Center.X &#177; dx, Center.Y &#177; dy)</c>
        /// </para>
        /// 
        /// <para>
        /// This functor does not perform range checking, as it has no knowledge of the 
        /// target's coordinate bounds.
        /// </para>
        /// </summary>
        /// <param name="dx">
        /// Horizontal offset from the center point.
        /// </param>
        /// <param name="dy">
        /// Vertical offset from the center point.
        /// </param>
        /// <returns>
        /// Unspecified. The caller is expected to ignore the return value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Invoke(int dx, int dy)
        {
            var func = _func;
            func.Invoke(_centerX - dx, _centerY - dy);
            func.Invoke(_centerX + dx, _centerY - dy);
            func.Invoke(_centerX - dx, _centerY + dy);
            func.Invoke(_centerX + dx, _centerY + dy);
            return 0;
        }
    }
}
