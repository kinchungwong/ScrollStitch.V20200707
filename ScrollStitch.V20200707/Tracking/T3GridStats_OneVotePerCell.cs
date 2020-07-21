using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// This class helps compute a histogram of trajectory labels by counting the number of 
    /// grid cells that contain samples that match the trajectory.
    /// 
    /// <para>
    /// In other words, each grid rectangle that contain one or more samples matching that 
    /// trajectory will contribute one vote (on behalf of all samples from the grid cell) 
    /// toward the histogram bin for that trajectory.
    /// </para>
    /// </summary>
    /// 
    public sealed class T3GridStats_OneVotePerCell 
        : T3GridStats_Base<IHistogram<int, int>>
    {
        private IHistogram<int, int> _hist;
        private HashSet<(CellIndex, int)> _hasSeen;

        public T3GridStats_OneVotePerCell(T3GridStats host)
            : base(host)
        {
            _hist = HistogramUtility.CreateIntHistogram<int>();
            _hasSeen = new HashSet<(CellIndex, int)>();
        }

        public override void Add(Point point, int hashValue, int label, CellIndex cellIndex)
        {
            var seenKey = (cellIndex, label);
            if (_hasSeen.Contains(seenKey))
            {
                return;
            }
            _hasSeen.Add(seenKey);
            _hist.Add(label);
        }

        public override IHistogram<int, int> GetResult()
        {
            return _hist;
        }

        public override object GetResultAsObject() => GetResult();
    }
}
