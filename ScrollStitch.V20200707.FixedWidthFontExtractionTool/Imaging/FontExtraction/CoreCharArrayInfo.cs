using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.FontExtraction
{
    using Data;
    using Text;

    public sealed class CoreCharArrayInfo 
        : ICharArray
    {
        public Range CharRange { get; }

        public int RowCount { get; }

        public int ColumnCount { get; }

        public char[,] CharArray { get; }

        public CoreCharArrayInfo(Range charRange, int rowCount, int columnCount)
        {
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }
            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            }
            if (charRange.Count > rowCount * columnCount)
            {
                throw new ArgumentException(
                    "The specified row count and column count is insufficient for showing all characters.");
            }
            CharRange = charRange;
            RowCount = rowCount;
            ColumnCount = columnCount;
            CharArray = new char[rowCount, columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < columnCount; ++col)
                {
                    int offset = row * columnCount + col;
                    int charValue = charRange.Start + offset;
                    char c = (charValue < charRange.Stop) ? (char)charValue : ' ';
                    CharArray[row, col] = c;
                }
            }
        }
    }
}
