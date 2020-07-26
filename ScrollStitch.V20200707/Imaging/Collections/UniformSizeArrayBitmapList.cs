using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Collections
{
    using Data;

    /// <summary>
    /// A specialized collection containing a list of bitmaps (via common interface 
    /// <see cref="IArrayBitmap{T}"/>), where all bitmaps have the same size.
    /// 
    /// <para>
    /// This class is functionally equivalent to <see cref="UniformSizeRowSourceList{T}"/>,
    /// differing only in the choice of bitmap interface.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Pixel type of the bitmaps.
    /// </typeparam>
    /// 
    public class UniformSizeArrayBitmapList<T>
        : IReadOnlyList<IArrayBitmap<T>>
        where T : struct
    {
        private List<IArrayBitmap<T>> _bitmaps;

        public IReadOnlyList<IArrayBitmap<T>> Bitmaps => _bitmaps.AsReadOnly();

        public Size BitmapSize { get; private set; }

        public int Count => _bitmaps.Count;

        public IArrayBitmap<T> this[int index] => _bitmaps[index];

        public UniformSizeArrayBitmapList()
        {
            _bitmaps = new List<IArrayBitmap<T>>();
        }

        public void Add(IArrayBitmap<T> bitmap)
        {
            if (_bitmaps.Count == 0)
            {
                BitmapSize = bitmap.Size;
            }
            else if (bitmap.Size != BitmapSize)
            {
                throw new InvalidOperationException("Bitmap size mismatch.");
            }
            _bitmaps.Add(bitmap);
        }

        public IEnumerator<IArrayBitmap<T>> GetEnumerator()
        {
            return ((IEnumerable<IArrayBitmap<T>>)_bitmaps).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_bitmaps).GetEnumerator();
        }
    }
}
