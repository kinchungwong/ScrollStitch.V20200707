using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.InternalRectUtilityTests
{
    using ScrollStitch.V20200707.Data;

    public class BruteForceInput : TestCaseData
    {
        public IList<int> InputList;

        public BruteForceInput(int i0, int i1, int i2, int i3)
            : base(i0, i1, i2, i3)
        {
            InputList = new int[] { i0, i1, i2, i3 };
        }

        public BruteForceInput(IList<int> inputList)
            : base(inputList.Cast<object>().ToArray())
        {
            InputList = inputList;
        }

        public void Deconstruct(out IList<int> inputList)
        {
            inputList = InputList;
        }

        public void Deconstruct(out int i0, out int i1, out int i2, out int i3)
        {
            i0 = InputList[0];
            i1 = InputList[1];
            i2 = InputList[2];
            i3 = InputList[3];
        }

        public IEnumerable<Rect> EnumerateInputRects()
        {
            foreach (int x in InputList)
            {
                foreach (int y in InputList)
                {
                    foreach (int w in InputList)
                    {
                        foreach (int h in InputList)
                        {
                            yield return new Rect(x, y, w, h);
                        }
                    }
                }
            }
        }
    }
}
