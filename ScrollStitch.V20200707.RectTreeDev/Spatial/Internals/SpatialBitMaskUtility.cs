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
    }
}
