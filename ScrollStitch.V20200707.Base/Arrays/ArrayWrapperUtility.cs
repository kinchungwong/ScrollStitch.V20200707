using ScrollStitch.V20200707.Arrays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ======
// TODO
// This class has not yet been checked for correctness or usefulness.
// ======

namespace ScrollStitch.V20200707.Arrays
{
    public static class ArrayWrapperUtility
    {
        public static IArray<T> Wrap<T>(T[] a)
        {
            return new ArrayWrapper<T>(a);
        }

        public static IArray2<T> Wrap<T>(T[,] a)
        {
            return new Array2Wrapper<T>(a);
        }

        public static IArray<T> AsArray<T>(this IArray2<T> a2, bool transposed)
        {
            return new ArrayDimAdapter_2as1<T>(a2, transposed);
        }
    }
}
