using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Caching;
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.Hash2D;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// 
    /// Collects a subset of hash points from the entire collection of images.
    /// 
    /// <para>
    /// For each processed image, the unique hash points are partitioned by a grid, and then sorted
    /// by the absolute value of their hash values. From each grid cell, only four of the unique hash 
    /// points having the smallest absolute hash value will be kept.
    /// </para>
    /// 
    /// </summary>
    ///
    public class LongRangeHashPoints
    {
        public static class Defaults
        {
            public static Size ApproxCellSize { get; } = new Size(128, 32);
        }

        /// <summary>
        /// Connects this class to a retrieve-only collection of input image sizes. 
        /// </summary>
        public IItemSource<Size> ImageSizes { get; set; }

        /// <summary>
        /// Connects this class to a retrieval-only collection of <see cref="Imaging.Hash2D.ImageHashPointList"/>
        /// with an integer key (the image ID).
        /// </summary>
        public IItemSource<ImageHashPointList> ImageHashPointSource { get; set; }

        public List<ImageHashPoint> LongRangeHashPointList { get; }

        public HashSet<int> HashValuePresence { get; }

        /// <summary>
        /// The total number of times a hash value has been observed again.
        /// </summary>
        public int HashValueRepeatCount { get; private set; }

        /// <summary>
        /// (Implementation detail.) Controls the granularity of the <see cref="Grid"/> that is internally 
        /// used inside the <see cref="ProcessImage(int)"/> method.
        /// 
        /// <para>
        /// This property is automatically initialized from <see cref="Defaults.ApproxCellSize"/>.
        /// </para>
        /// 
        /// </summary>
        public Size ApproxCellSize { get; set; }

        #region private
        private HashSet<int> _hasProcessedImage;
        #endregion

        /// <summary>
        /// Constructor.
        /// 
        /// <para>
        /// After construction, and before the first use, this instance still needs to be connected to a source 
        /// of image sizes and image hash points, by setting the following properties: <br/>
        /// ... <see cref="ImageSizes"/> <br/>
        /// ... <see cref="ImageHashPointSource"/> <br/>
        /// These properties are usually centrally provided by <see cref="ImageManager"/>.
        /// </para>
        /// </summary>
        public LongRangeHashPoints()
        {
            ApproxCellSize = Defaults.ApproxCellSize;
            LongRangeHashPointList = new List<ImageHashPoint>();
            HashValuePresence = new HashSet<int>();
            HashValueRepeatCount = 0;
            _hasProcessedImage = new HashSet<int>();
        }

        /// <summary>
        /// Adds a small subset of hash points from the specified image.
        /// 
        /// <para>
        /// Note. The grid used inside this method is unrelated to the grid used in the three-image 
        /// trajectory (T3) algorithms. The grid size used by this method on different images do 
        /// not need to be the same.
        /// </para>
        /// 
        /// </summary>
        /// <param name="imageKey"></param>
        /// 
        public void ProcessImage(int imageKey)
        {
            if (ImageSizes is null ||
                ImageHashPointSource is null)
            {
                throw new InvalidOperationException(
                    "The instance has not been connected to a source of image size and image hash points.");
            }
            if (_hasProcessedImage.Contains(imageKey))
            {
                return;
            }
            else
            {
                _hasProcessedImage.Add(imageKey);
            }
            var imageSize = ImageSizes[imageKey];
            var grid = Grid.Factory.CreateApproxCellSize(imageSize, new Size(128, 32));
            var hashPointList = ImageHashPointSource[imageKey];
            Dictionary<CellIndex, List<HashPoint>> cellHashPoints = new Dictionary<CellIndex, List<HashPoint>>();
            void addFunc(CellIndex ci, HashPoint hp)
            {
                if (!cellHashPoints.TryGetValue(ci, out var list))
                {
                    list = new List<HashPoint>();
                    cellHashPoints.Add(ci, list);
                }
                list.Add(hp);
            }
            foreach (var hashPoint in hashPointList.HashPoints)
            {
                CellIndex ci = grid.FindCell(hashPoint.Point);
                addFunc(ci, hashPoint);
            }
            int CompareHashPointAbsValue(HashPoint hp1, HashPoint hp2)
            {
                int v1 = Math.Abs(hp1.HashValue);
                int v2 = Math.Abs(hp2.HashValue);
                return (v1 > v2) ? 1 : (v1 < v2) ? -1 : 0;
            }
            foreach (var kvp in cellHashPoints)
            {
                var list = kvp.Value;
                list.Sort(CompareHashPointAbsValue);
                foreach (var hp in list.Take(4))
                {

                    LongRangeHashPointList.Add(new ImageHashPoint(imageKey, hp.HashValue, hp.Point));
                    if (!HashValuePresence.Add(hp.HashValue))
                    {
                        HashValueRepeatCount += 1;
                    }
                }
            }
        }
    }
}
