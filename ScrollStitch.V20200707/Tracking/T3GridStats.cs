using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// Given a three-image trajectory computed by <see cref="T3Movements"/>, gather one or more 
    /// grid-based statistics by picking one of the three images and a specific grid for 
    /// that image as reference.
    /// </summary>
    public class T3GridStats
    {
        public T3Movements T3Movements { get; }

        public int ImageKey { get; }

        public Grid Grid { get; }

        public IReadOnlyList<Point> Points => T3Movements.Points[ImageKey];

        public IReadOnlyList<int> HashValues => T3Movements.HashValues;

        public IReadOnlyList<int> Labels => T3Movements.Labels;

        public UniqueList<(Movement, Movement)> Movements => T3Movements.Movements;

        public IReadOnlyDictionary<int, int> LabelPointCounts => T3Movements.LabelPointCounts;

        public UniqueList<T3GridStats_Base> StatCollectors { get; }

        public List<object> Results { get; private set; }

        /// <summary>
        /// Creates the statistics gatherer.
        /// 
        /// <para>
        /// The caller must specify the image key and the grid.
        /// </para>
        /// </summary>
        /// <param name="t3movements">A three-image trajectory with the underlying data.</param>
        /// <param name="imageKey">One of the three images used as the reference.</param>
        /// <param name="grid">A grid having the correct size for the chosen image.</param>
        /// 
        public T3GridStats(T3Movements t3movements, int imageKey, Grid grid)
        {
            if (t3movements.ImageKeys.IndexOf(imageKey) < 0)
            {
                throw new IndexOutOfRangeException(nameof(imageKey));
            }
            T3Movements = t3movements;
            ImageKey = imageKey;
            Grid = grid;
            StatCollectors = new UniqueList<T3GridStats_Base>();
        }

        /// <summary>
        /// Attach a specific type of statistics collector to the class.
        /// 
        /// <para>
        /// Some types of statistics collectors require additional configuration from the caller,
        /// before they can be attached to this class.
        /// </para>
        /// </summary>
        /// <param name="statCollector"></param>
        /// 
        public void Add(T3GridStats_Base statCollector)
        {
            StatCollectors.Add(statCollector);
        }

        /// <summary>
        /// Iterates through all underlying data from the three-image trajectory, namely the hash points 
        /// and trajectory labels.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Process()
        {
            // ====== REMARK ======
            // For performance reasons, 
            // This method makes all member fields local.
            // ======
            var grid = Grid;
            var points = Points;
            var hashValues = HashValues;
            var labels = Labels;
            int count = points.Count;
            if (hashValues.Count != count ||
                labels.Count != count)
            {
                throw new Exception();
            }
            var collectors = StatCollectors;
            if ((collectors?.Count ?? 0) == 0)
            {
                return;
            }
            for (int index = 0; index < count; ++index)
            {
                Point point = points[index];
                int hashValue = hashValues[index];
                int label = labels[index];
                CellIndex cellIndex = grid.FindCell(point);
                foreach (var collector in collectors)
                {
                    collector.Add(point, hashValue, label, cellIndex);
                }
            }
            _PostProcessCopyResults();
        }

        private void _PostProcessCopyResults()
        {
            int collectorCount = (StatCollectors?.Count ?? 0);
            Results = new List<object>(capacity: collectorCount);
            for (int k = 0; k < collectorCount; ++k)
            {
                var collector = StatCollectors.ItemAt(k);
                Results.Add(collector.GetResultAsObject());
            }
        }
    }
}
