using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.InternalRectUtilityTests
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial.Internals;

    public class TryComputeIntersectionTests
    {
        [Test]
        [TestCaseSource(typeof(BruteForceTestCases), nameof(BruteForceTestCases.TestCases_WithReturn_Cached))]
        public BruteForceReturnData SmokeTest_BruteForce(int i0, int i1, int i2, int i3)
        {
            var testRects = new BruteForceInput(i0, i1, i2, i3).EnumerateInputRects().ToArray();
            // ======
            // Code fragment under test
            // ------
            Rect? Call_ShouldNotThrow(Rect r1, Rect r2)
            {
                return InternalRectUtility.NoInline.TryComputeIntersection(r1, r2);
            }
            // ======
            // Assignments to dummy variables merely to ensure that compiler optimization doesn't 
            // eliminate any computational code-under-test due to non-use of function return values.
            // ------
            BruteForceReturnData returnData = new BruteForceReturnData();
            void UpdateReturnData(Rect? maybeInter)
            {
                if (maybeInter.HasValue)
                {
                    var value = maybeInter.Value;
                    returnData.SumX += value.X;
                    returnData.SumY += value.Y;
                    returnData.SumWidth += value.Width;
                    returnData.SumHeight += value.Height;
                }
                else
                {
                    returnData.NoValueCount += 1;
                }
            }
            // ======
            // Brute force test many cases, and then assert that the aggregate matches what is 
            // known to be correct on at least one validated implementations.
            // ------
            foreach (Rect r1 in testRects)
            {
                foreach (Rect r2 in testRects)
                {

                    Rect? maybeInter = default;
                    Assert.That(
                        () =>
                        {
                            maybeInter = Call_ShouldNotThrow(r1, r2);
                        }, Throws.Nothing);
                    UpdateReturnData(maybeInter);
                }
            }
            return returnData;
        }

        [Test]
        [TestCaseSource(typeof(BruteForceTestCases), nameof(BruteForceTestCases.TestCases_NoReturn_Cached))]
        public void When_AnyWidthHeightNonPos_AlwaysNull(int i0, int i1, int i2, int i3)
        {
            var testRects = new BruteForceInput(i0, i1, i2, i3).EnumerateInputRects().ToArray();
            // ======
            // Brute force test many cases, and then assert against a description.
            // The code-under-test is expected to never throw; therefore doesn't need 
            // to be executed inside an exception-thrower invoker.
            // ------
            foreach (Rect r1 in testRects)
            {
                foreach (Rect r2 in testRects)
                {
                    bool precond = (r1.Width > 0 && r1.Height > 0 && r2.Width > 0 && r2.Height > 0);
                    bool hasValue = InternalRectUtility.NoInline.TryComputeIntersection(r1, r2).HasValue;
                    // ======
                    // Description:
                    // "It never occurs that the precondition fails yet the code-under-test seem to produce a result."
                    // ------
                    // TODO convert to NUnit fluent style.
                    // ------
                    bool expr = !(!precond && hasValue);
                    Assert.IsTrue(expr, 
                        "Expression (!(!precond && hasValue)) is expected to be true at all times. \n" + 
                        $"Where precond is (r1.Width > 0 && r1.Height > 0 && r2.Width > 0 && r2.Height > 0) \n" + 
                        $"with actual values ({r1.Width} > 0 && {r1.Height} > 0 && {r2.Width} > 0 && {r2.Height} > 0) \n" +
                        $"and actual hasValue is {hasValue}.");
                }
            }
        }
    }
}
