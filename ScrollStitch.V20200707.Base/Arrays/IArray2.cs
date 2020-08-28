using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ======
// TODO
// This class has not yet been checked for correctness or usefulness.
// ======

namespace ScrollStitch.V20200707.Arrays
{
    public interface IArray2<T>
        : IArray2Info
    {
        T this[int idx0, int idx1] 
        { 
            get; 
            set; 
        }
    }
}
