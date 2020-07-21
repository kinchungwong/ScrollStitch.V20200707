using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Interlocked = System.Threading.Interlocked;

namespace ScrollStitch.V20200707.Imaging.IO
{
    public class LockedByteBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        private BitmapData _bitmapData;
        public BitmapData BitmapData => _bitmapData;
        public PixelFormat PixelFormat { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }
        private ImageLockMode _lockFlags { get; }

        public int Width { get; }
        public int Height { get; }
        private int _bytesPerPixel { get; }
        public int ArrayElementsPerRow { get; }

        private IntPtr Scan0;
        private int Stride;

        public LockedByteBitmap(Bitmap bitmap, bool canRead, bool canWrite)
        {
            Bitmap = bitmap;
            PixelFormat = bitmap.PixelFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;
            CanRead = canRead;
            CanWrite = canWrite;
            _lockFlags = _GetFlags(canRead, canWrite);
            _bytesPerPixel = _GetBytesPerPixel(PixelFormat);
            ArrayElementsPerRow = Width * _bytesPerPixel;
            var rect = new Rectangle(0, 0, Width, Height);
            _bitmapData = Bitmap.LockBits(rect, _lockFlags, PixelFormat);
            Scan0 = BitmapData.Scan0;
            Stride = BitmapData.Stride;
        }

        public void Dispose()
        {
            Scan0 = IntPtr.Zero;
            Stride = 0;
            // Ensures UnlockBits() called exactly once.
            var oldBitmapData = Interlocked.Exchange(ref _bitmapData, null);
            if (oldBitmapData is null ||
                Bitmap is null)
            {
                return;
            }
            Bitmap.UnlockBits(oldBitmapData);
        }

        public void CopyRow(int row, byte[] dest, int destStart)
        {
            if (!CanRead)
            {
                throw new InvalidOperationException();
            }
            if (destStart + ArrayElementsPerRow > dest.Length)
            {
                throw new InvalidOperationException();
            }
            Marshal.Copy(Scan0 + Stride * row, dest, destStart, ArrayElementsPerRow);
        }

        public void WriteRow(int row, byte[] source, int sourceStart)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException();
            }
            if (sourceStart + ArrayElementsPerRow > source.Length)
            {
                throw new InvalidOperationException();
            }
            Marshal.Copy(source, sourceStart, Scan0 + Stride * row, ArrayElementsPerRow);
        }

        private static int _GetBytesPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 4;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format8bppIndexed:
                    return 1;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static ImageLockMode _GetFlags(bool canRead, bool canWrite)
        {
            int code = (canRead ? 1 : 0) + (canWrite ? 2 : 0);
            switch (code)
            {
                case 1:
                    return ImageLockMode.ReadOnly;
                case 2:
                    return ImageLockMode.WriteOnly;
                case 3:
                    return ImageLockMode.ReadWrite;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
