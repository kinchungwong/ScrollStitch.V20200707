using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;

    /// <summary>
    /// Paints a filled circle on the bitmap.
    /// 
    /// <para>
    /// Speed is important, as the code may need to be run on large batches of images.
    /// </para>
    /// 
    /// <para>
    /// Reminder about non-accuracy.<br/>
    /// This implementation is not intended to be pixel-accurate. <br/>
    /// Anti-aliasing will not be supported. <br/>
    /// Blending (alpha matte) will not be supported. <br/>
    /// </para>
    /// </summary>
    public class FilledCircle
        : IDrawCmd
    {
        public Point Center { get; }

        public int Radius { get; }

        public int Color { get; }

        public FilledCircle(Point center, int radius, int color)
        {
            Center = center;
            Radius = radius;
            Color = color;
        }

        public void Draw(IntBitmap dest)
        {
            int[] destArray = dest.Data;
            int color = Color;
            int width = dest.Width;
            int height = dest.Height;
            int minY = Math.Max(0, Center.Y - Radius);
            int maxY = Math.Min(height - 1, Center.Y + Radius);
            for (int y = minY; y <= maxY; ++y)
            {
                int dy = y - Center.Y;
                int halfX = (int)Math.Round(Math.Sqrt(Radius * Radius - dy * dy));
                if (halfX < 0)
                {
                    continue;
                }
                int minX = Math.Max(0, Center.X - halfX);
                int maxX = Math.Min(width - 1, Center.X + halfX);
                if (maxX < minX)
                {
                    continue;
                }
                int fillStart = y * width + minX;
                int fillCount = maxX - minX + 1;
                Arrays.BuiltinArrayMethods.NoInline.ArrayFill(destArray, color, fillStart, fillCount);
            }
        }
    }
}
