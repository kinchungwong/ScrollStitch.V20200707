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
    using ScrollStitch.V20200707.Text;
    using ScrollStitch.V20200707.Utility;

    public class T3Classifier
    {
        public T3Movements MovementsClass { get; set; }

        public T3GridStats_OneVotePerCell LabelCellCountsClass { get; set; }

        public T3ClassifierThreshold Threshold { get; set; } = new T3ClassifierThreshold();

        public UniqueList<(Movement, Movement)> Movements => MovementsClass.Movements;

        public UniqueList<int> ImageKeys => MovementsClass?.ImageKeys;

        public int ImageKey0 => ImageKeys?.ItemAt(0) ?? -1;

        public int ImageKey1 => ImageKeys?.ItemAt(1) ?? -1;

        public int ImageKey2 => ImageKeys?.ItemAt(2) ?? -1;

        public IReadOnlyDictionary<int, int> LabelPointCounts => MovementsClass?.LabelPointCounts;

        public int LabelPointCountsTotal => MovementsClass?.HashValues.Count ?? 0;

        public IHistogram<int, int> LabelCellCounts => LabelCellCountsClass?.GetResult();

        public int LabelCellCountsTotal => _StcArrayOfSize(LabelCellCountsClass?.Grid.GridSize ?? null);

        public T3ClassifierFlags ClassifyMovement((Movement, Movement) m012)
        {
            int label = Movements.IndexOf(m012);
            return ClassifyMovement(label, m012);
        }

        public T3ClassifierFlags ClassifyMovement(int label)
        {
            var m012 = Movements.ItemAt(label);
            return ClassifyMovement(label, m012);
        }

        public T3ClassifierFlags ClassifyMovement(int label, (Movement, Movement) m012)
        { 
            (Movement m01, Movement m12) = m012;
            int m01x = m01.DeltaX;
            int m01y = m01.DeltaY;
            int m12x = m12.DeltaX;
            int m12y = m12.DeltaY;
            var flags = T3ClassifierFlags.None;
            bool hasRejected = false;
            bool isZero = (m01x, m01y, m12x, m12y) == (0, 0, 0, 0);
            if (isZero)
            {
                flags |= T3ClassifierFlags.Stationary;
            }
            var pointVoteRatio = new PercentFromRatio(LabelPointCounts[label], LabelPointCountsTotal);
            if (pointVoteRatio.Numerator < Threshold.MinimumValidPointVote ||
                pointVoteRatio.Ratio < Threshold.MinimumValidPointVoteFraction)
            {
                hasRejected = true;
                flags |= T3ClassifierFlags.RejectedTooFewPoints;
            }
            var cellVoteRatio = new PercentFromRatio(LabelCellCounts[label], LabelCellCountsTotal);
            if (cellVoteRatio.Numerator < Threshold.MinimumValidCellVote ||
                cellVoteRatio.Ratio < Threshold.MinimumValidCellVoteFraction)
            {
                hasRejected = true;
                flags |= T3ClassifierFlags.RejectedTooFewCells;
            }
            if (!hasRejected)
            {
                flags |= T3ClassifierFlags.Accepted;
            }
            return flags;
        }

        private int _StcArrayOfSize(Size? sz)
        {
            if (!sz.HasValue)
            {
                return 0;
            }
            return sz.Value.Width * sz.Value.Height;
        }
    }
}
