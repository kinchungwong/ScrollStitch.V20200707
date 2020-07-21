using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Compare
{
    using Data;
    using Logging;
    using Spatial;
    using HashCode;
    using Utility;

    /// <summary>
    /// Performs a fast image comparison by randomly selecting a subset of horizontal image strips 
    /// and comparing. The fraction of coverage can be configured.
    /// </summary>
    public class FastImageComparer
    {
        public static bool ShouldBenchmark { get; set; } = false;

        public double SampleFrac { get; set; } = 0.1;

        public bool UseTimeBasedSeed { get; set; } = true;

        public int MinHashValue => (int)Math.Round(int.MinValue * SampleFrac);

        public int MaxHashValue => (int)Math.Round(int.MaxValue * SampleFrac);

        public FastImageComparer()
        { 
        }

        public bool Compare(IntBitmap bitmap1, IntBitmap bitmap2)
        {
            if (!bitmap1.Size.Equals(bitmap2.Size))
            {
                throw new ArgumentException();
            }
            using (var timer = ShouldBenchmark ? new MethodTimer() : null)
            {
                int imageW = bitmap1.Width;
                int imageH = bitmap1.Height;
                int[] data1 = bitmap1.Data;
                int[] data2 = bitmap2.Data;
                Grid grid = Grid.Factory.CreateApproxCellSize(imageW, imageH, 32, 1);
                int gridW = grid.GridWidth;
                int gridH = grid.GridHeight;
                for (int gy = 0; gy < gridH; ++gy)
                {
                    for (int gx = 0; gx < gridW; ++gx)
                    {
                        var ci = new CellIndex(gx, gy);
                        if (!_ShouldProcessCell(ci)) continue;
                        var cr = grid.GetCellRect(ci);
                        int x0 = cr.X;
                        int y0 = cr.Y;
                        int cw = cr.Width;
                        int startIndex = y0 * imageW + x0;
                        for (int dx = 0; dx < cw; ++dx)
                        {
                            if (data1[startIndex + dx] != data2[startIndex + dx])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        private bool _ShouldProcessCell(CellIndex ci)
        {
            int hashValue = _GetCellHashValue(ci);
            return (hashValue >= MinHashValue && hashValue <= MaxHashValue);
        }

        private int _GetCellHashValue(CellIndex ci)
        {
            if (UseTimeBasedSeed)
            {
                uint seed = (uint)(ulong)Stopwatch.GetTimestamp();
                return new HashCodeBuilder(seed).Ingest(ci.CellX, ci.CellY).GetHashCode();
            }
            return ci.GetHashCode();
        }
    }
}
