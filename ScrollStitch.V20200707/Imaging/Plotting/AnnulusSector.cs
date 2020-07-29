using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using Internals;

    /// <summary>
    /// Paints a filled annulus sector on the bitmap.
    /// 
    /// <para>
    /// Mathematically, annulus sector is defined in polar terms, as the set of pixels <c>(rho, theta)</c>
    /// that satisfy:
    /// </para>
    /// <code>
    /// L001    (rho &gt;= rhoMin &amp;&amp; rho &lt;= rhoMax) &amp;&amp; <br/> 
    /// L002    (theta &gt;= thetaMin &amp;&amp; theta &lt;= thetaMax) <br/>
    /// </code>
    /// <para>
    /// for the range specified in <c>rhoMin, rhoMax, thetaMin,</c> and <c>thetaMax</c>.
    /// </para>
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
    /// 
    /// </summary>
    public class AnnulusSector 
        : IDrawCmd
    {
        public Point Center { get; }

        public int RadiusBegin { get; }

        public int RadiusEnd { get; }

        public int DegreeBegin { get; }

        public int DegreeEnd { get; }

        public int Color { get; }

        private List<Action<IntBitmap>> _quadrantCmds;

        public AnnulusSector(Point center, int radiusBegin, int radiusEnd, int degreeBegin, int degreeEnd, int color)
        {
            Center = center;
            RadiusBegin = radiusBegin;
            RadiusEnd = radiusEnd;
            DegreeBegin = degreeBegin;
            DegreeEnd = degreeEnd;
            Color = color;
            _CtorInitQuadrants();
        }

        public void Draw(IntBitmap dest)
        {
            foreach (var qcmd in _quadrantCmds)
            {
                qcmd(dest);
            }
        }

        private void _CtorInitQuadrants()
        {
            _quadrantCmds = new List<Action<IntBitmap>>();
            Range degreeRange = new Range(DegreeBegin, DegreeEnd);
            for (int kq = 0; kq < 4; ++kq)
            {
                var qrange = new Range(kq * 90, kq * 90 + 90);
                var qinter = _RangeIntersect(qrange, degreeRange);
                if (qinter.IsEmpty)
                {
                    continue;
                }
                bool flipX = false;
                bool flipY = false;
                Range flippedRange;
                switch (kq)
                {
                    case 0:
                        flippedRange = qinter;
                        break;
                    case 1:
                        flipX = true;
                        flippedRange = new Range(180 - qinter.Stop, 180 - qinter.Start);
                        break;
                    case 2:
                        flipX = true;
                        flipY = true;
                        flippedRange = new Range(qinter.Start - 180, qinter.Stop - 180);
                        break;
                    case 3:
                        flipY = true;
                        flippedRange = new Range(360 - qinter.Stop, 360 - qinter.Start);
                        break;
                    default:
                        throw new Exception();
                }
                var quadrantLoop = new AnnulusSectorQuadrantLoop(RadiusBegin, RadiusEnd, flippedRange.Start, flippedRange.Stop);
                Action<IntBitmap> qcmd = new Action<IntBitmap>(
                    (IntBitmap bitmap) =>
                    {
                        var pixelSetter = new OffsetFlipPixelSetter(bitmap, Color, Center, flipX, flipY);
                        quadrantLoop.Invoke(pixelSetter);
                    });
                _quadrantCmds.Add(qcmd);
            }
        }

        private static Range _RangeIntersect(Range a, Range b)
        {
            bool ae = a.IsEmpty;
            bool be = b.IsEmpty;
            if (ae || be)
            {
                return Range.Nothing;
            }
            int start = Math.Max(a.Start, b.Start);
            int stop = Math.Min(a.Stop, b.Stop);
            if (start < stop)
            {
                return new Range(start, stop);
            }
            return Range.Nothing;
        }
    }
}
