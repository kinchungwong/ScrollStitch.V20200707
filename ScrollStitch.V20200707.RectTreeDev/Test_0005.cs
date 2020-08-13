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
    using IntegerBaseFormatter = ScrollStitch.V20200707.Text.IntegerBaseFormatter;

    public class Test_0005
    {
        public const bool VerificationMode = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0005()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            var boundRect = new Rect(0, 0, 16, 16);
            int stepSize = 3;
            for (int y = -1; y <= 16; ++y)
            {
                for (int x = -1; x <= 16; ++x)
                {
                    var rectToEncode = new Rect(x, y, 1, 1);
                    bool b = RectMaskUtility.TryEncodeRect(boundRect, stepSize, rectToEncode, out ulong xmask, out ulong ymask);
                    string xstr = IntegerBaseFormatter.Format(xmask, 2, 20);
                    string ystr = IntegerBaseFormatter.Format(ymask, 2, 20);
                    string bstr = b ? "T" : "F";
                    Console.WriteLine($"(x={x,2}, y={y,2}) {bstr} {xstr} {ystr}");
                }
            }
            Program.PauseIfInteractive();
        }
    }
}
