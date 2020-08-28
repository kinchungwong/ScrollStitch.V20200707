using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Bitwise
{
    public static class BitwiseUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Add(int a, int b)
        {
            return unchecked(a + b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Add(uint a, uint b)
        {
            return unchecked(a + b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Add(long a, long b)
        {
            return unchecked(a + b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Add(ulong a, ulong b)
        {
            return unchecked(a + b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Xor(int a, int b)
        {
            return a ^ b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Xor(uint a, uint b)
        {
            return a ^ b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Xor(long a, long b)
        {
            return a ^ b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Xor(ulong a, ulong b)
        {
            return a ^ b;
        }

#if false
        public static UInt128 Xor(UInt128 a, UInt128 b)
        {
            return UInt128.Xor(a, b);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rotate(int input, int amount)
        {
            uint uinput = unchecked((uint)input);
            uint uoutput = Rotate(uinput, amount);
            return unchecked((int)uoutput);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Rotate(uint input, int amount)
        {
            unchecked
            {
                uint leftAmount = unchecked((uint)amount) & 31u;
                if (leftAmount == 0u)
                {
                    return input;
                }
                uint rightAmount = 32u - leftAmount;
                return (input << (int)leftAmount) | (input >> (int)rightAmount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Rotate(long input, int amount)
        {
            ulong uinput = unchecked((ulong)input);
            ulong uoutput = Rotate(uinput, amount);
            return unchecked((long)uoutput);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Rotate(ulong input, int amount)
        {
            unchecked
            {
                uint leftAmount = unchecked((uint)amount) & 63u;
                if (leftAmount == 0u)
                {
                    return input;
                }
                uint rightAmount = 64u - leftAmount;
                return (input << (int)leftAmount) | (input >> (int)rightAmount);
            }
        }

#if false
        public static UInt128 Rotate(UInt128 input, int amount)
        {
            return UInt128.Rotate(input, amount);
        }
#endif
    }
}
