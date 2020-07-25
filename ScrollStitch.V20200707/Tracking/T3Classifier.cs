using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using Collections;
    using Data;
    using Utility;

    public class T3Classifier
    {
        public T3Movements MovementsClass { get; }

        public T3CellLabels CellLabelsClass { get; }

        public T3ClassifierThreshold Threshold { get; }

        public UniqueList<(Movement, Movement)> Movements => MovementsClass.Movements;

        public UniqueList<int> ImageKeys => MovementsClass.ImageKeys;

        public int ImageKey0 => ImageKeys.ItemAt(0);

        public int ImageKey1 => ImageKeys.ItemAt(1);

        public int ImageKey2 => ImageKeys.ItemAt(2);

        public IReadOnlyDictionary<int, int> LabelPointCounts => MovementsClass.LabelPointCounts;

        public int LabelPointCountsTotal => MovementsClass.HashValues.Count;

        public IReadOnlyDictionary<int, int> LabelCellCounts { get; private set; }

        public int LabelCellCountsTotal { get; private set; }

        public T3Classifier(T3Movements movements, T3CellLabels cellLabels, T3ClassifierThreshold threshold)
        {
            MovementsClass = movements;
            CellLabelsClass = cellLabels;
            Threshold = threshold;
            _CtorInitLabelCellCounts();
        }

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
            var details = ClassifyMovementWithDetails(label, m012);
            return details.Flags;
        }

        public Details ClassifyMovementWithDetails(int label, (Movement, Movement) m012)
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
            return (flags, pointVoteRatio, cellVoteRatio);
        }

        public struct Details
        {
            public T3ClassifierFlags Flags { get; set; }

            public PercentFromRatio PointVoteRatio { get; set; }

            public PercentFromRatio CellVoteRatio { get; set; }

            public void Deconstruct(out T3ClassifierFlags flags, out PercentFromRatio pointVoteRatio, out PercentFromRatio cellVoteRatio)
            {
                flags = Flags;
                pointVoteRatio = PointVoteRatio;
                cellVoteRatio = CellVoteRatio;
            }

            public static implicit operator Details((T3ClassifierFlags Flags, PercentFromRatio PointVoteRatio, PercentFromRatio CellVoteRatio) value)
            {
                return new Details()
                {
                    Flags = value.Flags,
                    PointVoteRatio = value.PointVoteRatio,
                    CellVoteRatio = value.CellVoteRatio
                };
            }

            public static implicit operator (T3ClassifierFlags Flags, PercentFromRatio PointVoteRatio, PercentFromRatio CellVoteRatio)(Details value)
            {
                return (value.Flags, value.PointVoteRatio, value.CellVoteRatio);
            }
        }

        private void _CtorInitLabelCellCounts()
        {
            var gridSize = CellLabelsClass.MainGrid.GridSize;
            LabelCellCountsTotal = gridSize.Width * gridSize.Height;
            var labelCellCounts = new Dictionary<int, int>();
            foreach (var kvp in CellLabelsClass.LabelCellList)
            {
                int label = kvp.Key;
                var cellIndexList = kvp.Value;
                labelCellCounts.Add(label, cellIndexList.Count);
            }
            LabelCellCounts = labelCellCounts.AsReadOnly();
        }
    }
}
