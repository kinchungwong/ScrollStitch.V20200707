using ScrollStitch.V20200707.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    public static class AxisSubdivFactory
    {
        public static AxisSubdiv CreateNearlyUniform(int inputLength, int subdivCount)
        {
            int count = subdivCount;
            int startStopCount = count + 1;
            var startStopList = new List<int>(capacity: startStopCount);
            for (int k = 0; k < startStopCount; ++k)
            {
                int startOrStop = (int)Math.Round((double)k * inputLength / count);
                startStopList.Add(startOrStop);
            }
            var list = new List<Range>(capacity: count);
            for (int k = 0; k < count; ++k)
            {
                int start = startStopList[k];
                int stop = startStopList[k + 1];
                list.Add(new Range(start, stop));
            }
            return new AxisSubdiv(inputLength, list);
        }

        public static AxisSubdiv CreateLeftAligned(int inputLength, int subdivLength)
        {
            int count = (inputLength + subdivLength - 1) / subdivLength;
            var list = new List<Range>(capacity: count);
            for (int k = 0; k < count; ++k)
            {
                int start = k * subdivLength;
                int stop = Math.Min(start + subdivLength, inputLength);
                list.Add(new Range(start, stop));
            }
            return new AxisSubdiv(inputLength, list);
        }
    }
}
