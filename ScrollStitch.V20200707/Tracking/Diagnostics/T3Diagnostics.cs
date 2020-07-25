using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.Diagnostics
{
    using Collections;
    using Collections.Specialized;
    using Data;
    using Spatial;
    using Text;
    using Utility;
    using BaseDigitsArray = Text.IntegerBaseFormatter.Internals.BaseDigitsArray;

    public class T3Diagnostics
    {
        #region Properties
        public T3Main MainClass { get; }

        public enum Stage
        {
            First = 1,
            Second = 2
        }

        public Stage Which { get; set; } = Stage.Second;

        public T3ClassifierThreshold Threshold => MainClass.FilterThreshold;

        public T3Classifier Classifier => MainClass.Classifier;

        public UniqueList<int> ImageKeys => MainClass.ImageKeys;

        public int ImageKey0 => ImageKeys.ItemAt(0);

        public int ImageKey1 => ImageKeys.ItemAt(1);

        public int ImageKey2 => ImageKeys.ItemAt(2);

        public int MainImageKey => MainClass.MainImageKey;

        public Grid MainGrid => MainClass.MainGrid;

        public Size MainImageSize => MainClass.MainImageSize;

        public T3Movements MovementsClass
        {
            get
            {
                switch (Which)
                {
                    case Stage.First:
                        return MainClass.FirstStageMovements;
                    case Stage.Second:
                        return MainClass.SecondStageMovements;
                    default:
                        return null;
                }
            }
        }

        public UniqueList<(Movement, Movement)> Movements => MovementsClass.Movements;

        public T3CellLabels CellLabels
        {
            get
            {
                switch (Which)
                {
                    case Stage.First:
                        return MainClass.FirstStageCellLabels;
                    case Stage.Second:
                        return MainClass.SecondStageCellLabels;
                    default:
                        return null;
                }
            }
        }

        public IReadOnlyDictionary<CellIndex, IReadOnlyList<int>> CellLabelList => CellLabels.CellLabelList;

        public IReadOnlyDictionary<int, IReadOnlyList<CellIndex>> LabelCellList => CellLabels.LabelCellList;
        #endregion

        #region Execution Options
        /// <summary>
        /// To reduce clutter, movements that contain only one sample point can be suppressed from output.
        /// </summary>
        public bool HideSingleSamples { get; set; } = true;
        #endregion

        public T3Diagnostics(T3Main mainClass, Stage which)
        {
            MainClass = mainClass;
            Which = which;
        }

        public void ReportMovementStats(IMultiLineTextOutput mlto)
        {
            // ====== REMINDER ======
            // This function requires that all statistics must come from either T3Main First Stage or 
            // the Second Stage, without mixing up. 
            // Mixing up will cause errors (IndexOutOfBounds, KeyNotFound, etc) and/or nonsensical results.
            // ======
            var movementsClass = MovementsClass;
            var hashPointsClass = MovementsClass.HashPoints;
            var classifier = Classifier;
            var movements = movementsClass.Movements;
            int movementCount = movements.Count;
            int hiddenRowCount = 0;
            var unmatchedCounts = hashPointsClass.UnmatchedPointCounts;
            //
            mlto.AppendLine($"Three-image trajectories for images ({ImageKey0}, {ImageKey1}, {ImageKey2}):");
            var mltoStationary = new MultiLineTextOutput();
            var mltoAccepted = new MultiLineTextOutput();
            var mltoRejected = new MultiLineTextOutput();
            //
            StringBuilder sb = new StringBuilder();
            for (int label = 0; label < movementCount; ++label)
            {
                var m012 = movements.ItemAt(label);
                var details = classifier.ClassifyMovementWithDetails(label, m012);
                var flags = details.Flags;
                var pointVoteRatio = details.PointVoteRatio;
                var cellVoteRatio = details.CellVoteRatio;
                bool isAccepted = flags.HasFlag(T3ClassifierFlags.Accepted);
                bool isStationary = flags.HasFlag(T3ClassifierFlags.Stationary);
                bool isSingleSample = 
                    pointVoteRatio.Numerator == 1 ||
                    cellVoteRatio.Numerator == 1;
                if (HideSingleSamples && isSingleSample && !isStationary)
                {
                    ++hiddenRowCount;
                    continue;
                }
                (Movement m01, Movement m12) = m012;
                sb.Append($"m01=({m01.DeltaX}, {m01.DeltaY}), ");
                sb.Append($"m12=({m12.DeltaX}, {m12.DeltaY}), ");
                sb.Append($"pointVotes={pointVoteRatio}), ");
                sb.Append($"cellVotes={cellVoteRatio}), ");
                sb.Append($"(flags={flags})");
                if (isStationary)
                {
                    mltoStationary.AppendLine(sb.ToString());
                }
                else if (isAccepted)
                {
                    mltoAccepted.AppendLine(sb.ToString());
                }
                else
                {
                    mltoRejected.AppendLine(sb.ToString());
                }
                sb.Clear();
            }
            mltoStationary.CopyTo(mlto);
            mltoAccepted.CopyTo(mlto);
            mltoRejected.CopyTo(mlto);
            if (HideSingleSamples && hiddenRowCount > 0)
            {
                mlto.AppendLine($"({hiddenRowCount} rows hidden because of {nameof(HideSingleSamples)} flag.)");
            }
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

        public void RenderCellFlags(IMultiLineTextOutput mlto, BaseDigitsArray baseDigitsArray)
        {
            var grid = MainGrid;
            var cellLabelList = CellLabelList;
            var labelCellList = LabelCellList;
            // 
            const int maxUsableBitsForUInt64 = 64;
            int toBase = baseDigitsArray.Base;
            int labelCount = Math.Min(MovementsClass.Movements.Count, maxUsableBitsForUInt64);
            int formatWidth = (int)Math.Max(1, Math.Ceiling(Math.Log(2.0) * labelCount / Math.Log(toBase)));
            //
            ulong GetCellLabelAsUInt64(CellIndex ci)
            {
                if (!cellLabelList.TryGetValue(ci, out var labels))
                {
                    return 0uL;
                }
                ulong result = 0uL;
                foreach (int label in labels)
                {
                    if (label < 0 || label >= maxUsableBitsForUInt64)
                    {
                        continue;
                    }
                    result |= (1uL << label);
                }
                return result;
            }
            //
            string GetRowColumnText(int row, int column)
            {
                var ci = new CellIndex(column, row);
                ulong value = GetCellLabelAsUInt64(ci);
                return IntegerBaseFormatter.Format(value, baseDigitsArray, formatWidth);
            }
            //
            var tgs = TextGridSource.Create(rowCount: grid.GridHeight, columnCount: grid.GridWidth, 
                getTextFunc: GetRowColumnText);
            var tgf = new TextGridFormatter(tgs);
            tgf.Indent = 0;
            tgf.ColumnSpacing = 1;
            tgf.Generate(mlto);
        }
    }
}
