using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Internals
{
    public interface IHistArith<T>
    {
        T Zero { get; }

        T One { get; }

        Func<T, T, T> AddFunc { get; }
    }
}
