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

        public int Take { get; set; }

        public CurrentTestSet()
        {
            Take = int.MaxValue;
        }
    }
}
