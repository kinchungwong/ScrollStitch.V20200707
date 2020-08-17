using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.RectMaskTests
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;
    using ScrollStitch.V20200707.Text;

    public class RectRelationsTests
    {
        [Test(Description = "This is not a test. This is a basic demo of binary relations between rectangles via RectRelations.")]
        public void RunDemo()
        {
            const ulong allBitsSet64 = ~0uL;
            const uint allBitsSet32 = ~0u;
            const ushort allBitsSet16 = unchecked((ushort)allBitsSet32);
            const byte allBitsSet8 = unchecked((byte)allBitsSet32);
            int rowCount = 8;
            int colCount = 8;
            string RowColIndexAsBinaryString(int value)
            {
                if (value < 0)
                {
                    return "*";
                }
                return IntegerBaseFormatter.Format((uint)value, toBase: 2, minOutputWidth: 3, paddingCharOrDefault: '0');
            }
            const int numRelations = 7; // must match below.
            RectRelations.Identical rrIdentical = default;
            RectRelations.IdenticalNT rrIdenticalNT = default;
            RectRelations.Intersect rrIntersect = default;
            RectRelations.Encompassing rrEncompassing = default;
            RectRelations.EncompassingNT rrEncompassingNT = default;
            RectRelations.EncompassedBy rrEncompassedBy = default;
            RectRelations.EncompassedByNT rrEncompassedByNT = default;
            string TestFuncWithMaskType<MaskType>(MaskType maskOne, MaskType maskTwo)
                where MaskType : struct, IRectMaskArith<MaskType>
            {
                // IRectRelation{TStruct, TRectMask} is an interface that is self-constrained 
                // as well as being specific to one type of mask (bit width).
                // In order to pass generic arguments to it, we must use dynamic.
                dynamic dynamicOne = maskOne;
                dynamic dynamicTwo = maskTwo;
                char[] cs = new char[numRelations];
                cs[0] = (rrIdentical.TestMaybe(dynamicOne, dynamicTwo)) ? 'e' : '_';
                cs[1] = (rrIdenticalNT.TestMaybe(dynamicOne, dynamicTwo)) ? 'E' : '_';
                cs[2] = (rrIntersect.TestMaybe(dynamicOne, dynamicTwo)) ? '&' : '_';
                cs[3] = (rrEncompassing.TestMaybe(dynamicOne, dynamicTwo)) ? 'd' : '_';
                cs[4] = (rrEncompassingNT.TestMaybe(dynamicOne, dynamicTwo)) ? 'D' : '_';
                cs[5] = (rrEncompassedBy.TestMaybe(dynamicOne, dynamicTwo)) ? 's' : '_';
                cs[6] = (rrEncompassedByNT.TestMaybe(dynamicOne, dynamicTwo)) ? 'S' : '_';
                return new string(cs);
            }
            string TestFunc(int row, int col)
            {
                var maskOne128 = new RectMask128(allBitsSet64, (uint)row);
                var maskTwo128 = new RectMask128(allBitsSet64, (uint)col);
                var str128 = TestFuncWithMaskType(maskOne128, maskTwo128);
                //
                var maskOne64 = new RectMask64(allBitsSet32, (uint)row);
                var maskTwo64 = new RectMask64(allBitsSet32, (uint)col);
                var str64 = TestFuncWithMaskType(maskOne64, maskTwo64);
                //
                var maskOne32 = new RectMask32(allBitsSet16, (ushort)row);
                var maskTwo32 = new RectMask32(allBitsSet16, (ushort)col);
                var str32 = TestFuncWithMaskType(maskOne32, maskTwo32);
                //
                var maskOne16 = new RectMask16(allBitsSet8, (byte)row);
                var maskTwo16 = new RectMask16(allBitsSet8, (byte)col);
                var str16 = TestFuncWithMaskType(maskOne16, maskTwo16);
                //
                var maskOne8 = new RectMask8(allBitsSet8 & 0x0F, (byte)row);
                var maskTwo8 = new RectMask8(allBitsSet8 & 0x0F, (byte)col);
                var str8 = TestFuncWithMaskType(maskOne8, maskTwo8);
                //
                if (!string.Equals(str8, str16, StringComparison.Ordinal) ||
                    !string.Equals(str8, str32, StringComparison.Ordinal) ||
                    !string.Equals(str8, str64, StringComparison.Ordinal) ||
                    !string.Equals(str8, str128, StringComparison.Ordinal))
                {
                    Assert.Fail("To troubleshoot this test, perform interactive debugging on this test function.");
                }
                return str8;
            }
            string RowHeaderFunc(int row)
            {
                return RowColIndexAsBinaryString(row);
            }
            string ColumnHeaderFunc(int col)
            {
                return RowColIndexAsBinaryString(col);
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
