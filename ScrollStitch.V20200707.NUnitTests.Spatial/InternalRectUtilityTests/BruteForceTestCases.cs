using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.InternalRectUtilityTests
{
    public class BruteForceTestCases
    {
        public static readonly Lazy<IEnumerable<TestCaseData>> TestCases_WithReturn_Lazy 
            = new Lazy<IEnumerable<TestCaseData>>(() => TestCases_WithReturn);

        public static IEnumerable<TestCaseData> TestCases_WithReturn_Cached 
            => TestCases_WithReturn_Lazy.Value;

        public static IEnumerable<TestCaseData> TestCases_WithReturn
        {
            get
            {
                yield return
                    new BruteForceInput(-1, 0, 1, 2)
                    .Returns(new BruteForceReturnData(560, 560, 896, 896, 64752));
                yield return
                    new BruteForceInput(0, 1, 2, 3)
                    .Returns(new BruteForceReturnData(13104, 13104, 9744, 9744, 58480));
                yield return
                    new BruteForceInput(0, 3, 11, 127)
                    .Returns(new BruteForceReturnData(236106, 236106, 126828, 126828, 59452));
                yield return
                    new BruteForceInput(1, 2, 3, 4)
                    .Returns(new BruteForceReturnData(91520, 91520, 51392, 51392, 34560));
                yield return
                    new BruteForceInput(-2, -1, 0, 1)
                    .Returns(new BruteForceReturnData(-8, -8, 16, 16, 65520));
                yield return
                    new BruteForceInput(-2, -1, 1, 2)
                    .Returns(new BruteForceReturnData(96, 96, 672, 672, 64960));
                yield return
                    new BruteForceInput(-3, -2, -1, 0)
                    .Returns(new BruteForceReturnData(0, 0, 0, 0, 65536));
                yield return
                    new BruteForceInput(-3, -2, -1, 1)
                    .Returns(new BruteForceReturnData(-20, -20, 16, 16, 65520));
            }
        }

        public static readonly Lazy<IEnumerable<TestCaseData>> TestCases_NoReturn_Lazy
            = new Lazy<IEnumerable<TestCaseData>>(() => TestCases_NoReturn);

        public static IEnumerable<TestCaseData> TestCases_NoReturn_Cached
            => TestCases_NoReturn_Lazy.Value;

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
}
