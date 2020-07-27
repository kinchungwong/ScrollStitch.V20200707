using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    /// <summary>
    /// Provides commonly used basic functionality on arrays.
    /// 
    /// <para>
    /// Some of these functions are backward shims for built-in functions that are available on 
    /// newer versions of .NET runtime. Refer to documentation on each method for details.
    /// </para>
    /// </summary>
    public static class BuiltinArrayMethods
    {
        public static class Inline
        {
            // ====== .NET Shim ======
            //
            // ======

            /// <summary>
            /// DotNet shim for <see cref="System.Array.Fill"/>.
            /// 
            /// <para>
            /// Remark. <see cref="System.Array.Fill"/> is available in: <br/>
            /// ... .NET 5.0 <br/>
            /// ... .NET Core 2.0 <br/>
            /// ... .NET Standard 2.1
            /// </para>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <param name="value"></param>
            /// <param name="startIndex"></param>
            /// <param name="count"></param>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ArrayFill<T>(T[] array, T value, int startIndex, int count)
            {
#if false
                System.Array.Fill(array, value, startIndex, count);
#else
                if (array is null) NoInline.Throw();
                if (count < 0) NoInline.Throw();
                if (count == 0) return;
                if (startIndex < 0) NoInline.Throw();
                int length = array.Length;
                if (count > length) NoInline.Throw();
                if (startIndex > length) NoInline.Throw();
                int stopIndex = checked(startIndex + count);
                if (stopIndex > length) NoInline.Throw();
                for (int k = stopIndex - 1; k >= startIndex; --k)
                {
                    array[k] = value;
                }
#endif
            }
        }

        public static class NoInline
        {
            /// <inheritdoc cref="Inline.ArrayFill{T}(T[], T, int, int)"/>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void ArrayFill<T>(T[] array, T value, int startIndex, int count)
            {
                Inline.ArrayFill(array, value, startIndex, count);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void Throw()
            {
                throw new Exception();
            }
        }
    }
}
