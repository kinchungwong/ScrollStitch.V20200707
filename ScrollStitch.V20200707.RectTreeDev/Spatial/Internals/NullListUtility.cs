using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    public static class NullListUtility
    {
        public static NullList<T> CreateNullList<T>()
        {
            return new NullList<T>();
        }
    }
}
