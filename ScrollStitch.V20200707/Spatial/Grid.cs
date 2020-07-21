using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;

    /// <summary>
    /// Represents a subdivision of an input field (the coordinates space of an image)
    /// into a grid consisting of rows and columns of cells.
    /// 
    /// <para>
    /// See also: <seealso cref="AxisSubdiv"/>
    /// </para>
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Width of the input field; usually the width of an image.
        /// </summary>
        public int InputWidth { get; }

        /// <summary>
        /// Height of the input field; usually the height of an image.
        /// </summary>
        public int InputHeight { get; }

        /// <summary>
        /// Width of the grid; Number of grid columns.
        /// </summary>
        public int GridWidth { get; }

        /// <summary>
        /// Height of the grid; number of grid rows.
        /// </summary>
        public int GridHeight { get; }

        /// <summary>
        /// Input field size; usually the size of an image.
        /// </summary>
        public Size InputSize => new Size(InputWidth, InputHeight);

        /// <summary>
        /// Grid size; the number of rows and columns of cells inside the grid.
        /// </summary>
        public Size GridSize => new Size(GridWidth, GridHeight);

        /// <summary>
        /// The spatial sub-division of the horizontal axis.
        /// </summary>
        public AxisSubdiv HorzSubdiv { get; }

        /// <summary>
        /// The spatial sub-division of the vertical axis.
        /// </summary>
        public AxisSubdiv VertSubdiv { get; }

        /// <summary>
        /// The ranges of input X-coordinates corresponding to each horizontal subdivision.
        /// </summary>
        public IReadOnlyList<Range> HorzRanges => HorzSubdiv.Ranges;

        /// <summary>
        /// The ranges of input Y-coordinates corresponding to each vertical subdivision.
        /// </summary>
        public IReadOnlyList<Range> VertRanges => VertSubdiv.Ranges;

        /// <summary>
        /// Given an input point, find the corresponding cell and return its cell index.
        /// </summary>
        /// <param name="p">
        /// The input point (x and y coordinates)
        /// </param>
        /// <returns>
        /// </returns>
        public CellIndex this[Point p] => FindCell(p);

        /// <summary>
        /// Given a cell index, return the bounding rectangle.
        /// </summary>
        /// <param name="ci">The cell index (column and row)</param>
        /// <returns>
        /// </returns>
        public Rect this[CellIndex ci] => GetCellRect(ci);

        /// <summary>
        /// Constructor.
        /// 
        /// <para>
        /// Remark. It is usually more convenient to use the factory methods on <see cref="Factory"/>.
        /// </para>
        /// </summary>
        /// <param name="horzSubdiv"></param>
        /// <param name="vertSubdiv"></param>
        public Grid(AxisSubdiv horzSubdiv, AxisSubdiv vertSubdiv)
        {
            HorzSubdiv = horzSubdiv;
            VertSubdiv = vertSubdiv;
            InputWidth = horzSubdiv.InputLength;
            InputHeight = vertSubdiv.InputLength;
            GridWidth = horzSubdiv.Count;
            GridHeight = vertSubdiv.Count;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="grid"></param>
        public Grid(Grid grid)
        {
            HorzSubdiv = grid.HorzSubdiv;
            VertSubdiv = grid.VertSubdiv;
            InputWidth = grid.InputWidth;
            InputHeight = grid.InputHeight;
            GridWidth = grid.GridWidth;
            GridHeight = grid.GridHeight;
        }

        /// <summary>
        /// A static class providing factory methods for <see cref="Grid"/>.
        /// </summary>
        public static class Factory
        {
            public static Grid CreateNearlyUniform(Size inputSize, Size gridSize)
            {
                return CreateNearlyUniform(inputSize.Width, inputSize.Height, gridSize.Width, gridSize.Height);
            }

            public static Grid CreateNearlyUniform(int inputWidth, int inputHeight, int gridWidth, int gridHeight)
            {
                var horzSubdiv = AxisSubdivFactory.CreateNearlyUniform(inputWidth, gridWidth);
                var vertSubdiv = AxisSubdivFactory.CreateNearlyUniform(inputHeight, gridHeight);
                return new Grid(horzSubdiv, vertSubdiv);
            }

            public static Grid CreateApproxCellSize(Size inputSize, Size approxCellSize)
            {
                return CreateApproxCellSize(inputSize.Width, inputSize.Height, approxCellSize.Width, approxCellSize.Height);
            }

            public static Grid CreateApproxCellSize(int inputWidth, int inputHeight, int approxCellWidth, int approxCellHeight)
            {
                int gridWidth = (int)Math.Round((double)inputWidth / approxCellWidth);
                int gridHeight = (int)Math.Round((double)inputHeight / approxCellHeight);
                return CreateNearlyUniform(inputWidth, inputHeight, gridWidth, gridHeight);
            }

            public static Grid CreateLeftTopAligned(Size inputSize, Size cellSize)
            {
                return CreateLeftTopAligned(inputSize.Width, inputSize.Height, cellSize.Width, cellSize.Height);
            }

            public static Grid CreateLeftTopAligned(int inputWidth, int inputHeight, int cellWidth, int cellHeight)
            {
                var horzSubdiv = AxisSubdivFactory.CreateLeftAligned(inputWidth, cellWidth);
                var vertSubdiv = AxisSubdivFactory.CreateLeftAligned(inputHeight, cellHeight);
                return new Grid(horzSubdiv, vertSubdiv);
            }
        }

        public CellIndex FindCell(Point inputPoint)
        {
            if (!HorzSubdiv.TryFindAnyContainingRange(inputPoint.X, out var _, out int cellX) ||
                !VertSubdiv.TryFindAnyContainingRange(inputPoint.Y, out var _, out int cellY))
            {
                throw new InvalidOperationException();
            }
            return new CellIndex(cellX, cellY);
        }

        public Rect GetCellRect(CellIndex cellIndex)
        {
            Range hr = HorzRanges[cellIndex.CellX];
            Range vr = VertRanges[cellIndex.CellY];
            return new Rect(hr.Start, vr.Start, hr.Count, vr.Count);
        }

        public Rect GetContainingCellRect(Point inputPoint)
        {
            return GetCellRect(FindCell(inputPoint));
        }

        public void ForEach(Action<CellIndex, Rect> cellFunc)
        {
            ForEach((CellIndex ci) => cellFunc(ci, GetCellRect(ci)));
        }

        public void ForEach(Action<Rect> cellRectFunc)
        {
            ForEach((CellIndex ci) => cellRectFunc(GetCellRect(ci)));
        }

        public void ForEach(Action<CellIndex> cellFunc)
        {
            int gw = GridWidth;
            int gh = GridHeight;
            for (int cy = 0; cy < gh; ++cy)
            {
                for (int cx = 0; cx < gw; ++cx)
                {
                    var ci = new CellIndex(cx, cy);
                    cellFunc(ci);
                }
            }
        }
    }
}
