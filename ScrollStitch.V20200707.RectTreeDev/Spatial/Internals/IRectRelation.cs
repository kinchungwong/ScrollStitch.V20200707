﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="IRectRelation"/> (non-generic, zero arity) describes a binary relation between two 
    /// rectangle, and a test method that takes two rectangles.
    /// 
    /// <para>
    /// Important. <br/>
    /// Some rectangle relations are not symmetric. That is, switching the order between the two may produce
    /// a different result. Therefore, always pay attention to the ordering of the two arguments.
    /// </para>
    /// 
    /// <para>
    /// Some examples of non-symmetric rectangle relations are: <c>Encompassing</c>, <c>EncompassedBy</c>.
    /// </para>
    /// </summary>
    public interface IRectRelation
    {
        bool Test(Rect rectFirst, Rect rectSecond);
    }

    /// <summary>
    /// <see cref="IRectRelation{TStruct, TRectMask}"/> describes a binary relation between two rectangles,
    /// a test method using the two rectangles' coordinates, plus another faster but less specific test 
    /// via bitwise comparison.
    /// 
    /// <para>
    /// Important. <br/>
    /// Some rectangle relations are not symmetric. That is, switching the order between the two may produce
    /// a different result. Therefore, always pay attention to the ordering of the two arguments.
    /// </para>
    /// </summary>
    /// <typeparam name="TStruct">
    /// </typeparam>
    /// 
    /// <typeparam name="TRectMask">
    /// </typeparam>
    /// 
    public interface IRectMaskRelation<TStruct, TRectMask>
        : IRectRelation
        where TStruct: struct, IRectMaskRelation<TStruct, TRectMask>
        where TRectMask: struct, IRectMaskArith<TRectMask>
    {
        /// <summary>
        /// <c>TestMaybe()</c> performs a bit mask test that predicts a possibility that the actual test 
        /// (<c>Test()</c>) may return true.
        /// 
        /// <para>
        /// This function can produce false positives, which need to be followed up with a call to 
        /// <see cref="Test(Rect, Rect)"/> in order to reject them.
        /// </para>
        /// 
        /// <para>
        /// This function cannot produce false negatives. In other words, if <c>TestMaybe()</c> returns false, 
        /// it is not necessary to call <see cref="Test(Rect, Rect)"/>.
        /// </para>
        /// </summary>
        /// <param name="maskFirst"></param>
        /// <param name="maskSecond"></param>
        /// <returns></returns>
        /// 
        bool TestMaybe(TRectMask maskFirst, TRectMask maskSecond);

#if false
        // methods defined on interface IRectRelation
        // bool Test(Rect rectFirst, Rect rectSecond); /*redundant*/
#endif
    }
}
