using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Internals
{
    public struct HistArith<T>
        : IHistArith<T>
    {
        public T Zero { get; set; }

        public T One { get; set; }

        public Func<T, T, T> AddFunc { get; set; }
    }
}
