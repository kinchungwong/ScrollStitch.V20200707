using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    public struct FromTo
        : IEquatable<FromTo>
    {
        public (int, int) ValueTuple { get; }

        public int From => ValueTuple.Item1;

        public int To => ValueTuple.Item2;

        public FromTo(int from, int to)
        {
            ValueTuple = (from, to);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case FromTo other:
                    return ValueTuple.Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(FromTo other)
        {
            return ValueTuple.Equals(other);
        }

        public override int GetHashCode()
        {
            return ValueTuple.GetHashCode();
        }
    }
}
