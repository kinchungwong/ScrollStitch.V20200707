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
    using ScrollStitch.V20200707.Spatial.RectTreeInternals;

    public class Test_0001
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0001()
        { 
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            var settings = new NodeSettings()
            {
                //EachListToNodeThreshold = 32,
                TotalListToNodeThreshold = 32
            };
            int maxItemW = 256;
            int maxItemH = 256;
            Node startNode = new Node(new NodeBounds(new Rect(0, 0, 1024, 1024)), settings);
            var random = new Random(12345678);
            Size randExpSize()
            {
                int width = (int)Math.Round((1.0 + random.NextDouble()) * Math.Pow(2.0, 7.0 * random.NextDouble()));
                int height = (int)Math.Round((1.0 + random.NextDouble()) * Math.Pow(2.0, 7.0 * random.NextDouble()));
                width = Math.Min(maxItemW, width);
                height = Math.Min(maxItemH, height);
                return new Size(width, height);
            }
            int insertCount = 10000000;
            DateTime startTime = DateTime.Now;
            for (int insertIndex = 0; insertIndex < insertCount; ++insertIndex)
            {
                int randX = random.Next(1000);
                int randY = random.Next(1000);
                Size randSize = randExpSize();
                var randRect = new Rect(randX, randY, randSize.Width, randSize.Height);
                startNode.Add(randRect, insertIndex);
            }
            DateTime stopTime = DateTime.Now;
            Console.WriteLine($"{(stopTime - startTime).TotalMilliseconds:F6} msecs");
            if (Program.PrintVerbose)
            {
                RecursiveDiagnostics diag = new RecursiveDiagnostics(startNode);
                var output = diag.Generate();
                output.ToConsole();
                Program.PauseIfInteractive();
            }
        }
    }
}
