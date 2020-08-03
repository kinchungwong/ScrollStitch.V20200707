using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.FontExtraction
{
    using Data;
    using Text;

    /// <summary>
    /// This class creates a text template that is used to assist in image font extraction.
    /// 
    /// <para>
    /// This class prints out all ASCII printable characters (in the char range 32 to 127)
    /// in a template with a predefined layout.
    /// </para>
    /// 
    /// <para>
    /// After rendering the text with a fixed-width ASCII font, a screenshot of the rendered
    /// text can then be processed with <see cref="FixedWidthFontRectExtractor"/>.
    /// </para>
    /// 
    /// <para>
    /// Reminder. <br/>
    /// <see cref="FixedWidthFontRectExtractor"/> requires access to the same text template 
    /// class that generated the text captured in the screenshot. Usually, the character array
    /// from <see cref="DuplexCharArrayInfo"/> should be used. 
    /// </para>
    /// 
    /// <para>
    /// The main difference between <see cref="DuplexCharArrayInfo"/> and <see cref="CoreCharArrayInfo"/> 
    /// is that alignment marks are added outside the four corners of the text array, and that each 
    /// character is duplicated in a 2-by-2 cell. These modifications to the character array are needed 
    /// by the <see cref="FixedWidthFontRectExtractor"/> algorithm.
    /// </para>
    /// </summary>
    /// 
    public sealed class CoreCharArrayInfo 
        : ICharArray
    {
        public Range CharRange { get; }

        public int RowCount { get; }

        public int ColumnCount { get; }

        public char[,] CharArray { get; }

        public int? ShuffleSeed { get; }

        public char FillChar { get; }

        public CoreCharArrayInfo(Range charRange, int rowCount, int columnCount, char fillChar = ' ', int? shuffleSeed = null)
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
            ShuffleSeed = shuffleSeed;
            FillChar = fillChar;
        }

        public void Process()
        {
            int rows = RowCount;
            int cols = ColumnCount;
            int cellCount = rows * cols;
            char[] cs = new char[cellCount];
            for (int charOffset = 0; charOffset < cellCount; ++charOffset)
            {
                cs[charOffset] = 
                    (charOffset < CharRange.Count) 
                    ? (char)(charOffset + CharRange.Start) 
                    : FillChar;
            }
            if (ShuffleSeed.HasValue)
            {
                _Shuffle(cs, ShuffleSeed.Value);
            }
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    int charOffset = row * cols + col;
                    CharArray[row, col] = cs[charOffset];
                }
            }
        }

        private static void _Shuffle(char[] cs, int seed)
        {
            var random = new Random(seed);
            int length = cs.Length;
            for (int ko = 1; ko < length; ++ko)
            {
                int ki = random.Next(ko + 1);
                if (ki != ko)
                {
                    char swap = cs[ki];
                    cs[ki] = cs[ko];
                    cs[ko] = swap;
                }
            }
        }
    }
}
