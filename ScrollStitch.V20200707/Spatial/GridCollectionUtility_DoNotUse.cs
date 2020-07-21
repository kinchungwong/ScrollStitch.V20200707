using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using ScrollStitch.V20200707.Utility;

    [Obsolete("Use namespace ScrollStitch.V20200707.Arrays instead.")]
    public static class GridCollectionUtility_DoNotUse
    {
        public static Array ToArray<TDest, TSource>(IReadOnlyGridCollection<TSource> source, DestType destType)
        {
            int width = source.GridWidth;
            int height = source.GridHeight;
            switch (destType)
            {
                case DestType.Array2_RowColumn:
                    {
                        var dest = new TDest[height, width];
                        CopyByRowColumn(source, dest);
                        return dest;
                    }
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public static void CopyByRowColumn<T1, T2>(IReadOnlyGridCollection<T1> source, T2[,] dest)
        {
            if (!_ValidateDestRank2(DestType.Array2_RowColumn, source, dest, out var ex))
            {
                throw ex;
            }
            int width = source.GridWidth;
            int height = source.GridHeight;
            for (int row = 0; row < height; ++row)
            {
                for (int column = 0; column < width; ++column)
                {
                    var ci = new CellIndex(column, row);
                    dest[row, column] = DynamicCast.Cast<T2>(source[ci]);
                }
            }
        }

        public static void CopyByRowColumn<T>(IReadOnlyGridCollection<T> source, T[,] dest)
        {
            if (!_ValidateDestRank2(DestType.Array2_RowColumn, source, dest, out var ex))
            {
                throw ex;
            }
            int width = source.GridWidth;
            int height = source.GridHeight;
            for (int row = 0; row < height; ++row)
            {
                for (int column = 0; column < width; ++column)
                {
                    var ci = new CellIndex(column, row);
                    dest[row, column] = source[ci];
                }
            }
        }

        public static void CopyByXandY<T1, T2>(IReadOnlyGridCollection<T1> source, T2[,] dest)
        {
            if (!_ValidateDestRank2(DestType.Array2_XandY, source, dest, out var ex))
            {
                throw ex;
            }
            int width = source.GridWidth;
            int height = source.GridHeight;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var ci = new CellIndex(x, y);
                    dest[x, y] = DynamicCast.Cast<T2>(source[ci]);
                }
            }
        }

        public static void CopyByXandY<T>(IReadOnlyGridCollection<T> source, T[,] dest)
        {
            if (!_ValidateDestRank2(DestType.Array2_XandY, source, dest, out var ex))
            {
                throw ex;
            }
            int width = source.GridWidth;
            int height = source.GridHeight;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var ci = new CellIndex(x, y);
                    dest[x, y] = source[ci];
                }
            }
        }

        public enum DestType
        { 
            Array2_RowColumn,
            Array2_XandY,
            Array1_RowMajor,
            Array1_ColumnMajor
        }

        private static bool _ValidateDestRank2(DestType destType, IGridCollection source, Array dest, out Exception ex)
        {
            ex = null;
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (dest is null)
            {
                throw new ArgumentNullException(nameof(dest));
            }
            if (dest.Rank != 2)
            {
                ex = new Exception(
                    $"Given Grid {source.Grid.GridSize} " + 
                    $"and DestType {destType}, " + 
                    $"expects array rank 2, actual array rank {dest.Rank}");
                return false;
            }
            int width = source.GridWidth;
            int height = source.GridHeight;
            int destLen0 = dest.GetLength(0);
            int destLen1 = dest.GetLength(1);
            int destWidth, destHeight, expectDestLen0, expectDestLen1;
            switch (destType)
            {
                case DestType.Array2_RowColumn:
                    destWidth = destLen1;
                    destHeight = destLen0;
                    expectDestLen0 = height;
                    expectDestLen1 = width;
                    break;
                case DestType.Array2_XandY:
                    destWidth = destLen0;
                    destHeight = destLen1;
                    expectDestLen0 = width;
                    expectDestLen1 = height;
                    break;
                default:
                    throw new Exception();
            }
            if (destWidth != width ||
                destHeight != height)
            {
                ex = new Exception(
                    $"Given Grid {source.Grid.GridSize} " +
                    $"and DestType {destType}, " +
                    $"expected array size [{expectDestLen0}, {expectDestLen1}], " + 
                    $"actual array size [{destLen0}, {destLen1}]");
                return false;
            }
            return true;
        }
    }
}
