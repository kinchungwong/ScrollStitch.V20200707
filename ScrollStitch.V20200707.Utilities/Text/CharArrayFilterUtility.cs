using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public static class CharArrayFilterUtility
    {
        public static string RemoveControlAndHighChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            char[] chars = input.ToCharArray();
            int length = chars.Length;
            int ko = 0;
            for (int ki = 0; ki < length; ++ki)
            {
                char c = chars[ki];
                if (c < 32 || c >= 128)
                {
                    continue;
                }
                if (ko != ki)
                {
                    chars[ko] = c;
                }
                ++ko;
            }
            if (ko == 0)
            {
                return string.Empty;
            }
            return new string(chars, 0, ko);
        }
    }
}
