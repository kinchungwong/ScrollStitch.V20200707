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
    public class Array2Wrapper<T>
        : IArray2<T>
    {
        public T[,] Target { get; }

        public int Length => Target.Length;

        public int Length0 => Target.GetLength(0);

        public int Length1 => Target.GetLength(1);

        public int GetLength(int dim) => Target.GetLength(dim);

        public T this[int idx0, int idx1]
        {
            get => Target[idx0, idx1];
            set => Target[idx0, idx1] = value;
        }

        public Array2Wrapper(T[,] target)
        {
            Target = target;
        }
    }
}
