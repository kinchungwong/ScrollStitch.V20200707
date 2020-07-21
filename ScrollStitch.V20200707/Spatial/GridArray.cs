using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Arrays;

    public class GridArray<T>
        : IGridCollection
        , IGridCollection<T>
        , IReadOnlyGridCollection<T>
        , IArray2<T> 
        , IReadOnlyArray2<T>
        , IArray2Info
    {
        public Grid Grid { get; }

        public IArray2<T> Array { get; }

        public int GridWidth => Grid.GridWidth;

        public int GridHeight => Grid.GridHeight;

        public T this[CellIndex ci]
        {
            get => Array[ci.CellY, ci.CellX];
            set => Array[ci.CellY, ci.CellX] = value;
        }

        #region explicit properties IArray2<T>
        int IArray2Info.Length => Array.Length;

        int IArray2Info.Length0 => Array.Length0;

        int IArray2Info.Length1 => Array.Length1;

        T IArray2<T>.this[int idx0, int idx1]
        {
            get => Array[idx0, idx1];
            set => Array[idx0, idx1] = value;
        }

        T IReadOnlyArray2<T>.this[int idx0, int idx1]
        {
            get => Array[idx0, idx1];
        }
        #endregion

        public GridArray(Grid grid, IArray2<T> array)
        {
            if (array.Length0 != grid.GridHeight ||
                array.Length1 != grid.GridWidth)
            {
                throw new ArgumentException();
            }
            Grid = grid;
            Array = array;
        }

        public GridArray(Grid grid, T[,] array)
            : this(grid, ArrayWrapperUtility.Wrap(array))
        { 
        }

        public GridArray(Grid grid)
            : this(grid, new T[grid.GridHeight, grid.GridWidth])
        {
        }

        public GridArray(Grid grid, Func<CellIndex, T> cellInitFunc)
            : this(grid, new T[grid.GridHeight, grid.GridWidth])
        {
            Grid.ForEach(
                (CellIndex ci) =>
                {
                    this[ci] = cellInitFunc(ci);
                });
        }

        public GridArray(Grid grid, T initValue)
            : this(grid, new T[grid.GridHeight, grid.GridWidth])
        {
            Grid.ForEach(
                (CellIndex ci) =>
                {
                    this[ci] = initValue;
                });
        }


        public void ForEach(Action<CellIndex, T> cellFunc)
        {
            Grid.ForEach(
                (CellIndex ci) =>
                {
                    cellFunc(ci, this[ci]);
                });
        }

        #region IArray2Info explicit methods
        int IArray2Info.GetLength(int dim)
        {
            return Array.GetLength(dim);
        }
        #endregion
    }
}
