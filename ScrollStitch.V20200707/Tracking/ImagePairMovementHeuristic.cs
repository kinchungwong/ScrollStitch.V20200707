using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using Data;
    using Collections.Specialized;

    /// <summary>
    /// Provides 
    /// </summary>
    public class ImagePairMovementHeuristic
    {
        public int FirstPointCount { get; set; }

        public int SecondPointCount { get; set; }

        public int CommonPointCount { get; set; }

        public int UnmatchedFirstCount { get; set; }

        public int UnmatchedSecondCount { get; set; }

        public IHistogram<Movement, int> MovementHistogram { get; set; }

        public List<KeyValuePair<Movement, int>> SortedFilteredHistogram { get; set; }

        public int ZeroMovementVotes { get; set; }
        
        public int BestNonZeroMovementVotes { get; set; }
        
        public int SecondNonZeroMovementVotes { get; set; }

        public Movement? BestNonZeroMovement { get; set; }

        public Movement? SecondNonZeroMovement { get; set; }

        public ImagePairMovementScore Score { get; set; }

        public ImagePairMovementFlag Flag { get; set; }

        public ImagePairMovementHeuristic(ImagePairMovementDetail detail)
        {
            FirstPointCount = detail.FirstPoints.Count;
            SecondPointCount = detail.SecondPoints.Count;
            CommonPointCount = detail.CommonPoints.Count;
            UnmatchedFirstCount = detail.UnmatchedFirstPoints.Count;
            UnmatchedSecondCount = detail.UnmatchedSecondPoints.Count;
            MovementHistogram = detail.MovementHistogram;
            SortedFilteredHistogram = detail.GetSortedHistogram(0.01);
            _CtorAnalyzeHist();
            _CtorComputeScore();
        }

        private void _CtorAnalyzeHist()
        {
            foreach (var kvp in SortedFilteredHistogram)
            {
                var movement = kvp.Key;
                int votes = kvp.Value;
                if (movement.DeltaX == 0 &&
                    movement.DeltaY == 0)
                {
                    ZeroMovementVotes = votes;
                }
                else if (!BestNonZeroMovement.HasValue)
                {
                    BestNonZeroMovement = movement;
                    BestNonZeroMovementVotes = votes;
                }
                else if (!SecondNonZeroMovement.HasValue)
                {
                    SecondNonZeroMovement = movement;
                    SecondNonZeroMovementVotes = votes;
                }
            }
        }

        private void _CtorComputeScore()
        {
            Score = new ImagePairMovementScore();
            int total = CommonPointCount + Math.Max(UnmatchedFirstCount, UnmatchedSecondCount);

            //Score.StillScore = ZeroMovementVotes;
            //Score.MovedScore;
            //Score.MovedChurn;
            //Score.BreakScore;
            //Score.StationaryFrac;

            if (UnmatchedFirstCount >= 0.25 * FirstPointCount ||
                UnmatchedSecondCount >= 0.25 * SecondPointCount)
            {
                Flag = ImagePairMovementFlag.Break;
            }
            else if (!BestNonZeroMovement.HasValue ||
                BestNonZeroMovementVotes <= 0.05 * ZeroMovementVotes)
            {
                if (CommonPointCount >= 0.9 * total &&
                    ZeroMovementVotes >= 0.9 * CommonPointCount)
                {
                    Flag = ImagePairMovementFlag.Still;
                }
                else
                {
                    Flag = ImagePairMovementFlag.Unexplained;
                }
            }
            else if (SecondNonZeroMovementVotes <= 0.2 * BestNonZeroMovementVotes)
            {
                if (BestNonZeroMovementVotes >= 0.2 * CommonPointCount)
                {
                    Flag = ImagePairMovementFlag.Moved;
                }
                else
                {
                    Flag = ImagePairMovementFlag.Unexplained;
                }
            }
            else
            {
                Flag = ImagePairMovementFlag.Unexplained;
            }
        }

        public void ToString(StringBuilder sb, string indent = null)
        {
            if (indent is null)
            {
                indent = string.Empty;
            }
            sb.AppendLine($"{indent}FirstPointCount: {FirstPointCount}");
            sb.AppendLine($"{indent}SecondPointCount: {SecondPointCount}");
            sb.AppendLine($"{indent}CommonPointCount: {CommonPointCount}");
            sb.AppendLine($"{indent}UnmatchedFirstCount: {UnmatchedFirstCount}");
            sb.AppendLine($"{indent}UnmatchedSecondCount: {UnmatchedSecondCount}");
            sb.AppendLine($"{indent}ZeroMovementVotes: {ZeroMovementVotes}");
            if (BestNonZeroMovement.HasValue)
            {
                sb.AppendLine($"{indent}BestNonZeroMovementVotes: {BestNonZeroMovementVotes}");
                sb.AppendLine($"{indent}BestNonZeroMovement: {BestNonZeroMovement.Value}");
                if (SecondNonZeroMovement.HasValue)
                {
                    sb.AppendLine($"{indent}SecondNonZeroMovementVotes: {SecondNonZeroMovementVotes}");
                    sb.AppendLine($"{indent}SecondNonZeroMovement: {SecondNonZeroMovement.Value}");
                }
            }
            sb.AppendLine($"{indent}Classification: {Flag}");
        }
    }
}
