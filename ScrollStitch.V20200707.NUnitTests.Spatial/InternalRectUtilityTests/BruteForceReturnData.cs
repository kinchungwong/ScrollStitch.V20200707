using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.InternalRectUtilityTests
{
    public class BruteForceReturnData
        : IEquatable<BruteForceReturnData>
    {
        public long SumX;
        public long SumY;
        public long SumWidth;
        public long SumHeight;
        public long NoValueCount;

        public BruteForceReturnData()
        {
        }

        public BruteForceReturnData(long sumX, long sumY, long sumWidth, long sumHeight, long noValueCount)
        {
            SumX = sumX;
            SumY = sumY;
            SumWidth = sumWidth;
            SumHeight = sumHeight;
            NoValueCount = noValueCount;
        }

        public void Deconstruct(out long sumX, out long sumY, out long sumWidth, out long sumHeight, out long noValueCount)
        {
            sumX = SumX;
            sumY = SumY;
            sumWidth = SumWidth;
            sumHeight = SumHeight;
            noValueCount = NoValueCount;
        }

        public bool Equals(BruteForceReturnData other)
        {
            return SumX == other.SumX &&
                SumY == other.SumY &&
                SumWidth == other.SumWidth &&
                SumHeight == other.SumHeight &&
                NoValueCount == other.NoValueCount;

        }
    }
}
