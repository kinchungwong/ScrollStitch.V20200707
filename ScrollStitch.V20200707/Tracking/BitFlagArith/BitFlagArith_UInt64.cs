using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.BitFlagArith
{
    public struct BitFlagArith_UInt64
        : IBitFlagArith<ulong>
    {
        public int NumUsableBits => 64;

        public ulong GetBitPositionMask(int bitIndex) => (1uL << bitIndex);

        public ulong Or(ulong t1, ulong t2) => (t1 | t2);
    }
}
