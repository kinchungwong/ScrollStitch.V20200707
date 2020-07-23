using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Drawing;

namespace ScrollStitch.V20200707
{
    using Imaging;
    using Imaging.IO;
    using Imaging.Compare;
    using Imaging.ScreenCapture;

    public class Main_ScreenCaptureLoop
    {
        public string ScreenshotBaseFolder { get; set; }

        public bool AllowBlankNickname { get; set; } = true;

        public string Nickname { get; set; }

        public string ScreenshotOutputFolder { get; set; }

        public List<string> CapturedFiles { get; private set; }

        public Func<bool> ShouldContinue { get; set; }

        public Func<IntBitmap, bool> ShouldSaveImage { get; set; }

        public double MinCaptureIntervalMilliseconds { get; set; } = 80.0;

        public bool VerifyOutputFolderIsNew { get; set; } = true;

        public double ImageCompareSampleFrac { get; set; } = 0.02;

        public bool SuppressConsoleOutputDuringCapture { get; set; } = false;

        #region private
        private double _systemCaptureDelayNume = 0.0;
        private double _systemCaptureDelayDenom = 0.0;
        #endregion

        public Main_ScreenCaptureLoop()
        {
        }

        public void ConfigureFromInteractiveConsole()
        {
            if (string.IsNullOrEmpty(ScreenshotBaseFolder))
            {
                return;
            }
            Console.WriteLine($"Output will be stored into a new folder nested under: {ScreenshotBaseFolder}");
            Console.WriteLine("Enter a nickname for output folder:");
            Console.Write("> ");
            string nickname = Console.ReadLine();
            nickname = _SanitizeFolderName(nickname);
            if (!string.IsNullOrEmpty(nickname))
            {
                Nickname = _GetFolderDateTimePrefix() + "_" + nickname;
            }
            else 
            { 
                if (!AllowBlankNickname)
                {
                    return;
                }
                Nickname = _GetFolderDateTimePrefix();
            }
            ScreenshotOutputFolder = Path.Combine(ScreenshotBaseFolder, Nickname);
            Console.WriteLine($"Output will be stored into: {ScreenshotOutputFolder}");
            Console.WriteLine();
        }

        public void RunCaptureLoop()
        {
            if (string.IsNullOrEmpty(ScreenshotOutputFolder))
            {
                Console.WriteLine("No output folder specified. ");
                Console.WriteLine("Screen Capture Loop will not run. Press enter key to exit.");
                Console.ReadLine();
                return;
            }
            if (!Directory.Exists(ScreenshotOutputFolder))
            {
                Directory.CreateDirectory(ScreenshotOutputFolder);
            }
            else if (VerifyOutputFolderIsNew)
            {
                Console.WriteLine("VerifyOutputFolderIsNew flag is specified but the output folder already exists.");
                Console.WriteLine("Screen Capture Loop will not run. Press enter key to exit.");
                Console.ReadLine();
                return;
            }
            if (true)
            {
                Console.WriteLine("Press enter key to start capturing.");
                Console.WriteLine("Once capturing start, capturing can be stopped by pressing enter key.");
                Console.ReadLine();
            }
            CapturedFiles = new List<string>();
            _InternalCaptureLoop();
        }

        private void _InternalCaptureLoop()
        {
            int attempts = 0;
            DateTime lastCaptureTime = DateTime.Now - TimeSpan.FromDays(1);
            IntBitmap lastImage = null;
            var imageComparer = new FastImageComparer()
            { 
                SampleFrac = ImageCompareSampleFrac
            };
            while (ShouldContinue?.Invoke() ?? true)
            {
                if (Console.KeyAvailable && Console.Read() == 13)
                {
                    Console.WriteLine("Stopping capture...");
                    return;
                }
                double expectedSystemCaptureDelay = _systemCaptureDelayNume / Math.Max(1.0, _systemCaptureDelayDenom);
                var timestamp1 = DateTime.Now;
                double msecsSinceLast = (timestamp1 - lastCaptureTime).TotalMilliseconds;
                if (msecsSinceLast + expectedSystemCaptureDelay * 0.5 + 1.0 < MinCaptureIntervalMilliseconds)
                {
                    Thread.Sleep(1);
                    continue;
                }
                ++attempts;
                IntBitmap image = ScreenCaptureUtility.CaptureScreenshot();
                var timestamp2 = DateTime.Now;
                var captureDelay = timestamp2 - timestamp1;
                double captureDelayMsecs = captureDelay.TotalMilliseconds;
                _systemCaptureDelayNume = _systemCaptureDelayNume * 0.9 + captureDelayMsecs;
                _systemCaptureDelayDenom = _systemCaptureDelayDenom * 0.9 + 1.0;
                var timestamp = timestamp1 + TimeSpan.FromMilliseconds(captureDelayMsecs * 0.5);
                var actualInterval = timestamp - lastCaptureTime;
                lastCaptureTime = timestamp;
                if (ShouldSaveImage?.Invoke(image) ?? true)
                {
                    bool imageSame = false;
                    if (!(lastImage is null))
                    {
                        imageSame = imageComparer.Compare(lastImage, image);
                    }
                    if (!imageSame)
                    {
                        string filename = _GetImageFilename(CapturedFiles.Count, timestamp);
                        string filePath = Path.Combine(ScreenshotOutputFolder, filename);
                        if (!SuppressConsoleOutputDuringCapture)
                        {
                            Console.WriteLine($"Capturing... (" + 
                                $"attempts: {attempts}, " + 
                                $"saved: {CapturedFiles.Count}, " + 
                                $"interval: {actualInterval.TotalMilliseconds:F0}, " + 
                                $"delay: {captureDelayMsecs:F0})");
                            Console.WriteLine("... Saving to " + filename);
                        }
                        CapturedFiles.Add(filePath);
                        image.SaveToFile(filePath);
                    }
                }
                if (!(lastImage is null))
                {
                    lastImage.Dispose();
                }
                lastImage = image;
            }
        }

        private string _SanitizeFolderName(string childFolderName)
        {
            var invalids = new HashSet<char>(Path.GetInvalidFileNameChars());
            var charArray = childFolderName.ToCharArray();
            var len = charArray.Length;
            int ko = 0;
            for (int ki = 0; ki < len; ++ki)
            {
                char c = charArray[ki];
                if (!invalids.Contains(c))
                {
                    if (ko != ki)
                    {
                        charArray[ko] = c;
                    }
                    ++ko;
                }
            }
            return new string(charArray, 0, ko);
        }

        private string _GetImageFilename(int capturedCount, DateTime timestamp)
        {
            var ampm = (timestamp.Hour >= 12 ? "pm" : "am");
            //string strTimestamp = timestamp.ToString("yyyy-MM-dd_HH.mm.ss.fff") + ampm;
            string strTimestamp = capturedCount.ToString("D4") + "_" + timestamp.ToString("HHmmssfff");
            string filename = strTimestamp + ".png";
            return filename;
        }

        private string _GetFolderDateTimePrefix()
        {
            var timestamp = DateTime.Now;
            var ampm = (timestamp.Hour >= 12 ? "pm" : "am");
            string strTimestamp = timestamp.ToString("yyyy-MM-dd_HH.mm.ss") + ampm;
            return strTimestamp;
        }
    }
}
