using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using HashCode;

    public class PointListCmd
        : IDrawCmd
    {
        public Point CursorPoint { get; }
        public PointList RelativePoints { get; }
        public int Color { get; }

        public PointListCmd(Point cursor, PointList relativePoints, int color)
        {
            CursorPoint = cursor;
            RelativePoints = relativePoints;
            Color = color;
        }

        public void Draw(IntBitmap dest)
        {
            int w = dest.Width;
            int h = dest.Height;
            foreach (var p in RelativePoints)
            {
                int x = CursorPoint.X + p.X;
                int y = CursorPoint.Y + p.Y;
                if (x < 0 || y < 0 ||
                    x >= w || y >= h)
                {
                    continue;
                }
                dest.Data[y * w + x] = Color;
            }
        }
        public override int GetHashCode()
        {
            var helper = new HashCodeBuilder(GetType());
            helper.Ingest(CursorPoint.X);
            helper.Ingest(CursorPoint.Y);
            helper.Ingest(Color);
            helper.Ingest(RelativePoints.GetHashCode());
            return helper.GetHashCode();
        }
    }
}
