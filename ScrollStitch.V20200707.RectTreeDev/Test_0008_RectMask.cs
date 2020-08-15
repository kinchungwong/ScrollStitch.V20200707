using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.RectTreeDev
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;
    using ScrollStitch.V20200707.Text;

    public class Test_0008_RectMask
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0008_RectMask()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
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
            Console.WriteLine(Program.Banner);
            mlto.ToConsole();
            Console.WriteLine(Program.Banner);
            Program.PauseIfInteractive();
        }

        /// <summary>
        /// Tests a snippet of code that shows the unexpectedness of using the triviality-allowing
        /// version of using MaybeEncompassing.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_ShowEncompassTrivialityUnexpectedness()
        {
            Console.WriteLine(Program.Banner);
            const uint something = 1u;
            RectMask64 firstMask = new RectMask64(something, 32u);
            RectMask64 secondMask = new RectMask64(something, 0u);
            bool maybeInter12 = firstMask.MaybeIntersecting(secondMask);
            bool maybeEncomp12 = firstMask.MaybeEncompassing(secondMask);
            bool maybeEncompNT12 = firstMask.MaybeEncompassingNT(secondMask);
            if (!maybeInter12 && maybeEncomp12)
            {
                Console.WriteLine("Haha! Gotcha! (no NT)");
            }
            else
            {
                Console.WriteLine("This is fine, I'm okay with the events that are unfolding currently.");
            }
            if (!maybeInter12 && maybeEncompNT12)
            {
                Console.WriteLine("Haha! Gotcha! (with NT)");
            }
            Console.WriteLine(Program.Banner);
            Program.PauseIfInteractive();
        }
    }
}
