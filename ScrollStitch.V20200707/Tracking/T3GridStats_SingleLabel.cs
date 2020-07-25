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
    /// Given a specific trajectory label, this class helps compute a <see cref="GridArray{T}"/> 
    /// of <see cref="int"/> which contains, within each cell rectangle, the number of samples 
    /// confirming that particular trajectory.
    /// </summary>
    /// 
    [Obsolete]
    public sealed class T3GridStats_SingleLabel
        : T3GridStats_Base<GridArray<int>>
    {
        private int _labelOfInterest;
        private GridArray<int> _array;

        public T3GridStats_SingleLabel(T3GridStats host, int labelOfInterest)
            : base(host)
        {
            _labelOfInterest = labelOfInterest;
            _array = new GridArray<int>(host.Grid);
        }

        public override void Add(Point point, int hashValue, int label, CellIndex cellIndex)
        {
            if (label != _labelOfInterest)
            {
                return;
            }
            int oldValue = _array[cellIndex];
            int newValue = oldValue + 1;
            _array[cellIndex] = newValue;
        }

        public override GridArray<int> GetResult()
        {
            return _array;
        }

        public override object GetResultAsObject() => GetResult();
    }
}
