using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    using Data;

    /// <summary>
    /// Provides read-only or writeable access to a bitmap's row data. 
    /// 
    /// <para>
    /// The caller can ask for the bitmap data from a particular row to be copied into a buffer.
    /// <br/>
    /// The caller can ask for the bitmap data to be copied from a caller-provided buffer into a 
    /// particular row, if the bitmap is writeable.
    /// </para>
    /// 
    /// <para>
    /// This class does not provide direct access to the backing array on the bitmap. 
    /// <br/>
    /// Use <see cref="BitmapRowDirect{T}"/> (implements <see cref="IBitmapRowDirect{T}"/>) 
    /// if direct access is needed.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// </typeparam>
    public class BitmapRowAccess<T>
        : IBitmapRowAccess<T>
        where T : struct
    {
        #region private
        private IArrayBitmap<T> _target;
        #endregion

        public int Width => _target.Width;

        public int Height => _target.Height;

        public Size Size => new Size(_target.Width, _target.Height);

        public Type DataType => typeof(T);

        public Type ArrayType => typeof(T[]);

        public bool CanRead => true;

        public bool CanWrite { get; }

        public BitmapRowAccess(IArrayBitmap<T> target)
            : this(target, canWrite: false)
        {
        }

        public BitmapRowAccess(IArrayBitmap<T> target, bool canWrite)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            _target = target;
            CanWrite = canWrite;
        }

        public void CopyRow(int row, T[] dest, int destStart)
        {
            _ValidateRow(row);
            _ValidateRowBuffer(dest, destStart);
            int targetWidth = _target.Width;
            var targetData = _target.Data;
            int targetRowStart = row * targetWidth;
            for (int x = 0; x < targetWidth; ++x)
            {
                dest[destStart + x] = targetData[targetRowStart + x];
            }
        }

        public void WriteRow(int row, T[] source, int sourceStart)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException();
            }
            _ValidateRow(row);
            _ValidateRowBuffer(source, sourceStart);
            int targetWidth = _target.Width;
            var targetData = _target.Data;
            int targetRowStart = row * targetWidth;
            for (int x = 0; x < targetWidth; ++x)
            {
                targetData[targetRowStart + x] = source[sourceStart + x];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _ValidateRow(int row)
        {
            if (row < 0 || row >= _target.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _ValidateRowBuffer(T[] rowData, int rowDataStart)
        {
            int rowDataEnd = checked(rowDataStart + _target.Width);
            int rowDataLength = rowData?.Length ?? 0;
            if (rowDataEnd > rowDataLength)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
