using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using Caching;
    using Collections.Specialized;
    using Data;
    using Imaging;
    using Imaging.Hash2D;
    using ScrollStitch.V20200707.Collections;
    using Spatial;

    public class ImagePairGridMovement
    {
        #region nested static class "ImagePairGridMovement.Defaults"
        public static class Defaults
        {
            /// <summary>
            /// 
            /// TODO IMPORTANT
            /// the recommendation is 32x32.
            /// However, this also affects diagnostic visualization, so it is temporarily changed to 64x64.
            /// Remember to change it back.
            /// 
            /// </summary>
            [Obsolete]
            public static Size DefaultCellSize { get; } = new Size(32, 32);

            public static Grid DefaultGridCreationFunc(Size currBitmapSize)
            {
                int imageW = currBitmapSize.Width;
                int imageH = currBitmapSize.Height;
                int cellW = DefaultCellSize.Width;
                int cellH = DefaultCellSize.Height;
                return Grid.Factory.CreateApproxCellSize(imageW, imageH, cellW, cellH);
            }
        }
        #endregion

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
        /// A function that creates a grid given a bitmap size. This property is initialized with
        /// a default; the default function implementation should be sufficient for most use cases.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The grid needs to be created on-demand because this class does not make the assumption
        /// that all bitmaps must have the same size. In particular, this class is designed to
        /// perform on uncropped bitmaps (of the same size) and cropped bitmaps (where each bitmap
        /// may have a variable-size region-of-interest).
        /// </para>
        /// </remarks>
        /// 
        public Func<Size, Grid> GridCreationFunc { get; set; } = Defaults.DefaultGridCreationFunc;

        public ImagePairGridMovement(IItemSource<IntBitmap> hashBitmapSource, IItemSource<ImageHashPointList> hashPointSource)
        {
            HashBitmapSource = hashBitmapSource;
            HashPointSource = hashPointSource;
        }

        public class Result
        {
            public int CurrImageKey;
            public int PrevImageKey;
            public Grid Grid;
            public Dictionary<CellIndex, HashSet<Movement>> GridCellMovements;
        }

#if false
        internal struct I2
        {
            internal int i0;
            internal int i1;
        }

        internal class IxI2
        {
            internal Dictionary<int, I2> _dict { get; set; } = new Dictionary<int, I2>();
            internal I2 _defaultNewValue { get; set; } = new I2();

            internal I2 Get(int key)
            {
                bool exists = _dict.TryGetValue(key, out I2 i2);
                if (!exists)
                {
                    i2 = _defaultNewValue;
                }
                return i2;
            }

            internal I2 Write(int key, int which, int newValue)
            {
                bool exists = _dict.TryGetValue(key, out I2 i2);
                if (!exists)
                {
                    i2 = _defaultNewValue;
                }
                switch (which)
                {
                    case 0:
                        i2.i0 = newValue;
                        break;
                    case 1:
                        i2.i1 = newValue;
                        break;
                    default:
                        break;
                }
                if (exists)
                {
                    _dict[key] = i2;
                }
                else 
                {
                    _dict.Add(key, i2);
                }
                return i2;
            }
        }
#endif

        public struct AutoNew<T>
            where T : class, new()
        {
            private T _value;

            public bool HasValue => !(_value is null);
            public T Value
            {
                get
                {
                    if (_value is null)
                    {
                        _value = new T();
                    }
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }

            public static implicit operator T(AutoNew<T> self)
            {
                return self.Value;
            }
        }

        public class Processor
        {
            public ImagePairGridMovement Host;
            public int CurrImageKey;
            public int PrevImageKey;
            public IntBitmap CurrHash2D;
            public IntBitmap PrevHash2D;
            public ImageHashPointList CurrPoints;
            public ImageHashPointList PrevPoints;
            public Grid Grid;
            public IntKeyIntRow HashConcord; // concordance
            public GridArray<AutoNew<HashSet<Movement>>> GridMovementsRaw;
            public Dictionary<CellIndex, HashSet<Movement>> GridMovements;
            public Result Result;

            public Processor(ImagePairGridMovement host, int currImageKey, int prevImageKey)
            {
                Host = host;
                CurrImageKey = currImageKey;
                PrevImageKey = prevImageKey;
            }

            public void Process()
            {
                _Fetch();
                _ComputeHashConcord();
                _ComputeGridMovements();
            }

            private void _Fetch()
            {
                CurrHash2D = Host.HashBitmapSource[CurrImageKey];
                PrevHash2D = Host.HashBitmapSource[PrevImageKey];
                Grid = Host.GridCreationFunc(CurrHash2D.Size);
                CurrPoints = Host.HashPointSource[CurrImageKey];
                PrevPoints = Host.HashPointSource[PrevImageKey];
            }

            private void _ComputeHashConcord()
            {
                HashConcord = new IntKeyIntRow(2);
                _PopulateHashConcord_Curr();
                _PopulateHashConcord_Prev();
            }

            private void _PopulateHashConcord_Curr()
            {
                int currCount = CurrPoints.Count;
                for (int currIndex = 0; currIndex < currCount; ++currIndex)
                {
                    var currHP = CurrPoints[currIndex];
                    int key = currHP.HashValue;
                    HashConcord.Visit(key,
                        (_, exists, data) =>
                        {
                            data[0] = currIndex;
                            if (!exists) data[1] = -1;
                            return IntKeyIntRow.AfterVisit.Update;
                        });
                }
            }

            private void _PopulateHashConcord_Prev()
            {
                int prevCount = PrevPoints.Count;
                for (int prevIndex = 0; prevIndex < prevCount; ++prevIndex)
                {
                    var prevHP = PrevPoints[prevIndex];
                    int key = prevHP.HashValue;
                    HashConcord.Visit(key,
                        (_, exists, data) =>
                        {
                            if (!exists) data[0] = -1;
                            data[1] = prevIndex;
                            return IntKeyIntRow.AfterVisit.Update;
                        });
                }
            }

            private void _ComputeGridMovements()
            {
                GridMovementsRaw = new GridArray<AutoNew<HashSet<Movement>>>(Grid);
                HashConcord.VisitAll(
                    (key, exists, data) =>
                    {
                        int currIndex = data[0];
                        int prevIndex = data[1];
                        if (currIndex >= 0 && prevIndex >= 0)
                        {
                            var currP = CurrPoints[currIndex].Point;
                            var prevP = PrevPoints[prevIndex].Point;
                            Movement movement = currP - prevP;
                            CellIndex currCI = Grid.FindCell(currP);
                            GridMovementsRaw[currCI].Value.Add(movement);
                        }
                        return IntKeyIntRow.AfterVisit.NoChange;
                    });
                var zzz = new HashSet<Movement>[Grid.GridHeight, Grid.GridWidth];
                Arrays.ArrayCopyUtility.Convert(GridMovementsRaw.Array, zzz, (_) => _.HasValue ? _.Value : null);
                //var zzz = (HashSet<Movement>[,])GridCollectionUtility.ToArray<HashSet<Movement>, AutoNew<HashSet<Movement>>>(GridMovementsRaw, GridCollectionUtility.DestType.Array2_RowColumn);
                //GridMovements = new Dictionary<CellIndex, HashSet<Movement>>();
            }

            private void _AddToGridMovements(CellIndex ci, Movement m)
            {
                if (!GridMovements.TryGetValue(ci, out HashSet<Movement> movements))
                {
                    movements = new HashSet<Movement>();
                    GridMovements.Add(ci, movements);
                }
                movements.Add(m);
            }
        }

        /// <summary>
        /// Computes...
        /// </summary>
        /// <param name="currImageKey"></param>
        /// <param name="prevImageKey"></param>
        public Result Process(int currImageKey, int prevImageKey)
        {
            return Process_1(currImageKey, prevImageKey);
        }

        public Result Process_2(int currImageKey, int prevImageKey)
        {
            Processor proc = new Processor(this, currImageKey, prevImageKey);
            proc.Process();
            return new Result()
            {
                CurrImageKey = currImageKey,
                PrevImageKey = prevImageKey,
                Grid = proc.Grid,
                GridCellMovements = proc.GridMovements
            };
        }

        public Result Process_1(int currImageKey, int prevImageKey)
        {
            IntBitmap currHash2D = HashBitmapSource[currImageKey];
            IntBitmap prevHash2D = HashBitmapSource[prevImageKey];
            Grid grid = GridCreationFunc(currHash2D.Size);
            var currPoints = HashPointSource[currImageKey];
            var prevPoints = HashPointSource[prevImageKey];
            var prevDict = new Dictionary<int, Point>();
            foreach (var prevHP in prevPoints)
            {
                prevDict.Add(prevHP.HashValue, prevHP.Point);
            }
            var gridMovements = new Dictionary<CellIndex, HashSet<Movement>>();
            void AddToGridMovements(CellIndex ci, Movement m)
            {
                if (!gridMovements.TryGetValue(ci, out HashSet<Movement> movements))
                {
                    movements = new HashSet<Movement>();
                    gridMovements.Add(ci, movements);
                }
                movements.Add(m);
            }
            foreach (var currHP in currPoints)
            {
                Point currP = currHP.Point;
                int hashValue = currHP.HashValue;
                if (!prevDict.TryGetValue(hashValue, out Point prevP))
                {
                    continue;
                }
                CellIndex currCI = grid.FindCell(currP);
                Movement movement = currP - prevP;
                AddToGridMovements(currCI, movement);
            }
#if true
            IHistogram<Movement, int> movementPerCellHist = HistogramUtility.CreateIntHistogram<Movement>();
            foreach (var gcm in gridMovements)
            {
                CellIndex ci = gcm.Key;
                HashSet<Movement> hsm = gcm.Value;
                foreach (var m in hsm)
                {
                    movementPerCellHist.Add(m);
                }
            }
            HashSet<Movement> tooFewMovements = new HashSet<Movement>();
            foreach (var kvp in movementPerCellHist)
            {
                var movement = kvp.Key;
                int votes = kvp.Value;
                if (votes <= 10)
                {
                    tooFewMovements.Add(movement);
                }
            }
            var gridMovementsFilt = new Dictionary<CellIndex, HashSet<Movement>>();
            foreach (var gcm in gridMovements)
            {
                CellIndex ci = gcm.Key;
                HashSet<Movement> hsm = gcm.Value;
                HashSet<Movement> hsmFilt = new HashSet<Movement>();
                foreach (var m in hsm)
                {
                    if (tooFewMovements.Contains(m))
                    {
                        continue;
                    }
                    hsmFilt.Add(m);
                }
                if (hsmFilt.Count > 0)
                {
                    gridMovementsFilt.Add(ci, hsmFilt);
                }
            }
#endif
            return new Result()
            { 
                CurrImageKey = currImageKey,
                PrevImageKey = prevImageKey,
                Grid = grid,
#if false
                GridCellMovements = gridMovements
#else
                GridCellMovements = gridMovementsFilt
#endif
            };
        }
    }
}
