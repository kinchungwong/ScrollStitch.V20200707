using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals.RectTreeV1
{
    public class NodeSettings
    {
        public int ListInitialCapacity { get; set; } = 32;
        public int EachListToNodeThreshold { get; set; } = 128;
        public int TotalListToNodeThreshold { get; set; } = 128;

        public NodeSettings()
        { 
        }
    }
}
