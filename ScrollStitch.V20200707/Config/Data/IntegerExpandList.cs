using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class IntegerExpandList
    {
        public List<IntegerFirstLastSkip> Items { get; set; }

        public IntegerExpandList()
            : this(new List<IntegerFirstLastSkip>())
        {
        }

        public IntegerExpandList(List<IntegerFirstLastSkip> items)
        {
            Items = items;
        }

        public static bool TryParse(string s, out IntegerExpandList result)
        {
            result = null;
            if (!CommaDelimitedNonEscaping.TryParse(s, out var commaSplit))
            {
                return false;
            }
            List<IntegerFirstLastSkip> parsedItems = new List<IntegerFirstLastSkip>();
            foreach (var commaSplitItem in commaSplit.Items)
            {
                if (!IntegerFirstLastSkip.TryParse(commaSplitItem, out var firstLastSkip))
                {
                    return false;
                }
                parsedItems.Add(firstLastSkip);
            }
            result = new IntegerExpandList(parsedItems);
            return true;
        }

        public IEnumerable<int> Enumerate()
        {
            if ((Items?.Count ?? 0) == 0)
            {
                yield break;
            }
            foreach (var item in Items)
            {
                foreach (var value in item.Enumerate())
                {
                    yield return value;
                }
            }
        }
    }
}
