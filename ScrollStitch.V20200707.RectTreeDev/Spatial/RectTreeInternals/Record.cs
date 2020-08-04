using ScrollStitch.V20200707.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.RectTreeInternals
{
    public struct Record
    {
        public Rect Rect { get; }

        public int Index { get; }

        public ItemFlag Flag { get; }

        public Record(Rect rect, int index, ItemFlag flag)
        {
            Rect = rect;
            Index = index;
            Flag = flag;
        }
    }
}
