using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScrollStitch.V20200707.SpatialUnitTests
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial.Internals;

    [TestClass]
    public class InternalRectUtilityTests
    {
#if false
        [TestMethod]
#endif
        public void HasIntersect_SmokeTest()
        {
        }

#if false
        [TestMethod]
#endif
        public void TryComputeIntersection_SmokeTest()
        {
        }

#if false 
        // Due to abandonment of this project, all unit tests within this project 
        // are to be disabled.
        [TestMethod]
#endif
        public void TryComputeIntersection_BruteForceTest_SmallInt_MustNotThrow()
        {
            int[] smallInts = new int[] { -1, 0, 1, 2 };
            List<Rect> testRects = new List<Rect>();
            foreach (int x in smallInts)
            {
                foreach (int y in smallInts)
                {
                    foreach (int w in smallInts)
                    {
                        foreach (int h in smallInts)
                        {
                            testRects.Add(new Rect(x, y, w, h));
                        }
                    }
                }
            }
            // Assignments to dummy variables merely to ensure that compiler optimization doesn't 
            // eliminate any computational code-under-test due to non-use of function return values.
            int dummy0 = 0;
            int dummy1 = 0;
            foreach (Rect r1 in testRects)
            {
                foreach (Rect r2 in testRects)
                {
                    Rect? maybeInter = InternalRectUtility.NoInline.TryComputeIntersection(r1, r2);
                    dummy0 += (maybeInter.HasValue ? maybeInter.Value.Width : 0);
                    dummy1 += (maybeInter.HasValue ? 1 : 0);
                }
            }
#if false
            // ====== Remark ======
            // It is not the goal for this one test to assert for dummy0 and dummy1.
            // This one test is to make sure the code-under-test never throws whatever the input arguments.
            // ------
            Assert.AreEqual(896, actual: dummy0);
            Assert.AreEqual(784, actual: dummy1);
#else
            Assert.AreNotEqual(0, actual: dummy0);
            Assert.AreNotEqual(0, actual: dummy1);
#endif
        }

#if false
        [TestMethod]
#endif
        public void ContainsWithin_SmokeTest()
        { 
        }
    }
}
