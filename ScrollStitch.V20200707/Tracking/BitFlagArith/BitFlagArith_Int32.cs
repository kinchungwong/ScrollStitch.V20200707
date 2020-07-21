using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.BitFlagArith
{
    public struct BitFlagArith_Int32
        : IBitFlagArith<int>
    {
        public int NumUsableBits => 31;

        public int GetBitPositionMask(int bitIndex) => (1 << bitIndex);

        public int Or(int t1, int t2) => (t1 | t2);
    }
}
