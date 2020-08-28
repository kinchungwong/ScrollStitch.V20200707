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
    public interface IArray<T>
    {
        int Length { get; }

        int GetLength(int dim);

        T this[int idx0] { get; set; }
    }
}
