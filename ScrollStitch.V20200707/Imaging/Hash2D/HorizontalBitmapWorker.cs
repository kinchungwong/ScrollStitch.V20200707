using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using Data;
    using Spatial;

    public class HorizontalBitmapWorker
    {
        public Hash2DProcessor Host { get; }
        public StageConfig Config { get; }
        public Size ImageSize { get; }
        public IntBitmap InputBitmap { get; }
        public IntBitmap OutputBitmap { get; }

        #region private
        private int _width;
        private int _height;
        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        public HorizontalBitmapWorker(Hash2DProcessor host, StageConfig config, IntBitmap input, IntBitmap output)
        {
            Hash2DProcessor._ValidateBitmapsSameSize(input, output);
            Host = host;
            Config = config;
            InputBitmap = input;
            OutputBitmap = output;
            _width = input.Width;
            _height = input.Height;
            ImageSize = new Size(_width, _height);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Process()
        {
            if (Host.Parallelize && 
                _height >= Host.ParallelStripCount &&
                !Thread.CurrentThread.IsThreadPoolThread)
            {
                _ProcessParallelize();
            }
            else 
            {
                for (int row = 0; row < _height; ++row)
                {
                    ProcessRow(row);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _ProcessParallelize()
        {
            AxisSubdiv subdiv = AxisSubdivFactory.CreateNearlyUniform(_height, Host.ParallelStripCount);
            int stripCount = subdiv.Count;
            var stripTasks = new Task[stripCount];
            for (int stripIndex = 0; stripIndex < stripCount; ++stripIndex)
            {
                Range rowRange = subdiv[stripIndex];
                stripTasks[stripIndex] = Task.Run(() => _ProcessRowRange(rowRange));
            }
            Task.WaitAll(stripTasks);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _ProcessRowRange(Range rowRange)
        {
            rowRange.ForEach(ProcessRow);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ProcessRow(int row)
        {
            int[] inputData = InputBitmap.Data;
            int[] outputData = OutputBitmap.Data;
            int rowStart = row * _width;
            int windowSize = Config.WindowSize;
            int fillValue = Config.FillValue;
            int skipStep = Config.SkipStep;
            int Get(int x)
            {
                return inputData[rowStart + x];
            }
            int SafeGet(int x)
            {
                return (x < 0) ? fillValue : Get(x);
            }
            void Store(int x, int result)
            {
                outputData[rowStart + x] = result;
            }
            for (int outX = 0; outX < _width; ++outX)
            {
                // The hash approach is intentionally crappy. 
                // There is no need to have good quality.
                // Rather, speed and deterministic output would be most important.
                //
                // Note: even though some of the constants are taken from FNV1a,
                // the code below is bastardized, and have NOT been checked for 
                // algorithmic correctness or fitness-of-purpose.
                //
                ulong uvalue = 2166136261u;
                for (int backX = windowSize - 1; backX >= 0; --backX)
                {
                    int inX = outX - backX * skipStep;
                    int item = SafeGet(inX);
                    ulong uitem = unchecked((uint)item);
                    uvalue *= 16777619u;
                    uvalue ^= uitem;
                    uvalue ^= uvalue >> 32;
                }
                Store(outX, unchecked((int)(uint)uvalue));
            }
        }
    }
}
