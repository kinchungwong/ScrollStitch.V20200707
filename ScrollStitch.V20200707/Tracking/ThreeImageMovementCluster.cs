using ScrollStitch.V20200707.Collections;
using ScrollStitch.V20200707.Collections.Specialized;
using ScrollStitch.V20200707.Data;
using ScrollStitch.V20200707.Imaging.Hash2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    public class ThreeImageMovementCluster
    {
        public ImageManager ImageManager { get; }

        public HashPointTable HashPointTable { get; }

        /// <summary>
        /// The three image keys to be analyzed. 
        /// 
        /// <para>
        /// These image keys correspond to the keys assigned by <see cref="ThreeImageMovementCluster.ImageManager"/>.
        /// </para>
        /// </summary>
        public UniqueList<int> ImageKeys { get; }

        /// <summary>
        /// The column IDs of each of the three images.
        /// 
        /// <para>
        /// The column IDs correspond to the column index used with the row data 
        /// that is stored in <see cref="ThreeImageMovementCluster.HashPointTable"/>.
        /// </para>
        /// </summary>
        public UniqueList<int> ImageColumnIds { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageManager"></param>
        /// <param name="hashPointTable"></param>
        /// 
        /// <param name="imageKeys">
        /// The three image keys to be analyzed. <br/>
        /// The array length must be three. <br/>
        /// The array must not contain duplicate image keys. <br/>
        /// These three image keys must be present in both <paramref name="imageManager"/> 
        /// and <paramref name="hashPointTable"/>.
        /// </param>
        /// 
        public ThreeImageMovementCluster(
            ImageManager imageManager, 
            HashPointTable hashPointTable,
            int[] imageKeys)
        {
            void throwNotThree()
            {
                throw new InvalidOperationException(
                    $"{nameof(ThreeImageMovementCluster)} " +
                    "requires a minimum of three images and their lists of hash points.");
            }
            if ((imageKeys?.Length ?? 0) != 3)
            {
                throwNotThree();
            }
            ImageManager = imageManager;
            HashPointTable = hashPointTable;
            ImageKeys = new UniqueList<int>();
            ImageColumnIds = new UniqueList<int>();
            for (int k = 0; k < 3; ++k)
            {
                int imageKey = imageKeys[k];
                int columnId = HashPointTable.ImageKeys.IndexOf(imageKey);
                if (columnId < 0)
                {
                    // image key does not exist on HashPointTable
                    throwNotThree();
                }
                ImageKeys.Add(imageKey);
                ImageColumnIds.Add(columnId);
            }
            if (ImageKeys.Count != 3 ||
                ImageColumnIds.Count != 3)
            {
                // two  or more image keys are duplicates
                throwNotThree();
            }
        }

        public void Process()
        {
            int[] hashValues = HashPointTable.GetHashValues();
            int[] columns = ImageColumnIds.ToArray();
            var pointIndexArray = HashPointTable.GetPointIndexArray(hashValues, columns);
            int hashPointCount = hashValues.Length;
            if (pointIndexArray.GetLength(0) != hashPointCount)
            {
                throw new Exception();
            }
            if (pointIndexArray.GetLength(1) != 3)
            {
                throw new Exception();
            }
            var hashPointLists = new List<ImageHashPointList>();
            for (int k = 0; k < 3; ++k)
            {
                hashPointLists.Add(ImageManager.InputHashPoints.Get(ImageKeys[k]));
            }
            var m012Hist = HistogramUtility.CreateIntHistogram<(int, int, int, int)>();
            for (int k = 0; k < hashPointCount; ++k)
            {
                int idx0 = pointIndexArray[k, 0];
                int idx1 = pointIndexArray[k, 1];
                int idx2 = pointIndexArray[k, 2];
                if (idx0 < 0 || idx1 < 0 || idx2 < 0)
                {
                    continue;
                }
                Point p0 = hashPointLists[0][idx0].Point;
                Point p1 = hashPointLists[1][idx1].Point;
                Point p2 = hashPointLists[2][idx2].Point;
                Movement m01 = p1 - p0;
                Movement m12 = p2 - p1;
                var tup = ValueTuple.Create(m01.DeltaX, m01.DeltaY, m12.DeltaX, m12.DeltaY);
                m012Hist.Add(tup);
            }
            foreach (var kvp in m012Hist)
            {
                var k = kvp.Key;
                int v = kvp.Value;
                (int m01x, int m01y, int m12x, int m12y) = k;
                Console.WriteLine($"m01=({m01x}, {m01y}), m12=({m12x}, {m12y}) : {v}");
            }
            Console.ReadLine();
        }
    }
}
