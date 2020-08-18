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

    public class ConstructorTests
    {
        [Test]
        public void SmokeTest_100_10000_ratio10()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 10000, ratio: 10.0);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(3));
                Assert.That(frp.RadiusList[0], Is.EqualTo(100));
                Assert.That(frp.RadiusList[1], Is.EqualTo(1000));
                Assert.That(frp.RadiusList[2], Is.EqualTo(10000));
            });
        }

        [Test]
        public void SmokeTest_100_1000_ratio316of100()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: 3.16);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(3));
                Assert.That(frp.RadiusList[0], Is.EqualTo(100));
                Assert.That(frp.RadiusList[1], Is.InRange(316, 317));
                Assert.That(frp.RadiusList[2], Is.EqualTo(1000));
            });
        }

        [Test]
        public void SmokeTest_100_1000000_ratio10()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000000, ratio: 10.0);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(5));
                Assert.That(frp.RadiusList[0], Is.EqualTo(100));
                Assert.That(frp.RadiusList[1], Is.EqualTo(1000));
                Assert.That(frp.RadiusList[2], Is.EqualTo(10000));
                Assert.That(frp.RadiusList[3], Is.EqualTo(100000));
                Assert.That(frp.RadiusList[4], Is.EqualTo(1000000));
            });
        }

        [Test]
        public void SmokeTest_256_1024_ratio2()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 256, maxRadius: 1024, ratio: 2.0);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(3));
                Assert.That(frp.RadiusList[0], Is.EqualTo(256));
                Assert.That(frp.RadiusList[1], Is.EqualTo(512));
                Assert.That(frp.RadiusList[2], Is.EqualTo(1024));
            });
        }

        [Test]
        public void SmokeTest_256_65536_ratio2()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 256, maxRadius: 65536, ratio: 2.0);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(9));
                Assert.That(frp.RadiusList[0], Is.EqualTo(256));
                Assert.That(frp.RadiusList[1], Is.EqualTo(512));
                Assert.That(frp.RadiusList[2], Is.EqualTo(1024));
                Assert.That(frp.RadiusList[3], Is.EqualTo(2048));
                Assert.That(frp.RadiusList[4], Is.EqualTo(4096));
                Assert.That(frp.RadiusList[5], Is.EqualTo(8192));
                Assert.That(frp.RadiusList[6], Is.EqualTo(16384));
                Assert.That(frp.RadiusList[7], Is.EqualTo(32768));
                Assert.That(frp.RadiusList[8], Is.EqualTo(65536));
            });
        }

        [Test]
        public void SmokeTest_1000_3000_ratio_1point2()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 1000, maxRadius: 3000, ratio: 1.2);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(7));
                Assert.That(frp.RadiusList[0], Is.EqualTo(1000));
                Assert.That(frp.RadiusList[1], Is.EqualTo(1201));
                Assert.That(frp.RadiusList[2], Is.EqualTo(1442));
                Assert.That(frp.RadiusList[3], Is.EqualTo(1732));
                Assert.That(frp.RadiusList[4], Is.EqualTo(2080));
                Assert.That(frp.RadiusList[5], Is.EqualTo(2498));
                Assert.That(frp.RadiusList[6], Is.EqualTo(3000));
            });
        }

        [Test]
        public void SmokeTest_1000_3000_ratio_1point201()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 1000, maxRadius: 3000, ratio: 1.201);
            Assert.Multiple(() =>
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.EqualTo(7));
                Assert.That(frp.RadiusList[0], Is.EqualTo(1000));
                Assert.That(frp.RadiusList[1], Is.EqualTo(1201));
                Assert.That(frp.RadiusList[2], Is.EqualTo(1442));
                Assert.That(frp.RadiusList[3], Is.EqualTo(1732));
                Assert.That(frp.RadiusList[4], Is.EqualTo(2080));
                Assert.That(frp.RadiusList[5], Is.EqualTo(2498));
                Assert.That(frp.RadiusList[6], Is.EqualTo(3000));
            });
        }

        [Test]
        public void DefaultCtor_ShouldSucceed_WithLevelsCreated()
        {
            FastRectPyramid frp = FastRectPyramid.Builder.Create();
            Assert.Multiple(() => 
            {
                Assert.That(frp.RadiusList, Is.Not.Null);
                Assert.That(frp.RadiusList.Count, Is.GreaterThan(1));
                Assert.That(Enumerable.First(frp.RadiusList), Is.EqualTo(FastRectPyramid.Constants.MinSupportedRadius));
                Assert.That(Enumerable.Last(frp.RadiusList), Is.EqualTo(FastRectPyramid.Constants.MaxSupportedRadius));
                int firstRadius = frp.RadiusList[0];
                int secondRadius = frp.RadiusList[1];
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
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: -1, maxRadius: 100, ratio: 2.0);
            });
        }

        [Test]
        public void NegativeMax_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: -1, ratio: 2.0);
            });
        }

        [Test]
        public void MinMaxWrongOrder_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 1000, maxRadius: 100, ratio: 2.0);
            });
        }

        [Test]
        public void RatioZero_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: 0.0);
            });
        }

        [Test]
        public void RatioOne_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: 1.0);
            });
        }

        [Test]
        public void RatioPointFive_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: 0.5);
            });
        }

        [Test]
        public void RatioMinusTwo_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: -2.0);
            });
        }

        [Test]
        public void RatioBelowMin_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                double ratio = 0.999 * FastRectPyramid.Constants.MinRatio;
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: ratio);
            });
        }

        [Test]
        public void RatioAboveMax_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                double ratio = 1.001 * FastRectPyramid.Constants.MaxRatio;
                FastRectPyramid frp = FastRectPyramid.Builder.Create(minRadius: 100, maxRadius: 1000, ratio: ratio);
            });
        }
    }
}
