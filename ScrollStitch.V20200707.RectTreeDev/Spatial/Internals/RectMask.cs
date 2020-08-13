using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    /// <summary>
    /// <see cref="RectMask8"/> stores an 8-bit mask with lower 4 bits for x-axis and 
    /// upper 4 bits for y-axis.
    /// </summary>
    public struct RectMask8 
        : IRectMaskArith<RectMask8>
    {
        /// <summary>
        /// An 8-bit mask with lower 4 bits for x-axis and upper 4 bits for y-axis.
        /// </summary>
        public byte XYValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask8(byte xyvalue)
        {
            XYValue = xyvalue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask8(byte xvalue, byte yvalue)
        {
            XYValue = unchecked((byte)(((uint)yvalue << 4) | (xvalue & 0x0Fu)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(RectMask8 other)
        {
            uint xyandxy = (uint)XYValue & other.XYValue;
            uint xandx = xyandxy & 0x0Fu;
            uint yandy = (xyandxy >> 4) & 0x0Fu;
            // TOOD the following can be further optimized to reduce number of branches.
            return (xandx != 0u) && (yandy != 0u);
        }
    }

    /// <summary>
    /// <see cref="RectMask16"/> stores a 16-bit mask with 8 bits for x-axis and 8 bits for y-axis.
    /// </summary>
    public struct RectMask16 
        : IRectMaskArith<RectMask16>
    {
        public byte XValue { get; }

        public byte YValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask16(byte xvalue, byte yvalue)
        {
            XValue = xvalue;
            YValue = yvalue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(RectMask16 other)
        {
            uint xandx = (uint)XValue & other.XValue;
            uint yandy = (uint)YValue & other.YValue;
            // TOOD the following can be further optimized to reduce number of branches.
            return (xandx != 0u) && (yandy != 0u);
        }
    }

    /// <summary>
    /// <see cref="RectMask32"/> stores a 32-bit mask with 16 bits for x-axis and 16 bits for y-axis.
    /// </summary>
    public struct RectMask32 
        : IRectMaskArith<RectMask32>
    {
        public ushort XValue { get; }

        public ushort YValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask32(ushort xvalue, ushort yvalue)
        {
            XValue = xvalue;
            YValue = yvalue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(RectMask32 other)
        {
            uint xandx = (uint)XValue & other.XValue;
            uint yandy = (uint)YValue & other.YValue;
            // TOOD the following can be further optimized to reduce number of branches.
            return (xandx != 0u) && (yandy != 0u);
        }
    }

    /// <summary>
    /// <see cref="RectMask64"/> stores a 64-bit mask with 32 bits for x-axis and 32 bits for y-axis.
    /// </summary>
    public struct RectMask64 
        : IRectMaskArith<RectMask64>
    {
        public uint XValue { get; }

        public uint YValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask64(uint xvalue, uint yvalue)
        {
            XValue = xvalue;
            YValue = yvalue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(RectMask64 other)
        {
            uint xandx = XValue & other.XValue;
            uint yandy = YValue & other.YValue;
            // TOOD the following can be further optimized to reduce number of branches.
            return (xandx != 0u) && (yandy != 0u);
        }
    }

    /// <summary>
    /// <see cref="RectMask128"/> stores a 128-bit mask with 64 bits for x-axis and 64 bits for y-axis.
    /// </summary>
    public struct RectMask128 
        : IRectMaskArith<RectMask128>
    {
        public ulong XValue { get; }

        public ulong YValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMask128(ulong xvalue, ulong yvalue)
        {
            XValue = xvalue;
            YValue = yvalue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(RectMask128 other)
        {
            ulong xandx = XValue & other.XValue;
            ulong yandy = YValue & other.YValue;
            // TOOD the following can be further optimized to reduce number of branches.
            return (xandx != 0uL) && (yandy != 0uL);
        }
    }
}
