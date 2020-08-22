using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ScrollStitch.V20200707.NUnitTests.Spatial.FilteredEnumeratorTests
{
    using ScrollStitch.V20200707.Collections;

    public class BasicDemo
    {
        [Test]
        public void IntFilterDemo()
        {
            List<int> list = Enumerable.Range(0, 30).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            foreach (int value in fe)
            {
                Console.WriteLine(value);
            }
        }

        [Test]
        public void Empty_ShouldBeEmpty()
        {
            List<int> list = Enumerable.Range(0, 0).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.Count(fe), Is.EqualTo(0));
        }

        [Test]
        public void SmallList_NoMatchingItem_ShouldBeEmpty()
        {
            List<int> list = Enumerable.Range(0, 1).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.Count(fe), Is.EqualTo(0));
        }

        [Test]
        public void TypicalList_NoMatchingItem_ShouldBeEmpty()
        {
            List<int> list = Enumerable.Range(0, 30).ToList();
            Func<int, bool> isNonInteger = (int _) => false;
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, isNonInteger);
            Assert.That(Enumerable.Count(fe), Is.EqualTo(0));
        }

        [Test]
        public void FirstIsMatch_FirstResultShouldMatch()
        {
            List<int> list = Enumerable.Range(1, 30).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.First(fe), Is.EqualTo(1));
        }

        [Test]
        public void FirstIsNotMatch_FirstResultShouldSkipToMatch()
        {
            List<int> list = Enumerable.Range(0, 30).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.First(fe), Is.EqualTo(1));
        }

        [Test]
        public void LastIsMatch_LastResultShouldMatch()
        {
            List<int> list = Enumerable.Range(0, 30).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.Last(fe), Is.EqualTo(29));
        }

        [Test]
        public void LastIsNotMatch_LastResultShouldOmitNonMatch()
        {
            List<int> list = Enumerable.Range(0, 31).ToList();
            Func<int, bool> pred = (int value) => ((value % 2) != 0);
            FilteredEnumerable<int> fe = new FilteredEnumerable<int>(list, pred);
            Assert.That(Enumerable.Last(fe), Is.EqualTo(29));
        }
    }
}
