using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    public interface IUniqueList<T>
    {
        int IndexOf(T t);
        T ItemAt(int index);
    }
}
