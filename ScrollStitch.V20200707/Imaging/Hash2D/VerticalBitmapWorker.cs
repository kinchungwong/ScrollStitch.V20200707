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
    using Memory;
    using Spatial;

    public class VerticalBitmapWorker
    {
        #region private static
        private static IArrayPoolClient<ulong> DefaultArrayPool { get; } = ExactLengthArrayPool<ulong>.DefaultInstance;
        #endregion

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
        public VerticalBitmapWorker(Hash2DProcessor host, StageConfig config, IntBitmap input, IntBitmap output)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong[] BufferCreate()
        {
            return DefaultArrayPool.Rent(_width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferReturn(ulong[] buffer)
        {
            DefaultArrayPool.Return(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferFill(ulong[] buffer, ulong value)
        {
            // Next two lines required for optimization
            int width = _width;
            _ValidateLength(buffer, width);
            for (int x = 0; x < width; ++x)
            {
                buffer[x] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferLoad(ulong[] buffer, int row)
        {
            // Next two lines required for optimization
            int width = _width;
            _ValidateLength(buffer, width);
            if (row < 0)
            {
                int fillValue = Config.FillValue;
                ulong uFillValue = unchecked((uint)fillValue);
                for (int x = 0; x < width; ++x)
                {
                    buffer[x] = uFillValue;
                }
                return;
            }
            int[] data = InputBitmap.Data;
            int rowStart = row * width;
            for (int x = 0; x < width; ++x)
            {
                int value = data[rowStart + x];
                ulong uValue = unchecked((uint)value);
                buffer[x] = uValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferStore(ulong[] buffer, int row)
        {
            // Next two lines required for optimization
            int width = _width;
            _ValidateLength(buffer, width);
            int[] data = OutputBitmap.Data;
            int rowStart = row * width;
            for (int x = 0; x < width; ++x)
            {
                ulong uValue = buffer[x];
                int value = unchecked((int)(uint)uValue);
                data[rowStart + x] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferMultiply(ulong[] buffer, ulong value)
        {
            // Next two lines required for optimization
            int width = _width;
            _ValidateLength(buffer, width);
            for (int x = 0; x < width; ++x)
            {
                buffer[x] = buffer[x] * value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferXor(ulong[] dest, ulong[] source)
        {
            // Next three lines required for optimization
            int width = _width;
            _ValidateLength(dest, width);
            _ValidateLength(source, width);
            for (int x = 0; x < width; ++x)
            {
                dest[x] = dest[x] * source[x];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferWrapBits(ulong[] buffer)
        {
            // Next two lines required for optimization
            int width = _width;
            _ValidateLength(buffer, width);
            for (int x = 0; x < width; ++x)
            {
                ulong value = buffer[x];
                buffer[x] = value ^ (value >> 32);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ProcessRow(int row)
        {
            int windowSize = Config.WindowSize;
            int skipStep = Config.SkipStep;
            var inputBuffer = BufferCreate();
            var hashBuffer = BufferCreate();
            BufferFill(hashBuffer, 2166136261u);
            for (int backX = windowSize - 1; backX >= 0; --backX)
            {
                int inRow = row - backX * skipStep;
                BufferLoad(inputBuffer, inRow);
                BufferMultiply(hashBuffer, 16777619u);
                BufferXor(hashBuffer, inputBuffer);
                BufferWrapBits(hashBuffer);
            }
            BufferStore(hashBuffer, row);
            BufferReturn(inputBuffer);
            BufferReturn(hashBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _ValidateLength(ulong[] arr, int expectedLen)
        {
            if (arr.Length != expectedLen)
            {
                throw new Exception("Internal error: unexpected array length mismatch.");
            }
        }
    }
}
