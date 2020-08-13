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

    public class Test_0003
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Test_0003()
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
            var rootBounds = new Rect(-2048, -2048, 4096, 4096);
            Node startNode = new Node(new NodeBounds(rootBounds), settings);
            //
            RectList<KeyValuePair<int, Rect>> rectList = new RectList<KeyValuePair<int, Rect>>((_) => _.Value);
            //
            int insertCount = 1_000_000;
            var rectGen = new RandomRectGenerator();
            TimeSpan nodeInsertTime = TimeSpan.Zero;
            TimeSpan listInsertTime = TimeSpan.Zero;
            for (int insertIndex = 0; insertIndex < insertCount; ++insertIndex)
            {
                var rect = rectGen.NextRect();
                DateTime insertSpot0 = DateTime.Now;
                startNode.Add(rect, insertIndex);
                DateTime insertSpot1 = DateTime.Now;
                rectList.Add(new KeyValuePair<int, Rect>(insertIndex, rect));
                DateTime insertSpot2 = DateTime.Now;
                nodeInsertTime += (insertSpot1 - insertSpot0);
                listInsertTime += (insertSpot2 - insertSpot1);
                if (Program.PrintVerbose)
                {
                    Console.WriteLine(rect);
                }
            }
            if (Program.PrintCriticalTiming)
            {
                Console.WriteLine($"{nameof(nodeInsertTime)}: {nodeInsertTime.TotalMilliseconds:F6} msecs");
                Console.WriteLine($"{nameof(listInsertTime)}: {listInsertTime.TotalMilliseconds:F6} msecs");
            }
            //
            int queryCount = 1_000;
            TimeSpan nodeQueryTime = TimeSpan.Zero;
            TimeSpan listQueryTime = TimeSpan.Zero;
            for (int queryIndex = 0; queryIndex < queryCount; ++queryIndex)
            {
                List<KeyValuePair<int, Rect>> nodeResultList = new List<KeyValuePair<int, Rect>>();
                List<KeyValuePair<int, Rect>> listResultList = new List<KeyValuePair<int, Rect>>();
                var rect = rectGen.NextRect();
                DateTime querySpot0 = DateTime.Now;
                startNode.Query(rect, (Rect r, int i) => nodeResultList.Add(new KeyValuePair<int, Rect>(i, r)));
                DateTime querySpot1 = DateTime.Now;
                foreach (var kvp in rectList.Enumerate(rect))
                {
                    listResultList.Add(kvp);
                }
                DateTime querySpot2 = DateTime.Now;
                nodeQueryTime += (querySpot1 - querySpot0);
                listQueryTime += (querySpot2 - querySpot1);
                bool hasFault = false;
                List<int> nodeResultInts = new List<int>();
                foreach (var kvp in nodeResultList)
                {
                    nodeResultInts.Add(kvp.Key);
                }
                nodeResultInts.Sort();
                List<int> listResultInts = new List<int>();
                foreach (var kvp in listResultList)
                {
                    listResultInts.Add(kvp.Key);
                }
                listResultInts.Sort();
                int nodeIndex = 0;
                int listIndex = 0;
                while (nodeIndex < nodeResultInts.Count ||
                    listIndex < listResultInts.Count)
                {
                    int? maybeNodeInt = (nodeIndex < nodeResultInts.Count) ? (nodeResultInts[nodeIndex]) : (int?)null;
                    int? maybeListInt = (listIndex < listResultInts.Count) ? (listResultInts[listIndex]) : (int?)null;
                    if (maybeNodeInt.HasValue && maybeListInt.HasValue)
                    {
                        if (maybeNodeInt.Value < maybeListInt.Value)
                        {
                            Console.WriteLine($"Index occurs in Node but not in List: {maybeNodeInt.Value}");
                            hasFault = true;
                            ++nodeIndex;
                        }
                        else if (maybeNodeInt.Value > maybeListInt.Value)
                        {
                            Console.WriteLine($"Index occurs in List but not in Node: {maybeListInt.Value}");
                            hasFault = true;
                            ++listIndex;
                        }
                        else
                        {
                            ++nodeIndex;
                            ++listIndex;
                        }
                    }
                    else if (maybeNodeInt.HasValue)
                    {
                        Console.WriteLine($"Index occurs in Node but not in List: {maybeNodeInt.Value}");
                        hasFault = true;
                        ++nodeIndex;
                    }
                    else if (maybeListInt.HasValue)
                    {
                        Console.WriteLine($"Index occurs in List but not in Node: {maybeListInt.Value}");
                        hasFault = true;
                        ++listIndex;
                    }
                    else
                    {
                        throw new Exception(); // impossible
                    }
                }
                if (Program.PrintVerbose || hasFault)
                {
                    Console.WriteLine(rect);
                }
                if (hasFault && Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
            //
            if (Program.PrintCriticalTiming)
            {
                Console.WriteLine($"{nameof(nodeQueryTime)}: {nodeQueryTime.TotalMilliseconds:F6} msecs");
                Console.WriteLine($"{nameof(listQueryTime)}: {listQueryTime.TotalMilliseconds:F6} msecs");
            }
            if (Program.PrintVerbose)
            {
                RecursiveDiagnostics diag = new RecursiveDiagnostics(startNode);
                var output = diag.Generate();
                output.ToConsole();
            }
            Program.PauseIfInteractive();
        }
    }
}
