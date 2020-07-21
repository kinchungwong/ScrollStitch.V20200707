using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.Diagnostics
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Text;
    using ScrollStitch.V20200707.Utility;

    public class T3Diagnostics
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

        /// <summary>
        /// To reduce clutter, movements that contain only one sample point can be suppressed from output.
        /// </summary>
        public bool HideSingleSamples { get; set; } = true;

        public void ReportMovementStats(IMultiLineTextOutput mlto)
        {
            var mltoZero = new MultiLineTextOutput();
            var mltoAccept = new MultiLineTextOutput();
            var mltoReject = new MultiLineTextOutput();
            StringBuilder sb = new StringBuilder();
            mlto.AppendLine($"Three-image trajectories for images ({ImageKey0}, {ImageKey1}, {ImageKey2}):");
            int movementCount = Movements.Count;
            int hiddenCount = 0;
            T3Classifier classifier = new T3Classifier()
            {
                MovementsClass = MovementsClass,
                LabelCellCountsClass = LabelCellCountsClass,
                Threshold = Threshold
            };
            for (int label = 0; label < movementCount; ++label)
            {
                var m012 = Movements.ItemAt(label);
                (Movement m01, Movement m12) = m012;
                int m01x = m01.DeltaX;
                int m01y = m01.DeltaY;
                int m12x = m12.DeltaX;
                int m12y = m12.DeltaY;
                var flags = classifier.ClassifyMovement(label, m012);
                bool isAccepted = flags.HasFlag(T3ClassifierFlags.Accepted);
                bool isStationary = flags.HasFlag(T3ClassifierFlags.Stationary);
                var pointVoteRatio = new PercentFromRatio(LabelPointCounts[label], LabelPointCountsTotal);
                var cellVoteRatio = new PercentFromRatio(LabelCellCounts[label], LabelCellCountsTotal);
                bool isSingleSample = 
                    (pointVoteRatio.Numerator == 1 ||
                    cellVoteRatio.Numerator == 1);
                if (HideSingleSamples && isSingleSample && !isStationary)
                {
                    ++hiddenCount;
                    continue;
                }
                sb.Append($"m01=({m01x}, {m01y}), ");
                sb.Append($"m12=({m12x}, {m12y}), ");
                sb.Append($"pointVotes={pointVoteRatio}), ");
                sb.Append($"cellVotes={cellVoteRatio}), ");
                sb.Append($"(flags={flags})");
                if (isStationary)
                {
                    mltoZero.AppendLine(sb.ToString());
                }
                else if (isAccepted)
                {
                    mltoAccept.AppendLine(sb.ToString());
                }
                else
                {
                    mltoReject.AppendLine(sb.ToString());
                }
                sb.Clear();
            }
            mltoZero.CopyTo(mlto);
            mltoAccept.CopyTo(mlto);
            mltoReject.CopyTo(mlto);
            if (HideSingleSamples && hiddenCount > 0)
            {
                mlto.AppendLine($"({hiddenCount} rows hidden because of {nameof(HideSingleSamples)} flag.)");
            }
            mlto.AppendLine();
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
