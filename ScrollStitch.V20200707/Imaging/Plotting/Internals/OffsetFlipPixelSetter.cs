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

    public struct OffsetFlipPixelSetter
        : IFuncInline<OffsetFlipPixelSetter, int, int, int>
    {
        private readonly int[] _array;
        private readonly int _width;
        private readonly int _height;
        private readonly int _colorValue;
        private readonly int _offsetX;
        private readonly int _maskX;
        private readonly int _offsetY;
        private readonly int _maskY;

        public OffsetFlipPixelSetter(IntBitmap bitmap, int colorValue, Point offset, bool flipX, bool flipY)
        {
            _array = bitmap.Data;
            _width = bitmap.Width;
            _height = bitmap.Height;
            _colorValue = colorValue;
            _offsetX = offset.X;
            _offsetY = offset.Y;
            _maskX = flipX ? -1 : 0;
            _maskY = flipY ? -1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                _array[index] = _colorValue;
            }
            return index;
        }
    }
}
