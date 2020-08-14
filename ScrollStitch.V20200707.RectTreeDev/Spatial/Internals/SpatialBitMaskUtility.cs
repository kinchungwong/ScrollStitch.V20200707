using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    /// <summary>
    /// <see cref="SpatialBitMaskUtility"/> contains static bit mask manipulation functions
    /// used in <see cref="RectMaskUtility"/>.
    /// 
    /// <para>
    /// This is a static utility class.
    /// </para>
    /// </summary>
    public static class SpatialBitMaskUtility
    {
        /// <summary>
        /// Given a bit mask with exactly one bit set, this function returns a 32-bit mask where 
        /// the input bit and all bits above are set.
        /// 
        /// <para>
        /// This function assumes its input is validated, that is, it contains exactly one bit set.
        /// </para>
        /// </summary>
        /// 
        /// <param name="singleFlag">
        /// A 32-bit mask where exactly one bit is set.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit mask where the input bit and all bits above are set.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SetAllBitsAbove(ulong singleFlag)
        {
            unchecked 
            {
                return ~(singleFlag - 1uL);
            }
        }

        /// <summary>
        /// Given a bit mask with exactly one bit set, this function returns a 32-bit mask where 
        /// the input bit and all bits below are set.
        /// 
        /// <para>
        /// This function assumes its input is validated, that is, it contains exactly one bit set.
        /// </para>
        /// </summary>
        /// 
        /// <param name="singleFlag">
        /// A 32-bit mask where exactly one bit is set.
        /// </param>
        /// 
        /// <returns>
        /// A 32-bit mask where the input bit and all bits below are set.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SetAllBitsBelow(ulong singleFlag)
        {
            unchecked 
            {
                return singleFlag | (singleFlag - 1uL);
            }
        }


        /// <summary>
        /// Tests whether any of the signed integer arguments are negative.
        /// 
        /// <para>
        /// This method works by testing for the sign bit. Moreover, it combines the sign bits 
        /// of all signed integer arguments together with bitwise-or, and then performs the 
        /// sign test only once. 
        /// </para>
        /// 
        /// <para>
        /// This method should only be used in performance critical functions containing 
        /// lots of integer validations inside <see cref="RectMaskUtility"/>. 
        /// <br/>
        /// The main drawback is that the caller will not know which of the signed integer 
        /// arguments are negative, and therefore unable to generate a meaningful exception or 
        /// error message.
        /// <br/>
        /// Typically, this method is used within a context where all integer arguments have 
        /// already been validated elsewhere. Thus, this function serves as a safety net for
        /// a very unlikely outcome.
        /// </para>
        /// </summary>
        /// <returns>
        /// True if one or more arguments are negative.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1)
        {
            return arg1 < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2)
        {
            return (arg1 | arg2) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3)
        {
            return (arg1 | arg2 | arg3) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4)
        {
            return (arg1 | arg2 | arg3 | arg4) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4, 
            int arg5)
        {
            return (arg1 | arg2 | arg3 | arg4 | arg5) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4, 
            int arg5, int arg6)
        {
            return (arg1 | arg2 | arg3 | arg4 | arg5 | arg6) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4, 
            int arg5, int arg6, int arg7)
        {
            return (arg1 | arg2 | arg3 | arg4 | arg5 | arg6 | arg7) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4, 
            int arg5, int arg6, int arg7, int arg8)
        {
            return (arg1 | arg2 | arg3 | arg4 | arg5 | arg6 | arg7 | arg8) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(int arg1, int arg2, int arg3, int arg4, 
            int arg5, int arg6, int arg7, int arg8, int arg9)
        {
            return (arg1 | arg2 | arg3 | arg4 | arg5 | arg6 | arg7 | arg8 | arg9) < 0;
        }

        /// <inheritdoc cref="FastNegativeTest(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastNegativeTest(params int[] args)
        {
            int combined = 0;
            foreach (int arg in args)
            {
                combined |= arg;
            }
            return combined < 0;
        }
    }
}
