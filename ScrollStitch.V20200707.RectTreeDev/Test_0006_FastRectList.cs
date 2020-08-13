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

    public class Test_0006_FastRectList
    {
        public const bool VerificationMode = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0006_FastRectList()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            var boundRect = new Rect(-2048, -2048, 4096, 4096);
            //int rectCount = 1;
            //int maxTestCount = 1;
            int rectCount = 100_000;
            int maxTestCount = 1_000_000_000;
            var rectGen = new RandomRectGenerator();
            FastRectList fastRectList = new FastRectList(boundRect, capacity: rectCount);
            // We still precompute the rectangles because random rectangle generation is actually a 
            // time-consuming process.
            var rects = new List<Rect>(capacity: rectCount);
            for (int k = 0; k < rectCount; ++k)
            {
                rects.Add(rectGen.NextRect());
            }
            DateTime timestamp1 = DateTime.Now;
#if false
            for (int k = 0; k < rectCount; ++k)
            {
                fastRectList.Add(rects[k]);
            }
#else
            fastRectList.AddRange(rects);
#endif
            DateTime timestamp2 = DateTime.Now;
            int testCount = 0;
            int overlapCount = 0;
            for (int k = 0; k < rectCount; ++k)
            {
                Rect r = rects[k];
#if false
                foreach (int k2 in fastRectList.Enumerate(r))
                {
                    ++overlapCount;
                }
#else
                overlapCount += fastRectList.GetCount(r);
#endif
                testCount += rectCount;
                if (testCount >= maxTestCount)
                {
                    break;
                }
            }
            DateTime timestamp3 = DateTime.Now;
            int verifyCount = 0;
            int verifyOverlapCount = 0;
            if (VerificationMode)
            {
                for (int k = 0; k < rectCount; ++k)
                {
                    Rect r = rects[k];
                    for (int k2 = 0; k2 < rectCount; ++k2)
                    {
                        Rect r2 = rects[k2];
                        if (InternalRectUtility.NoInline.HasIntersect(r, r2))
                        {
                            ++verifyOverlapCount;
                        }
                    }
                    verifyCount += rectCount;
                    if (verifyCount >= maxTestCount)
                    {
                        break;
                    }
                }
                if (overlapCount != verifyOverlapCount)
                {
                    Program.BreakIfDebuggerAttached();
                }
            }
            TimeSpan encodeTime = (timestamp2 - timestamp1);
            TimeSpan flagTestTime = (timestamp3 - timestamp2);
            if (Program.PrintCriticalTiming)
            {
                Console.WriteLine($"Tests performed: {testCount}");
                Console.WriteLine($"Overlap count: {overlapCount}");
                if (VerificationMode &&
                    (overlapCount != verifyOverlapCount))
                {
                    Console.WriteLine(Program.Banner);
                    Console.WriteLine("###### FAILED VERIFICATION ######");
                    Console.WriteLine(Program.Banner);
                }
                // 
                double encodeTimeFloat = encodeTime.TotalMilliseconds;
                double encodePerCall = encodeTimeFloat / rectCount;
                double flagTestTimeFloat = flagTestTime.TotalMilliseconds;
                double flagTestPerCall = flagTestTimeFloat / testCount;
                //
                Console.WriteLine($"{nameof(encodeTime)}: {encodeTimeFloat:F6} msecs / {rectCount} = {encodePerCall:F9}");
                Console.WriteLine($"{nameof(flagTestTime)}: {flagTestTimeFloat:F6} msecs / {testCount} = {flagTestPerCall:F9}");
            }
            Program.PauseIfInteractive();
        }
    }
}
