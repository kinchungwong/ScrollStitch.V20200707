using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Functional
{
    /// <summary>
    /// A static facility that initializes compile-time constant-size arrays for use 
    /// within the <c>Hash2D.Functional</c> namespace.
    /// </summary>
    /// 
    public static class Hash2DVectorFactory
    {
        public const int Length = 1024;

        public static uint[] Create() => NoInline.Create();

        public static void Fill(uint[] buffer, uint value) => NoInline.Fill(buffer, value);

        public static bool IsValid(uint[] arr) => Inline.IsValid(arr);

        public static void CopyTo(ArraySegment<int> source, uint[] buffer) => NoInline.CopyTo(source, buffer);

        public static void CopyTo(uint[] buffer, ArraySegment<int> dest) => NoInline.CopyTo(buffer, dest);

        public static void ValidateElseThrow(uint[] arr) => Inline.ValidateElseThrow(arr);

        public static void ValidateElseThrow(int[] arr, int offset, int count) => Inline.ValidateElseThrow(arr, offset, count);

        public static class Inline
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint[] Create()
            {
                return new uint[Length];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Fill(uint[] buffer, uint value)
            {
                ValidateElseThrow(buffer);
                for (int k = 0; k < Length; ++k)
                {
                    buffer[k] = value;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsValid(uint[] arr)
            {
                return (arr?.Length ?? 0) == Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CopyTo(ArraySegment<int> source, uint[] buffer)
            {
                ValidateElseThrow(buffer);
                var _source = source;
                int[] sourceArray = _source.Array;
                int sourceOffset = _source.Offset;
                int sourceCount = _source.Count;
                ValidateElseThrow(sourceArray, sourceOffset, sourceCount);
                int copyCount = Math.Min(sourceCount, Length);
#if true
                int bytesToCopy = copyCount * sizeof(int);
                Buffer.BlockCopy(sourceArray, sourceOffset, buffer, 0, bytesToCopy);
#else
            for (int k = 0; k < copyCount; ++k)
            {
                int value = sourceArray[sourceOffset + k];
                buffer[k] = unchecked((uint)value);
            }
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CopyTo(uint[] buffer, ArraySegment<int> dest)
            {
                ValidateElseThrow(buffer);
                var _dest = dest;
                int[] destArray = _dest.Array;
                int destOffset = _dest.Offset;
                int destCount = _dest.Count;
                ValidateElseThrow(destArray, destOffset, destCount);
                int copyCount = Math.Min(destCount, Length);
#if true
                int bytesToCopy = copyCount * sizeof(int);
                Buffer.BlockCopy(buffer, 0, destArray, destOffset, bytesToCopy);
#else
            for (int k = 0; k < copyCount; ++k)
            {
                uint value = buffer[k];
                destArray[destOffset + k] = unchecked((int)value);
            }
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ValidateElseThrow(uint[] arr)
            {
                if (arr is null)
                {
                    NoInline.Throw();
                }
                if (!IsValid(arr))
                {
                    NoInline.Throw();
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ValidateElseThrow(int[] arr, int offset, int count)
            {
                if (arr is null)
                {
                    NoInline.Throw();
                }
                int length = arr.Length;
                if (offset < 0 || offset > length)
                {
                    NoInline.Throw();
                }
                if (count < 0)
                {
                    NoInline.Throw();
                }
                int stop = unchecked(offset + count);
                if (stop < offset ||
                    stop > length)
                {
                    NoInline.Throw();
                }
            }
        }

        /// <summary>
        /// Provides the option of calling a function that is not inlined.
        /// 
        /// <para>
        /// Intended usage. <br/>
        /// This is for performance investigations only. <br/>
        /// This can be used for comparing the costs and benefits of inlining particular functions. <br/>
        /// This can also be used in interactive debugging, where breakpoints can be set inside the 
        /// non-inline function entry point, and also the disassembly is easier for human understanding
        /// due to clear delineation.
        /// </para>
        /// </summary>
        public static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static uint[] Create()
            {
                return Inline.Create();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void Fill(uint[] buffer, uint value)
            {
                Inline.Fill(buffer, value);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void CopyTo(ArraySegment<int> source, uint[] buffer)
            {
                Inline.CopyTo(source, buffer);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void CopyTo(uint[] buffer, ArraySegment<int> dest)
            {
                Inline.CopyTo(buffer, dest);
            }
            
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void Throw()
            {
                throw new Exception();
            }
        }
    }
}
