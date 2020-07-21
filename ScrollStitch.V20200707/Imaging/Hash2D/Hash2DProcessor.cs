using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using Data;
    using Logging;
    using ScrollStitch.V20200707.Utility;

    public class Hash2DProcessor
    {
        #region private
        private List<StageConfig> _stages = new List<StageConfig>();
        #endregion

        public IReadOnlyList<StageConfig> Stages => _stages.AsReadOnly();

        public bool Parallelize { get; set; } = true;
        public int ParallelStripCount { get; set; } = 32;

        public Hash2DProcessor()
        {
        }

        public void AddStage(
            Direction direction, int windowSize, 
            int skipStep = StageConfig.Defaults.SkipStep, 
            int fillValue = StageConfig.Defaults.FillValue)
        {
            _stages.Add(new StageConfig(direction, windowSize, skipStep, fillValue));
        }

        public void AddStage(StageConfig stage)
        {
            _stages.Add(stage);
        }

        public IntBitmap Process(IntBitmap input)
        {
            int width = input.Width;
            int height = input.Height;
            IntBitmap output = _Rent(width, height);
            Process(input, output);
            return output;
        }

        public void Process(IntBitmap input, IntBitmap output)
        {
            _ValidateBitmapsSameSize(input, output);
            int width = input.Width;
            int height = input.Height;
            int stageCount = Stages.Count;
            if (stageCount == 0)
            {
                var rect = new Rect(0, 0, width, height);
                BitmapCopyUtility.CopyRect(input, rect, output, Point.Origin);
                return;
            }
            if (stageCount == 1)
            {
                _ProcessStage(0, input, output);
                return;
            }
            IntBitmap lastResult = null;
            for (int kStage = 0; kStage < stageCount; ++kStage)
            {
                IntBitmap thisStageInput = (kStage == 0) ? input : lastResult;
                IntBitmap thisStageOutput = (kStage + 1 == stageCount) ? output : _Rent(width, height);
                _ProcessStage(kStage, thisStageInput, thisStageOutput);
                if (!(lastResult is null))
                {
                    _Return(lastResult);
                }
                if (kStage + 1 < stageCount)
                {
                    lastResult = thisStageOutput;
                }
            }
        }

        internal static void _ValidateBitmapsSameSize(IntBitmap input, IntBitmap output)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            int width = input.Width;
            int height = input.Height;
            if (output.Width != width ||
                output.Height != height)
            {
                throw new InvalidOperationException("Bitmap size mismatch.");
            }
        }

        private void _ProcessStage(int stageIndex, IntBitmap input, IntBitmap output)
        {
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ProcessStage)}({stageIndex})"))
            {
                StageConfig config = Stages[stageIndex];
                switch (config.Direction)
                {
                    case Direction.Horizontal:
                        var hp = new HorizontalBitmapWorker(this, config, input, output);
                        hp.Process();
                        return;
                    case Direction.Vertical:
                        var vp = new VerticalBitmapWorker(this, config, input, output);
                        vp.Process();
                        return;
                    default:
                        throw new Exception();
                }
            }
        }

        private IntBitmap _Rent(int width, int height)
        {
            // IntBitmap will automatically use a suitable array, if possible.
            return new IntBitmap(width, height);
        }

        private void _Return(IntBitmap bitmap)
        {
            bitmap.Dispose();
        }
    }
}
