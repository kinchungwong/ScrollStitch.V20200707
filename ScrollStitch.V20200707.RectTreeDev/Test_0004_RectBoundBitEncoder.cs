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

    public class Test_0004_RectBoundBitEncoder
    {
        public const bool VerificationMode = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0004_RectBoundBitEncoder()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            var boundRect = new Rect(-2048, -2048, 4092, 4092);
            var encoder = new RectBoundBitEncoder(boundRect, 132);
            int rectCount = 100_000;
            int maxTestCount = 100_000_000;
            var rectGen = new RandomRectGenerator();
            var rects = new List<Rect>(capacity: rectCount);
            var flags = new List<ulong>(capacity: rectCount);
            for (int k = 0; k < rectCount; ++k)
            {
                rects.Add(rectGen.NextRect());
            }
            DateTime timestamp1 = DateTime.Now;
            for (int k = 0; k < rectCount; ++k)
            {
                flags.Add(encoder.Encode(rects[k]));
            }
            DateTime timestamp2 = DateTime.Now;
            int testCount = 0;
            int overlapCount = 0;
            int overlapPreciseCount = 0;
            for (int k1 = 1; k1 < rectCount; ++k1)
            {
                for (int k0 = 0; k0 < k1; ++k0)
                {
                    ++testCount;
                    bool hasOverlapByBit = encoder.Test(flags[k0], flags[k1]);
                    if (hasOverlapByBit)
                    {
                        ++overlapCount;
                    }
                    if (VerificationMode)
                    {
                        bool hasOverlapByCoords = InternalRectUtility.NoInline.HasIntersect(rects[k0], rects[k1]);
                        if (hasOverlapByCoords)
                        {
                            ++overlapPreciseCount;
                        }
                        if (!hasOverlapByBit && hasOverlapByCoords)
                        {
                            Program.BreakIfDebuggerAttached();
                        }
                    }
                    if (testCount >= maxTestCount)
                    {
                        break;
                    }
                }
                if (testCount >= maxTestCount)
                {
                    break;
                }
            }
            DateTime timestamp3 = DateTime.Now;
            TimeSpan encodeTime = (timestamp2 - timestamp1);
            TimeSpan flagTestTime = (timestamp3 - timestamp2);
            if (Program.PrintCriticalTiming)
            {
                Console.WriteLine($"Tests performed: {testCount}");
                Console.WriteLine($"Overlap count: {overlapCount}");
                if (VerificationMode)
                {
                    Console.WriteLine($"Precise overlap count: {overlapPreciseCount}");
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
