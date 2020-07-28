using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    /// <summary>
    /// Denotes a split of a string by colon, and which the whole string doesn't contain
    /// any escaping or quote characters
    /// </summary>
    public class ColonDelimitedNonEscaping
    {
        internal static readonly char[] _forbidden = { '"', '\'', '\\' };

        public string[] Items { get; set; }

        public ColonDelimitedNonEscaping(string[] items)
        {
            Items = items;
        }

        public static bool TryParse(string input, out ColonDelimitedNonEscaping result)
        {
            int firstForbidden = input.IndexOfAny(_forbidden);
            if (firstForbidden >= 0)
            {
                result = null;
                return false;
            }
            result = new ColonDelimitedNonEscaping(input.Split(':'));
            return true;
        }
    }
}
