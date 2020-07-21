using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;

    public static class RectTreeUtility
    {
        /// <summary>
        /// This is an alternate implementation of methods on Rect
        /// 
        /// This is because the Intersect methods on Rect have not yet been tested.
        /// 
        /// 
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool HasPositiveOverlap(Rect r1, Rect r2)
        {
            if (r1.Width < 1 ||
                r1.Height < 1 ||
                r2.Width < 1 ||
                r2.Height < 1)
            {
                return false;
            }
            int left = Math.Max(r1.Left, r2.Left);
            int right = Math.Min(r1.Right, r2.Right);
            int top = Math.Max(r1.Top, r2.Top);
            int bottom = Math.Min(r1.Bottom, r2.Bottom);
            return (right > left &&
                bottom > top);
        }

    }
}
