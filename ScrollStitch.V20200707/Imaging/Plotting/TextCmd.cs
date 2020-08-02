using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    using Data;
    using TextInternals;
    using RowAccess;
    using V20200707.Functional;

    public class TextCmd
        : IDrawCmd
    {
        public class Builder
        {
            public IEnumerable<string> Lines { get; set; }

            public IFixedWidthBitmapFont Font { get; set; }

            public int HorizontalCondensation { get; set; }

            public int VerticalCondensation { get; set; }

            public Point AnchorPoint { get; set; }

            public HorzAlign HorzAlign { get; set; }

            public VertAlign VertAlign { get; set; }

            public TextAnchor Anchor
            {
                // Due to TextAnchor being an immutable struct, this property works by 
                // construct-on-get and deconstruct-on-set.
                get => new TextAnchor(AnchorPoint, HorzAlign, VertAlign);
                set => (AnchorPoint, HorzAlign, VertAlign) = value;
            }

            public Builder()
            {
                Font = AsciiConsoleFont_10x14.DefaultInstance;
            }

            public Builder(IEnumerable<string> lines)
                : this()
            {
                Lines = lines;
            }

            public Builder(Builder other)
            {
                Lines = other.Lines;
                Font = other.Font ?? AsciiConsoleFont_10x14.DefaultInstance;
                HorizontalCondensation = other.HorizontalCondensation;
                VerticalCondensation = other.VerticalCondensation;
                AnchorPoint = other.AnchorPoint;
                HorzAlign = other.HorzAlign;
                VertAlign = other.VertAlign;
            }

            public TextCmd Create()
            {
                if (Lines is null)
                {
                    throw new ArgumentNullException(nameof(Lines));
                }
                if (Font is null)
                {
                    throw new ArgumentNullException(nameof(Font));
                }
                return new TextCmd(this);
            }
        }

        private readonly Builder _builder;

        public IReadOnlyList<string> Lines { get; private set; }

        public IFixedWidthBitmapFont Font => _builder.Font;

        public int HorizontalCondensation => _builder.HorizontalCondensation;

        public int VerticalCondensation => _builder.VerticalCondensation;

        public Point AnchorPoint => _builder.AnchorPoint;

        public HorzAlign HorzAlign => _builder.HorzAlign;

        public VertAlign VertAlign => _builder.VertAlign;

        public TextAnchor Anchor => 
            new TextAnchor(_builder.AnchorPoint, _builder.HorzAlign, _builder.VertAlign);

        public Size CharSize => Font.CharSize;

        public Size CharGridSize { get; private set; }

        public Size RenderedSize { get; private set; }

        public Point RenderedTopLeft { get; private set; }

        public TextCmd(Builder builder)
        {
            _builder = new Builder(builder);
            if (_builder.Lines is null)
            {
                throw new ArgumentNullException($"{nameof(builder.Lines)}");
            }
            if (_builder.Font is null)
            {
                throw new ArgumentNullException($"{nameof(builder.Font)}");
            }
            _ConvertLineCollectionOnCopy(_builder.Lines);
            _CtorInitCharCounts();
            _CtorEstimateRenderedSize();
        }

        public void Draw(IntBitmap bitmap)
        {
            if (HorizontalCondensation <= 0 &&
                VerticalCondensation <= 0)
            {
                _DirectCopy(bitmap);
            }
            else 
            {
                _InternalBlend(bitmap);
            }
        }

        private void _DirectCopy(IntBitmap dest)
        {
            int destWidth = dest.Width;
            int destHeight = dest.Height;
            int lineCount = Lines.Count;
            for (int lineIndex = 0; lineIndex < lineCount; ++lineIndex)
            {
                var line = Lines[lineIndex];
                int charCount = line.Length;
                for (int charIndex = 0; charIndex < charCount; ++charIndex)
                {
                    int charValue = line[charIndex];
                    int startX = RenderedTopLeft.X + charIndex * (CharSize.Width - HorizontalCondensation);
                    int startY = RenderedTopLeft.Y + lineIndex * (CharSize.Height - VerticalCondensation);
                    var fontRA = Font.GetBitmapRowsForChar(charValue);
                    var destRect = new Rect(startX, startY, CharSize.Width, CharSize.Height);
                    if (_IsCompletelyOutOfBounds(destWidth, destHeight, destRect))
                    {
                        continue;
                    }
                    var destRA = BitmapRowAccessUtility.Wrap(dest, destRect, canWrite: true, allowOutOfBounds: true);
                    BitmapRowAccessUtility.Copy(fontRA, destRA);
                }
            }
        }

        private void _InternalBlend(IntBitmap dest)
        {
            int destWidth = dest.Width;
            int destHeight = dest.Height;
            int lineCount = Lines.Count;
            for (int lineIndex = 0; lineIndex < lineCount; ++lineIndex)
            {
                var line = Lines[lineIndex];
                int charCount = line.Length;
                for (int charIndex = 0; charIndex < charCount; ++charIndex)
                {
                    int charValue = line[charIndex];
                    int startX = RenderedTopLeft.X + charIndex * (CharSize.Width - HorizontalCondensation);
                    int startY = RenderedTopLeft.Y + lineIndex * (CharSize.Height - VerticalCondensation);
                    var fontRA = Font.GetBitmapRowsForChar(charValue);
                    var destRect = new Rect(startX, startY, CharSize.Width, CharSize.Height);
                    if (_IsCompletelyOutOfBounds(destWidth, destHeight, destRect))
                    {
                        continue;
                    }
                    var destRA = BitmapRowAccessUtility.Wrap(dest, destRect, canWrite: true, allowOutOfBounds: true);
                    BitmapRowAccessUtility.Blend(fontRA, destRA, default(InternalBlendFunc));
                }
            }
        }

        private bool _IsCompletelyOutOfBounds(int bitmapWidth, int bitmapHeight, Rect rect)
        {
            return rect.Left >= bitmapWidth ||
                rect.Top >= bitmapHeight ||
                rect.Right <= 0 ||
                rect.Bottom <= 0;
        }

        private void _ConvertLineCollectionOnCopy(IEnumerable<string> lines)
        {
            switch (lines)
            {
                case null:
                    Lines = null;
                    break;
                case List<string> list:
                    Lines = list.AsReadOnly();
                    break;
                case string[] arrList:
                    Lines = Array.AsReadOnly(arrList);
                    break;
                case IReadOnlyList<string> rolist:
                    Lines = rolist;
                    break;
                default:
                    Lines = new List<string>(lines).AsReadOnly();
                    break;
            }
        }

        private void _CtorInitCharCounts()
        {
            int maxLength = 0;
            foreach (var line in Lines)
            {
                maxLength = Math.Max(maxLength, line.Length);
            }
            CharGridSize = new Size(maxLength, Lines.Count);
        }

        private void _CtorEstimateRenderedSize()
        {
            if (CharGridSize.Width <= 0 ||
                CharGridSize.Height <= 0)
            {
                RenderedSize = new Size(0, 0);
                return;
            }
            int renderedWidth = 
                CharGridSize.Width * (CharSize.Width - HorizontalCondensation) 
                + HorizontalCondensation;
            int renderedHeight = 
                CharGridSize.Height * (CharSize.Height - VerticalCondensation) 
                + VerticalCondensation;
            RenderedSize = new Size(renderedWidth, renderedHeight);
            int renderedLeft;
            switch (HorzAlign)
            {
                case HorzAlign.Left:
                    renderedLeft = AnchorPoint.X;
                    break;
                case HorzAlign.Center:
                    renderedLeft = AnchorPoint.X - renderedWidth / 2;
                    break;
                case HorzAlign.Right:
                    renderedLeft = AnchorPoint.X - renderedWidth;
                    break;
                default:
                    renderedLeft = 0;
                    break;
            }
            int renderedTop;
            switch (VertAlign)
            {
                case VertAlign.Top:
                    renderedTop = AnchorPoint.Y;
                    break;
                case VertAlign.Middle:
                    renderedTop = AnchorPoint.Y - renderedHeight / 2;
                    break;
                case VertAlign.Bottom:
                    renderedTop = AnchorPoint.Y - renderedHeight;
                    break;
                default:
                    renderedTop = 0;
                    break;
            }
            RenderedTopLeft = new Point(renderedLeft, renderedTop);
        }

        public struct InternalBlendFunc
            : IFuncInline<InternalBlendFunc, int, int, int>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Invoke(int source, int dest)
            {
                unchecked 
                {
                    int db = dest & 255;
                    int dg = (dest >> 8) & 255;
                    int dr = (dest >> 16) & 255;
                    int sb = source & 255;
                    int sg = (source >> 8) & 255;
                    int sr = (source >> 16) & 255;
                    int rb = Math.Max(db, sb);
                    int rg = Math.Max(dg, sg);
                    int rr = Math.Max(dr, sr);
                    return rb | (rg << 8) | (rr << 16);
                }
            }
        }
    }
}
