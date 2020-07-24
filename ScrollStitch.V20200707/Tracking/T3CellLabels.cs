using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// <see cref="T3CellLabels"/> computes a cross reference between grid cells and trajectory labels.
    /// 
    /// <para>
    /// Given: <br/>
    /// ... A three-image trajectory (as in <see cref="T3Movements"/>), and
    /// <br/>
    /// ... With one of the three images selected as the "main image" (via <see cref="MainImageKey"/>), and
    /// <br/>
    /// ... A grid for that main image,
    /// <br/>
    /// This class computes:
    /// <br/>
    /// ... The list of trajectory labels for each cell rectangle, and
    /// <br/>
    /// ... The list of cells (by <see cref="CellIndex"/>) for each trajectory label.
    /// </para>
    /// </summary>
    public class T3CellLabels
    {
        public T3Movements Movements { get; }

        public int MainImageKey { get; }

        public Grid MainGrid { get; }

        public T3HashPoints HashPoints => Movements.HashPoints;

        public IReadOnlyDictionary<int, IReadOnlyList<Point>> Points => HashPoints.Points;

        public IReadOnlyList<Point> MainImagePoints => Points[MainImageKey];

        public IReadOnlyList<int> Labels => Movements.Labels;

        public IReadOnlyDictionary<CellIndex, IReadOnlyList<int>> CellLabelList { get; private set; }

        public IReadOnlyDictionary<int, IReadOnlyList<CellIndex>> LabelCellList { get; private set; }

        public static class Factory
        {
            public static T3CellLabels Create(
                T3Movements movements,
                int mainImageKey,
                Grid mainGrid)
            {
                var cellLabels = new T3CellLabels(movements, mainImageKey, mainGrid);
                cellLabels._Process();
                return cellLabels;
            }

            public static T3CellLabels Create(
                T3Movements movements,
                int mainImageKey,
                Grid mainGrid,
                IReadOnlyDictionary<CellIndex, IReadOnlyList<int>> cellLabelList,
                IReadOnlyDictionary<int, IReadOnlyList<CellIndex>> labelCellList)
            {
                return new T3CellLabels(movements, mainImageKey, mainGrid, cellLabelList, labelCellList);
            }
        }

        private T3CellLabels(
            T3Movements movements, 
            int mainImageKey, 
            Grid mainGrid,
            IReadOnlyDictionary<CellIndex, IReadOnlyList<int>> cellLabelList = null,
            IReadOnlyDictionary<int, IReadOnlyList<CellIndex>> labelCellList = null)
        {
            _CtorValidateMainImageKey(movements, mainImageKey);
            Movements = movements;
            MainImageKey = mainImageKey;
            MainGrid = mainGrid;
            CellLabelList = cellLabelList;
            LabelCellList = labelCellList;
        }

        private void _Process()
        {
            if (!(CellLabelList is null) &&
                !(LabelCellList is null))
            {
                return;
            }
            // ====== REMARK ======
            // Deep properties are converted into local variables 
            // for performance and bounds-checking reasons.
            // ======
            var mainPoints = MainImagePoints;
            var labels = Labels;
            int pointCount = mainPoints.Count;
            if (labels.Count != pointCount)
            {
                _Throw();
            }
            Grid mainGrid = MainGrid;
            int maxGridCellCount = _AreaFromSize(mainGrid.GridSize);
            int maxMovementCount = Movements.Movements.Count;
            var hasSeenCellLabel = new HashSet<(CellIndex, int)>();
            var cellLabelList = new Dictionary<CellIndex, List<int>>(capacity: maxGridCellCount);
            var labelCellList = new Dictionary<int, List<CellIndex>>(capacity: maxMovementCount);
            void AddCellLabelList((CellIndex ci, int label) cellIndexAndLabel)
            {
                (CellIndex ci, int label) = cellIndexAndLabel;
                if (!cellLabelList.TryGetValue(ci, out List<int> ls))
                {
                    ls = new List<int>();
                    cellLabelList.Add(ci, ls);
                }
                ls.Add(label);
            }
            void AddLabelCellList((CellIndex ci, int label) cellIndexAndLabel)
            {
                (CellIndex ci, int label) = cellIndexAndLabel;
                if (!labelCellList.TryGetValue(label, out List<CellIndex> cis))
                {
                    cis = new List<CellIndex>();
                    labelCellList.Add(label, cis);
                }
                cis.Add(ci);
            }
            for (int pointIndex = 0; pointIndex < pointCount; ++pointIndex)
            {
                Point pointOnMainImage = mainPoints[pointIndex];
                int label = labels[pointIndex];
                CellIndex cellIndex = mainGrid.FindCell(pointOnMainImage);
                var cellIndexAndLabel = (cellIndex, label);
                if (hasSeenCellLabel.Contains(cellIndexAndLabel))
                {
                    continue;
                }
                hasSeenCellLabel.Add(cellIndexAndLabel);
                AddCellLabelList(cellIndexAndLabel);
                AddLabelCellList(cellIndexAndLabel);
            }
            foreach (var kvp in cellLabelList)
            {
                kvp.Value.Sort();
            }
            foreach (var kvp in labelCellList)
            {
                kvp.Value.Sort();
            }
            CellLabelList = cellLabelList.AsReadOnly();
            LabelCellList = labelCellList.AsReadOnly();
        }

        private static void _CtorValidateMainImageKey(T3Movements movements, int mainImageKey)
        {
            if (movements.ImageKeys.IndexOf(mainImageKey) < 0)
            {
                int imageKey0 = movements.ImageKeys.ItemAt(0);
                int imageKey1 = movements.ImageKeys.ItemAt(1);
                int imageKey2 = movements.ImageKeys.ItemAt(2);
                throw new IndexOutOfRangeException(
                    message: $"Argument ({nameof(mainImageKey)}={mainImageKey}) not found. " +
                    $"ImageKeys are: ({imageKey0}, {imageKey1}, {imageKey2}).");
            }
        }

        private void _Throw()
        {
            throw new Exception();
        }

        private static int _AreaFromSize(Size sz)
        {
            return checked(sz.Width * sz.Height);
        }
    }
}
