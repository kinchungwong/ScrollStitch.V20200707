using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class IntegerFirstLastSkip
    {
        public int First { get; set; }

        public int Last { get; set; }

        public int Step { get; set; }

        public static bool TryParse(string s, out IntegerFirstLastSkip result)
        {
            result = null;
            if (!ColonDelimitedNonEscaping.TryParse(s, out var colonSplit))
            {
                return false;
            }
            var items = colonSplit.Items;
            int first, last, step;
            if (items.Length == 1)
            {
                if (!int.TryParse(items[0], out first))
                {
                    return false;
                }
                last = first;
                step = 1;
            }
            else if (items.Length == 2)
            {
                if (!int.TryParse(items[0], out first) ||
                    !int.TryParse(items[1], out last))
                {
                    return false;
                }
                step = 1;
            }
            else if (items.Length == 3)
            {
                if (!int.TryParse(items[0], out first) ||
                    !int.TryParse(items[2], out last) ||
                    !int.TryParse(items[1], out step))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            result = new IntegerFirstLastSkip()
            {
                First = first,
                Last = last,
                Step = step
            };
            return true;
        }

        public IEnumerable<int> Enumerate()
        {
            if (Step > 0 && Last >= First)
            {
                for (int k = First; k <= Last; k += Step)
                {
                    yield return k;
                }
            }
            else if (Step < 0 && Last <= First)
            {
                for (int k = First; k >= Last; k += Step)
                {
                    yield return k;
                }
            }
            else if (First == Last)
            {
                yield return First;
            }
            else
            {
                yield break;
            }
        }
    }
}
