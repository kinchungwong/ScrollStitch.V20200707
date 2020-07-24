using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowCompare
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.RowAccess;

    /// <summary>
    /// Performs a pixel-wise bitmap comparison and gathers row-based and column-based summary
    /// statistics.
    /// </summary>
    /// 
    /// <typeparam name="TPixel">
    /// The type of pixel values to be compared.
    /// </typeparam>
    /// 
    /// <typeparam name="TSum">
    /// The type to be used for reporting the summary pixel differences per row and column.
    /// </typeparam>
    /// 
    public interface IBitmapComparer<TPixel, TSum>
        where TPixel : struct
        where TSum : struct
    {
        /// <summary>
        /// The first of the two bitmaps to be compared.
        /// 
        /// <para>
        /// If the pixel data comes from an <see cref="IBitmapRowSource{int}"/> which is not an
        /// <see cref="IntBitmap"/>, the property <see cref="BitmapSource1"/> may be used instead.
        /// In this case, the <see cref="Bitmap1"/> property may be <see langword="null"/>.
        /// </para>
        /// </summary>
        IntBitmap Bitmap1 { get; }

        /// <summary>
        /// The second of the two bitmaps to be compared.
        /// 
        /// <para>
        /// If the pixel data comes from an <see cref="IBitmapRowSource{int}"/> which is not an
        /// <see cref="IntBitmap"/>, the property <see cref="BitmapSource2"/> may be used instead.
        /// In this case, the <see cref="Bitmap2"/> property may be <see langword="null"/>.
        /// </para>
        /// </summary>
        IntBitmap Bitmap2 { get; }

        /// <summary>
        /// A cropping rectangle that is applied to the first bitmap.
        /// </summary>
        Rect Rect1 { get; }

        /// <summary>
        /// A cropping rectangle that is applied to the second bitmap.
        /// </summary>
        Rect Rect2 { get; }

        /// <summary>
        /// An alternative source of bitmap pixels that is not an <see cref="IntBitmap"/>.
        /// </summary>
        IBitmapRowSource<int> BitmapSource1 { get; }

        /// <summary>
        /// An alternative source of bitmap pixels that is not an <see cref="IntBitmap"/>.
        /// </summary>
        IBitmapRowSource<int> BitmapSource2 { get; }

        /// <summary>
        /// A summary statistics that is computed for each bitmap row.
        /// 
        /// <para>
        /// After computation, the length of this array will be equal to the bitmap height.
        /// </para>
        /// </summary>
        int[] RowSummary { get; set; }

        /// <summary>
        /// A summary statistics that is computed for each bitmap column.
        /// 
        /// <para>
        /// After computation, the length of this array will be equal to the bitmap width.
        /// </para>
        /// </summary>
        int[] ColumnSummary { get; set; }

        /// <summary>
        /// Specifies the first bitmap for comparison. Cropping is not performed.
        /// </summary>
        /// <param name="bitmap">
        /// </param>
        void SetBitmap1(IntBitmap bitmap);

        /// <summary>
        /// Specifies the first bitmap for comparison. Cropping is applied.
        /// </summary>
        /// <param name="bitmap">
        /// </param>
        void SetBitmap1(IntBitmap bitmap, Rect rect);

        /// <summary>
        /// Specifies an <see cref="IBitmapRowSource{int}"/> to be used as the 
        /// first source of pixels.
        /// </summary>
        /// <param name="rowSource"></param>
        void SetBitmap1(IBitmapRowSource<int> rowSource);

        /// <summary>
        /// Specifies the second bitmap for comparison. Cropping is not performed.
        /// </summary>
        /// <param name="bitmap">
        /// </param>
        void SetBitmap2(IntBitmap bitmap);

        /// <summary>
        /// Specifies the second bitmap for comparison. Cropping is applied.
        /// </summary>
        /// <param name="bitmap">
        /// </param>
        void SetBitmap2(IntBitmap bitmap, Rect rect);

        /// <summary>
        /// Specifies an <see cref="IBitmapRowSource{int}"/> to be used as the 
        /// second source of pixels.
        /// </summary>
        /// <param name="rowSource"></param>
        void SetBitmap2(IBitmapRowSource<int> rowSource);

        /// <summary>
        /// Compares the two bitmaps and computes the row and column summaries.
        /// 
        /// <para>
        /// Upon completion, the properties <see cref="RowSummary"/> and <see cref="ColumnSummary"/> 
        /// will be populated.
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// The sum of differences over the entire bitmap.
        /// </returns>
        /// 
        TSum Compare();
    }
}
