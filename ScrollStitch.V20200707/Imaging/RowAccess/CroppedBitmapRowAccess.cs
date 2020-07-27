﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    using Data;

    public class CroppedBitmapRowAccess<T>
        : IBitmapRowAccess<T>
        where T : struct
    {
        #region private
        private IArrayBitmap<T> _target;
        private Rect _rect;
        private T _oobValue;
        #endregion

        public int Width => _rect.Width;

        public int Height => _rect.Height;
        
        public Size Size => new Size(_rect.Width, _rect.Height);

        public Type DataType => typeof(T);

        public Type ArrayType => typeof(T[]);

        public bool CanRead => true;

        public bool CanWrite { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect)
            : this(target, rect, canWrite: false, allowOutOfBounds: false)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect, bool canWrite)
            : this(target, rect, canWrite: canWrite, allowOutOfBounds: false)
        { 
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect, bool canWrite, bool allowOutOfBounds, T outOfBoundsValue = default)
        {
            _CtorValidateTargetAndRect(target, rect, allowOutOfBounds);
            _target = target;
            _rect = rect;
            CanWrite = canWrite;
            _oobValue = outOfBoundsValue;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CopyRow(int row, T[] dest, int destStart)
        {
            _ValidateRow(row);
            _ValidateRowBuffer(dest, destStart);
            int targetRow = row + _rect.Y;
            if (targetRow < 0 ||
                targetRow >= _target.Height)
            {
#if true
                Arrays.BuiltinArrayMethods.NoInline.ArrayFill(dest, _oobValue, destStart, _rect.Width);
#else
                for (int dx = 0; dx < _rect.Width; ++dx)
                {
                    dest[destStart + dx] = _oobValue;
                }
#endif
                return;
            }
            int targetRowStart = targetRow * _target.Width;
            int targetLeft = _rect.Left;
            int targetRight = _rect.Right;
            int targetLeftValid = Math.Max(targetLeft, 0);
            int targetRightValid = Math.Min(targetRight, _target.Width);
#if true
            Arrays.BuiltinArrayMethods.NoInline.ArrayFill(
                dest, 
                _oobValue, 
                destStart, 
                targetLeftValid - targetLeft);
            System.Array.Copy(
                _target.Data,
                targetRowStart + targetLeftValid,
                dest,
                destStart + targetLeftValid - targetLeft,
                targetRightValid - targetLeftValid);
            Arrays.BuiltinArrayMethods.NoInline.ArrayFill(
                dest,
                _oobValue,
                destStart + targetRightValid - targetLeft,
                targetRight - targetRightValid);
#else
            for (int targetX = targetLeft; targetX < targetLeftValid; ++targetX)
            {
                dest[destStart + targetX - targetLeft] = _oobValue;
            }
            for (int targetX = targetLeftValid; targetX < targetRightValid; ++targetX)
            {
                dest[destStart + targetX - targetLeft] = _target.Data[targetRowStart + targetX];
            }
            for (int targetX = targetRightValid; targetX < targetRight; ++targetX)
            {
                dest[destStart + targetX - targetLeft] = _oobValue;
            }
#endif
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteRow(int row, T[] source, int sourceStart)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException();
            }
            _ValidateRow(row);
            _ValidateRowBuffer(source, sourceStart);
            int targetRow = row + _rect.Y;
            if (targetRow < 0 ||
                targetRow >= _target.Height)
            {
                return;
            }
            int targetRowStart = targetRow * _target.Width;
            int targetLeft = _rect.Left;
            int targetRight = _rect.Right;
            int targetLeftValid = Math.Max(targetLeft, 0);
            int targetRightValid = Math.Min(targetRight, _target.Width);
#if true
            System.Array.Copy(
                source,
                sourceStart + targetLeftValid - targetLeft,
                _target.Data,
                targetRowStart + targetLeftValid,
                targetRightValid - targetLeftValid);
#else
            for (int targetX = targetLeftValid; targetX < targetRightValid; ++targetX)
            {
                _target.Data[targetRowStart + targetX] = source[sourceStart + targetX - targetLeft];
            }
#endif
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _CtorValidateTargetAndRect(IArrayBitmap<T> target, Rect rect, bool allowOutOfBounds)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (!rect.IsPositive)
            {
                throw new ArgumentException(nameof(rect));
            }
            if (!allowOutOfBounds)
            {
                if (rect.Left < 0 ||
                    rect.Top < 0 ||
                    rect.Right > target.Width ||
                    rect.Bottom > target.Height)
                {
                    throw new ArgumentException(nameof(rect));
                }
            }
        }

        private void _ValidateRow(int row)
        {
            if (row < 0 || row >= _rect.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }
        }

        private void _ValidateRowBuffer(T[] rowData, int rowDataStart)
        {
            int rowDataEnd = checked(rowDataStart + _rect.Width);
            int rowDataLength = rowData?.Length ?? 0;
            if (rowDataEnd > rowDataLength)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
