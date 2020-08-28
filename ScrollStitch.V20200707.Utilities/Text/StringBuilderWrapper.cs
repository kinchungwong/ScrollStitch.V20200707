using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public static partial class StringBuilderWrapperMethods
    {
        public static IStringBuilder GetIStringBuilder(this StringBuilder sb)
        {
            return new StringBuilderWrapper(sb);
        }
    }

    public class StringBuilderWrapper
        : IStringBuilder
    {
        public StringBuilder StringBuilder { get; set; }

        public StringBuilderWrapper(StringBuilder sb)
        {
            StringBuilder = sb;
        }

        public IStringBuilder Append(double value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(char[] value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(object value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(ulong value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(uint value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(ushort value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(decimal value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(float value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(int value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(short value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(char value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(long value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(sbyte value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(byte value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(char[] value, int startIndex, int charCount)
        {
            StringBuilder.Append(value, startIndex, charCount);
            return this;
        }

        public IStringBuilder Append(string value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder Append(string value, int startIndex, int count)
        {
            StringBuilder.Append(value, startIndex, count);
            return this;
        }

        public IStringBuilder Append(char value, int repeatCount)
        {
            StringBuilder.Append(value, repeatCount);
            return this;
        }

        public IStringBuilder Append(bool value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public IStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            StringBuilder.AppendFormat(provider, format, args);
            return this;
        }

        public IStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            StringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return this;
        }

        public IStringBuilder AppendFormat(string format, params object[] args)
        {
            StringBuilder.AppendFormat(format, args);
            return this;
        }

        public IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0)
        {
            StringBuilder.AppendFormat(provider, format, arg0);
            return this;
        }

        public IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1)
        {
            StringBuilder.AppendFormat(provider, format, arg0, arg1);
            return this;
        }

        public IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
        {
            StringBuilder.AppendFormat(provider, format, arg0, arg1, arg2);
            return this;
        }

        public IStringBuilder AppendFormat(string format, object arg0)
        {
            StringBuilder.AppendFormat(format, arg0);
            return this;
        }

        public IStringBuilder AppendFormat(string format, object arg0, object arg1)
        {
            StringBuilder.AppendFormat(format, arg0, arg1);
            return this;
        }

        public IStringBuilder AppendLine()
        {
            StringBuilder.AppendLine();
            return this;
        }

        public IStringBuilder AppendLine(string value)
        {
            StringBuilder.AppendLine(value);
            return this;
        }

        public IStringBuilder Clear()
        {
            StringBuilder.Clear();
            return this;
        }

        public IStringBuilder CopyTo(IStringBuilder dest)
        {
            dest.Append(StringBuilder.ToString());
            return this;
        }
    }
}
