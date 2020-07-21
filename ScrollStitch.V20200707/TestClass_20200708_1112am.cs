using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707
{
    //using Bitwise;
    using Caching;
    using Collections;
    using Data;
    using HashCode;
    using Imaging;
    using Imaging.Hash2D;
    using Imaging.IO;
    //using Imaging.Plotting;
    using Logging;
    using Memory;
    using Spatial;
    using TestSets;
    using Text;
    using Tracking;
    using Tracking.Diagnostics;
    using Utility;

    public class TestClass_20200708_1112am
    {
        #region config flags
        public const bool Verbose = true;
        public const bool ShouldReadFromKeyboard = true;
        public const double HashPointSampleRate = 0.05;
        #endregion

        #region diagnstics flags
        public const bool ShouldSave_IPGM = false;
        public const bool ShouldPrintArrayPoolDiagnostics = true;
        public const bool ShouldPrintLogging = true;
        public const bool ShouldPrintThreeImageTrajectoryDiagnostics = true;
        #endregion

        /// <summary>
        /// Manages 
        /// </summary>
        public ImageManager ImageManager 
        { 
            get; 
            set; 
        } = new ImageManager();

        public ITestSet TestSet
        {
            get => ImageManager.TestSet;
            set => ImageManager.TestSet = value;
        }

        public ItemFactory<byte[]> InputFileBlobs 
        {
            get => ImageManager.InputFileBlobs;
            set => ImageManager.InputFileBlobs = value;
        }

        public Dictionary<int, Size> ImageSizes 
        {
            get => ImageManager.ImageSizes;
            set => ImageManager.ImageSizes = value; 
        }

        public ItemFactory<IntBitmap> InputColorBitmaps 
        {
            get => ImageManager.InputColorBitmaps;
            set => ImageManager.InputColorBitmaps = value;
        }

        public ItemFactory<IntBitmap> InputHashBitmaps 
        {
            get => ImageManager.InputHashBitmaps;
            set => ImageManager.InputHashBitmaps = value;
        }

        public ItemFactory<ImageHashPointList> InputHashPoints
        {
            get => ImageManager.InputHashPoints;
            set => ImageManager.InputHashPoints = value;
        }

        /// <summary>
        /// The two-image movement tracking between the current and the previous images.
        ///
        /// Note: the item is null for (imageIndex == 0) because it does not have 
        /// a corresponding previous image.
        /// </summary>
        public ItemFactory<ImagePairMovement> ImagePairMovements
        {
            get;
            set;
        } = new ItemFactory<ImagePairMovement>();

#if true
        public HashSet<int> HasSavedDiagnostics { get; private set; } = new HashSet<int>();
        public HashSet<int> HasSavedIPGM { get; private set; } = new HashSet<int>();
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public TestClass_20200708_1112am(string[] args)
        {
            //TestSet = new Run_1(50);
            //TestSet = new US_Death_Projections();
            //TestSet = new Huge_Run(100);
            //TestSet = new Huge_Run.Subset_49243();
            //TestSet = new TestSet_ConsoleAsk();
            TestSet = new Run_20200403(200);
            // ======
            InputFileBlobs.FactoryFunc = _LoadInputBlob;
            // ======
            InputColorBitmaps.FactoryFunc = _LoadInputColor;
            InputColorBitmaps.DisposeFunc = _DisposeIntBitmap;
            // ======
            InputHashBitmaps.FactoryFunc = _ComputeHashBitmap;
            InputHashBitmaps.DisposeFunc = _DisposeIntBitmap;
            // ======
            InputHashPoints.FactoryFunc = _ComputeHashPoints;
            // ======
            ImagePairMovements.FactoryFunc = _ComputeTwoImageMovements;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private byte[] _LoadInputBlob(int index)
        {
            string filename = TestSet[index];
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_LoadInputBlob)}(index={index})"))
            {
                byte[] blob = File.ReadAllBytes(filename);
                return blob;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntBitmap _LoadInputColor(int index)
        {
            byte[] blob = InputFileBlobs.Get(index);
            IntBitmap colorBitmap;
            Size imageSize;
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_LoadInputColor)}(index={index})")) 
            {
                using (var ms = new MemoryStream(blob, writable: false))
                {
                    colorBitmap = BitmapIoUtility.LoadAsIntBitmap(ms);
                    imageSize = colorBitmap.Size;
                }
            }
            if (!ImageSizes.ContainsKey(index))
            {
                ImageSizes.Add(index, imageSize);
            }
            return colorBitmap;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntBitmap _ComputeHashBitmap(int index)
        {
            IntBitmap colorBitmap = InputColorBitmaps.Get(index);
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ComputeHashBitmap)}(index={index})"))
            {
                var h2dp = new Hash2DProcessor();
                h2dp.AddStage(direction: Direction.Vertical, windowSize: 6, skipStep: 1, fillValue: 0);
                h2dp.AddStage(direction: Direction.Vertical, windowSize: 6, skipStep: 6, fillValue: 0);
                h2dp.AddStage(direction: Direction.Horizontal, windowSize: 6, skipStep: 1, fillValue: 0);
                h2dp.AddStage(direction: Direction.Horizontal, windowSize: 6, skipStep: 6, fillValue: 0);
                h2dp.AddStage(direction: Direction.Horizontal, windowSize: 6, skipStep: 36, fillValue: 0);
                IntBitmap hashBitmap = h2dp.Process(colorBitmap);
                return hashBitmap;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ImageHashPointList _ComputeHashPoints(int index)
        {
            IntBitmap hashBitmap = InputHashBitmaps.Get(index);
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ComputeHashPoints)}(index={index})"))
            {
                var uhpf = new UniqueHashPointFilter(HashPointSampleRate);
                uhpf.Process(hashBitmap);
                var hp = uhpf.UniqueHashPoints;
                hp.ImageIndex = index;
                return hp;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ImagePairMovement _ComputeTwoImageMovements(int index)
        {
            int prevIndex = index - 1;
            if (prevIndex < 0)
            {
                return null;
            }
            ImageHashPointList prevList = InputHashPoints.Get(prevIndex);
            ImageHashPointList currList = InputHashPoints.Get(index);
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ComputeTwoImageMovements)}(index={index})"))
            {
                ImagePairMovement twoImageMovement = new ImagePairMovement(prevList, currList);
                twoImageMovement.Process();
                return twoImageMovement;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _DisposeIntBitmap(int _, IntBitmap intBitmap)
        {
            intBitmap?.Dispose();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run()
        {
            if (ShouldReadFromKeyboard)
            {
                Console.WriteLine("Press enter key to start.");
                Console.ReadKey();
            }
            using (var timer = new MethodTimer())
            {
                for (int imageIndex = 0; imageIndex < TestSet.Count; ++imageIndex)
                {
                    PrefetchColor(imageIndex);
                    PrefetchHash(imageIndex);
                    Run(imageIndex);
                    TryCleanup(imageIndex);
                }
            }
            if (ShouldPrintArrayPoolDiagnostics)
            {
                PrintArrayPoolDiagnostics();
            }
            if (ShouldPrintLogging)
            {
                Console.WriteLine(new string('-', 76));
                Logging.Sinks.LogMemorySink.DefaultPumpToConsole();
                Console.WriteLine(new string('-', 76));
            }
            if (ShouldReadFromKeyboard)
            {
                Console.WriteLine("Execution finished. Press enter key to terminate.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Execution finished.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PrefetchColor(int imageIndex)
        {
            int prefetchIndex = imageIndex + 10;
            if (prefetchIndex < 0 ||
                prefetchIndex >= TestSet.Count)
            {
                return;
            }
            using (var timer = new MethodTimer())
            {
                InputColorBitmaps.Get(prefetchIndex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PrefetchHash(int imageIndex)
        {
            int prefetchIndex = imageIndex + 5;
            if (prefetchIndex < 0 ||
                prefetchIndex >= TestSet.Count)
            {
                return;
            }
            using (var timer = new MethodTimer())
            {
                InputHashBitmaps.Get(prefetchIndex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run(int imageIndex)
        {
            if (Verbose)
            {
                var shortName = Path.GetFileNameWithoutExtension(TestSet[imageIndex]);
                Console.WriteLine($"Image [{imageIndex}] {shortName}");
            }
            using (var timer = new MethodTimer())
            {
                InputHashPoints.Get(imageIndex);
                //ImagePairMovements.Get(imageIndex);
            }
            Run_TrajectoryThree(imageIndex);
            //Run_ThreeImageMovementFilter(imageIndex);
            //Run_MultiImageTracking(imageIndex);
            //Run_IPGM(imageIndex);
            //RunDiagnostics(imageIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_TrajectoryThree(int imageIndex)
        {
            if (imageIndex < 2)
            {
                return;
            }
            int[] imageKeys = new int[]
            {
                imageIndex - 2, imageIndex - 1, imageIndex
            };
            var sz = ImageManager.ImageSizes[imageIndex];
            var g = Grid.Factory.CreateApproxCellSize(sz, new Size(64, 64));
            T3HashPoints t3hp;
            T3Movements t3m;
            T3GridStats t3gs;
            T3GridStats_OneVotePerCell ovpc;
            object _cellFlags;
            using (var timer = new MethodTimer($"{nameof(Run_TrajectoryThree)}(imageIndex = {imageIndex})"))
            {
                t3hp = T3HashPoints.Factory.Create(ImageManager, imageKeys);
                t3m = T3Movements.Factory.Create(t3hp);
                t3gs = new T3GridStats(t3m, imageIndex, g);
                ovpc = new T3GridStats_OneVotePerCell(t3gs);
                t3gs.Add(ovpc);
                var cellFlags = T3GridStats_CellFlags.Create<ulong>(t3gs);
                _cellFlags = cellFlags;
                t3gs.Add(cellFlags);
                t3gs.Process();
            }
            if (ShouldPrintThreeImageTrajectoryDiagnostics)
            {
                var t3diag = new T3Diagnostics()
                {
                    MovementsClass = t3m,
                    LabelCellCountsClass = ovpc
                };
                var mlto = new MultiLineTextOutput();
                t3diag.ReportMovementStats(mlto);
                mlto.ToConsole();
                Console.WriteLine(new string('-', 76));
                if (ShouldReadFromKeyboard)
                {
                    Console.WriteLine("Press enter key to continue...");
                    Console.ReadLine();
                    Console.WriteLine(new string('-', 76));
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_ThreeImageMovementFilter(int imageIndex)
        {
            if (imageIndex < 2)
            {
                return;
            }
            int[] imageKeys = new int[]
            { 
                imageIndex - 2, imageIndex - 1, imageIndex 
            };
            var timf = new ThreeImageMovementFilter(imageKeys, ImageManager);
            using (var timer = new MethodTimer())
            {
                timf.Process();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_MultiImageTracking(int imageIndex)
        {
            if (imageIndex < 2)
            {
                return;
            }
            var mit = new MultiImageTracking();
            mit.HashBitmapSource = InputHashBitmaps;
            mit.HashPointSource = InputHashPoints;
            mit.ImageSizes = new ItemSource<Size>(ImageSizes);
            var inputKeys = new List<int>();
            for (int backIndex = 0; backIndex < 10; ++backIndex)
            {
                int histIndex = imageIndex - backIndex;
                if (histIndex >= 0)
                {
                    inputKeys.Add(histIndex);
                }
            }
            mit.Process(inputKeys);
            if (imageIndex - 2 < 0)
            {
                return;
            }
            int[] threeIds = new int[]
            {
                imageIndex, imageIndex - 1, imageIndex - 2
            };
            var threeIMC = new ThreeImageMovementCluster(ImageManager, mit.HashPointTable, threeIds);
            threeIMC.Process();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_IPGM(int imageIndex)
        {
            int currIndex = imageIndex;
            int prevIndex = imageIndex - 1;
            if (prevIndex < 0)
            {
                return;
            }
            IntBitmap diagBitmap;
            using (var timer = new MethodTimer())
            {
                ImagePairGridMovement ipgm = new ImagePairGridMovement(InputHashBitmaps, InputHashPoints);
                var result = ipgm.Process(currIndex, prevIndex);
                var diag = new Tracking.DiagViz.Diag_IPGM(InputColorBitmaps, ipgm);
                diagBitmap = diag.Render(result);
            }
            if (ShouldSave_IPGM &&
                !HasSavedIPGM.Contains(imageIndex))
            {
                HasSavedIPGM.Add(imageIndex);
                var shortName = Path.GetFileNameWithoutExtension(TestSet[imageIndex]);
                var outFolder = @"C:\Users\kinch\Screenshots\TestClass_20200708_1112am";
                diagBitmap.SaveToFile(Path.Combine(outFolder, $"IPGM_{imageIndex}_{shortName}.png"));
            }
            diagBitmap.Dispose();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RunDiagnostics(int imageIndex)
        {
            var m = ImagePairMovements.Get(imageIndex);
            if (m is null)
            {
                return;
            }
            var flag = m.Heuristic.Flag;
            if (flag == ImagePairMovementFlag.Break ||
                flag == ImagePairMovementFlag.Unexplained)
            {
                var sb = new StringBuilder();
                m.Heuristic.ToString(sb, new string(' ', 8));
                Console.WriteLine(sb.ToString());
            }
            if (flag == ImagePairMovementFlag.Break ||
                flag == ImagePairMovementFlag.Unexplained)
            {
                TrySaveDiagnostics(imageIndex - 1);
                TrySaveDiagnostics(imageIndex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void TrySaveDiagnostics(int imageIndex)
        {
            if (imageIndex < 0)
            {
                return;
            }
            if (HasSavedDiagnostics.Contains(imageIndex))
            {
                return;
            }
            HasSavedDiagnostics.Add(imageIndex);
            var shortName = Path.GetFileNameWithoutExtension(TestSet[imageIndex]);
            var outFolder = @"C:\Users\kinch\Screenshots\TestClass_20200708_1112am";
            InputHashBitmaps.Get(imageIndex).SaveToFile(Path.Combine(outFolder, $"Hash_{imageIndex}_{shortName}.png"));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void TryCleanup(int imageIndex)
        {
            if (imageIndex < 0)
            {
                return;
            }
            InputFileBlobs.Remove(imageIndex);
            InputColorBitmaps.Remove(imageIndex);
            InputHashBitmaps.Remove(imageIndex - 3);
            InputHashPoints.Remove(imageIndex - 3);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PrintArrayPoolDiagnostics()
        {
            PrintArrayPoolDiagnostics<int>();
            PrintArrayPoolDiagnostics<byte>();
            PrintArrayPoolDiagnostics<ulong>();
            //PrintArrayPoolDiagnostics<Point>();
            //PrintArrayPoolDiagnostics<HashPoint>();
        }

        public void PrintArrayPoolDiagnostics<T>()
        {
            Console.WriteLine(ExactLengthArrayPool<T>.DefaultInstance.Unsafe_GetMemoryUsage().ToString());
            Console.WriteLine();
        }
    }
}
