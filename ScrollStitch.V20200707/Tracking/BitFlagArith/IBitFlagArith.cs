using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.BitFlagArith
{
    public interface IBitFlagArith<T>
        where T : struct
    {
        int NumUsableBits { get; }

        T GetBitPositionMask(int bitIndex);

        T Or(T t1, T t2);
    }
}
