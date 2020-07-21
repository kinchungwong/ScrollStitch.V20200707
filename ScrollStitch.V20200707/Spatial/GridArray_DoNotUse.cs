using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    /// <summary>
    /// A <see cref="GridArray"/> mimics a 2D array but allows (requires) the use of <see cref="CellIndex"/> 
    /// when accessing the array content.
    /// 
    /// When compared to a <see cref="Dictionary{TKey, TValue}"/> with <see cref="CellIndex"/> as key, 
    /// the benefits of a GridArray are: upfront memory allocation; lower management overhead.
    /// 
    /// </summary>
    [Obsolete("Use GridArray instead.")]
    public class GridArray_DoNotUse<T> 
        : IGridCollection
        , IGridCollection<T>
        , IReadOnlyGridCollection<T>
    {
        #region private
        private readonly T[] _array;
        private readonly int _width;
        private readonly int _height;
        #endregion

        public Grid Grid { get; }
        public int GridWidth => Grid.GridWidth;
        public int GridHeight => Grid.GridHeight;

        public T this[CellIndex ci]
        {
            get => _array[_CItoL(ci)];
            set => _array[_CItoL(ci)] = value;
        }

        public GridArray_DoNotUse(Grid grid)
        {
            Grid = grid;
            _width = grid.GridWidth;
            _height = grid.GridHeight;
            int area = _width * _height;
            _array = new T[area];
        }

        private int _CItoL(CellIndex ci)
        {
            return ci.CellY * _width + ci.CellX;
        }
    }
}
