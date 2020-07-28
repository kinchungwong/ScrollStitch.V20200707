using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Compare
{
    using Data;
    using RowAccess;
    using ByteRgbx = ColorUtility.ByteRgbx;
    using IntRgbx = ColorUtility.IntRgbx;
    using ColorInt = ColorUtility.ColorInt;
    using SourceList = Collections.UniformSizeRowSourceList<int>;

    /// <summary>
    /// Compares one reference bitmap against a list of target bitmaps, and produces an output bitmap
    /// where each pixel is set to the RGB difference between the reference pixel and the nearest-valued
    /// target pixel.
    /// </summary>
    public class MultiImageComparer
    {
        /// <summary>
        /// The size of bitmaps.
        /// </summary>
        public Size BitmapSize { get; }

        /// <summary>
        /// Reference bitmap.
        /// </summary>
        public IBitmapRowSource<int> ReferenceBitmap { get; }

        /// <summary>
        /// List of target bitmaps.
        /// </summary>
        public SourceList TargetBitmaps { get; }

        /// <summary>
        /// Number of target bitmaps.
        /// </summary>
        public int TargetCount => TargetBitmaps.Count;

        /// <summary>
        /// The output bitmap of this class. 
        /// 
        /// <para>
        /// For each pixel, each of the target pixels (extracted from the same coordinates from each of the 
        /// target bitmaps) are compared to the reference pixel. The target pixel that is closest to the
        /// reference pixel (in terms of the sum of absolute RGB differences) is chosen, and their absolute 
        /// RGB difference (expressed as a RGB color value) is written to this output bitmap.
        /// </para>
        /// 
        /// <para>
        /// The caller may either specify the output bitmap to be used when calling 
        /// <see cref="MultiImageComparer.MultiImageComparer"/>. If it is not specified at the constructor.
        /// this class will allocate an output bitmap, which the caller may take ownership after 
        /// <see cref="Process"/> finished successfully.
        /// </para>
        /// </summary>
        public IntBitmap NearestDifferenceBitmap { get; private set; }

        /// <summary>
        /// Experimental feature. <br/>
        /// DO NOT set to true, except during development testing.
        /// </summary>
        private bool UseParallel => false;

        private bool CanUseParallel => BitmapSize.Height >= 256;

        /// <summary>
        /// Initializes <see cref="MultiImageComparer"/> with a reference bitmap and one or more target bitmaps.
        /// </summary>
        /// 
        /// <param name="referenceBitmap">
        /// </param>
        /// 
        /// <param name="targetBitmaps">
        /// </param>
        /// 
        /// <param name="nearestDifferenceBitmap">
        /// Specifies a bitmap to be used as the output.
        /// <para>
        /// This parameter is optional. If not specified, this class will automatically allocate one, which the 
        /// caller may take ownership after <see cref="Process"/> finished successfully.
        /// </para>
        /// </param>
        /// 
        public MultiImageComparer(IBitmapRowSource<int> referenceBitmap, SourceList targetBitmaps, 
            IntBitmap nearestDifferenceBitmap = null)
        {
            _ValidateNotNull(referenceBitmap, nameof(referenceBitmap));
            _ValidateNotNull(targetBitmaps, nameof(targetBitmaps));
            _ValidateCountNotZero(targetBitmaps, nameof(targetBitmaps));
            //
            ReferenceBitmap = referenceBitmap;
            BitmapSize = referenceBitmap.Size;
            //
            _ValidateSizeSame(BitmapSize, targetBitmaps.BitmapSize, $"{nameof(targetBitmaps)}.BitmapSize");
            TargetBitmaps = new SourceList(targetBitmaps, isReadOnly: true);
            //
            NearestDifferenceBitmap = nearestDifferenceBitmap ?? new IntBitmap(BitmapSize);
            _ValidateSizeSame(BitmapSize, NearestDifferenceBitmap.Size, $"{nameof(nearestDifferenceBitmap)}.Size");
        }

        public void Process()
        {
            if (UseParallel && CanUseParallel)
            {
                Process_Parallel();
            }
            else
            {
                Process_NoParallel();
            }
        }

        private void Process_NoParallel()
        {
            Size size = BitmapSize;
            Range fullRowRange = new Range(0, size.Height);
            _ProcessRowRange(fullRowRange);
        }

        private void Process_Parallel()
        {
            Size size = BitmapSize;
            var subdiv = Spatial.AxisSubdivFactory.CreateNearlyUniform(size.Height, 16);
            var ranges = subdiv.Ranges;
            int rangeCount = ranges.Count;
            Task[] tasks = new Task[rangeCount];
            for (int k = 0; k < rangeCount; ++k)
            {
                var range = ranges[k];
                tasks[k] = Task.Run(() => _ProcessRowRange(range));
            }
            Task.WaitAll(tasks);
        }

        private void _ProcessRowRange(Range rowRange)
        {
            new RowRangeProcessor(this, rowRange).Process();
        }

        private class RowRangeProcessor
        {
            private MultiImageComparer _host;
            private Size _bitmapSize;
            private int _targetCount;
            private Range _rowRange;
            private ArraySegment<int> _refSegment;
            private ArraySegment<int>[] _targetSegments;
            private ArraySegment<int> _outputSegment;

            internal RowRangeProcessor(MultiImageComparer host, Range rowRange)
            {
                _host = host;
                _bitmapSize = host.BitmapSize;
                _targetCount = host.TargetCount;
                _rowRange = rowRange;
            }

            internal void Process()
            {
                _rowRange.ForEach((int row) => _ProcessRow(row));
            }

            private void _ProcessRow(int row)
            {
                _PopulateReferenceSegment(row);
                _PopulateTargetSegments(row);
                _EnsureOutputSegmentAllocated();
                _CompareSegment();
                _WriteOutputSegment(row);
            }

            private void _PopulateReferenceSegment(int row)
            {
                _PopulateSegment(_host.ReferenceBitmap, row, ref _refSegment);
            }

            private void _PopulateTargetSegments(int row)
            {
                _EnsureArrayLengthAllocated(ref _targetSegments, _targetCount);
                for (int targetIndex = 0; targetIndex < _targetCount; ++targetIndex)
                {
                    _PopulateSegment(_host.TargetBitmaps[targetIndex], row, ref _targetSegments[targetIndex]);
                }
            }

            private void _EnsureOutputSegmentAllocated()
            {
                _EnsureSegmentBufferAllocated(ref _outputSegment);
            }

            private void _EnsureArrayLengthAllocated<T>(ref T[] arr, int expectedLength)
            {
                if (arr is null ||
                    arr.Length != expectedLength)
                {
                    arr = new T[expectedLength];
                }
            }

            private void _PopulateSegment(IBitmapRowSource<int> source, int row, ref ArraySegment<int> segment)
            {
                switch (source)
                {
                    case IBitmapRowDirect<int> direct:
                        segment = direct.GetRowDirect(row);
                        return;
                    default:
                        _EnsureSegmentBufferAllocated(ref segment);
                        source.CopyRow(row, segment.Array, segment.Offset);
                        return;
                }
            }

            /// <summary>
            /// This initialization method is needed if the <see cref="ArraySegment"/> needs to have 
            /// its own backing memory, intended to be used as the buffer when calling 
            /// <see cref="IBitmapRowSource{T}.CopyRow(int, T[], int)"/>.
            /// </summary>
            /// 
            /// <param name="bitmapSize"></param>
            /// <param name="segment"></param>
            /// 
            private void _EnsureSegmentBufferAllocated(ref ArraySegment<int> segment)
            {
                int expectedLength = _bitmapSize.Width;
                if (segment.Array is null ||
                    segment.Count != expectedLength)
                {
                    segment = new ArraySegment<int>(new int[expectedLength]);
                }
            }

            private void _CompareSegment()
            {
                _RowCompareFunction(_refSegment, _targetSegments, _outputSegment);
            }

            private void _WriteOutputSegment(int row)
            {
                var wrapWrite = BitmapRowAccessUtility.WrapWrite(_host.NearestDifferenceBitmap);
                wrapWrite.WriteRow(row, _outputSegment.Array, 0);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _RowCompareFunction(ArraySegment<int> refSegment, ArraySegment<int>[] targetSegments, 
            ArraySegment<int> outputSegment)
        {
            var r = refSegment;
            var os = outputSegment;
            int targetCount = targetSegments.Length;
            int pixelCount = refSegment.Count;
            for (int index = 0; index < pixelCount; ++index)
            {
                int bestSum = int.MaxValue;
                int bestColorDiff = default;
                int curRefColorValue = r.Array[r.Offset + index];
                for (int targetIndex = 0; targetIndex < targetCount; ++targetIndex)
                {
                    var t = targetSegments[targetIndex];
                    int curTargetColorValue = t.Array[t.Offset + index];

                    // ====== 
                    // Pending performance issue in code generation.
                    // Currently, _AbsDiff_Ver3() is able to workaround the issue.
                    // ======
#if false
                    (int curSum, int curColorDiff) = NoInline._AbsDiff(curRefColorValue, curTargetColorValue);
#elif false
                    (int curSum, int curColorDiff) = NoInline._AbsDiff_Ver2(curRefColorValue, curTargetColorValue);
#elif true
                    NoInline._AbsDiff_Ver3(curRefColorValue, curTargetColorValue, out int curSum, out int curColorDiff);
#endif
                    if (curSum < bestSum)
                    {
                        bestSum = curSum;
                        bestColorDiff = curColorDiff;
                    }
                }
                os.Array[os.Offset + index] = bestColorDiff;
            }
        }

        private static void _ValidateNotNull<T>(T arg, string paramName)
            where T : class
        {
            if (arg is null)
            {
                throw new ArgumentNullException(paramName: paramName);
            }
        }

        private static void _ValidateCountNotZero<T>(IReadOnlyCollection<T> arg, string paramName)
        {
            if (arg.Count == 0)
            {
                throw new ArgumentException(
                    paramName: paramName,
                    message: $"{typeof(T).Name} cannot be empty.");
            }
        }

        private static void _ValidateSizeSame(Size expectedSize, Size actualSize, string paramName)
        {
            if (!actualSize.Equals(expectedSize))
            {
                throw new ArgumentException(
                    paramName: paramName,
                    message: $"Size mismatch. Expected: {expectedSize}, Actual: {actualSize}");
            }
        }

        /// <summary>
        /// These internal implementation details are exposed for performance tuning only. 
        /// This is to resolve a suboptimal JIT code generation issue that is still pending.
        /// </summary>
        /// 
        public static class Inline
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static (int, ByteRgbx) _AbsDiff(ByteRgbx left, ByteRgbx right)
            {
                int dr = Math.Abs(left.R - right.R);
                int dg = Math.Abs(left.G - right.G);
                int db = Math.Abs(left.B - right.B);
                return ((dr + dg + db), new IntRgbx(dr, dg, db, 0).ToByteRgbx());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static (int, int) _AbsDiff_Ver1(int left, int right)
            {
                (int sum, ByteRgbx diff) = _AbsDiff(new ColorInt(left).ToByteRgbx(), new ColorInt(right).ToByteRgbx());
                return (sum, new ColorInt(diff).Value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static (int, int) _AbsDiff_Ver2(int left, int right)
            {
                unchecked
                {
                    byte b0 = (byte)(left & 255);
                    byte g0 = (byte)((left >> 8) & 255);
                    byte r0 = (byte)((left >> 16) & 255);
                    byte b1 = (byte)(right & 255);
                    byte g1 = (byte)((right >> 8) & 255);
                    byte r1 = (byte)((right >> 16) & 255);
                    byte bd = (byte)((b0 > b1) ? (b0 - b1) : (b1 - b0));
                    byte gd = (byte)((g0 > g1) ? (g0 - g1) : (g1 - g0));
                    byte rd = (byte)((r0 > r1) ? (r0 - r1) : (r1 - r0));
                    int colorD = bd | (gd << 8) | (rd << 16);
                    return ((bd + gd + rd), colorD);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void _AbsDiff_Ver3(int left, int right, out int sum, out int diff)
            {
                unchecked
                {
                    byte b0 = (byte)(left & 255);
                    byte g0 = (byte)((left >> 8) & 255);
                    byte r0 = (byte)((left >> 16) & 255);
                    byte b1 = (byte)(right & 255);
                    byte g1 = (byte)((right >> 8) & 255);
                    byte r1 = (byte)((right >> 16) & 255);
                    byte bd = (byte)((b0 > b1) ? (b0 - b1) : (b1 - b0));
                    byte gd = (byte)((g0 > g1) ? (g0 - g1) : (g1 - g0));
                    byte rd = (byte)((r0 > r1) ? (r0 - r1) : (r1 - r0));
                    sum = bd + gd + rd;
                    diff = bd | (gd << 8) | (rd << 16);
                }
            }
        }

        /// <summary>
        /// These internal implementation details are exposed for performance tuning only. 
        /// This is to resolve a suboptimal JIT code generation issue that is still pending.
        /// </summary>
        /// 
        public static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static (int, ByteRgbx) _AbsDiff(ByteRgbx left, ByteRgbx right)
            {
                return Inline._AbsDiff(left, right);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static (int, int) _AbsDiff_Ver1(int left, int right)
            {
                return Inline._AbsDiff_Ver1(left, right);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static (int, int) _AbsDiff_Ver2(int left, int right)
            {
                return Inline._AbsDiff_Ver2(left, right);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void _AbsDiff_Ver3(int left, int right, out int sum, out int diff)
            {
                Inline._AbsDiff_Ver3(left, right, out sum, out diff);
            }

            /// <summary>
            /// For internal errors that are rare or impossible (i.e. arising from incorrect code, 
            /// or from conditions that may arise beyond normal software execution circumstances)
            /// </summary>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void _Throw()
            {
                throw new Exception();
            }
        }
    }
}
