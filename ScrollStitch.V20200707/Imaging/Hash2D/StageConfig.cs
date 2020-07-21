using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    /// <summary>
    /// Parameters intended for one stage of processing in <see cref="Hash2DProcessor"/>.
    /// </summary>
    public class StageConfig
    {
        public static class Defaults
        {
            public const int SkipStep = 1;
            public const int FillValue = 0;
        }

        /// <summary>
        /// Direction of one-dimensional hash processing.
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Number of input samples to be taken during the one-dimensional hash processing.
        /// </summary>
        /// <remarks>
        /// The said number of input samples are not necessarily contiguous. This is controlled by the field <see cref="SkipStep"/>.
        /// </remarks>
        public int WindowSize { get; }

        /// <summary>
        /// Controls whether the said number of input samples are contiguous or spaced. Specifying the <see cref="SkipStep"/> value of one (1)
        /// means that they are taken contiguously. Specifying two (2) means that every other pixel from the image will be used.
        /// </summary>
        public int SkipStep { get; }

        /// <summary>
        /// A substitution value to be used when the input pixel coordinates (computed from the current output coordinates, 
        /// <see cref="WindowSize"/>, and <see cref="SkipStep"/>) refer to coordinates outside the input image's boundary.
        /// </summary>
        public int FillValue { get; }

        public StageConfig(Direction direction, int windowSize, int skipStep = Defaults.SkipStep, int fillValue = Defaults.FillValue)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                case Direction.Vertical:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            if (windowSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSize));
            }
            if (skipStep < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(skipStep));
            }
            Direction = direction;
            WindowSize = windowSize;
            SkipStep = skipStep;
            FillValue = fillValue;
        }
    }
}
