using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ScrollStitch.V20200707.Imaging.IO
{
    public static class BitmapIoConvertUtility
    {
        public static ByteBitmap ToByteBitmap(this Bitmap input)
        {
            var output = new ByteBitmap(input.Width, input.Height);
            CopyTo(input, output);
            return output;
        }

        public static IntBitmap ToIntBitmap(this Bitmap input)
        {
            var output = new IntBitmap(input.Width, input.Height);
            CopyTo(input, output);
            return output;
        }

        public static Bitmap ToBitmap(this ByteBitmap input)
        {
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb);
            CopyTo(input, output);
            return output;
        }

        public static Bitmap ToBitmap(this IntBitmap input)
        {
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb);
            CopyTo(input, output);
            return output;
        }

        public static void CopyTo(Bitmap input, ByteBitmap output)
        {
            _ValidateSizeElseThrow(input.Size, output.Size);
            int width = input.Width;
            int height = input.Height;
            var format = input.PixelFormat;
            switch (format)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(input, true, false))
                        {
                            var converter = new Bgrx8888ToGray8(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.CopyRow(row, output.Data, row * output.Width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format24bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(input, true, false))
                        {
                            var converter = new Bgr888ToGray8(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.CopyRow(row, output.Data, row * output.Width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format8bppIndexed:
                    {
                        throw new NotImplementedException("To be implemented");
                    }
                default:
                    throw new Exception($"Unsupported pixel format: 0x{format:x8}");
            }
        }

        public static void CopyTo(Bitmap input, IntBitmap output)
        {
            _ValidateSizeElseThrow(input.Size, output.Size);
            int width = input.Width;
            int height = input.Height;
            var format = input.PixelFormat;
            switch (format)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(input, true, false))
                        {
                            var converter = new Bgrx8888ToBgr32(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.CopyRow(row, output.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format24bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(input, true, false))
                        {
                            var converter = new Bgr888ToBgr32(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.CopyRow(row, output.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format8bppIndexed:
                    {
                        throw new NotImplementedException("To be implemented");
                    }
                default:
                    throw new Exception($"Unsupported pixel format: 0x{format:x8}");
            }
        }

        public static void CopyTo(ByteBitmap input, Bitmap output)
        {
            _ValidateSizeElseThrow(input.Size, output.Size);
            int width = input.Width;
            int height = input.Height;
            var format = output.PixelFormat;
            switch (format)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(output, false, true))
                        {
                            var converter = new Bgrx8888ToGray8(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.WriteRow(row, input.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format24bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(output, false, true))
                        {
                            var converter = new Bgr888ToGray8(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.WriteRow(row, input.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format8bppIndexed:
                    {
                        throw new NotImplementedException("To be implemented");
                    }
                default:
                    throw new Exception($"Unsupported pixel format: 0x{format:x8}");
            }
        }

        public static void CopyTo(IntBitmap input, Bitmap output)
        {
            _ValidateSizeElseThrow(input.Size, output.Size);
            int width = input.Width;
            int height = input.Height;
            var format = output.PixelFormat;
            switch (format)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(output, false, true))
                        {
                            var converter = new Bgrx8888ToBgr32(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.WriteRow(row, input.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format24bppRgb:
                    {
                        using (var lockedBitmap = new LockedByteBitmap(output, false, true))
                        {
                            var converter = new Bgr888ToBgr32(lockedBitmap);
                            for (int row = 0; row < height; ++row)
                            {
                                converter.WriteRow(row, input.Data, row * width);
                            }
                        }
                    }
                    return;
                case PixelFormat.Format8bppIndexed:
                    {
                        throw new NotImplementedException("To be implemented");
                    }
                default:
                    throw new Exception($"Unsupported pixel format: 0x{format:x8}");
            }
        }

        private static void _ValidateSizeElseThrow(Size size1, Size size2)
        {
            if (size1.Width != size2.Width ||
                size1.Height != size2.Height)
            {
                throw new InvalidOperationException("Bitmap size mismatch");
            }
        }
    }
}
