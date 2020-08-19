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

    public class BuilderTests
    {
        [Test]
        public void SmokeTest_100_10000_ratio10()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 10000, ratio: 10.0);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(3));
                Assert.That(radiusList[0], Is.EqualTo(100));
                Assert.That(radiusList[1], Is.EqualTo(1000));
                Assert.That(radiusList[2], Is.EqualTo(10000));
            });
        }

        [Test]
        public void SmokeTest_100_1000_ratio316of100()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: 3.16);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(3));
                Assert.That(radiusList[0], Is.EqualTo(100));
                Assert.That(radiusList[1], Is.InRange(316, 317));
                Assert.That(radiusList[2], Is.EqualTo(1000));
            });
        }

        [Test]
        public void SmokeTest_100_1000000_ratio10()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000000, ratio: 10.0);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(5));
                Assert.That(radiusList[0], Is.EqualTo(100));
                Assert.That(radiusList[1], Is.EqualTo(1000));
                Assert.That(radiusList[2], Is.EqualTo(10000));
                Assert.That(radiusList[3], Is.EqualTo(100000));
                Assert.That(radiusList[4], Is.EqualTo(1000000));
            });
        }

        [Test]
        public void SmokeTest_256_1024_ratio2()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 256, maxRadius: 1024, ratio: 2.0);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(3));
                Assert.That(radiusList[0], Is.EqualTo(256));
                Assert.That(radiusList[1], Is.EqualTo(512));
                Assert.That(radiusList[2], Is.EqualTo(1024));
            });
        }

        [Test]
        public void SmokeTest_256_65536_ratio2()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 256, maxRadius: 65536, ratio: 2.0);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(9));
                Assert.That(radiusList[0], Is.EqualTo(256));
                Assert.That(radiusList[1], Is.EqualTo(512));
                Assert.That(radiusList[2], Is.EqualTo(1024));
                Assert.That(radiusList[3], Is.EqualTo(2048));
                Assert.That(radiusList[4], Is.EqualTo(4096));
                Assert.That(radiusList[5], Is.EqualTo(8192));
                Assert.That(radiusList[6], Is.EqualTo(16384));
                Assert.That(radiusList[7], Is.EqualTo(32768));
                Assert.That(radiusList[8], Is.EqualTo(65536));
            });
        }

        [Test]
        public void SmokeTest_1000_3000_ratio_1point2()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 1000, maxRadius: 3000, ratio: 1.2);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(7));
                Assert.That(radiusList[0], Is.EqualTo(1000));
                Assert.That(radiusList[1], Is.EqualTo(1201));
                Assert.That(radiusList[2], Is.EqualTo(1442));
                Assert.That(radiusList[3], Is.EqualTo(1732));
                Assert.That(radiusList[4], Is.EqualTo(2080));
                Assert.That(radiusList[5], Is.EqualTo(2498));
                Assert.That(radiusList[6], Is.EqualTo(3000));
            });
        }

        [Test]
        public void SmokeTest_1000_3000_ratio_1point201()
        {
            var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 1000, maxRadius: 3000, ratio: 1.201);
            Assert.Multiple(() =>
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.EqualTo(7));
                Assert.That(radiusList[0], Is.EqualTo(1000));
                Assert.That(radiusList[1], Is.EqualTo(1201));
                Assert.That(radiusList[2], Is.EqualTo(1442));
                Assert.That(radiusList[3], Is.EqualTo(1732));
                Assert.That(radiusList[4], Is.EqualTo(2080));
                Assert.That(radiusList[5], Is.EqualTo(2498));
                Assert.That(radiusList[6], Is.EqualTo(3000));
            });
        }

        [Test]
        public void DefaultCtor_ShouldSucceed_WithLevelsCreated()
        {
            var radiusList = FastRectPyramid.Builder.InitDefaultRadiusList();
            Assert.Multiple(() => 
            {
                Assert.That(radiusList, Is.Not.Null);
                Assert.That(radiusList.Count, Is.GreaterThan(1));
                Assert.That(Enumerable.First(radiusList), Is.EqualTo(FastRectPyramid.Constants.MinSupportedRadius));
                Assert.That(Enumerable.Last(radiusList), Is.EqualTo(FastRectPyramid.Constants.MaxSupportedRadius));
                int firstRadius = radiusList[0];
                int secondRadius = radiusList[1];
                double ratio = (double)secondRadius / firstRadius;
                // ======
                // Because the ratio argument (or default ratio) is only treated as a suggestion
                // whereas the actual ratio is computed on a evenly-spaced logarithmic scale, 
                // the actual ratio between levels can be far from the ratio specified for the constructor.
                // ======
                Assert.That(ratio, Is.InRange(
                    FastRectPyramid.Constants.DefaultRatio * 0.5, 
                    FastRectPyramid.Constants.DefaultRatio * 2.0));
            });
        }

        [Test]
        public void NegativeMin_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: -1, maxRadius: 100, ratio: 2.0);
            });
        }

        [Test]
        public void NegativeMax_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: -1, ratio: 2.0);
            });
        }

        [Test]
        public void MinMaxWrongOrder_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 1000, maxRadius: 100, ratio: 2.0);
            });
        }

        [Test]
        public void RatioZero_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: 0.0);
            });
        }

        [Test]
        public void RatioOne_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: 1.0);
            });
        }

        [Test]
        public void RatioPointFive_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: 0.5);
            });
        }

        [Test]
        public void RatioMinusTwo_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: -2.0);
            });
        }

        [Test]
        public void RatioBelowMin_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                double ratio = 0.999 * FastRectPyramid.Constants.MinRatio;
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: ratio);
            });
        }

        [Test]
        public void RatioAboveMax_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                double ratio = 1.001 * FastRectPyramid.Constants.MaxRatio;
                var radiusList = FastRectPyramid.Builder.InitRadiusList(minRadius: 100, maxRadius: 1000, ratio: ratio);
            });
        }
    }
}
