using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.BitFlagArith
{
    public struct BitFlagArith_UInt32
        : IBitFlagArith<uint>
    {
        public int NumUsableBits => 32;

        public uint GetBitPositionMask(int bitIndex) => (1u << bitIndex);

        public uint Or(uint t1, uint t2) => (t1 | t2);
    }
}
