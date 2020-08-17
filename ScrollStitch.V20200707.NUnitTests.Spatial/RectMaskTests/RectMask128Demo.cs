using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.RectMaskTests
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;
    using ScrollStitch.V20200707.Text;

    public class RectMask128Demo
    {
        [Test(Description="This is not a test. This is a basic demo of bitwise testing methods implemented in RectMask128.")]
        public void RunDemo()
        {
            // ======
            // Some of the rectangular relations such as "identical" and "encompassed-by" are not defined on the
            // IRectMaskArith object itself as they are implemented through a rearrangement of arguments. 
            // For a more thorough demonstration of all rectangular relations, refer to RectRelationsTests class.
            // ======
            // This demo class only shows RectMask128. For a demo that uses all of the concrete IRectMaskArith 
            // implementations, refer to RectRelationsTests class.
            // ======
            /// <see cref="RectRelationsTests"/>
            // ======
            const ulong allBitsSet64 = ~0uL;
            int rowCount = 16;
            int colCount = 16;
            string ToBinaryString(ulong value)
            {
                return IntegerBaseFormatter.Format(value, 2, 4, '0');
            }
            string TestFunc(int row, int col)
            {
                RectMask128 maskOne = new RectMask128(allBitsSet64, (uint)row);
                RectMask128 maskTwo = new RectMask128(allBitsSet64, (uint)col);
                char[] cs = new char[3];
                cs[0] = (maskOne.MaybeIntersecting(maskTwo)) ? 'I' : '_';
                cs[1] = (maskOne.MaybeEncompassingNT(maskTwo)) ? 'E' : '_'; // "strong E"
                cs[2] = (maskOne.MaybeEncompassing(maskTwo)) ? 'e' : '_'; // "weak E"
                return new string(cs);
            }
            string RowHeaderFunc(int row)
            {
                return ToBinaryString((uint)row);
            }
            string ColumnHeaderFunc(int col)
            {
                return ToBinaryString((uint)col);
            }
            var tgs = TextGridSource.Create(rowCount, colCount, TestFunc);
            var tgs2 = TextGridSource.CreateWithHeaders(tgs, RowHeaderFunc, ColumnHeaderFunc);
            var tgf = new TextGridFormatter(tgs2);
            var mlto = new MultiLineTextOutput();
            tgf.Indent = 2;
            tgf.ColumnSpacing = 2;
            tgf.Generate(mlto);
            mlto.ToConsole();
        }
    }
}
