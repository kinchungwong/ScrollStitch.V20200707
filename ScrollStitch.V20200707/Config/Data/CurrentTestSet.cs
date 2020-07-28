using ScrollStitch.V20200707.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class CurrentTestSet
    {
        public string TestSetName { get; set; }

        public IntegerExpandList Items { get; set; }

        public CurrentTestSet()
        {
            // Do not assign Items by default.
            // Where Items is null, the entire list of files should be used.
        }
    }
}
