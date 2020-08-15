using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    /// <summary>
    /// <see cref="IRectMaskArith"/>, the non-generic interface, is a tag interface (which is empty 
    /// by definition) for <see cref="IRectMaskArith{T}"/>, the generic interface.
    /// 
    /// <para>
    /// It is not necessary for concrete implementations to explicitly inherit the non-generic tag 
    /// interface, as it is already implicitly implemented through the generic one.
    /// </para>
    /// </summary>
    public interface IRectMaskArith
    {
        // empty by definition
    }

    /// <summary>
    /// <see cref="IRectMaskArith{T}"/> describes a spatial bit mask used for accelerated rectangular overlap 
    /// testing between rectangles.
    /// 
    /// <para>
    /// Such spatial bit masks are typically used in collection classes that implement <see cref="IRectQuery{T}"/>.
    /// </para>
    /// 
    /// <para>
    /// The X-axis and Y-axis is subdivided into some bands of interest. Each band on each axis is assigned a bit. 
    /// This allows fast overlap checking for two rectangles via bit testing. The <see cref="MaybeIntersecting(T)"/> 
    /// function tests whether overlapping bits are set on at least one horizontal band and at least one vertical band.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The concrete implementation of bit mask. 
    /// 
    /// <para>
    /// This type must be a struct, which is enforced with the self-referencing constraint.
    /// </para>
    /// 
    /// <para>
    /// It is highly recommended that the <see cref="MaybeIntersecting(T)"/> method on concrete implementation be 
    /// marked with <see cref="MethodImplOptions.AggressiveInlining"/>.
    /// </para>
    /// </typeparam>
    /// 
    public interface IRectMaskArith<T>
        : IRectMaskArith
        where T : struct, IRectMaskArith<T>
    {
        /// <summary>
        /// Performs the rect mask intersection test.
        /// </summary>
        /// 
        /// <param name="other">
        /// A second instance of the same type.
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
        bool MaybeIntersecting(T other);
    }
}
