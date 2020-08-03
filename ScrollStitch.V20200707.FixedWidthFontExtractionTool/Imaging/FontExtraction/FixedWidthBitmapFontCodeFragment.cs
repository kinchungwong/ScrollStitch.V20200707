using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.FontExtraction
{
    using Data;
    using ScrollStitch.V20200707.Text;

    public class FixedWidthBitmapFontCodeFragment
    {
        public string FontClassName { get; }
        
        public Size CharSize { get; }
        
        public Range CharRange { get; }
        
        public string Base64String { get; }

        public MultiLineTextOutput StringBuilderAppendStatements { get; private set; }

        public FixedWidthBitmapFontCodeFragment(string fontClassName, Size charSize, Range charRange, string base64String)
        {
            FontClassName = fontClassName;
            CharSize = charSize;
            CharRange = charRange;
            Base64String = base64String;
        }

        private void _GenerateStringBuilderAppendStatements()
        {
            if (!(StringBuilderAppendStatements is null))
            {
                return;
            }
            const char qm = '"';
            const int indent = 16;
            StringBuilderAppendStatements = new MultiLineTextOutput();
            var lineSB = new StringBuilder();
            int length = Base64String.Length;
            int nextStart = 0;
            while (nextStart < length)
            {
                int start = nextStart;
                int stop = Math.Min(length, start + 64);
                lineSB.Append(' ', indent);
                lineSB.Append("sb.Append(");
                lineSB.Append(qm);
                lineSB.Append(Base64String, start, (stop - start));
                lineSB.Append(qm);
                lineSB.Append(");");
                StringBuilderAppendStatements.AppendLine(lineSB.ToString());
                lineSB.Clear();
                nextStart = stop;
            }
        }

        public string Generate()
        {
            // ====== TODO ======
            // This code is not optimized for performance because it is rarely used - 
            // only when generating a new font resource, which is done manually.
            // ======
            _GenerateStringBuilderAppendStatements();
            var stmts = new StringBuilder();
            StringBuilderAppendStatements.ToStringBuilder(stmts);
            return CodeFragmentTemplate
                .Replace("$(FontClassName)", FontClassName)
                .Replace("$(CharWidth)", CharSize.Width.ToString())
                .Replace("$(CharHeight)", CharSize.Height.ToString())
                .Replace("$(CharRangeStart)", CharRange.Start.ToString())
                .Replace("$(CharRangeStop)", CharRange.Stop.ToString())
                .Replace("$(StringBuilderAppendStatements)", stmts.ToString());
        }

        public static readonly string CodeFragmentTemplate =
@"
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.TextInternals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging.IO;

    public sealed class $(FontClassName)
        : FixedWidthBitmapFontBase
    {
        public static $(FontClassName) DefaultInstance { get; } = new $(FontClassName)();

        public $(FontClassName)()
        {
            CharSize = new Size($(CharWidth), $(CharHeight));
            CharRange = new Range($(CharRangeStart), $(CharRangeStop));
            _SingleColumnImageFunc = () => StaticResource.LoadImageFromResource();
        }

#region static resource
        public static class StaticResource
        {
            public static IntBitmap LoadImageFromResource()
            {
                using (var strm = new MemoryStream(GetResourceFileBytes(), writable: false))
                {
                    return BitmapIoUtility.LoadAsIntBitmap(strm);
                }
            }

            public static byte[] GetResourceFileBytes()
            {
                return Convert.FromBase64String(GetResourceDataAsBase64String());
            }

            public static string GetResourceDataAsBase64String()
            {
                StringBuilder sb = new StringBuilder();
$(StringBuilderAppendStatements)
                return sb.ToString();
            }
        }
#endregion
    }
}
";
    }
}
