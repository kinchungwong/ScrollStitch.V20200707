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
        public int ProcessNewDataWhenCountReaches { get; set; } = 1024;

        public bool ChildGrid_2x2_1x1 { get; set; } = false;

        public bool ChildGrid_4x4_1x1 { get; set; } = false;

        public bool ChildGrid_4x4_1x2 { get; set; } = false;

        public bool ChildGrid_4x4_2x2 { get; set; } = false;

        public bool ChildGrid_4x4_3x3 { get; set; } = false;

        public bool ChildGrid_4x4_1x4 { get; set; } = false;

        public bool ChildGrid_5x5_1x1 { get; set; } = true;

        public bool ChildGrid_5x5_2x2 { get; set; } = false;

        public bool ChildGrid_5x5_3x3 { get; set; } = false;

        public bool ChildGrid_7x7_3x3 { get; set; } = true;

        public bool ChildGrid_8x8_1x1 { get; set; } = false;

        public bool ChildGrid_8x8_2x2 { get; set; } = false;

        public bool ChildGrid_8x8_3x3 { get; set; } = false;
    }
}
