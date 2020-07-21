using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thread = System.Threading.Thread;
using Path = System.IO.Path;
using Screen = System.Windows.Forms.Screen;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Point = System.Drawing.Point;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ScrollStitch.V20200707.Imaging.ScreenCapture
{
    using ScrollStitch.V20200707.Imaging.IO;
    using ScrollStitch.V20200707.Logging;
    using ScrollStitch.V20200707.Utility;

    public static class ScreenCaptureUtility
    {
        public static bool ShouldBenchmark { get; set; } = false;

        public static IntBitmap CaptureScreenshot()
        {
            using (var timer = ShouldBenchmark ? new MethodTimer() : null)
            {
                using (var bitmap = _CaptureScreenshotAsBitmap())
                {
                    return bitmap.ToIntBitmap();
                }
            }
        }

        public static Bitmap _CaptureScreenshotAsBitmap()
        {
            using (var timer = ShouldBenchmark ? new MethodTimer() : null)
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                var bitmap = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                try
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }
                }
                catch
                {
                    bitmap.Dispose();
                    throw;
                }
                return bitmap;
            }
        }
    }
}
