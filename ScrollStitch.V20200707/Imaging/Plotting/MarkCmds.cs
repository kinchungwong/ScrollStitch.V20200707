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

    public static class MarkCmds
    {
        public class Ex : PointListCmd
        {
            private static readonly int[] _coords = new int[]
                { 
                    -1, -1, +1, -1, 0, 0, -1, +1, +1, +1
                };
            
            private static readonly Lazy<PointList> _lzPointList = 
                new Lazy<PointList>(() => new PointList(_coords));

            public static PointList PointList => _lzPointList.Value;

            public Ex(Point cursor, int color)
                : base(cursor, PointList, color)
            {
            }
        }

        public class Plus : PointListCmd
        {
            private static readonly int[] _coords = new int[]
                {
                    0, -1, -1, 0, 0, 0, +1, 0, 0, +1
                };

            private static readonly Lazy<PointList> _lzPointList =
                new Lazy<PointList>(() => new PointList(_coords));

            public static PointList PointList => _lzPointList.Value;

            public Plus(Point cursor, int color)
                : base(cursor, PointList, color)
            {
            }
        }
    }
}
