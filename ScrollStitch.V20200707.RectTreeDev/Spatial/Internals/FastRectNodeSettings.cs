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
    }
}
