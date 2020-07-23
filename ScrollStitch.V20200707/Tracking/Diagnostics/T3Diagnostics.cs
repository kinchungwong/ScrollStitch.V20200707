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

        public T3GridStats_CellFlags<ulong> CellFlagsClass { get; set; }

        public Dictionary<int, T3GridStats_SingleLabel> SingleLabelGridHistograms { get; set; } = new Dictionary<int, T3GridStats_SingleLabel>();

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
            var unmatchedCounts = MovementsClass.HashPoints.UnmatchedPointCounts;
            int umc0 = unmatchedCounts[ImageKey0];
            int umc1 = unmatchedCounts[ImageKey1];
            int umc2 = unmatchedCounts[ImageKey2];
            mlto.AppendLine($"Image {ImageKey0} has {umc0} unmatched points.");
            mlto.AppendLine($"Image {ImageKey1} has {umc1} unmatched points.");
            mlto.AppendLine($"Image {ImageKey2} has {umc2} unmatched points.");
            mlto.AppendLine();
        }

        public void RenderCellFlags(IMultiLineTextOutput mlto, int toBase = 2)
        {
            // ======
            // We intentionally allow toBase to be non-power-of-two values, because this causes 
            // an whimsical "string hashing" effect that turns out to be practically valuable for 
            // visually identifying clusters of relevant image content.
            // ======
            var baseDigitsArray = IntegerBaseFormatter.GetBaseDigitsArrayForBase(toBase);
            RenderCellFlags(mlto, baseDigitsArray);
        }

        public void RenderCellFlags(IMultiLineTextOutput mlto, IntegerBaseFormatter.Internals.BaseDigitsArray baseDigitsArray)
        {
            if (CellFlagsClass is null)
            {
                return;
            }
            GridArray<ulong> cellFlags = CellFlagsClass.GetResult();
            if (cellFlags is null)
            {
                return;
            }
            int toBase = baseDigitsArray.Base;
            int maxUsableBitsForUInt64 = 64;
            int labelCount = Math.Min(Movements.Count, maxUsableBitsForUInt64);
            int formatWidth = (int)Math.Max(1, Math.Ceiling(Math.Log(2.0) * labelCount / Math.Log(toBase)));
            string CellValueToStringFunc(ulong value)
            {
                return IntegerBaseFormatter.Format(value, baseDigitsArray, formatWidth);
            }
            var myTGS = new Internal_TextGridHook(cellFlags, CellValueToStringFunc);
            var myTGF = new TextGridFormatter(myTGS);
            myTGF.Indent = 0;
            myTGF.ColumnSpacing = 1;
            myTGF.Generate(mlto);
        }

        private class Internal_TextGridHook
            : ITextGridSource
        {
            public GridArray<ulong> Array { get; }

            public Func<ulong, string> ToStringFunc { get; }

            public int RowCount => Array.GridHeight;

            public int ColumnCount => Array.GridWidth;

            internal Internal_TextGridHook(GridArray<ulong> array, Func<ulong, string> toStringFunc)
            {
                Array = array;
                ToStringFunc = toStringFunc;
            }

            public string GetItem(int row, int column)
            {
                var ci = new CellIndex(column, row);
                return ToStringFunc(Array[ci]);
            }
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
