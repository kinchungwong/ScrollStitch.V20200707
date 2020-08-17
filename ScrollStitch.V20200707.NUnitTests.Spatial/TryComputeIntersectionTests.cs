using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace ScrollStitch.V20200707.NUnitTests.Spatial
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial.Internals;
    using System;
    using System.Linq;

    public class TryComputeIntersectionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        { 
        }

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
        }

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

        public class BruteForceTestCases
        { 
            public static IEnumerable<TestCaseData> TestCases_WithReturn
            {
                get
                {
                    yield return 
                        new BruteForceInput(new int[] { -1, 0, 1, 2 }).SetName("m1_0_1_2")
                        .Returns(new BruteForceReturnData(560, 560, 896, 896, 64752));
                    yield return 
                        new BruteForceInput(new int[] { 0, 1, 2, 3 }).SetName("0_1_2_3")
                        .Returns(new BruteForceReturnData(13104, 13104, 9744, 9744, 58480));
                    yield return 
                        new BruteForceInput(new int[] { 0, 3, 11, 127 }).SetName("0_3_11_127")
                        .Returns(new BruteForceReturnData(236106, 236106, 126828, 126828, 59452));
                    yield return 
                        new BruteForceInput(new int[] { 1, 2, 3, 4 }).SetName("1_2_3_4")
                        .Returns(new BruteForceReturnData(91520, 91520, 51392, 51392, 34560));
                    yield return
                        new BruteForceInput(new int[] { -2, -1, 0, 1 }).SetName("m2_m1_0_1")
                        .Returns(new BruteForceReturnData(-8, -8, 16, 16, 65520));
                    yield return
                        new BruteForceInput(new int[] { -2, -1, 1, 2 }).SetName("m2_m1_1_2")
                        .Returns(new BruteForceReturnData(96, 96, 672, 672, 64960));
                    yield return
                        new BruteForceInput(new int[] { -3, -2, -1, 0 }).SetName("m3_m2_m1_0")
                        .Returns(new BruteForceReturnData(0, 0, 0, 0, 65536));
                    yield return
                        new BruteForceInput(new int[] { -3, -2, -1, 1 }).SetName("m3_m2_m1_1")
                        .Returns(new BruteForceReturnData(-20, -20, 16, 16, 65520));
                }
            }

            public static IEnumerable<TestCaseData> TestCases_NoReturn
            {
                get
                {
                    foreach (var v in TestCases_WithReturn)
                    {
                        yield return new TestCaseData(v.Arguments).SetName(v.TestName);
                    }
                }
            }
        }

        private static List<Rect> _RectsFromAllIntegerCombinations(params int[] intArgs)
        {
            List<int> inputList = intArgs.ToList();
            List<Rect> testRects = new List<Rect>();
            foreach (int x in inputList)
            {
                foreach (int y in inputList)
                {
                    foreach (int w in inputList)
                    {
                        foreach (int h in inputList)
                        {
                            testRects.Add(new Rect(x, y, w, h));
                        }
                    }
                }
            }
            return testRects;
        }

        [Test]
        [TestCaseSource(typeof(BruteForceTestCases), nameof(BruteForceTestCases.TestCases_WithReturn))]
        public BruteForceReturnData TryComputeIntersection_SmokeTest_BruteForce(int i0, int i1, int i2, int i3)
        {
            IList<Rect> testRects = _RectsFromAllIntegerCombinations(i0, i1, i2, i3);
            // ======
            // Code fragment under test
            // ------
            Rect? Call_TryComputeIntersection_ShouldNotThrow(Rect r1, Rect r2)
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
                            maybeInter = Call_TryComputeIntersection_ShouldNotThrow(r1, r2);
                        }, Throws.Nothing);
                    UpdateReturnData(maybeInter);
                }
            }
            return returnData;
        }

        [Test]
        [TestCaseSource(typeof(BruteForceTestCases), nameof(BruteForceTestCases.TestCases_NoReturn))]
        public void TryComputeIntersection_ForAll_WhenAnyWidthHeightNonPositive_AlwaysNull(int i0, int i1, int i2, int i3)
        {
            IList<Rect> testRects = _RectsFromAllIntegerCombinations(i0, i1, i2, i3);
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
