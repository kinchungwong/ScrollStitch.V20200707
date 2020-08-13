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
    using ScrollStitch.V20200707.Spatial.RectTreeInternals;

    public class Test_0002
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0002()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            int insertCount = 10_000_000;
            var rectGen = new RandomRectGenerator();
            Dictionary<int, Rect> dict = new Dictionary<int, Rect>();
            for (int insertIndex = 0; insertIndex < insertCount; ++insertIndex)
            {
                var rect = rectGen.NextRect();
                dict.Add(insertIndex, rect);
                if (Program.PrintVerbose)
                {
                    Console.WriteLine(rect);
                }
            }
            DateTime startTime = DateTime.Now;
            RectDictionary<int, Rect> rectDict = new RectDictionary<int, Rect>(dict);
            rectDict.Process();
            DateTime stopTime = DateTime.Now;
            Console.WriteLine($"{(stopTime - startTime).TotalMilliseconds:F6} msecs");
            if (Program.PrintVerbose)
            {
                RecursiveDiagnostics diag = new RecursiveDiagnostics(rectDict._rootNode);
                var output = diag.Generate();
                output.ToConsole();
                Program.PauseIfInteractive();
            }
        }
    }
}
