using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Utility
{
    public static class DynamicCast
    {
        public static T Cast<T>(dynamic input)
        {
            return (T)input;
        }
    }
}
