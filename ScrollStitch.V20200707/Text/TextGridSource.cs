using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public static class TextGridSource
    {
        public static ITextGridSource Create(string[,] textArray)
        {
            return new ArrayTextGridSource(textArray);
        }

        public static ITextGridSource Create(int rowCount, int columnCount, 
            Func<int, int, string> getTextFunc)
        {
            return new FunctionTextGridSource(rowCount, columnCount, getTextFunc);
        }

        public static ITextGridSource CreateWithHeaders(ITextGridSource source, 
            Func<int, string> rowHeaderFunc, Func<int, string> columnHeaderFunc)
        {
            return new RowHeaderTextGridSource(source, rowHeaderFunc, columnHeaderFunc);
        }

        public class ArrayTextGridSource
            : ITextGridSource
        {
            public string[,] TextArray { get; }

            public int RowCount => TextArray.GetLength(0);

            public int ColumnCount => TextArray.GetLength(1);

            public string GetItem(int row, int column)
            {
                return TextArray[row, column];
            }

            public ArrayTextGridSource(string[,] textArray)
            {
                TextArray = textArray;
            }
        }

        public class FunctionTextGridSource
            : ITextGridSource
        {
            public delegate string GetTextItemDelegate(int row, int column);

            public GetTextItemDelegate GetTextFunc { get; }

            public int RowCount { get; }

            public int ColumnCount { get; }

            public FunctionTextGridSource(int rowCount, int columnCount, GetTextItemDelegate getTextFunc)
            {
                RowCount = rowCount;
                ColumnCount = columnCount;
                GetTextFunc = getTextFunc;
            }

            public FunctionTextGridSource(int rowCount, int columnCount, Func<int, int, string> getTextFunc)
                : this(rowCount, columnCount, new GetTextItemDelegate(getTextFunc))
            {
            }

            public string GetItem(int row, int column)
            {
                return GetTextFunc(row, column);
            }
        }

        /// <summary>
        /// Given an <see cref="ITextGridSource"/> instance, this class inserts a row header to the left 
        /// (as the first column), and inserts a column header to the top (as the first row).
        /// </summary>
        public class RowHeaderTextGridSource
            : ITextGridSource
        {
            public delegate string GetHeaderForRowDelegate(int row);
            public delegate string GetHeaderForColumnDelegate(int column);

            public ITextGridSource Source { get; }

            public GetHeaderForRowDelegate HeaderForRow { get; }

            public GetHeaderForColumnDelegate HeaderForColumn { get; }

            public bool HasHeaderForRow => !(HeaderForRow is null);

            public bool HasHeaderForColumn => !(HeaderForColumn is null);

            public int RowCount => _sourceRowCount + (HasHeaderForColumn ? 1 : 0);

            public int ColumnCount => _sourceColumnCount + (HasHeaderForRow ? 1 : 0);

            #region private
            private int _sourceRowCount;
            private int _sourceColumnCount;
            #endregion

            public RowHeaderTextGridSource(ITextGridSource source, 
                GetHeaderForRowDelegate headerForRow,
                GetHeaderForColumnDelegate headerForColumn)
            {
                Source = source;
                HeaderForRow = headerForRow;
                HeaderForColumn = headerForColumn;
                _sourceRowCount = source.RowCount;
                _sourceColumnCount = source.ColumnCount;
            }

            public RowHeaderTextGridSource(ITextGridSource source,
                Func<int, string> headerForRow,
                Func<int, string> headerForColumn)
                : this(source, 
                      (headerForRow is null) ? null : new GetHeaderForRowDelegate(headerForRow),
                      (headerForColumn is null) ? null : new GetHeaderForColumnDelegate(headerForColumn))
            {
            }

            public string GetItem(int row, int column)
            {
                if (HasHeaderForColumn)
                {
                    row -= 1;
                }
                if (HasHeaderForRow)
                {
                    column -= 1;
                }
                if (row >= _sourceRowCount)
                {
                    throw new IndexOutOfRangeException(message: nameof(row));
                }
                if (column >= _sourceColumnCount)
                {
                    throw new IndexOutOfRangeException(message: nameof(column));
                }
                if (row >= 0 && column >= 0)
                {
                    return Source.GetItem(row, column);
                }
                if (row == -1 && column == -1)
                {
                    return string.Empty;
                }
                else if (row == -1)
                {
                    if (!HasHeaderForColumn)
                    {
                        throw new InvalidOperationException(nameof(HeaderForColumn));
                    }
                    return HeaderForColumn(column);
                }
                else if (column == -1)
                {
                    if (!HasHeaderForRow)
                    {
                        throw new InvalidOperationException(nameof(HeaderForRow));
                    }
                    return HeaderForRow(row);
                }
                throw new Exception();
            }
        }
    }
}
