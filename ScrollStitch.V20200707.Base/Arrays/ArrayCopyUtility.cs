using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    public static class ArrayCopyUtility
    {
        public static void Copy<T>(T[] source, T[] dest)
        {
            Copy(ArrayWrapperUtility.Wrap(source), ArrayWrapperUtility.Wrap(dest));
        }

        public static void Copy<T>(IArray<T> source, T[] dest)
        {
            Copy(source, ArrayWrapperUtility.Wrap(dest));
        }

        public static void Copy<T>(T[] source, IArray<T> dest)
        {
            Copy(ArrayWrapperUtility.Wrap(source), dest);
        }

        public static void Copy<T>(IArray<T> source, IArray<T> dest)
        {
            _SameSizeElseThrow(source, dest);
            int len = source.Length;
            for (int k = 0; k < len; ++k)
            {
                dest[k] = source[k];
            }
        }

        public static void Convert<T1, T2>(T1[] source, T2[] dest, Func<T1, T2> func)
        {
            Convert(ArrayWrapperUtility.Wrap(source), ArrayWrapperUtility.Wrap(dest), func);
        }

        public static void Convert<T1, T2>(IArray<T1> source, T2[] dest, Func<T1, T2> func)
        {
            Convert(source, ArrayWrapperUtility.Wrap(dest), func);
        }

        public static void Convert<T1, T2>(T1[] source, IArray<T2> dest, Func<T1, T2> func)
        {
            Convert(ArrayWrapperUtility.Wrap(source), dest, func);
        }

        public static void Convert<T1, T2>(IArray<T1> source, IArray<T2> dest, Func<T1, T2> func)
        {
            _SameSizeElseThrow(source, dest);
            int len = source.Length;
            for (int k = 0; k < len; ++k)
            {
                dest[k] = func(source[k]);
            }
        }

        public static void Copy<T>(T[,] source, T[,] dest)
        {
            Copy(ArrayWrapperUtility.Wrap(source), ArrayWrapperUtility.Wrap(dest));
        }

        public static void Copy<T>(IArray2<T> source, T[,] dest)
        {
            Copy(source, ArrayWrapperUtility.Wrap(dest));
        }

        public static void Copy<T>(T[,] source, IArray2<T> dest)
        {
            Copy(ArrayWrapperUtility.Wrap(source), dest);
        }

        public static void Copy<T>(IArray2<T> source, IArray2<T> dest)
        {
            _SameSizeElseThrow(source, dest);
            int len0 = source.Length0;
            int len1 = source.Length1;
            for (int idx0 = 0; idx0 < len0; ++idx0)
            {
                for (int idx1 = 0; idx1 < len1; ++idx1)
                {
                    dest[idx0, idx1] = source[idx0, idx1];
                }
            }
        }

        public static void Convert<T1, T2>(T1[,] source, T2[,] dest, Func<T1, T2> func)
        {
            Convert(ArrayWrapperUtility.Wrap(source), ArrayWrapperUtility.Wrap(dest), func);
        }

        public static void Convert<T1, T2>(IArray2<T1> source, T2[,] dest, Func<T1, T2> func)
        {
            Convert(source, ArrayWrapperUtility.Wrap(dest), func);
        }

        public static void Convert<T1, T2>(T1[,] source, IArray2<T2> dest, Func<T1, T2> func)
        {
            Convert(ArrayWrapperUtility.Wrap(source), dest, func);
        }

        public static void Convert<T1, T2>(IArray2<T1> source, IArray2<T2> dest, Func<T1, T2> func)
        {
            _SameSizeElseThrow(source, dest);
            int len0 = source.Length0;
            int len1 = source.Length1;
            for (int idx0 = 0; idx0 < len0; ++idx0)
            {
                for (int idx1 = 0; idx1 < len1; ++idx1)
                {
                    dest[idx0, idx1] = func(source[idx0, idx1]);
                }
            }
        }

        private static void _SameSizeElseThrow<T1, T2>(IArray<T1> source, IArray<T2> dest)
        {
            _NotNull(source);
            _NotNull(dest);
            int len = source.Length;
            if (dest.Length != len)
            {
                throw new ArgumentException();
            }
        }

        private static void _SameSizeElseThrow<T1, T2>(IArray2<T1> source, IArray2<T2> dest)
        {
            _NotNull(source);
            _NotNull(dest);
            int len0 = source.Length0;
            int len1 = source.Length1;
            if (dest.Length0 != len0 ||
                dest.Length1 != len1)
            {
                throw new ArgumentException();
            }
        }

        private static void _NotNull<T>(T t)
            where T : class
        {
            if (t is null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
