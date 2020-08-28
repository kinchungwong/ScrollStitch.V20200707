using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    public interface IReadOnlyArray2<T>
        : IArray2Info
    {
        T this[int idx0, int idx1] 
        { 
            get; 
        }
    }
}
