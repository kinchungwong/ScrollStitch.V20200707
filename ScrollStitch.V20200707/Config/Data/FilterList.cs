using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class FilterList
    {
        public FilterListFlags Flags { get; set; }

        public List<string> Items { get; set; }

        public FilterList()
        {
            Items = new List<string>();
        }
    }
}
