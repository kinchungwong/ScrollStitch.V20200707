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

    public class Test_0007_FastRectNode
    {
        public const bool VerificationMode = true;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0007_FastRectNode()
        {
        }

        /// <summary>
        /// Benchmark for adding rectangles to the data structure.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_AdditionTest()
        {
            var boundRect = new Rect(-2048, -2048, 4096, 4096);
#if false
            int rectCount = 1;
#elif false
            int rectCount = 100;
#elif false
            int rectCount = 1000;
#elif false
            int rectCount = 10_000;
#elif true
            int rectCount = 100_000;
#elif false
            int rectCount = 1_000_000;
#elif false
            int rectCount = 10_000_000;
#elif false
            int rectCount = 100_000_000;
#elif false
            int rectCount = 1_000_000_000;
#elif false
            int rectCount = 2_147_000_000;
#endif
            var rectGen = new RandomRectGenerator();
            FastRectNodeSettings settings = new FastRectNodeSettings();
            FastRectNode<int> fastRectNode = new FastRectNode<int>(boundRect, settings);
            // We still precompute the rectangles because random rectangle generation is actually a 
            // time-consuming process.
            var rects = new List<Rect>(capacity: rectCount);
            for (int k = 0; k < rectCount; ++k)
            {
                rects.Add(rectGen.NextRect());
            }
            DateTime timestamp1 = DateTime.Now;
#if true
            int repeatCount = 1;
            for (int repeatIndex = 0; repeatIndex < repeatCount; ++repeatIndex)
            {
                fastRectNode.Clear();
                for (int k = 0; k < rectCount; ++k)
                {
                    fastRectNode.Add(rects[k], k);
                }
            }
#endif
            DateTime timestamp2 = DateTime.Now;
            TimeSpan additionTime = (timestamp2 - timestamp1);
            if (Program.PrintCriticalTiming)
            {
                double additionTimeFloat = additionTime.TotalMilliseconds;
                double additionPerCall = additionTimeFloat / rectCount;
                Console.WriteLine($"{nameof(additionTime)}: {additionTimeFloat:F6} msecs / {rectCount} = {additionPerCall:F9}");
            }
            Program.PauseIfInteractive();
        }

        /// <summary>
        /// Benchmark for searching inside the data structure.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_SearchTest()
        {
            Program.PauseIfInteractive();
        }
    }
}
