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
    /// <see cref="SpacedCharArrayInfo"/> takes a <see cref="DuplexCharArrayInfo"/> as input, and 
    /// inserts blank rows and columns after each row and column in the input.
    /// 
    /// <para>
    /// Given char <c>C</c>, <br/>
    /// The 2-by-2 cell is generated:<br/>
    /// <c>C _ </c> <br/>
    /// <c>_ _ </c> <br/>
    /// Where <c>C</c> denotes the original character, and <c>_</c> denotes a blank space.
    /// </para>
    /// </summary>
    public sealed class SpacedCharArrayInfo 
        : ICharArray
    {
        public DuplexCharArrayInfo DuplexInfo { get; }

        public PaddedCharArrayInfo PaddedInfo => DuplexInfo.PaddedInfo;

        public CoreCharArrayInfo CoreInfo => PaddedInfo.CoreInfo;

        /// <summary>
        /// The horizontal duplex factor. This is a constant value of two (2).
        /// 
        /// <para>
        /// Thus, the value of <see cref="ColumnCount"/> is four times that of <see cref="PaddedCharArrayInfo.ColumnCount"/>.
        /// </para>
        /// 
        /// <para>
        /// Remark. Do not change this value. The initialization code is hard-coded for this value.
        /// </para>
        /// </summary>
        public int HorizontalFactor => 2;

        /// <summary>
        /// The vertical duplex factor. This is a constant value of two (2).
        /// 
        /// <para>
        /// Thus, the value of <see cref="RowCount"/> is four times that of <see cref="PaddedCharArrayInfo.RowCount"/>.
        /// </para>
        /// 
        /// <para>
        /// Remark. Do not change this value. The initialization code is hard-coded for this value.
        /// </para>
        /// </summary>
        public int VerticalFactor => 2;

        public int RowCount { get; }

        public int ColumnCount { get; }

        public char[,] CharArray { get; }

        public SpacedCharArrayInfo(DuplexCharArrayInfo duplexInfo)
        {
            if (duplexInfo is null)
            {
                throw new ArgumentNullException(nameof(duplexInfo));
            }
            DuplexInfo = duplexInfo;
            RowCount = duplexInfo.RowCount * VerticalFactor;
            ColumnCount = duplexInfo.ColumnCount * HorizontalFactor;
            CharArray = new char[RowCount, ColumnCount];
            for (int row = 0; row < RowCount; ++row)
            {
                int sourceRow = row / VerticalFactor;
                int spacingRow = row % VerticalFactor;
                for (int col = 0; col < ColumnCount; ++col)
                {
                    int sourceCol = col / HorizontalFactor;
                    int spacingCol = col % HorizontalFactor;
                    if (spacingRow == 0 && spacingCol == 0)
                    {
                        CharArray[row, col] = duplexInfo.CharArray[sourceRow, sourceCol];
                    }
                    else
                    {
                        CharArray[row, col] = ' ';
                    }
                }
            }
        }
    }
}
