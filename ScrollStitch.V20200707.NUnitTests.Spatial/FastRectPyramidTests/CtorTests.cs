using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace ScrollStitch.V20200707.NUnitTests.Spatial.FastRectPyramidTests
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial.Internals;

    public class CtorTests
    {
        [Test]
        public static void NullRadius_ShouldThrow()
        {
            FastRectPyramid<int> pyramid = null;
            Assert.Throws<ArgumentException>(() =>
            {
                pyramid = new FastRectPyramid<int>(childRadiusList: null);
            });
        }

        [Test]
        public static void EmptyRadius_ShouldThrow()
        {
            var radiusList = new List<int>();
            FastRectPyramid<int> pyramid = null;
            Assert.Throws<ArgumentException>(() =>
            {
                pyramid = new FastRectPyramid<int>(radiusList);
            });
        }

        [Test]
        public static void JustMinRadius_ShouldSucceed()
        {
            var radiusList = new List<int>()
            {
                FastRectPyramid.Constants.MinSupportedRadius
            };
            FastRectPyramid<int> pyramid = null;
            Assert.DoesNotThrow(() =>
            {
                pyramid = new FastRectPyramid<int>(radiusList);
            });
            Assert.Multiple(() =>
            {
                Assert.That(pyramid, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.EqualTo(radiusList));
            });
        }

        [Test]
        public static void JustMaxRadius_ShouldSucceed()
        {
            var radiusList = new List<int>()
            {
                FastRectPyramid.Constants.MaxSupportedRadius
            };
            FastRectPyramid<int> pyramid = null;
            Assert.DoesNotThrow(() =>
            {
                pyramid = new FastRectPyramid<int>(radiusList);
            });
            Assert.Multiple(() =>
            {
                Assert.That(pyramid, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.EqualTo(radiusList));
            });
        }

        [Test]
        public static void JustMinAndMaxRadius_ShouldSucceed()
        {
            var radiusList = new List<int>()
            {
                FastRectPyramid.Constants.MinSupportedRadius,
                FastRectPyramid.Constants.MaxSupportedRadius
            };
            FastRectPyramid<int> pyramid = null;
            Assert.DoesNotThrow(() =>
            {
                pyramid = new FastRectPyramid<int>(radiusList);
            });
            Assert.Multiple(() =>
            {
                Assert.That(pyramid, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.EqualTo(radiusList));
            });
        }

        [Test]
        public static void MinRadiusAndAddOne_UpToMaxLevelsCreated_ShouldSucceed()
        {
            var radiusList = new List<int>();
            for (int k = 0; k < FastRectPyramid.Constants.MaxLevelsCreated; ++k)
            {
                radiusList.Add(FastRectPyramid.Constants.MinSupportedRadius + k);
            }
            FastRectPyramid<int> pyramid = null;
            Assert.DoesNotThrow(() =>
            {
                pyramid = new FastRectPyramid<int>(radiusList);
            });
            Assert.Multiple(() =>
            {
                Assert.That(pyramid, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.Not.Null);
                Assert.That(pyramid.RadiusList, Is.EqualTo(radiusList));
            });
        }
    }
}
