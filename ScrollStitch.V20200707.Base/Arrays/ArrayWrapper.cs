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
    public class ArrayWrapper<T>
        : IArray<T>
    {
        public T[] Target { get; }

        public int Length => Target.Length;

        public int GetLength(int dim) => Target.GetLength(dim);

        public T this[int idx] 
        {
            get => Target[idx];
            set => Target[idx] = value;
        }

        public ArrayWrapper(T[] target)
        {
            Target = target;
        }
    }
}
