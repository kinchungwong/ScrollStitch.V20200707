using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Data;

    public static class BitmapCopyUtility
    {
        public static void CopyRect(ByteBitmap source, Rect sourceRect, ByteBitmap dest, Point destTopLeft)
        {
            var destRect = new Rect(destTopLeft, sourceRect.Size);
            _ValidateRectInSizeElseThrow(source.Size, sourceRect);
            _ValidateRectInSizeElseThrow(dest.Size, destRect);
            int copyWidth = sourceRect.Width;
            int copyHeight = sourceRect.Height;
            for (int dy = 0; dy < copyHeight; ++dy)
            {
                int sourceStart = (sourceRect.Y + dy) * source.Width + sourceRect.X;
                int destStart = (destTopLeft.Y + dy) * dest.Width + destTopLeft.X;
                Array.Copy(source.Data, sourceStart, dest.Data, destStart, copyWidth);
            }
        }

        public static void CopyRect(IntBitmap source, Rect sourceRect, IntBitmap dest, Point destTopLeft)
        {
            var destRect = new Rect(destTopLeft, sourceRect.Size);
            _ValidateRectInSizeElseThrow(source.Size, sourceRect);
            _ValidateRectInSizeElseThrow(dest.Size, destRect);
            int copyWidth = sourceRect.Width;
            int copyHeight = sourceRect.Height;
            for (int dy = 0; dy < copyHeight; ++dy)
            {
                int sourceStart = (sourceRect.Y + dy) * source.Width + sourceRect.X;
                int destStart = (destTopLeft.Y + dy) * dest.Width + destTopLeft.X;
                Array.Copy(source.Data, sourceStart, dest.Data, destStart, copyWidth);
            }
        }

        public static IntBitmap Clone(IntBitmap source)
        {
            int width = source.Width;
            int height = source.Height;
            IntBitmap dest = new IntBitmap(width, height);
            var sourceRect = new Rect(0, 0, width, height);
            CopyRect(source, sourceRect, dest, Point.Origin);
            return dest;
        }

        public static void BlendRect(double sourceFrac, IntBitmap source, Rect sourceRect, IntBitmap dest, Point destTopLeft)
        {
            int sourceWeight = (int)Math.Round(256.0 * sourceFrac);
            int destWeight = 256 - sourceWeight;
            var destRect = new Rect(destTopLeft, sourceRect.Size);
            _ValidateRectInSizeElseThrow(source.Size, sourceRect);
            _ValidateRectInSizeElseThrow(dest.Size, destRect);
            int copyWidth = sourceRect.Width;
            int copyHeight = sourceRect.Height;
            var sourceData = source.Data;
            var destData = dest.Data;
            for (int dy = 0; dy < copyHeight; ++dy)
            {
                int sourceStart = (sourceRect.Y + dy) * source.Width + sourceRect.X;
                int destStart = (destTopLeft.Y + dy) * dest.Width + destTopLeft.X;
                for (int pixelOffset = 0; pixelOffset < copyWidth; ++pixelOffset)
                {
                    int sourceValue = sourceData[sourceStart + pixelOffset];
                    _ToColor(sourceValue, out byte sr, out byte sg, out byte sb);
                    int destValue = destData[destStart + pixelOffset];
                    _ToColor(destValue, out byte dr, out byte dg, out byte db);
                    int br = (sr * sourceWeight + dr * destWeight + 128) / 256;
                    int bg = (sg * sourceWeight + dg * destWeight + 128) / 256;
                    int bb = (sb * sourceWeight + db * destWeight + 128) / 256;
                    int blendedValue = _ToColor((byte)br, (byte)bg, (byte)bb);
                    destData[destStart + pixelOffset] = blendedValue;
                }
            }
        }

        public static void UniformModifyRect(Func<byte, byte> valueFunc, ByteBitmap dest, Rect destRect)
        {
            _ValidateRectInSizeElseThrow(dest.Size, destRect);
            int fillWidth = destRect.Width;
            int fillHeight = destRect.Height;
            for (int dy = 0; dy < fillHeight; ++dy)
            {
                int destStart = (destRect.Y + dy) * dest.Width + destRect.X;
                for (int pixelIndex = destStart + fillWidth - 1; pixelIndex >= destStart; --pixelIndex)
                {
                    dest.Data[pixelIndex] = valueFunc(dest.Data[pixelIndex]);
                }
            }
        }

        private static void _ValidateRectInSizeElseThrow(Size size, Rect rect)
        {
            if (size.Width < 0 ||
                size.Height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (rect.Width < 0 ||
                rect.Height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rect));
            }
            if (rect.Left < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rect.Left));
            }
            if (rect.Top < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rect.Top));
            }
            if (rect.Right > size.Width)
            {
                throw new ArgumentOutOfRangeException(nameof(rect.Right));
            }
            if (rect.Bottom > size.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(rect.Bottom));
            }
        }

        private static void _ToColor(int intValue, out byte r, out byte g, out byte b)
        {
            unchecked
            {
                r = (byte)((intValue >> 16) & 255);
                g = (byte)((intValue >> 8) & 255);
                b = (byte)(intValue & 255);
            }
        }

        private static int _ToColor(byte r, byte g, byte b)
        {
            return b | (g << 8) | (r << 16);
        }
    }
}
