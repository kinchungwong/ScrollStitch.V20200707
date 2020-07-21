using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using HashCode;

    public class DotCmd
        : IDrawCmd
    {
        public Point Point { get; }
        public int Color { get; }

        public DotCmd(Point point, int color)
        {
            Point = point;
            Color = color;
        }

        public void Draw(IntBitmap dest)
        {
            int w = dest.Width;
            dest.Data[Point.Y * w + Point.X] = Color;
        }

        public override int GetHashCode()
        {
            var helper = new HashCodeBuilder(GetType());
            helper.Ingest(Point.X);
            helper.Ingest(Point.Y);
            helper.Ingest(Color);
            return helper.GetHashCode();
        }
    }
}
