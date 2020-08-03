using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.TextInternals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.RowAccess;

    public abstract class FixedWidthBitmapFontBase
        : IFixedWidthBitmapFont
    {
        public Size CharSize { get; protected set; }

        public Range CharRange { get; protected set; }

        public IntBitmap SingleColumnImage => _SingleColumnImageLazy.Value;

        protected Lazy<IntBitmap> _SingleColumnImageLazy { get; }

        protected Func<IntBitmap> _SingleColumnImageFunc { get; set; }

        protected FixedWidthBitmapFontBase()
        {
            // ====== Reminder ======
            // The property _SingleColumnImageFunc is typically not assigned at construction time.
            // Therefore, the only way to correctly capture access to a valid function, 
            // is to capture the class instance itself, and then to access the function via the
            // captured class instance.
            // ======
            var capturedThis = this;
            _SingleColumnImageLazy = new Lazy<IntBitmap>(() => capturedThis._SingleColumnImageFunc());
        }

        public Rect GetRectForChar(int charValue)
        {
            if (!CharRange.Contains(charValue))
            {
                throw new ArgumentOutOfRangeException(nameof(charValue));
            }
            int charRowStart = (charValue - CharRange.Start) * CharSize.Height;
            return new Rect(0, charRowStart, CharSize.Width, CharSize.Height);
        }

        public List<KeyValuePair<int, Rect>> GetAllCharRects()
        {
            var list = new List<KeyValuePair<int, Rect>>(capacity: CharRange.Count);
            CharRange.ForEach((int charValue) =>
            {
                list.Add(new KeyValuePair<int, Rect>(charValue, GetRectForChar(charValue)));
            });
            return list;
        }

        public IntBitmap GetImageForChar(int charValue)
        {
            var rect = GetRectForChar(charValue);
            return BitmapCopyUtility.CropRect(SingleColumnImage, rect);
        }

        public void CopyTo(int charValue, IBitmapRowAccess<int> dest)
        {
            BitmapRowAccessUtility.Copy(GetBitmapRowsForChar(charValue), dest);
        }

        public IBitmapRowAccess<int> GetBitmapRowsForChar(int charValue)
        {
            var rect = GetRectForChar(charValue);
            return BitmapRowAccessUtility.Wrap(SingleColumnImage, rect, false, false);
        }
    }
}
