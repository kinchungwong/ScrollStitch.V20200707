using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ======
// TODO
// This class has not yet been checked for correctness or usefulness.
// ======

namespace ScrollStitch.V20200707.Text
{
    public static class IntegerBaseFormatter
    {
        public static string Format(
            ulong value,
            int toBase,
            int minOutputWidth = 1,
            char? paddingCharOrDefault = null)
        {
            Internals.BaseDigitsArray baseDigits = GetBaseDigitsArrayForBase(toBase);
            return Format(value, baseDigits, minOutputWidth, paddingCharOrDefault);
        }

        public static string Format(
            ulong value, 
            Internals.BaseDigitsArray baseDigits, 
            int minOutputWidth = 1, 
            char? paddingCharOrDefault = null)
        {
            if (baseDigits is null)
            {
                _ThrowNull(nameof(baseDigits));
            }
            ulong toBase = (ulong)baseDigits.Base;
            char padding = paddingCharOrDefault ?? baseDigits.ItemAt(0);
            char[] results = new char[64];
            int kout = results.Length - 1;
            while (value != 0uL)
            {
                ulong digitValue = value % toBase;
                value /= toBase;
                results[kout--] = baseDigits.ItemAt((int)digitValue);
            }
            if (minOutputWidth < 0)
            {
                minOutputWidth = 0;
            }
            else if (minOutputWidth > results.Length)
            {
                minOutputWidth = results.Length;
            }
            while (results.Length - kout - 1 < minOutputWidth)
            {
                results[kout--] = padding;
            }
            return new string(results, kout + 1, results.Length - kout - 1);
        }

        public static Internals.BaseDigitsArray GetBaseDigitsArrayForBase(int toBase)
        {
            switch (toBase)
            {
                case 2:
                    return Constants.Base2;
                case 8:
                    return Constants.Base8;
                case 10:
                    return Constants.Base10;
                case 16:
                    return Constants.Base16;
                case 26:
                    return Constants.BaseAZ;
                case 52:
                    return Constants.BaseAZaz;
                case 62:
                    return Constants.Base09AZaz;
                case 64:
                    return Constants.Base64;
                case 85:
                    return Constants.RFC1924;
                default:
                    throw new NotImplementedException(
                        message:
                        $"Method {nameof(IntegerBaseFormatter)}.{nameof(GetBaseDigitsArrayForBase)}() " +
                        $"is not implemented for base {toBase}.");
            }
        }

        private static void _ThrowNull(string paramName)
        {
            throw new ArgumentNullException(paramName: paramName);
        }

        public static class Constants
        {
            public static readonly Internals.BaseDigitsArray RFC1924 = new Internals.RFC1924();
            public static readonly Internals.BaseDigitsArray Base2 = new Internals.FromSlice(RFC1924, 0, 2);
            public static readonly Internals.BaseDigitsArray Base8 = new Internals.FromSlice(RFC1924, 0, 8);
            public static readonly Internals.BaseDigitsArray Base10 = new Internals.FromSlice(RFC1924, 0, 10);
            public static readonly Internals.BaseDigitsArray Base16 = new Internals.FromSlice(RFC1924, 0, 16);
            public static readonly Internals.BaseDigitsArray BaseAZ = new Internals.FromSlice(RFC1924, 10, 26);
            public static readonly Internals.BaseDigitsArray BaseAZaz = new Internals.FromSlice(RFC1924, 10, 52);
            public static readonly Internals.BaseDigitsArray Base09AZaz = new Internals.FromSlice(RFC1924, 0, 62);
            public static readonly Internals.BaseDigitsArray Base64 = new Internals.Base64();
        }

        public static class Internals
        {
            /// <summary>
            /// <see cref="BaseDigitsArray"/> is an abstract base class that provides a unique set of characters for 
            /// encoding an unsigned integer value in a non-standard integer base.
            /// </summary>
            public abstract class BaseDigitsArray
            {
                public int Base => _charArray.Length;

                public char[] Digits => (_charArray.Clone() as char[]);

                internal readonly char[] _charArray;
                internal readonly bool[] _boolArray;

                protected BaseDigitsArray(char[] charArray, bool[] boolArray)
                {
                    _charArray = charArray;
                    _boolArray = boolArray;
                }

                protected BaseDigitsArray((char[] charArray, bool[] boolArray) args)
                    : this(args.charArray, args.boolArray)
                { 
                }

                public bool Contains(char c)
                {
                    int i = c;
                    if (i < 0 || i >= _boolArray.Length)
                    {
                        return false;
                    }
                    return _boolArray[i];
                }

                public char ItemAt(int index)
                {
                    if (index < 0 || index >= _charArray.Length)
                    {
                        return (char)0;
                    }
                    return _charArray[index];
                }

                protected static void _Throw()
                {
                    throw new Exception();
                }
            }

            /// <summary>
            /// The base digits for RFC1924, which is a base-85 encoding.
            /// <br/>
            /// <inheritdoc cref="BaseDigitsArray"/>
            /// </summary>
            public sealed class RFC1924
                : BaseDigitsArray
            {
                public RFC1924()
                    : base(_CtorArgs())
                { 
                }

                private static (char[] charArray, bool[] boolArray) _CtorArgs()
                {
                    char[] charArray = new char[85];
                    int kout = 0;
                    for (char c = '0'; c <= '9'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    for (char c = 'A'; c <= 'Z'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    for (char c = 'a'; c <= 'z'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    char[] otherChars = new char[]
                    {
                        '!', '#', '$', '%', '&', 
                        '(', ')', '*', '+', '-', 
                        ';', '<', '=', '>', '?', 
                        '@', '^', '_', '`', '{', 
                        '|', '}', '~'
                    };
                    foreach (char c in otherChars)
                    {
                        charArray[kout++] = c;
                    }
                    if (kout != charArray.Length)
                    {
                        _Throw(); // impossible
                    }
                    bool[] boolArray = new bool[128];
                    foreach (char c in charArray)
                    {
                        int i = c;
                        if (i < 0 || i >= boolArray.Length)
                        {
                            _Throw(); // impossible
                        }
                        boolArray[i] = true;
                    }
                    return (charArray, boolArray);
                }
            }

            /// <summary>
            /// The base digits for Base64.
            /// <br/>
            /// Note that non-digit characters permitted in Base64, such as newlines, whitespaces,
            /// padding, and line-continuation indicators, are not included in the returned array.
            /// <br/>
            /// Interpreters for Base64 should skip over non-digit characters and implement a 
            /// delimiter handling strategy of its own.
            /// <br/>
            /// <inheritdoc cref="BaseDigitsArray"/>
            /// </summary>
            public sealed class Base64
                : BaseDigitsArray
            {
                public Base64()
                    : base(_CtorArgs())
                {
                }

                private static (char[] charArray, bool[] boolArray) _CtorArgs()
                {
                    char[] charArray = new char[64];
                    int kout = 0;
                    for (char c = 'A'; c <= 'Z'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    for (char c = 'a'; c <= 'z'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    for (char c = '0'; c <= '9'; ++c)
                    {
                        charArray[kout++] = c;
                    }
                    char[] otherChars = new char[]
                    {
                        '+', '/'
                    };
                    foreach (char c in otherChars)
                    {
                        charArray[kout++] = c;
                    }
                    if (kout != charArray.Length)
                    {
                        _Throw(); // impossible
                    }
                    bool[] boolArray = new bool[128];
                    foreach (char c in charArray)
                    {
                        int i = c;
                        if (i < 0 || i >= boolArray.Length)
                        {
                            _Throw(); // impossible
                        }
                        boolArray[i] = true;
                    }
                    return (charArray, boolArray);
                }
            }

            /// <summary>
            /// <see cref="FromSlice"/> creates an array of characters to be used as digits 
            /// in integer base conversion. The array of characters is created by taking a slice
            /// (sub-sequence) from an existing instance of <see cref="BaseDigitsArray"/>.
            /// <br/>
            /// <inheritdoc cref="BaseDigitsArray"/>
            /// </summary>
            public sealed class FromSlice
                : BaseDigitsArray
            {
                public FromSlice(BaseDigitsArray other, int offset, int count)
                    : base(_CtorArgs(other, offset, count))
                { 
                }

                private static (char[] charArray, bool[] boolArray) _CtorArgs(BaseDigitsArray other, int offset, int count)
                {
                    char[] charArray = new char[count];
                    for (int index = 0; index < count; ++index)
                    {
                        charArray[index] = other.ItemAt(offset + index);
                    }
                    bool[] boolArray = new bool[128];
                    foreach (char c in charArray)
                    {
                        int i = c;
                        if (i < 0 || i >= boolArray.Length)
                        {
                            _Throw(); // impossible
                        }
                        boolArray[i] = true;
                    }
                    return (charArray, boolArray);
                }
            }
        }
    }
}
