using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    public static class ColorUtility
    {
        public struct ColorInt
        {
            public int Value { get; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColorInt(int value)
            {
                Value = value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColorInt(ByteRgbx u8x4)
            {
                Value = unchecked((int)(
                    u8x4.B | 
                    ((uint)u8x4.G << 8) | 
                    ((uint)u8x4.R << 16) | 
                    ((uint)u8x4.X << 24)));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ColorInt(ByteRgb u8x3)
            {
                Value = unchecked((int)(
                    u8x3.B |
                    ((uint)u8x3.G << 8) |
                    ((uint)u8x3.R << 16)));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgbx ToByteRgbx()
            {
                byte r, g, b, x;
                unchecked
                {
                    uint u = (uint)Value;
                    x = (byte)((u >> 24) & 255u);
                    r = (byte)((u >> 16) & 255u);
                    g = (byte)((u >> 8) & 255u);
                    b = (byte)(u & 255u);
                }
                return new ByteRgbx(r, g, b, x);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgb ToByteRgb()
            {
                byte r, g, b;
                unchecked
                {
                    uint u = (uint)Value;
                    r = (byte)((u >> 16) & 255u);
                    g = (byte)((u >> 8) & 255u);
                    b = (byte)(u & 255u);
                }
                return new ByteRgb(r, g, b);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgbx ToIntRgbx()
            {
                (byte r, byte g, byte b, byte x) = ToByteRgbx();
                return new IntRgbx(r, g, b, x);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgb ToIntRgb()
            {
                (byte r, byte g, byte b) = ToByteRgb();
                return new IntRgb(r, g, b);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FloatRgbx ToFloatRgbx()
            {
                (byte r, byte g, byte b, byte x) = ToByteRgbx();
                return new FloatRgbx(r, g, b, x);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FloatRgb ToFloatRgb()
            {
                (byte r, byte g, byte b) = ToByteRgb();
                return new FloatRgb(r, g, b);
            }
        }

        public struct ByteRgb
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgb(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out byte r, out byte g, out byte b)
            {
                r = R;
                g = G;
                b = B;
            }
        }

        public struct ByteRgbx
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte X { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgbx(byte r, byte g, byte b, byte x)
            {
                R = r;
                G = g;
                B = b;
                X = x;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out byte r, out byte g, out byte b)
            {
                r = R;
                g = G;
                b = B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out byte r, out byte g, out byte b, out byte x)
            {
                r = R;
                g = G;
                b = B;
                x = X;
            }
        }

        public struct IntRgb
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgb(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out int r, out int g, out int b)
            {
                r = R;
                g = G;
                b = B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgb ToByteRgb()
            {
                return new ByteRgb(_Cast(R), _Cast(G), _Cast(B));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static byte _Cast(int value)
            {
                return unchecked(
                    (value > byte.MaxValue) ? byte.MaxValue : 
                    (value < byte.MinValue) ? byte.MinValue : 
                    (byte)value);
            }
        }

        public struct IntRgbx
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
            public int X { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgbx(int r, int g, int b, int x)
            {
                R = r;
                G = g;
                B = b;
                X = x;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out int r, out int g, out int b)
            {
                r = R;
                g = G;
                b = B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out int r, out int g, out int b, out int x)
            {
                r = R;
                g = G;
                b = B;
                x = X;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgbx ToByteRgbx()
            {
                return new ByteRgbx(_Cast(R), _Cast(G), _Cast(B), _Cast(X));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FloatRgbx ToFloatRgbx()
            {
                return new FloatRgbx(R, G, B, X);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static byte _Cast(int value)
            {
                return unchecked(
                    (value > byte.MaxValue) ? byte.MaxValue :
                    (value < byte.MinValue) ? byte.MinValue :
                    (byte)value);
            }
        }

        public struct FloatRgb
        {
            public float R { get; set; }
            public float G { get; set; }
            public float B { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FloatRgb(float r, float g, float b)
            {
                R = r;
                G = g;
                B = b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out float r, out float g, out float b)
            {
                r = R;
                g = G;
                b = B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgb ToIntRgb()
            {
                return new IntRgb(_Cast(R), _Cast(G), _Cast(B));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgb ToByteRgb()
            {
                return ToIntRgb().ToByteRgb();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int _Cast(float value)
            {
                return (int)Math.Round(value);
            }
        }

        public struct FloatRgbx
        {
            public float R { get; set; }
            public float G { get; set; }
            public float B { get; set; }
            public float X { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FloatRgbx(float r, float g, float b, float x)
            {
                R = r;
                G = g;
                B = b;
                X = x;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out float r, out float g, out float b)
            {
                r = R;
                g = G;
                b = B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out float r, out float g, out float b, out float x)
            {
                r = R;
                g = G;
                b = B;
                x = X;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IntRgbx ToIntRgbx()
            {
                return new IntRgbx(_Cast(R), _Cast(G), _Cast(B), _Cast(X));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteRgbx ToByteRgbx()
            {
                return ToIntRgbx().ToByteRgbx();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int _Cast(float value)
            {
                return (int)Math.Round(value);
            }
        }
    }
}
