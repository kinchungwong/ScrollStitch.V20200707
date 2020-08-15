using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    public class FastRectNodeSettings
    {
        /// <summary>
        /// Specifies a trigger for processing newly added data when their numbers has reached 
        /// this threshold.
        /// </summary>
        public int ProcessNewDataWhenCountReaches 
        { 
            get; 
            set; 
        }

        public FastRectNodeChildFactory ChildFactory 
        { 
            get; 
            set; 
        }

        public FastRectNodeSettings()
            : this(highAspectRatioMode: false)
        { 
        }

        public FastRectNodeSettings(bool highAspectRatioMode)
        {
            ProcessNewDataWhenCountReaches = Defaults.ProcessNewDataWhenCountReaches;
            ChildFactory = Defaults.CreateDefaultChildFactoryProfile(highAspectRatioMode);
        }

        public static class Defaults
        {
            public const int ProcessNewDataWhenCountReaches = 256;

            public static FastRectNodeChildFactory CreateDefaultChildFactoryProfile(bool highAspectRatioMode)
            {
                // ====== Developer advice ======
                // Use graphical child node visualizer to compare between these configurations.
                // Also check for markdown documentation in the source folder,
                // and online resources comparing wide quadtree implementations.
                // ======
                var childFactoryBuilder = new FastRectNodeChildFactory.Builder();
                // ======
                // Generates a 5x5 grid of child nodes.
                // A "quinvigint-tree" performs better than a "quad-tree".
                // Enabling this mode adds 25 child nodes for each parent node.
                // ------
                childFactoryBuilder.Add(5, 5, 1, 1, 1, 1);
                // ======
                // Generates a 3x3 grid of child nodes.
                // Three sliding windows (on horz/vert axis), each covering 43 percent of that axis,
                // with overlaps. 
                // Enabling this mode adds 9 child nodes for each parent node.
                // ------
                childFactoryBuilder.Add(7, 7, 3, 3, 2, 2);
                // ======
                // If the data set contains high aspect ratio rectangles, enable this mode to
                // allow their partitioning without piling up at the root node's straddle list.
                // ------
                if (highAspectRatioMode)
                {
                    // ======
                    // Three sliding windows (on the partitioning axis), 
                    // Each covering 43 percent on that axis, with overlaps. 
                    // Limiting to three partitions is a compromise between:
                    // ... allowing their partitioning at all, versus
                    // ... controlling the number of child nodes.
                    // Keep in mind that they can be recursively partitioned.
                    // Thus, 
                    // ... 2nd level nodes will cover pow(0.43, 2) == 0.18 on the partitioning axis,
                    // ... 3rd level nodes will cover pow(0.43, 3) == 0.08 on the partitioning axis.
                    // Enabling this mode
                    // ... adds 6 child nodes for each parent node.
                    // ------
                    childFactoryBuilder.Add(7, 1, 3, 1, 2, 1);
                    childFactoryBuilder.Add(1, 7, 1, 3, 1, 2);
                }
                return childFactoryBuilder.Create();
            }
        }
    }
}
