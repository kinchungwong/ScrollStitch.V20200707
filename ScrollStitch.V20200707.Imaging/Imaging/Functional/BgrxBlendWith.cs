using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// ======
// TODO
// This class has not yet been checked for correctness or usefulness.
// ======

namespace ScrollStitch.V20200707.Imaging.Functional
{
    using ScrollStitch.V20200707.Functional;

    public struct BgrxBlendWith
        : IFuncInline<BgrxBlendWith, int, int, int>
    {
        private const int _denomBits = 16;
        private const int _halfDenomBits = _denomBits - 1;
        private const int _denom = (1 << _denomBits);
        private const int _halfDenom = (1 << _halfDenomBits);
        private int _nume;
        
        public int Percent
        {
            get
            {
                return (_nume * 100 + _halfDenom) >> _denomBits;
            }
            set
            {
                _nume = (value * _denom + 50) / 100;
            }
        }

        public double Frac
        {
            get
            {
                return (double)_nume / _denom;
            }
            set
            {
                _nume = (int)Math.Round(value * _denom / 100.0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Invoke(int bgrx0, int bgrx1)
        {
            _Decompose(bgrx0, out byte r0, out byte g0, out byte b0);
            _Decompose(bgrx1, out byte r1, out byte g1, out byte b1);
            int r = _Blend(r0, r1);
            int g = _Blend(g0, g1);
            int b = _Blend(b0, b1);
            return _ClampAndCompose(r, g, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _Decompose(int bgrxValue, out byte red, out byte green, out byte blue)
        {
            unchecked
            {
                red = (byte)((bgrxValue >> 16) & 255);
                green = (byte)((bgrxValue >> 8) & 255);
                blue = (byte)(bgrxValue & 255);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _Compose(byte red, byte green, byte blue)
        {
            return blue | (green << 8) | (red << 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _ClampAndCompose(int red, int green, int blue)
        {
            return _Compose(_Clamp(red), _Clamp(green), _Clamp(blue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte _Clamp(int value)
        {
            return (value > 255) ? (byte)255 : (value < 0) ? (byte)0 : unchecked((byte)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int _Blend(int v0, int v1)
        {
            return v0 + ((v1 - v0) * _nume + 0) >> _denomBits;
        }
    }
}
