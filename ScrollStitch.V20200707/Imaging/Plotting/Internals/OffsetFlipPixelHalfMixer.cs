using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.Internals
{
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Data;

    public struct OffsetFlipPixelHalfMixer
        : IFunc<OffsetFlipPixelHalfMixer, int, int, int>
    {
        private readonly int[] _array;
        private readonly int _width;
        private readonly int _height;
        private readonly int _colorValue;
        private readonly int _colorR;
        private readonly int _colorG;
        private readonly int _colorB;
        private readonly int _offsetX;
        private readonly int _maskX;
        private readonly int _offsetY;
        private readonly int _maskY;

        public OffsetFlipPixelHalfMixer(IntBitmap bitmap, int colorValue, Point offset, bool flipX, bool flipY)
        {
            _array = bitmap.Data;
            _width = bitmap.Width;
            _height = bitmap.Height;
            _colorValue = colorValue;
            _colorR = (colorValue >> 16) & 255;
            _colorG = (colorValue >> 8) & 255;
            _colorB = colorValue & 255;
            _offsetX = offset.X;
            _offsetY = offset.Y;
            _maskX = flipX ? -1 : 0;
            _maskY = flipY ? -1 : 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Invoke(int x, int y)
        {
            int index = -1;
            unchecked
            {
                // ======
                // Branchless multiply.
                // ======
                x = (x ^ _maskX) - _maskX;
                y = (y ^ _maskY) - _maskY;
                x += _offsetX;
                y += _offsetY;
                if (unchecked((uint)x < (uint)_width && (uint)y < (uint)_height))
                {
                    index = y * _width + x;
                }
            }
            if (index >= 0)
            {
                int oldValue = _array[index];
                int newValue;
                unchecked
                {
                    int oldB = oldValue & 255;
                    int oldG = (oldValue >> 8) & 255;
                    int oldR = (oldValue >> 16) & 255;
#if true
                    int newB = (oldB + _colorB * 3 + 2) / 4;
                    int newG = (oldG + _colorG * 3 + 2) / 4;
                    int newR = (oldR + _colorR * 3 + 2) / 4;
#else
                        int newB = Math.Max(byte.MinValue, Math.Min(byte.MaxValue, oldB + ((_colorB - 128) * 2)));
                        int newG = Math.Max(byte.MinValue, Math.Min(byte.MaxValue, oldG + ((_colorG - 128) * 2)));
                        int newR = Math.Max(byte.MinValue, Math.Min(byte.MaxValue, oldR + ((_colorR - 128) * 2)));
#endif
                    newValue = newB | (newG << 8) | (newR << 16);
                }
                _array[index] = newValue;
            }
            return index;
        }
    }
}
