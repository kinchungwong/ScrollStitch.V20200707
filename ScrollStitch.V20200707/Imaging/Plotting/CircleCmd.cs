using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using V20200707.Data;
    using V20200707.Functional;
    using Plotting.Internals;

    /// <summary>
    /// Paints a non-filled circle (that is, the set of points on its perimeter) on the bitmap.
    /// </summary>
    public class CircleCmd
        : IDrawCmd
    {
        /// <summary>
        /// The center of the circle.
        /// </summary>
        public Point Center { get; }

        /// <summary>
        /// The radius.
        /// </summary>
        public int Radius { get; }

        /// <summary>
        /// The color to paint the circle with.
        /// </summary>
        public int Color { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public CircleCmd(Point center, int radius, int color)
        {
            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }
            Center = center;
            Radius = radius;
            Color = color;
        }

        /// <summary>
        /// Draws the non-filled circle on the bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Draw(IntBitmap bitmap)
        {
            var pixelSetter = new PixelSetter(bitmap, Color);
            var quadPixelSetter = QuadrantPixelSetterFactory.Create(Center, pixelSetter);
            var pointGen = new QuadrantArcPointGenerator(Radius);
            while (pointGen.MoveNext())
            {
                int x = pointGen.X;
                int y = pointGen.Y;
                quadPixelSetter.Invoke(x, y);
            }
        }
    }
}
