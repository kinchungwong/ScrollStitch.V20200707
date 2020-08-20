using System;
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
}
