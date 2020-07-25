using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    [Flags]
    public enum FilterListFlags
    {
        None = 0,
        Include = 1,
        Exclude = 2,
        Substring = 4
    }
}
