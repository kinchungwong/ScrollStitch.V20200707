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

        /// <summary>
        /// Performs the rect mask encompassing test. (See important usage notes.)
        /// 
        /// <para>
        /// Important usage note 1:
        /// <br/>
        /// This test is not symmetric.
        /// <br/>
        /// That is, the following expressions are NOT equivalent, and may give different results: <br/>
        /// <code>
        /// L001 ... RectMask first, second; <br/>
        /// L002 ... bool test12 = first.MaybeEncompassing(second); <br/>
        /// L003 ... bool test21 = second.MaybeEncompassing(first);
        /// </code>
        /// </para>
        /// 
        /// <para>
        /// Important usage note 2:
        /// <br/>
        /// Consider carefully whether to use the non-triviality version of this function.
        /// <br/>
        /// See <see cref="MaybeEncompassingNT"/> for details.
        /// </para>
        /// </summary>
        /// 
        /// <param name="other">
        /// A second instance of the same type.
        /// </param>
        /// 
        /// <returns>
        /// <para>
        /// True if, for each horizontal and vertical band occupied by the other instance, 
        /// those same bands are also occupied by the current instance.
        /// <br/>
        /// The caller should proceed to test the actual rectangle coordinates for encompassing.
        /// </para>
        /// 
        /// <para>
        /// False if there is at least one horizontal or vertical band occupied by the other instance
        /// which is not occupied by the current instance. 
        /// <br/>
        /// The caller will not need to perform the encompassing tests on the actual rectangle 
        /// coordinates because the current instance cannot possibly encompass the other instance.
        /// </para>
        /// </returns>
        /// 
        bool MaybeEncompassing(T other);

        /// <summary>
        /// The non-trivial ("NT") version of <see cref="MaybeEncompassing(T)"/>.
        /// 
        /// <para>
        /// The non-trivial version of bitwise test functions enforces the constraint that all bit masks
        /// must have at least one bit set horizontally and vertically, or else the test returns false.
        /// </para>
        /// 
        /// <para>
        /// Question: What is the significance of this non-triviality?
        /// <br/>
        /// Answer: Consider the following code. Without running the code, try predict its printout.
        /// <br/>
        /// (Pay attention to the negation before <c>maybeInter12</c>; the two masks obviously don't 
        /// have a non-trivial intersection.)
        /// </para>
        /// 
        /// <para>
        /// <code>
        /// L001 ... const uint something = 1u; <br/>
        /// L002 ... RectMask64 firstMask = new RectMask64(something, 32u); <br/>
        /// L003 ... RectMask64 secondMask = new RectMask64(something, 0u); <br/>
        /// L004 ... bool maybeInter12 = firstMask.MaybeIntersecting(secondMask); <br/>
        /// L005 ... bool maybeEncomp12 = firstMask.MaybeEncompassing(secondMask); <br/>
        /// L006 ... bool maybeEncompNT12 = firstMask.MaybeEncompassingNT(secondMask); <br/>
        /// L007 ... if (!maybeInter12 &amp;&amp; maybeEncomp12) <br/>
        /// L008 ... { <br/>
        /// L009 ... ... Console.WriteLine("Haha! Gotcha! (no NT)"); <br/>
        /// L010 ... } <br/>
        /// L011 ... if (!maybeInter12 &amp;&amp; maybeEncompNT12) <br/>
        /// L012 ... { <br/>
        /// L013 ... ... Console.WriteLine("Haha! Gotcha! (with NT)"); <br/>
        /// L014 ... }
        /// </code>
        /// </para>
        /// 
        /// <para>
        /// Interim Developer Note: 
        /// <br/>
        /// The current implementation of <see cref="MaybeIntersecting(T)"/> already satisfies non-triviality.
        /// <br/>
        /// It just happens that, during the implementation of <c>MaybeIntersecting</c>, the non-triviality
        /// implementation happens to produce smaller code, whereas in the implementation of 
        /// <c>MaybeEncompassing</c>, non-triviality requires more code to implement, which ultimately led to 
        /// a programmer's mistake by omission.
        /// </para>
        /// </summary>
        /// 
        bool MaybeEncompassingNT(T other);
    }
}
