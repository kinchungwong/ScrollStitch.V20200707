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

    public struct PixelSetter
        : IFuncInline<PixelSetter, int, int, int>
    {
        private readonly int[] _array;
        private readonly int _width;
        private readonly int _height;
        private readonly int _colorValue;

        public PixelSetter(IntBitmap bitmap, int colorValue)
        {
            _array = bitmap.Data;
            _width = bitmap.Width;
            _height = bitmap.Height;
            _colorValue = colorValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Invoke(int x, int y)
        {
            int index = y * _width + x;
            if (unchecked(
                (uint)x < (uint)_width && 
                (uint)y < (uint)_height &&
                (uint)index < (uint)_array.Length))
            {
                _array[index] = _colorValue;
            }
            return 0;
        }
    }
}
