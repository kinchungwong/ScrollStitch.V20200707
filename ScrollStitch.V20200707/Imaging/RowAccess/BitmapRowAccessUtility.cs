using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Imaging.Functional;

    public static class BitmapRowAccessUtility
    {
        public static IBitmapRowAccess<T> WrapRead<T>(IArrayBitmap<T> bitmap)
            where T : struct
        {
            return new BitmapRowAccess<T>(bitmap, canWrite: false);
        }

        public static IBitmapRowAccess<T> WrapWrite<T>(IArrayBitmap<T> bitmap)
            where T : struct
        {
            return new BitmapRowAccess<T>(bitmap, canWrite: true);
        }

        public static IBitmapRowDirect<T> WrapDirect<T>(IArrayBitmap<T> bitmap)
            where T : struct
        {
            return new BitmapRowDirect<T>(bitmap);
        }

        public static IBitmapRowAccess<T> WrapRead<T>(IArrayBitmap<T> bitmap, Rect rect)
            where T : struct
        {
            return new CroppedBitmapRowAccess<T>(bitmap, rect, canWrite: false, allowOutOfBounds: false);
        }

        public static IBitmapRowAccess<T> WrapWrite<T>(IArrayBitmap<T> bitmap, Rect rect)
            where T : struct
        {
            return new CroppedBitmapRowAccess<T>(bitmap, rect, canWrite: true, allowOutOfBounds: false);
        }

        public static IBitmapRowAccess<T> Wrap<T>(IArrayBitmap<T> bitmap, Rect rect, bool canWrite, bool allowOutOfBounds)
            where T : struct
        {
            return new CroppedBitmapRowAccess<T>(bitmap, rect, canWrite: canWrite, allowOutOfBounds: allowOutOfBounds);
        }

        public static void Copy<T>(IBitmapRowSource<T> source, IBitmapRowAccess<T> dest)
            where T : struct
        {
            _ValidateSourceDest(source, dest);
            int width = source.Width;
            int height = source.Height;
            T[] buffer = new T[width];
            for (int row = 0; row < height; ++row)
            {
                source.CopyRow(row, buffer, 0);
                dest.WriteRow(row, buffer, 0);
            }
        }

        public static void Copy<T>(IBitmapRowSource<T> source, IBitmapRowDirect<T> dest)
            where T : struct
        {
            _ValidateSourceDest(source, dest);
            int height = source.Height;
            for (int row = 0; row < height; ++row)
            {
                ArraySegment<T> destSeg = dest.GetRowDirect(row);
                source.CopyRow(row, destSeg.Array, destSeg.Offset);
            }
        }

        public static IntBitmap ToIntBitmap(IBitmapRowSource<int> source)
        {
            IntBitmap result = new IntBitmap(source.Size);
            Copy(source, WrapDirect(result));
            return result;
        }

        public static void Modify<T>(IBitmapRowSource<T> source, IBitmapRowAccess<T> dest, Func<T, T> modifyFunc)
            where T : struct
        {
            _ValidateSourceDest(source, dest);
            int width = source.Width;
            int height = source.Height;
            T[] buffer = new T[width];
            for (int row = 0; row < height; ++row)
            {
                source.CopyRow(row, buffer, 0);
                for (int col = 0; col < width; ++col)
                {
                    buffer[col] = modifyFunc(buffer[col]);
                }
                dest.WriteRow(row, buffer, 0);
            }
        }

        public static void Modify<T, TFunc>(IBitmapRowSource<T> source, IBitmapRowAccess<T> dest, TFunc modifyFunc)
            where T : struct
            where TFunc : struct, IFunc, IFunc<TFunc, T, T>
        {
            _ValidateSourceDest(source, dest);
            int width = source.Width;
            int height = source.Height;
            T[] buffer = new T[width];
            for (int row = 0; row < height; ++row)
            {
                source.CopyRow(row, buffer, 0);
                for (int col = 0; col < width; ++col)
                {
                    buffer[col] = modifyFunc.Invoke(buffer[col]);
                }
                dest.WriteRow(row, buffer, 0);
            }
        }

        public static void Blend(IBitmapRowSource<int> source, IBitmapRowAccess<int> dest, double destFrac)
        {
            var blendFunc = new BgrxBlendWith()
            {
                Frac = destFrac
            };
            _ValidateSourceDest(source, dest);
            int width = source.Width;
            int height = source.Height;
            int[] sourceBuffer = new int[width];
            int[] destBuffer = new int[width];
            for (int row = 0; row < height; ++row)
            {
                source.CopyRow(row, sourceBuffer, 0);
                dest.CopyRow(row, destBuffer, 0);
                for (int col = 0; col < width; ++col)
                {
                    destBuffer[col] = blendFunc.Invoke(sourceBuffer[col], destBuffer[col]);
                }
                dest.WriteRow(row, destBuffer, 0);
            }
        }

        private static void _ValidateSourceDest<T>(IBitmapRowSource<T> source, IBitmapRowAccess<T> dest)
            where T : struct
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (dest is null)
            {
                throw new ArgumentNullException(nameof(dest));
            }
            if (!source.CanRead ||
                !dest.CanWrite)
            {
                throw new InvalidOperationException();
            }
            _ValidateSameSize(source, dest);
        }

        private static void _ValidateSameSize(IScalarBitmapInfo info1, IScalarBitmapInfo info2)
        {
            if (info1.Width != info2.Width ||
                info1.Height != info2.Height)
            {
                throw new ArgumentException();
            }
        }
    }
}
