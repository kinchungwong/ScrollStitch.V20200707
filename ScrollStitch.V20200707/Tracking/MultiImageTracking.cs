using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Caching;
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging;
    using ScrollStitch.V20200707.Imaging.Hash2D;
    using ScrollStitch.V20200707.Logging;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Utility;

    public class MultiImageTracking
    {
        /// <summary>
        /// Connects this class to a retrieve-only collection of input image sizes. 
        /// </summary>
        public IItemSource<Size> ImageSizes { get; set; }

        /// <summary>
        /// Connects this class to a retrieve-only collection of Hash2D Bitmaps with an integer key
        /// (the image ID).
        /// </summary>
        public IItemSource<IntBitmap> HashBitmapSource { get; set; }

        /// <summary>
        /// Connects this class to a retrieval-only collection of <see cref="ImageHashPointList"/>
        /// with an integer key (the image ID).
        /// </summary>
        public IItemSource<ImageHashPointList> HashPointSource { get; set; }

        /// <summary>
        /// The approximate size of grid cell.
        /// </summary>
        public Size ApproxCellSize { get; set; } = new Size(32, 32);

        /// <summary>
        /// The list of image keys to be processed by this class.
        /// </summary>
        public UniqueList<int> ImageKeys { get; private set; }

        /// <summary>
        /// The <see cref="Grid"/> created for each image.
        /// 
        /// If all images have same dimensions, the grids would have been compatible. 
        /// However, this class is designed to operate without the assumption that all images 
        /// have same dimensions. This design is needed to allow processing of cropped images,
        /// where each cropped image may have a different size.
        /// </summary>
        public Dictionary<int, Grid> Grids { get; private set; }

        /// <summary>
        /// A table containing unique Hash2D values found in each image and the index
        /// into the respective image's point list.
        /// </summary>
        public HashPointTable HashPointTable { get; private set; }

        public MultiImageTracking()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Process(IReadOnlyList<int> imageKeys)
        {
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(Process)}"))
            {
                ImageKeys = new UniqueList<int>(imageKeys);
                _CreateGrids();
                _ComputeHashPointTable();
                //_Cleanup();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _CreateGrids()
        {
            Grids = new Dictionary<int, Grid>();
            foreach (int imageKey in ImageKeys)
            {
                Size imageSize = ImageSizes[imageKey];
                int iw = imageSize.Width;
                int ih = imageSize.Height;
                int cw = ApproxCellSize.Width;
                int ch = ApproxCellSize.Height;
                var grid = Grid.Factory.CreateApproxCellSize(iw, ih, cw, ch);
                Grids.Add(imageKey, grid);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _ComputeHashPointTable()
        {
            HashPointTable = new HashPointTable(HashPointSource, ImageKeys);
            HashPointTable.Process();
        }

        private void _Cleanup()
        {
            if (true)
            {
                HashPointTable.Clear();
                HashPointTable = null;
            }
        }
    }
}
