using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Specialized
{
    using HashCode;

    /// <summary>
    /// Provides very low level array methods that are needed by the 
    /// <see cref="ScrollStitch.V20200707.Imaging"/> namespace yet aren't generally useful 
    /// outside this namespace.
    /// 
    /// <para>
    /// Some of these methods are debugging aids or tools used for algorithm research and/or 
    /// runtime performance investigations.
    /// </para>
    /// </summary>
    public static class SpecializedArrayMethods
    {
        public static class Inline
        {
            /// <summary>
            /// Fills the target array with deterministic pseudorandom values.
            /// 
            /// <para>
            /// Intended usage.
            /// <br/>
            /// This method exists as a debugging aid. This method should not be used in production 
            /// code. 
            /// <br/>
            /// It is very computationally intensive compared to the <see cref="System.Array.Fill"/> 
            /// method, which fills the array uniformly with a given value.
            /// </para>
            /// 
            /// <para>
            /// Inlined versus non-inlined versions of the method.
            /// <br/>
            /// <see cref="SpecializedArrayMethods.Inline.ArrayFillNoise(int[], int, int, int)"/> 
            /// is marked with <see cref="MethodImplOptions.AggressiveInlining"/>.
            /// <br/>
            /// <see cref="SpecializedArrayMethods.NoInline.ArrayFillNoise(int[], int, int, int)"/> 
            /// is marked with <see cref="MethodImplOptions.NoInlining"/>.
            /// </para>
            /// 
            /// <para>
            /// Interactive debugging.
            /// <br/>
            /// In order to set a breakpoint, the calling function must call the non-inlining method,
            /// and the breakpoint should be set at the first line of the non-inlining method.
            /// </para>
            /// 
            /// <para>
            /// Performance profiling.
            /// <br/>
            /// If it is deemed advantageous to be able to accurately account for CPU cycles spent on
            /// this method, the caller should call the non-inlining method.
            /// </para>
            /// 
            /// <para>
            /// Important implementation notes.
            /// <br/>
            /// The implementation needs to be "reasonably deterministic" in that two executions 
            /// of the test application back-to-back need to reproduce the same output.
            /// <br/>
            /// Thus, this method tries to leverage as much deterministic sources of information 
            /// as possible, including the input parameters to this function, and the target 
            /// array's pre-existing data.
            /// </para>
            /// 
            /// </summary>
            /// 
            /// <param name="array"></param>
            /// <param name="seed"></param>
            /// <param name="startIndex"></param>
            /// <param name="count"></param>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ArrayFillNoise(int[] array, int seed, int startIndex, int count)
            {
                if (array is null) Private.Throw();
                if (count < 0) Private.Throw();
                if (count == 0) return;
                if (startIndex < 0) Private.Throw();
                int length = array.Length;
                if (count > length) Private.Throw();
                if (startIndex > length) Private.Throw();
                int stopIndex = checked(startIndex + count);
                if (stopIndex > length) Private.Throw();
                HashCodeBuilder hcb = HashCodeBuilder.ForType<int>();
                hcb.Ingest(array.Length, startIndex, count, seed);
                for (int k = stopIndex - 1; k >= startIndex; --k)
                {
                    hcb.Ingest(array[k]);
                }
                for (int k = stopIndex - 1; k >= startIndex; --k)
                {
                    hcb.Ingest(k);
                    int temp = hcb.GetHashCode();
                    int temp2 = unchecked((int)(((uint)temp & 0x00010101u) * 255u));
                    array[k] = temp2;
                }
            }
        }

        public static class NoInline
        {
            /// <inheritdoc cref="Inline.ArrayFillNoise(int[], int, int, int)"/>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void ArrayFillNoise(int[] array, int seed, int startIndex, int count)
            {
                Inline.ArrayFillNoise(array, seed, startIndex, count);
            }
        }

        private static class Private
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void Throw()
            {
                throw new Exception();
            }
        }
    }
}
