using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowCompare
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.RowAccess;

    /// <summary>
    /// Compares the number of pixel mismatches, and report row-wise and column-wise statistics.
    /// 
    /// <para>
    /// This comparison class performs exact comparison. For each pixel coordinate, if the integer 
    /// values do not compare equal, a vote of 1 is contributed to each of the summary arrays.
    /// </para>
    /// 
    /// <inheritdoc cref="IBitmapComparer{TPixel, TSum}"/>
    /// </summary>
    public class PixelMismatchCounter
        : IBitmapComparer<int, int>
    {
        /// <inheritdoc/>
        public IntBitmap Bitmap1 { get; set; }

        /// <inheritdoc/>
        public IntBitmap Bitmap2 { get; set; }

        /// <inheritdoc/>
        public Rect Rect1 { get; set; }

        /// <inheritdoc/>
        public Rect Rect2 { get; set; }

        /// <inheritdoc/>
        public IBitmapRowSource<int> BitmapSource1 { get; set; }

        /// <inheritdoc/>
        public IBitmapRowSource<int> BitmapSource2 { get; set; }

        /// <inheritdoc/>
        public int[] RowSummary { get; set; }

        /// <inheritdoc/>
        public int[] ColumnSummary { get; set; }

        /// <inheritdoc/>
        public void SetBitmap1(IntBitmap bitmap)
        {
            Bitmap1 = bitmap;
            Rect1 = new Rect(Point.Origin, bitmap.Size);
            BitmapSource1 = new BitmapRowDirect<int>(bitmap);
        }

        /// <inheritdoc/>
        public void SetBitmap1(IntBitmap bitmap, Rect rect)
        {
            _ValidateBitmapRect(bitmap, rect);
            Bitmap1 = bitmap;
            Rect1 = rect;
            BitmapSource1 = new CroppedBitmapRowAccess<int>(bitmap, rect, canWrite: false);
        }

        /// <inheritdoc/>
        public void SetBitmap1(IBitmapRowSource<int> rowSource)
        {
            Bitmap1 = null;
            Rect1 = new Rect(Point.Origin, rowSource.Size);
            BitmapSource1 = rowSource;
        }

        /// <inheritdoc/>
        public void SetBitmap2(IntBitmap bitmap)
        {
            Bitmap2 = bitmap;
            Rect2 = new Rect(Point.Origin, bitmap.Size);
            BitmapSource2 = new BitmapRowDirect<int>(bitmap);
        }

        /// <inheritdoc/>
        public void SetBitmap2(IntBitmap bitmap, Rect rect)
        {
            _ValidateBitmapRect(bitmap, rect);
            Bitmap2 = bitmap;
            Rect2 = rect;
            BitmapSource2 = new CroppedBitmapRowAccess<int>(bitmap, rect, canWrite: false);
        }

        /// <inheritdoc/>
        public void SetBitmap2(IBitmapRowSource<int> rowSource)
        {
            Bitmap2 = null;
            Rect2 = new Rect(Point.Origin, rowSource.Size);
            BitmapSource2 = rowSource;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Compare()
        {
            IBitmapRowSource<int> bitmap1 = BitmapSource1;
            IBitmapRowSource<int> bitmap2 = BitmapSource2;
            Size size = bitmap1.Size;
            if (bitmap2.Size != size)
            {
                throw new ArgumentException(nameof(bitmap2) + ".Size");
            }
            int width = size.Width;
            int height = size.Height;
            if ((RowSummary?.Length ?? 0) != height)
            {
                RowSummary = new int[height];
            }
            else 
            {
                Array.Clear(RowSummary, 0, height);
            }
            if ((ColumnSummary?.Length ?? 0) != width)
            {
                ColumnSummary = new int[width];
            }
            else
            {
                Array.Clear(ColumnSummary, 0, width);
            }
            RowComparerImpl impl = new RowComparerImpl();
            int totalSummary = 0;
            var direct1 = bitmap1 as IBitmapRowDirect<int>;
            var direct2 = bitmap2 as IBitmapRowDirect<int>;
            bool hasDirect1 = !(direct1 is null);
            bool hasDirect2 = !(direct2 is null);
            int[] buffer1 = hasDirect1 ? null : new int[size.Width];
            int[] buffer2 = hasDirect2 ? null : new int[size.Width];
            ArraySegment<int> rowData1 = hasDirect1 ? default : new ArraySegment<int>(buffer1);
            ArraySegment<int> rowData2 = hasDirect2 ? default : new ArraySegment<int>(buffer2);
            ArraySegment<int> columnSummary = new ArraySegment<int>(ColumnSummary);
            for (int row = 0; row < height; ++row)
            {
                if (hasDirect1)
                {
                    rowData1 = direct1.GetRowDirect(row);
                }
                else
                {
                    bitmap1.CopyRow(row, buffer1, 0);
                }
                if (hasDirect2)
                {
                    rowData2 = direct2.GetRowDirect(row);
                }
                else
                {
                    bitmap2.CopyRow(row, buffer2, 0);
                }
                int thisRowSummary = impl.Compare(rowData1, rowData2, columnSummary);
                RowSummary[row] += thisRowSummary;
            }
            return totalSummary;
        }

        private static void _ValidateBitmapRect(IScalarBitmapInfo bitmapInfo, Rect rect)
        {
            Size size = bitmapInfo.Size;
            string msg = null;
            if (rect.Left < 0)
            {
                msg = $"Rect.Left";
            }
            else if (rect.Right > size.Width)
            {
                msg = $"Rect.Right";
            }
            else if (rect.Top < 0)
            {
                msg = $"Rect.Top";
            }
            else if (rect.Bottom > size.Height)
            {
                msg = $"Rect.Bottom";
            }
            if (!(msg is null))
            {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// <para>
        /// <see cref="PixelMismatchCounter"/> implementation of the 
        /// <see cref="IBitmapRowComparer"/> interface.
        /// </para>
        /// <inheritdoc cref="IBitmapRowComparer{TPixel, TSum}"/>
        /// </summary>
        /// <inheritdoc cref="IBitmapRowComparer{TPixel, TSum}"/>
        public class RowComparerImpl
            : IBitmapRowComparer<int, int>
        {
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public int Compare(ArraySegment<int> input1, ArraySegment<int> input2, ArraySegment<int> outItemDiff)
            {
                var arr1 = input1.Array;
                var arr2 = input2.Array;
                if (arr1 is null) _Throw(nameof(input1));
                if (arr2 is null) _Throw(nameof(input2));
                //
                int count = input1.Count;
                if (input2.Count != count) 
                { 
                    _Throw(nameof(input2) + ".Count"); 
                }
                //
                int start1 = input1.Offset;
                int start2 = input2.Offset;
                //
                var arrDiff = outItemDiff.Array;
                bool hasArrDiff = !(arrDiff is null);
                if (hasArrDiff && outItemDiff.Count != count)
                {
                    _Throw(nameof(outItemDiff) + ".Count");
                }
                int diffStart = outItemDiff.Offset;
                //
                int subTotal = 0;
                //
                // ====== Performance note ======
                // The conditional is hoisted out of the loop.
                // ======
                //
                if (hasArrDiff)
                {
                    for (int k = 0; k < count; ++k)
                    {
                        var value1 = arr1[start1 + k];
                        var value2 = arr2[start2 + k];
                        if (value1 != value2)
                        {
                            arrDiff[diffStart + k] += 1;
                            subTotal += 1;
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < count; ++k)
                    {
                        var value1 = arr1[start1 + k];
                        var value2 = arr2[start2 + k];
                        if (value1 != value2)
                        {
                            subTotal += 1;
                        }
                    }
                }
                return subTotal;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void _Throw(string name)
            {
                throw new ArgumentException(name);
            }
        }
    }
}
