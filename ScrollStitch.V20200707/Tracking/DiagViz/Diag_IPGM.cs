using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.DiagViz
{
    using Data;
    using Caching;
    using Imaging;
    using Spatial;
    using Imaging.Plotting;

    /// <summary>
    /// A diagnostic visualization helper class for <see cref="ImagePairGridMovement"/>
    /// </summary>
    public class Diag_IPGM
    {
        public IItemSource<IntBitmap> ColorBitmapSource { get; set; }
        public ImagePairGridMovement Ipgm { get; }

        public Diag_IPGM(IItemSource<IntBitmap> colorBitmapSource, ImagePairGridMovement ipgm)
        {
            ColorBitmapSource = colorBitmapSource;
            Ipgm = ipgm;
        }

        public IntBitmap Render(ImagePairGridMovement.Result result)
        {
            int prevKey = result.PrevImageKey;
            int currKey = result.CurrImageKey;
            IntBitmap currColor = ColorBitmapSource[currKey];
            Grid grid = result.Grid;
            var dict = result.GridCellMovements;
            int iw = grid.InputWidth;
            int ih = grid.InputHeight;
            int ia = iw * ih;
            IntBitmap canvas = new IntBitmap(iw, ih);
            for (int i = 0; i < ia; ++i)
            {
                canvas.Data[i] = (currColor.Data[i] >> 2) & 0x003F3F3F;
            }
            foreach (var kvp in dict)
            {
                CellIndex ci = kvp.Key;
                Rect cr = grid.GetCellRect(ci);
                Point cc = new Point(cr.X + cr.Width / 2, cr.Y + cr.Height / 2);
                HashSet<Movement> hs = kvp.Value;
                foreach (var m in hs)
                {
                    // REMARK 
                    // Careful with sign (polarity)
                    // Given Movement movement = currP - prevP;
                    // we have prevP = currP - movement
#if false
                    Point cc2 = cc - m;
#else
                    // I dunno why, but flipping the sign seems to render the correct result.
                    Point cc2 = cc + m;
#endif
                    new LineCmd(cc, cc2, 0x00FFFFFF).Draw(canvas);
                }
            }
            return canvas;
        }
    }
}
