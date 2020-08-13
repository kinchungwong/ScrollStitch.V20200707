using ScrollStitch.V20200707.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.RectTreeDev
{
    public class RandomRectGenerator
    {
        private Random _rand;

        public RandomRectGenerator()
        {
            _rand = new Random();
        }

        public Rect NextRect()
        {
            int x = _rand.Next(-1024, 1024);
            int y = _rand.Next(-1024, 1024);
            double floatSide = Math.Pow(2.0, 6.0 * _rand.NextDouble());
            double aspect = 1.0 + 4.0 * _rand.NextDouble();
            int w = (int)Math.Round(floatSide * aspect);
            int h = (int)Math.Round(floatSide);
            if ((_rand.Next() & 1) != 0)
            {
                int swap = w;
                w = h;
                h = swap;
            }
            var rect = new Rect(x, y, w, h);
            return rect;
        }
    }
}
