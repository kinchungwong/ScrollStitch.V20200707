using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    using Data;
    using static Specialized.SpecializedArrayMethods.NoInline;
    using static Arrays.BuiltinArrayMethods.NoInline;

    /// <summary>
    /// <para>
    /// This is a static class. To create an instance, use the generic <see cref="CroppedBitmapRowAccess{T}"/> 
    /// class instead.
    /// </para>
    /// 
    /// <para>
    /// This static non-generic class only exists to allow a nested static class <see cref="DefaultSettings"/>
    /// which allows new instances of <see cref="CroppedBitmapRowAccess{T}"/> to be created with certain
    /// default settings. 
    /// </para>
    /// </summary>
    public static class CroppedBitmapRowAccess
    {
        /// <summary>
        /// Settings used to initialize every instance of <see cref="CroppedBitmapRowAccess{T}"/> created.
        /// </summary>
        public static class DefaultSettings
        {
            /// <inheritdoc cref="CroppedBitmapRowAccess{T}.RandomizeOutOfBoundValues"/>
            /// 
            public static bool RandomizeOutOfBoundValues { get; set; } = false;
        }
    }

    /// <summary>
    /// An implementation of the <see cref="IBitmapRowAccess{T}"/> interface which performs a
    /// virtual "crop" of the underlying bitmap.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The pixel type of the bitmap source.
    /// </typeparam>
    /// 
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

        /// <summary>
        /// <para>
        /// (Debug assistance feature.) <br/>
        /// When this flag is set, attempts to copy out-of-bounds values from a target bitmap via 
        /// <see cref="CopyRow(int, T[], int)"/> will result in those out-of-bounds pixels being
        /// set to deterministically-generated pseudorandom values.
        /// </para>
        /// <para>
        /// This flag defaults to false. When a new instance of <see cref="CroppedBitmapRowAccess{T}"/>
        /// is created, its constructor copies the flag value from the 
        /// <see cref="CroppedBitmapRowAccess.DefaultSettings"/> property of the same name.
        /// </para>
        /// </summary>
        public bool RandomizeOutOfBoundValues 
        { 
            get; 
            set; 
        } = CroppedBitmapRowAccess.DefaultSettings.RandomizeOutOfBoundValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect)
            : this(target, rect, canWrite: false, allowOutOfBounds: false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        /// <param name="canWrite"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect, bool canWrite)
            : this(target, rect, canWrite: canWrite, allowOutOfBounds: false)
        { 
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        /// <param name="canWrite"></param>
        /// <param name="allowOutOfBounds"></param>
        /// <param name="outOfBoundsValue"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public CroppedBitmapRowAccess(IArrayBitmap<T> target, Rect rect, bool canWrite, bool allowOutOfBounds, T outOfBoundsValue = default)
        {
            _CtorValidateTargetAndRect(target, rect, allowOutOfBounds);
            _target = target;
            _rect = rect;
            CanWrite = canWrite;
            _oobValue = outOfBoundsValue;
        }

        /// <inheritdoc cref="IBitmapRowSource{T}.CopyRow(int, T[], int)"/>
        /// 
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
                _StaticArrayFillImpl(
                    array: dest, 
                    value: _oobValue, 
                    startIndex: destStart, 
                    count: _rect.Width, 
                    randomize: RandomizeOutOfBoundValues, 
                    randomSeed: row);
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
            if (targetLeft >= _target.Width ||
                targetRight <= 0)
            {
#if true
                _StaticArrayFillImpl(
                    array: dest,
                    value: _oobValue,
                    startIndex: destStart,
                    count: _rect.Width,
                    randomize: RandomizeOutOfBoundValues,
                    randomSeed: row);
#else
                for (int dx = 0; dx < _rect.Width; ++dx)
                {
                    dest[destStart + dx] = _oobValue;
                }
#endif
                return;
            }
            int targetLeftValid = Math.Max(targetLeft, 0);
            int targetRightValid = Math.Min(targetRight, _target.Width);
#if true
            _StaticArrayFillImpl(
                array: dest,
                value: _oobValue,
                startIndex: destStart,
                count: targetLeftValid - targetLeft,
                randomize: RandomizeOutOfBoundValues,
                randomSeed: row);

            Array.Copy(
                _target.Data,
                targetRowStart + targetLeftValid,
                dest,
                destStart + targetLeftValid - targetLeft,
                targetRightValid - targetLeftValid);

            _StaticArrayFillImpl(
                array: dest,
                value: _oobValue,
                startIndex: destStart + targetRightValid - targetLeft,
                count: targetRight - targetRightValid,
                randomize: RandomizeOutOfBoundValues,
                randomSeed: row);

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

        /// <inheritdoc cref="IBitmapRowAccess{T}.WriteRow(int, T[], int)"/>
        /// 
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
            if (targetLeft >= _target.Width ||
                targetRight <= 0)
            {
                return;
            }
            int targetLeftValid = Math.Max(targetLeft, 0);
            int targetRightValid = Math.Min(targetRight, _target.Width);
#if true
            Array.Copy(
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _StaticArrayFillImpl(T[] array, T value, int startIndex, int count,
            bool randomize, int randomSeed)
        {
            if (randomize && (array is int[] intArray))
            {
                ArrayFillNoise(intArray, randomSeed, startIndex, count);
                return;
            }
            ArrayFill(array, value, startIndex, count);
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
