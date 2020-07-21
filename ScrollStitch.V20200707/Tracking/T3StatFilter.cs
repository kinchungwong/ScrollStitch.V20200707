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

    // ======
    // TODO INCOMPLETE
    // ======

    public class T3StatFilter
    {
        public enum LabelReason
        { 
            Unspecified = 0,
            Accepted = 1,
            Rejected_TooFewPoints = 2,
            Rejected_TooFewCells = 3
        }

        public T3Movements Movements { get; }

        public Dictionary<int, LabelReason> LabelReasons;

        private void _PopulateLabelReasons()
        { 

        }

        //private void _PopulateLabelHist()
        //{
        //    _labelHist = HistogramUtility.CreateIntHistogram<int>();
        //    foreach (int label in _labels)
        //    {
        //        _labelHist.Add(label);
        //    }
        //}

        //private void _
    }
}
