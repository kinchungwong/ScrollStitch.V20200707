using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public interface IStringBuilder
    {
        IStringBuilder Append(double value);
        IStringBuilder Append(char[] value);
        IStringBuilder Append(object value);
        IStringBuilder Append(ulong value);
        IStringBuilder Append(uint value);
        IStringBuilder Append(ushort value);
        IStringBuilder Append(decimal value);
        IStringBuilder Append(float value);
        IStringBuilder Append(int value);
        IStringBuilder Append(short value);
        IStringBuilder Append(char value);
        IStringBuilder Append(long value);
        IStringBuilder Append(sbyte value);
        IStringBuilder Append(byte value);
        IStringBuilder Append(char[] value, int startIndex, int charCount);
        IStringBuilder Append(string value);
        IStringBuilder Append(string value, int startIndex, int count);
        IStringBuilder Append(char value, int repeatCount);
        IStringBuilder Append(bool value);
        IStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args);
        IStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2);
        IStringBuilder AppendFormat(string format, params object[] args);
        IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0);
        IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1);
        IStringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2);
        IStringBuilder AppendFormat(string format, object arg0);
        IStringBuilder AppendFormat(string format, object arg0, object arg1);
        IStringBuilder AppendLine();
        IStringBuilder AppendLine(string value);
        IStringBuilder Clear();
        IStringBuilder CopyTo(IStringBuilder dest);
    }
}
