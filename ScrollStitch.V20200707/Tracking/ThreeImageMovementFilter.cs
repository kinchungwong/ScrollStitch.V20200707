using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// Refer to research notes (2020-07-20). 
    /// 
    /// 
    /// </summary>
    public class ThreeImageMovementFilter
    {
        public UniqueList<int> ImageKeys { get; }

        public ImageManager ImageManager { get; }

        #region private
        private Dictionary<int, Dictionary<int, Point>> _hashPointDicts;
        private Dictionary<int, (Movement, Movement)> _movementPairs;
        private IHistogram<(Movement, Movement), int> _movementPairHist;
        private UniqueList<(Movement, Movement)> _movementPairListFilteredByPercent;
        private IHistogram<(Movement, Movement), int> _movementPairHistByGridCount;
        private UniqueList<(Movement, Movement)> _movementPairListFilteredByGridCount;
        private Grid _grid1;
        #endregion

        public ThreeImageMovementFilter(IEnumerable<int> imageKeys, ImageManager imageManager)
        {
            ImageKeys = new UniqueList<int>(imageKeys);
            if (ImageKeys.Count != 3)
            {
                throw new InvalidOperationException("This class requires exactly three input images.");
            }
            ImageManager = imageManager;
        }

        public void Process()
        {
            _PopulateHashPointDicts();
            _PopulateMovementPairsAndHist();
            _FilterByHistPercent();
            _FilterByGridCount();
            _PrintToConsole();
        }

        private void _PopulateHashPointDicts()
        {
            _hashPointDicts = new Dictionary<int, Dictionary<int, Point>>();
            for (int k = 0; k < 3; ++k)
            {
                int imageKey = ImageKeys.ItemAt(k);
                var hashPoints = ImageManager.InputHashPoints.Get(imageKey).HashPoints;
                var dict = new Dictionary<int, Point>();
                foreach (var hp in hashPoints)
                {
                    dict.Add(hp.HashValue, hp.Point);
                }
                _hashPointDicts.Add(imageKey, dict);
            }
        }

        private void _PopulateMovementPairsAndHist()
        {
            var dict0 = _hashPointDicts[ImageKeys.ItemAt(0)];
            var dict1 = _hashPointDicts[ImageKeys.ItemAt(1)];
            var dict2 = _hashPointDicts[ImageKeys.ItemAt(2)];
            _movementPairs = new Dictionary<int, (Movement, Movement)>();
            _movementPairHist = HistogramUtility.CreateIntHistogram<(Movement, Movement)>();
            foreach (var kvp0 in dict0)
            {
                int hashValue = kvp0.Key;
                Point p0 = kvp0.Value;
                if (!dict1.TryGetValue(hashValue, out Point p1) ||
                    !dict2.TryGetValue(hashValue, out Point p2))
                {
                    continue;
                }
                Movement m01 = p1 - p0;
                Movement m12 = p2 - p1;
                var m012 = (m01, m12);
                _movementPairs.Add(hashValue, m012);
                _movementPairHist.Add(m012);
            }
        }

        private void _FilterByHistPercent()
        {
            int total = _movementPairs.Count;
            int threshold = total / 100;
            _movementPairListFilteredByPercent = new UniqueList<(Movement, Movement)>();
            void FilterAndAddFunc((Movement, Movement) m012, int votes)
            {
                if (votes < threshold) return;
                _movementPairListFilteredByPercent.Add(m012);
            }
            _movementPairHist.ForEach(FilterAndAddFunc);
        }

        /// <summary>
        /// We further filter trajectories as follows. 
        /// <br/>
        /// For each trajectory <c>(Movement m01, Movement m12)</c>, we list the hash point occurrences in support of 
        /// that trajectory.
        /// <br/>
        /// For each hash point occurrence on the list, we pick the point on the second image (<c>ImageKeys.ItemAt(1)</c>), 
        /// and sort that point into a grid. 
        /// <br/>
        /// If there are multiple hash point occurrences, in support of the same trajectory, and sort into the same 
        /// grid cell, these occurrences contribute a single vote.
        /// <br/>
        /// In other words, for each trajectory, we count the number of grid cells that contain evidence (hash point 
        /// occurrences) in support of that trajectory.
        /// <br/>
        /// Finally, we remove trajectories that are supported by fewer than a threshold number of grid cells.
        /// </summary>
        private void _FilterByGridCount()
        {
            int imageKey1 = ImageKeys.ItemAt(1);
            var imageSize1 = ImageManager.ImageSizes[imageKey1];
            _grid1 = Grid.Factory.CreateApproxCellSize(imageSize1, new Size(32, 32));
            var gridFlags = new GridArray<uint>(_grid1, 0u);
            var dict1 = _hashPointDicts[ImageKeys.ItemAt(1)];
            //var gridCounts = HistogramUtility.CreateIntHistogram<(Movement, Movement)>
            _movementPairHistByGridCount = HistogramUtility.CreateIntHistogram<(Movement, Movement)>();
            foreach (var hashAndMP in _movementPairs)
            {
                int hashValue = hashAndMP.Key;
                (Movement, Movement) m012 = hashAndMP.Value;
                int m012pos = _movementPairListFilteredByPercent.IndexOf(m012);
                if (m012pos < 0) continue;
                uint flagMask = (1u << m012pos);
                Point p1 = dict1[hashValue];
                CellIndex ci = _grid1.FindCell(p1);
                uint oldFlag = gridFlags[ci];
                if ((oldFlag & flagMask) == 0)
                {
                    gridFlags[ci] = oldFlag | flagMask;
                    _movementPairHistByGridCount.Add(m012);
                }
            }
            int gw = _grid1.GridWidth;
            int gh = _grid1.GridHeight;
            int gridCountThreshold = gw * gh / 100;
            _movementPairListFilteredByGridCount = new UniqueList<(Movement, Movement)>();
            void FilterAndAddFunc((Movement, Movement) m012, int gridCellVotes)
            {
                if (gridCellVotes < gridCountThreshold) return;
                _movementPairListFilteredByGridCount.Add(m012);
            }
            _movementPairHistByGridCount.ForEach(FilterAndAddFunc);
        }

        private void _PrintToConsole()
        {
            int[] imageKeys = ImageKeys.ToArray();
            Console.WriteLine(new string('-', 76));
            Console.WriteLine($"Movement statistics for images ({imageKeys[0]}, {imageKeys[1]}, {imageKeys[2]})");
            if (true)
            {
                int totalVotes = _movementPairs.Count;
                void PrintFunc_ByVote((Movement, Movement) m012, int votes)
                {
                    int m012pos = _movementPairListFilteredByPercent.IndexOf(m012);
                    bool rejected = (m012pos < 0);
                    string strReject = rejected ? "(Rejected)" : string.Empty;
                    Movement m01 = m012.Item1;
                    Movement m12 = m012.Item2;
                    double percent = 100.0 * votes / totalVotes;
                    Console.WriteLine($"(m01: {m01}, m12: {m12}, votes: {votes}, percent: {percent:F4}) {strReject}");
                }
                Console.WriteLine("Vote type is: (subsampled) hash point count");
                _movementPairHist.ForEach(PrintFunc_ByVote);
                Console.WriteLine();
            }
            if (true)
            {
                int totalCells = _grid1.GridWidth * _grid1.GridHeight;
                void PrintFunc_ByGridCount((Movement, Movement) m012, int gridCellVotes)
                {
                    int m012pos = _movementPairListFilteredByGridCount.IndexOf(m012);
                    bool rejected = (m012pos < 0);
                    string strReject = rejected ? "(Rejected)" : string.Empty;
                    Movement m01 = m012.Item1;
                    Movement m12 = m012.Item2;
                    double percent = 100.0 * gridCellVotes / totalCells;
                    Console.WriteLine($"(m01: {m01}, m12: {m12}, votes: {gridCellVotes}, percent: {percent:F4}) {strReject}");
                }
                Console.WriteLine("Vote type is: grid cell count");
                _movementPairHistByGridCount.ForEach(PrintFunc_ByGridCount);
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 76));
            Console.WriteLine("Press enter key to continue...");
            //Console.ReadLine();
            Console.WriteLine(new string('-', 76));
        }
    }
}
