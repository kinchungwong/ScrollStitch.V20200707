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
    /// Apply padding to the char array.
    /// 
    /// <para>
    /// At the four corners of the padded array, this class will insert an anchor mark of a plus 
    /// sign (<c>'+'</c>). The rest of the padding area will be left as blank. Thus, the four 
    /// anchor marks will become the only piece of text occupying the first and the last rows, 
    /// and the first and the last columns.
    /// </para>
    /// 
    /// <para>
    /// Illustration: <br/>
    /// <c>+ _ _ _ +</c> <br/>
    /// <c>_ t t t _</c> <br/>
    /// <c>+ _ _ _ +</c>
    /// </para>
    /// 
    /// <para>
    /// Legend: <br/>
    /// <c>'+'</c> is the anchor mark inserted by this class. <br/>
    /// <c>'_'</c> is a blank space. <br/>
    /// <c>'t'</c> represents the content from the <see cref="CoreCharArrayInfo"/>
    /// </para>
    /// </summary>
    public sealed class PaddedCharArrayInfo
        : ICharArray
    {
        public CoreCharArrayInfo CoreInfo { get; }

        /// <summary>
        /// Thickness of padding on each side (left, right, top, bottom) of the array.
        /// 
        /// <para>
        /// Remark. Do not change this value. The initialization code is hard-coded to
        /// generate a thickness of one.
        /// </para>
        /// </summary>
        public int PaddingThickness => 1;

        public int RowCount { get; }

        public int ColumnCount { get; }

        public char[,] CharArray { get; }

        public PaddedCharArrayInfo(CoreCharArrayInfo coreInfo)
        {
            if (coreInfo is null)
            {
                throw new ArgumentNullException(nameof(coreInfo));
            }
            CoreInfo = coreInfo;
            RowCount = CoreInfo.RowCount + PaddingThickness * 2;
            ColumnCount = CoreInfo.ColumnCount + PaddingThickness * 2;
            CharArray = new char[RowCount, ColumnCount];
            bool CheckIsPaddingRow(int row) => (row == 0 || row == RowCount - 1);
            bool CheckIsPaddingCol(int col) => (col == 0 || col == ColumnCount - 1);
            for (int row = 0; row < RowCount; ++row)
            {
                bool padR = CheckIsPaddingRow(row);
                for (int col = 0; col < ColumnCount; ++col)
                {
                    bool padC = CheckIsPaddingCol(col);
                    if (padR && padC)
                    {
                        CharArray[row, col] = '+';
                    }
                    else if (padR || padC)
                    {
                        CharArray[row, col] = ' ';
                    }
                    else
                    {
                        if (row < 1 || col < 1)
                        {
                            // impossible, unless logic error within this function
                            throw new Exception();
                        }
                        CharArray[row, col] = CoreInfo.CharArray[row - 1, col - 1];
                    }
                }
            }
        }
    }
}
