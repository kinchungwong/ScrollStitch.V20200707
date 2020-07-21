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

    public class LineCmd : IDrawCmd
    {
        public Point Point1 { get; }
        public Point Point2 { get; }
        public int Color { get; }

        public LineCmd(Point p1, Point p2, int color)
        {
            Point1 = p1;
            Point2 = p2;
            Color = color;
        }

        public void Draw(IntBitmap dest)
        {
            // TODO WARNING
            // Code has not yet been carefully checked for correctness.
            //
            var p1 = Point1;
            var p2 = Point2;
            var data = dest.Data;
            int width = dest.Width;
            int height = dest.Height;
            bool VP(Point p) => (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height);
            void P(Point p)
            {
                if (VP(p)) dest[p] = Color;
            }
            void Pxy(int x, int y)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    data[y * width + x] = Color;
                }
            }
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            int absDx = Math.Abs(dx);
            int absDy = Math.Abs(dy);
            int maxD = Math.Max(absDx, absDy);
            if (absDx == 0 && absDy == 0)
            {
                P(p1);
                return;
            }
            if (absDx == 0)
            {
                int minY = Math.Min(p1.Y, p2.Y);
                int maxY = Math.Max(p1.Y, p2.Y);
                for (int y = minY; y <= maxY; ++y)
                {
                    Pxy(p1.X, y);
                }
                return;
            }
            if (absDy == 0)
            {
                int minX = Math.Min(p1.X, p2.X);
                int maxX = Math.Max(p1.X, p2.X);
                for (int x = minX; x <= maxX; ++x)
                {
                    Pxy(x, p1.Y);
                }
                return;
            }
            // TODO WARNING
            // Code has not yet been carefully checked for correctness.
            //
            for (int k = 0; k <= maxD; ++k)
            {
                int x = p1.X + (int)Math.Round((double)dx * k / maxD);
                int y = p1.Y + (int)Math.Round((double)dy * k / maxD);
                if (x >= 0 && y >= 0 && x < width && y < height)
                {
                    Pxy(x, y);
                }
            }
        }

        public override int GetHashCode()
        {
            var helper = new HashCodeBuilder(GetType());
            helper.Ingest(Point1.X);
            helper.Ingest(Point1.Y);
            helper.Ingest(Point2.X);
            helper.Ingest(Point2.Y);
            helper.Ingest(Color);
            return helper.GetHashCode();
        }
    }
}
