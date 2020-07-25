using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class Hash2DSpec
    {
        public string Name { get; set; }

        public List<Hash2DPass> Passes { get; set; }

        public Hash2DSpec()
            : this(new List<Hash2DPass>())
        {
        }

        public Hash2DSpec(List<Hash2DPass> passes)
        {
            Passes = passes;
        }

        public Hash2DSpec(string name)
            : this()
        {
            Name = name;
        }

        public Hash2DSpec(string name, List<Hash2DPass> passes)
            : this(passes)
        {
            Name = name;
        }
    }
}
