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

    public class AddItemSmokeTests
    {
        private static readonly Lazy<Random> lzRandom = new Lazy<Random>(() => new Random());

        public static Random random => lzRandom.Value;

        public static Rect NextRandomRect()
        {
            int midX = random.Next(-32768, 32768);
            int midY = random.Next(-32768, 32768);
            int width = random.Next(1, 2048);
            int height = random.Next(1, 2048);
            return new Rect(midX - width / 2, midY - height / 2, width, height);
        }

        [Test]
        public void Default_Add_Random_1000()
        {
            const int addCount = 1000;
            FastRectPyramid<int> pyramid = FastRectPyramid<int>.Builder.Create();
            for (int k = 0; k < addCount; ++k)
            {
                pyramid.Add(new KeyValuePair<Rect, int>(NextRandomRect(), k));
            }
        }

        [Test]
        public void Default_Add_Random_1000_Check()
        {
            const int addCount = 1000;
            List<Rect> rects = new List<Rect>(capacity: addCount);
            for (int k = 0; k < addCount; ++k)
            {
                rects.Add(NextRandomRect());
            }
            FastRectPyramid<int> pyramid = FastRectPyramid<int>.Builder.Create();
            for (int k = 0; k < addCount; ++k)
            {
                pyramid.Add(new KeyValuePair<Rect, int>(rects[k], k));
            }
            for (int k = 0; k < addCount; ++k)
            {
                bool hasFound = pyramid.ContainsRect(rects[k]);
                Assert.That(hasFound, Is.True);
            }
            // The following is supposed to be a very unlikely event, 
            // but a small number of matches by chance are expected.
            int randomMatches = 0;
            for (int k = 0; k < addCount; ++k)
            {
                Rect randomRect = NextRandomRect();
                bool hasFound = pyramid.ContainsRect(randomRect);
                if (hasFound)
                {
                    ++randomMatches;
                }
            }
            Console.WriteLine($"Random matches (expects to be zero or a small number): {randomMatches}");
        }
    }
}
