using ScrollStitch.V20200707.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    public struct TextAnchor
    {
        public Point Point { get; }

        public HorzAlign HorzAlign { get; }

        public VertAlign VertAlign { get; }

        public TextAnchor(Point point, HorzAlign horzAlign, VertAlign vertAlign)
        {
            Point = point;
            HorzAlign = horzAlign;
            VertAlign = vertAlign;
        }

        public void Deconstruct(out Point point, out HorzAlign horzAlign, out VertAlign vertAlign)
        {
            point = Point;
            horzAlign = HorzAlign;
            vertAlign = VertAlign;
        }
    }
}
