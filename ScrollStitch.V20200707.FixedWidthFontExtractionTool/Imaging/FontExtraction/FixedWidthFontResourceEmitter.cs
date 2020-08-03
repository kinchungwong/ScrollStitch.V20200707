using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.FontExtraction
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Arrays;
    using ScrollStitch.V20200707.Imaging.IO;
    using ScrollStitch.V20200707.Imaging.RowAccess;

    public class FixedWidthFontResourceEmitter
    {
        public FixedWidthFontRectExtractor RectExtractor { get; }

        public Size UniformCharCropSize => RectExtractor.UniformCharCropSize;

        public Rect[,] UniformCharCropRects => RectExtractor.UniformCharCropRects;

        public int BackgroundColor => RectExtractor.BackgroundColor;

        public DuplexCharArrayInfo DuplexInfo => RectExtractor.DuplexInfo;

        public Range CharRange => DuplexInfo.PaddedInfo.CoreInfo.CharRange;

        public char[,] CharArray => DuplexInfo.CharArray;

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public IntBitmap ScreenshotImage => RectExtractor.CharTableBitmap;

        public IntBitmap SingleColumnImage { get; private set; }

        public Dictionary<int, Rect> CharValueRects { get; private set; }

        public byte[] ImageFileBytes { get; private set; }

        public string EncodedBase64 { get; private set; }

        public string CodeFragment { get; private set; }

        public FixedWidthFontResourceEmitter(FixedWidthFontRectExtractor rectExtractor)
        {
            RectExtractor = rectExtractor;
            _CtorValidateRowColumnCounts();
        }

        public void Process()
        {
            _FindCharValueRects();
            _GenerateImage();
            _EncodeImageFileBytes();
            _EncodeBase64String();
            _GenerateCodeFragment();
        }

        private void _FindCharValueRects()
        {
            CharValueRects = new Dictionary<int, Rect>();
            int rows = RowCount;
            int cols = ColumnCount;
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    int charValue = CharArray[row, col];
                    Rect charRect = UniformCharCropRects[row, col];
                    if (CharValueRects.ContainsKey(charValue))
                    {
                        continue;
                    }
                    if (!charRect.IsPositive)
                    {
                        continue;
                    }
                    CharValueRects.Add(charValue, charRect);
                }
            }
        }

        private void _GenerateImage()
        {
            int fontWidth = UniformCharCropSize.Width;
            int fontHeight = UniformCharCropSize.Height;
            int imageWidth = fontWidth;
            int imageHeight = fontHeight * CharRange.Count;
            SingleColumnImage = new IntBitmap(imageWidth, imageHeight);
            _FillBackgroundColor(SingleColumnImage);
            for (int charValue = CharRange.Start; charValue < CharRange.Stop; ++charValue)
            {
                if (!CharValueRects.TryGetValue(charValue, out Rect charRect))
                {
                    continue;
                }
                int destY = (charValue - CharRange.Start) * fontHeight;
                Rect destRect = new Rect(0, destY, fontWidth, fontHeight);
                var screenshotRA = BitmapRowAccessUtility.WrapRead(ScreenshotImage, charRect);
                var destRA = BitmapRowAccessUtility.WrapWrite(SingleColumnImage, destRect);
                BitmapRowAccessUtility.Copy(screenshotRA, destRA);
            }
        }

        private void _EncodeImageFileBytes()
        {
            using (var strm = SingleColumnImage.SaveToMemoryStream())
            {
                ImageFileBytes = strm.ToArray();
            }
        }

        private void _EncodeBase64String()
        {
            EncodedBase64 = Convert.ToBase64String(ImageFileBytes, 0, ImageFileBytes.Length);
        }

        private void _GenerateCodeFragment()
        {
            int charWidth = UniformCharCropSize.Width;
            int charHeight = UniformCharCropSize.Height;
            string uniqueID = _ComputeMD5(ImageFileBytes).Substring(0, 8);
            string fontClassName = $"FixedWidthFont_{charWidth}x{charHeight}_{uniqueID}";
            var fragment = new FixedWidthBitmapFontCodeFragment(fontClassName, UniformCharCropSize, CharRange, EncodedBase64);
            CodeFragment = fragment.Generate();
        }

        private void _FillBackgroundColor(IntBitmap dest)
        {
            BuiltinArrayMethods.NoInline.ArrayFill(SingleColumnImage.Data, BackgroundColor, 0, SingleColumnImage.Data.Length);
        }

        private void _CtorValidateRowColumnCounts()
        {
            int charRows = CharArray.GetLength(0);
            int charCols = CharArray.GetLength(1);
            int rectRows = UniformCharCropRects.GetLength(0);
            int rectCols = UniformCharCropRects.GetLength(1);
            if (charRows != rectRows ||
                charCols != rectCols)
            { 
                throw new InvalidOperationException(
                    "Mismatch in the number of rows and columns of characters between " + 
                    "the text template and the detected rectangles.");
            }
            RowCount = rectRows;
            ColumnCount = rectCols;
        }

        private static string _ComputeMD5(byte[] bytes)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBytes = md5.ComputeHash(bytes);
            string hashHex = string.Empty;
            foreach (var b in hashBytes)
            {
                hashHex += b.ToString("x2");
            }
            return hashHex;
        }
    }
}
