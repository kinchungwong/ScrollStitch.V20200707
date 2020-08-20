using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="IRectMaskRelation"/> describes a binary relation between two rectangles,
    /// a test method using the two rectangles' coordinates, plus another faster but less specific test 
    /// via bitwise operations on bit-encoded masks derived from the two rectangles.
    /// 
    /// <para>
    /// This interface is an enhancement over <see cref="IRectRelation"/>.
    /// </para>
    /// 
    /// <para>
    /// Important. <br/>
    /// Some rectangle relations are not symmetric. That is, switching the order between the two may produce
    /// a different result. Therefore, always pay attention to the ordering of the two arguments.
    /// </para>
    /// </summary>
    /// 
    public interface IRectMaskRelation
        : IRectRelation
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
        /// 
        /// <param name="maskFirst">
        /// The bit mask that encodes spatial information related to the first rectangle.
        /// </param>
        /// 
        /// <param name="maskSecond">
        /// The bit mask that encodes spatial information related to the first rectangle.
        /// </param>
        /// 
        /// <returns>
        /// True if there is a possibility that the two rectangles may return true when 
        /// <see cref="IRectRelation.Test(Rect, Rect)"/> is called. When this function
        /// returns true, it should be followed up by a test using the actual rectangles.
        /// 
        /// False if there is no possibility that the two rectangles could ever return true when 
        /// <see cref="IRectRelation.Test(Rect, Rect)"/> is called. When this function
        /// returns false, it is not necessary to test any further.
        /// </returns>
        /// 
        bool TestMaybe(IRectMaskArith maskFirst, IRectMaskArith maskSecond);
    }

    /// <summary>
    /// <see cref="IRectMaskRelation{TRectMask}"/> describes a binary relation between two rectangles,
    /// a test method using the two rectangles' coordinates, plus another faster but less specific test 
    /// via bitwise operations on bit-encoded masks derived from the two rectangles.
    /// 
    /// <para>
    /// This interface is an enhancement over <see cref="IRectRelation"/>.
    /// </para>
    /// 
    /// <para>
    /// Important. <br/>
    /// Some rectangle relations are not symmetric. That is, switching the order between the two may produce
    /// a different result. Therefore, always pay attention to the ordering of the two arguments.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TRectMask">
    /// The bit-encoded mask type. Each mask encodes some spatial information about a rectangle that
    /// can be used to accelerate rectangular relation tests.
    /// </typeparam>
    /// 
    public interface IRectMaskRelation<TRectMask>
        : IRectRelation
        where TRectMask : struct, IRectMaskArith<TRectMask>
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
    }
}
