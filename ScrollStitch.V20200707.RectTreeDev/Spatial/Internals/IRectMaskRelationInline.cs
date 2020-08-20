using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="IRectMaskRelationInline{TStruct, TRectMask}"/> is a constrain on top of 
    /// <see cref="IRectMaskRelation{TRectMask}"/> that forces the concrete implementation to be a struct.
    /// 
    /// <para>
    /// This constraint allows inter-procedural optimization between the consumer and the implementer
    /// of this interface. In particular, the compiler can generate code such that calls to the 
    /// <see cref="TestMaybe(TRectMask, TRectMask)"/> method is fully inlined.
    /// </para>
    /// 
    /// <para>
    /// Important. <br/>
    /// Some rectangle relations are not symmetric. That is, switching the order between the two may produce
    /// a different result. Therefore, always pay attention to the ordering of the two arguments.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TStruct">
    /// The type of the concrete implementation of this interface. This self-reference is what enforces 
    /// the constraints that are imposed on the concrete implementation.
    /// </typeparam>
    /// 
    /// <typeparam name="TRectMask">
    /// The bit-encoded mask type. Each mask encodes some spatial information about a rectangle that
    /// can be used to accelerate rectangular relation tests.
    /// </typeparam>
    /// 
    public interface IRectMaskRelationInline<TStruct, TRectMask>
        : IRectMaskRelation<TRectMask>
        where TStruct : struct, IRectMaskRelationInline<TStruct, TRectMask>
        where TRectMask : struct, IRectMaskArith<TRectMask>
    {
#if false
        // methods defined on interface IRectMaskRelation<TRectMask>
        bool TestMaybe(TRectMask maskFirst, TRectMask maskSecond); /*redundant*/
#endif

#if false
        // methods defined on interface IRectRelation
        bool Test(Rect rectFirst, Rect rectSecond); /*redundant*/
#endif
    }
}
