using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking.Diagnostics
{
    using Data;
    using Imaging;
    using Imaging.Compare;
    using Logging;
    using static Imaging.RowAccess.BitmapRowAccessUtility;
    using SourceList = Imaging.Collections.UniformSizeRowSourceList<int>;

    /// <summary>
    /// This class renders an output bitmap in which all unmatched content will be highly visible.
    /// </summary>
    public class T3UnmatchedContentRenderer
    {
        public T3Main MainClass { get; }

        #region private
        private readonly int _imageKey0;
        private readonly int _imageKey1;
        private readonly int _imageKey2;
        private IntBitmap _bitmap0;
        private IntBitmap _bitmap1;
        private IntBitmap _bitmap2;
        private SourceList _targetSources;
        private MultiImageComparer _mic;
        #endregion

        public T3UnmatchedContentRenderer(T3Main t3Main)
        {
            MainClass = t3Main;
            var imageKeys = MainClass.ImageKeys;
            _imageKey0 = imageKeys.ItemAt(0);
            _imageKey1 = imageKeys.ItemAt(1);
            _imageKey2 = imageKeys.ItemAt(2);
        }

        public IntBitmap Render()
        {
            _FetchBitmaps();
            _InitRowSources();
            IntBitmap result = _InvokeComparer();
            _Cleanup();
            return result;
        }

        private void _FetchBitmaps()
        {
            using (var timer = new MethodTimer($"{nameof(T3UnmatchedContentRenderer)}.{nameof(_FetchBitmaps)}"))
            {
                var inputColorBitmaps = MainClass.ImageManager.InputColorBitmaps;
                _bitmap0 = inputColorBitmaps.Get(_imageKey0);
                _bitmap1 = inputColorBitmaps.Get(_imageKey1);
                _bitmap2 = inputColorBitmaps.Get(_imageKey2);
            }
        }

        private void _InitRowSources()
        {
            using (var timer = new MethodTimer($"{nameof(T3UnmatchedContentRenderer)}.{nameof(_InitRowSources)}"))
            {
                Rect bounds = new Rect(Point.Origin, _bitmap1.Size);
                var movements = MainClass.SecondStageMovements.Movements;
                HashSet<Movement> backSet = new HashSet<Movement>();
                HashSet<Movement> forwardSet = new HashSet<Movement>();
                _targetSources = new SourceList();
                foreach (var m012 in movements)
                {
                    (Movement m01, Movement m12) = m012;
                    Movement m10 = Movement.Zero - m01;
                    if (!backSet.Contains(m10))
                    {
                        backSet.Add(m10);
                        if (m10 == Movement.Zero)
                        {
                            _targetSources.Add(WrapDirect(_bitmap0));

                        }
                        else
                        {
                            _targetSources.Add(Wrap(_bitmap0, bounds + m10, canWrite: false, allowOutOfBounds: true));
                        }
                    }
                    if (!forwardSet.Contains(m12))
                    {
                        forwardSet.Add(m12);
                        if (m12 == Movement.Zero)
                        {
                            _targetSources.Add(WrapDirect(_bitmap2));
                        }
                        else
                        {
                            _targetSources.Add(Wrap(_bitmap2, bounds + m12, canWrite: false, allowOutOfBounds: true));
                        }
                    }
                }
            }
        }

        private IntBitmap _InvokeComparer()
        {
            using (var timer = new MethodTimer($"{nameof(T3UnmatchedContentRenderer)}.{nameof(_InvokeComparer)}"))
            {
                MultiImageComparer mic = new MultiImageComparer(WrapDirect(_bitmap1), _targetSources);
                mic.Process();
                return mic.NearestDifferenceBitmap;
            }
        }

        private void _Cleanup()
        {
            _bitmap0 = null;
            _bitmap1 = null;
            _bitmap2 = null;
            _targetSources = null;
            _mic = null;
        }
    }
}
