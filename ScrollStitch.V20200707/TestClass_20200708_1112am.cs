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
    using Caching;
    using Collections;
    using Config;
    using Data;
    using HashCode;
    using Imaging;
    using Imaging.Hash2D;
    using Imaging.IO;
    using Logging;
    using Memory;
    using ScrollStitch.V20200707.Config.Data;
    using Spatial;
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
        public readonly Size T3MainApproxGridSize = new Size(128, 32);
        #endregion

        #region diagnstics flags
        public const bool ShouldSave_IPGM = false;
        public const bool ShouldPrintArrayPoolDiagnostics = true;
        public const bool ShouldPrintLogging = true;
        public const bool ShouldPrintThreeImageTrajectoryDiagnostics = true;
        public const bool ShouldRunT3UnmatchedContentRenderer = false;
        public const bool ShouldSaveT3UnmatchedContentRenderer = false;
        public const bool ShouldT3RenderCellFlags = false;
        public const bool ShouldPrintLongRangeHashPoints = true;
        public const bool ShouldPauseAfterEachPrint = false;
        #endregion

        /// <summary>
        /// Manages 
        /// </summary>
        public ImageManager ImageManager 
        { 
            get; 
            set; 
        } = new ImageManager();

        public IReadOnlyList<string> TestSet
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

        public ItemFactory<T3Main> T3MainClasses
        {
            get => ImageManager.T3MainClasses;
            set => ImageManager.T3MainClasses = value;
        }

#if true
        public HashSet<int> HasSavedDiagnostics { get; private set; } = new HashSet<int>();
        public HashSet<int> HasSavedIPGM { get; private set; } = new HashSet<int>();
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public TestClass_20200708_1112am(string[] args)
        {
            var config = TestClassConfig.DefaultInstance;
            var testSet = new TestSetEnumeratedFiles(config);
            TestSet = testSet.EnumerateFiles();
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
            // ======
            T3MainClasses.FactoryFunc = _ComputeT3Main;
            // ======
            ImageManager.LongRangeHashPoints.ImageSizes = new ItemSource<Size>(ImageManager.ImageSizes);
            ImageManager.LongRangeHashPoints.ImageHashPointSource = ImageManager.InputHashPoints;
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
                var h2dp = Hash2DProcessorFactory.CreateFromConfigByName("V66H666");
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
        private T3Main _ComputeT3Main(int imageIndex)
        {
            if (imageIndex < 2)
            {
                return null;
            }
            var imageKeys = new UniqueList<int>(new int[]
            {
                imageIndex - 2, imageIndex - 1, imageIndex
            });
            int mainImageKey = imageIndex;
            var threshold = new T3ClassifierThreshold()
            {
            };
            // ====== Ensure all dependencies are pre-fetched ======
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ComputeT3Main)}.Prefetch({imageIndex})"))
            {
                foreach (int imageKey in imageKeys)
                {
                    ImageManager.InputHashPoints.Get(imageKey);
                }
            }
            // ====== Actual computation of T3Main ======
            T3Main t3Main;
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ComputeT3Main)}({imageIndex})"))
            {
                t3Main = new T3Main(ImageManager, imageKeys, mainImageKey, threshold, T3MainApproxGridSize);
                t3Main.Process();
            }
            return t3Main;
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
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(Run)}(imageIndex = {imageIndex})"))
            {
                InputHashPoints.Get(imageIndex);
                //ImagePairMovements.Get(imageIndex);
                T3MainClasses.Get(imageIndex);
            }
            Run_TrajectoryThree(imageIndex);
            Run_LongRangeHashPoints(imageIndex);
            //Run_ThreeImageMovementFilter(imageIndex);
            //Run_MultiImageTracking(imageIndex);
            //Run_IPGM(imageIndex);
            //RunDiagnostics(imageIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run_TrajectoryThree(int imageIndex)
        {
            // Triggers the actual computation.
            T3Main t3Main = ImageManager.T3MainClasses.Get(imageIndex);
            if (t3Main is null)
            {
                return;
            }
            var imageKeys = t3Main.ImageKeys;
            var mainImageKey = t3Main.MainImageKey;
            var mainImageSize = t3Main.MainImageSize;
            var mainGrid = t3Main.MainGrid;
            var mlto = new MultiLineTextOutput();
            T3Diagnostics t3diag = null;
            if (ShouldPrintThreeImageTrajectoryDiagnostics)
            {
                t3diag = new T3Diagnostics(t3Main, T3Diagnostics.Stage.Second);
                mlto.AppendLine(new string('-', 76));
                t3diag.ReportMovementStats(mlto);
                mlto.AppendLine(new string('-', 76));
            }
            if (ShouldT3RenderCellFlags && !(t3diag is null))
            {
                t3diag.RenderCellFlags(mlto, IntegerBaseFormatter.Constants.RFC1924);
                mlto.AppendLine(new string('-', 76));
            }
            string EnsureOutputFolderCreated()
            {
                var outFolder_0 = Path.Combine(
                    ConfigVariableSubstitutions.DefaultInstance.Process(@"$(UserProfile)\Screenshots\Logs_20200727"),
                    TestClassConfig.DefaultInstance.CurrentTestSet.TestSetName + "_" +
                    ConfigVariableSubstitutions.DefaultInstance.Process(@"$(StartTimeMsecs)"));
                if (!Directory.Exists(outFolder_0))
                {
                    Directory.CreateDirectory(outFolder_0);
                }
                return outFolder_0;
            }
            string outFolder = null;
            T3UnmatchedContentRenderer umcr = null;
            IntBitmap umcrBitmap = null;
            if (ShouldRunT3UnmatchedContentRenderer)
            {
                umcr = new T3UnmatchedContentRenderer(t3Main);
                umcrBitmap = umcr.Render();
            }
            if (ShouldSaveT3UnmatchedContentRenderer && !(umcrBitmap is null))
            {
                outFolder = EnsureOutputFolderCreated();
                var shortName = Path.GetFileNameWithoutExtension(TestSet[imageIndex - 1]);
                umcrBitmap.SaveToFile(Path.Combine(outFolder, $"UMCR_{imageIndex}_{shortName}.png"));
            }
            if (!(umcrBitmap is null))
            {
                umcrBitmap.Dispose();
            }
            mlto.ToConsole();
            bool hasPrintedSomething = mlto.LineCount > 0;
            if (ShouldReadFromKeyboard && ShouldPauseAfterEachPrint && hasPrintedSomething)
            {
                Console.WriteLine("Press enter key to continue...");
                Console.ReadLine();
                Console.WriteLine(new string('-', 76));
            }
        }

        public void Run_LongRangeHashPoints(int imageIndex)
        {
            using (var timer = new MethodTimer($"{nameof(Run_LongRangeHashPoints)}(imageIndex = {imageIndex})"))
            {
                ImageManager.LongRangeHashPoints.ProcessImage(imageIndex);
            }
            var mlto = new MultiLineTextOutput();
            if (ShouldPrintLongRangeHashPoints)
            {
                int count = ImageManager.LongRangeHashPoints.LongRangeHashPointList.Count;
                int uniqueCount = ImageManager.LongRangeHashPoints.HashValuePresence.Count;
                int repeatCount = ImageManager.LongRangeHashPoints.HashValueRepeatCount;
                mlto.AppendLine(
                    $"LongRangeHashPoints[{imageIndex}] = (" + 
                    $"Count = {count}, " + 
                    $"UniqueCount = {uniqueCount}, " + 
                    $"RepeatCount = {repeatCount})");   
            }
            mlto.ToConsole();
            bool hasPrintedSomething = mlto.LineCount > 0;
            if (ShouldReadFromKeyboard && ShouldPauseAfterEachPrint && hasPrintedSomething)
            {
                Console.WriteLine("Press enter key to continue...");
                Console.ReadLine();
                Console.WriteLine(new string('-', 76));
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
            InputColorBitmaps.Remove(imageIndex - 3);
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
