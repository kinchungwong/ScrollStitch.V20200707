using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Collections
{
    using Data;
    using RowAccess;

    /// <summary>
    /// A specialized collection containing a list of bitmaps (via common interface 
    /// <see cref="IBitmapRowSource{T}"/>), where all bitmaps have the same size.
    /// 
    /// <para>
    /// This class is functionally equivalent to <see cref="UniformSizeArrayBitmapList{T}"/>,
    /// differing only in the choice of bitmap interface.
    /// </para>
    /// 
    /// <para>
    /// It is expressly allowed to insert bitmap instances that implement higher access levels. 
    /// In particular: <br/>
    /// ... <see cref="IBitmapRowAccess{T}"/> (which optionally provides write access), and  <br/>
    /// ... <see cref="IBitmapRowDirect{T}"/> (which provides direct zero-copy access) <br/>
    /// ... can be inserted into this collection. <br/>
    /// Users of this collection may use type checking to gain additional access levels. <br/>
    /// Refer to <see cref="ScrollStitch.V20200707.Imaging.RowAccess"/> namespace for more information.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Pixel type of the bitmaps.
    /// </typeparam>
    /// 
    public class UniformSizeRowSourceList<T>
        : IReadOnlyList<IBitmapRowSource<T>>
        where T : struct
    {
        private List<IBitmapRowSource<T>> _bitmaps;

        public IReadOnlyList<IBitmapRowSource<T>> Bitmaps => _bitmaps.AsReadOnly();

        public Size BitmapSize { get; private set; }

        public int Count => _bitmaps.Count;

        public IBitmapRowSource<T> this[int index] => _bitmaps[index];

        public UniformSizeRowSourceList()
        {
            _bitmaps = new List<IBitmapRowSource<T>>();
        }

        public void Add(IBitmapRowSource<T> bitmap)
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

        public IEnumerator<IBitmapRowSource<T>> GetEnumerator()
        {
            return ((IEnumerable<IBitmapRowSource<T>>)_bitmaps).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_bitmaps).GetEnumerator();
        }
    }
}
