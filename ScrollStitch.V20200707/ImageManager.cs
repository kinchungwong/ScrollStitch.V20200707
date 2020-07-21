using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707
{
    using Caching;
    using Collections;
    using Collections.Specialized;
    using Data;
    using Imaging;
    using Imaging.Hash2D;
    using TestSets;
    using Tracking;
    using Tracking.Bidirectional;
    using Utility;

    public class ImageManager
    {
        /// <summary>
        /// The list of test images.
        /// </summary>
        public ITestSet TestSet { get; set; }

        /// <summary>
        /// The compressed file loaded into memory as a byte array.
        /// </summary>
        public ItemFactory<byte[]> InputFileBlobs { get; set; } = new ItemFactory<byte[]>();

        /// <summary>
        /// The sizes of input images.
        /// </summary>
        public Dictionary<int, Size> ImageSizes { get; set; } = new Dictionary<int, Size>();

        /// <summary>
        /// The color input images.
        /// </summary>
        public ItemFactory<IntBitmap> InputColorBitmaps { get; set; } = new ItemFactory<IntBitmap>();

        /// <summary>
        /// The Hash2D computed from the color images.
        /// </summary>
        public ItemFactory<IntBitmap> InputHashBitmaps { get; set; } = new ItemFactory<IntBitmap>();

        /// <summary>
        /// The collection of hash points 
        /// </summary>
        public ItemFactory<ImageHashPointList> InputHashPoints { get; set; } = new ItemFactory<ImageHashPointList>();

        /// <summary>
        /// The collection of 
        /// </summary>
        public Dictionary<FromTo, ImageMovementGrid> ImageMovementGrids { get; set; } = new Dictionary<FromTo, ImageMovementGrid>();
    }
}
