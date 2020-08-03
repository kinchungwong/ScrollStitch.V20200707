using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public class CharArrayFormatterOptions
    {
        public int BlankLines { get; set; } = 4;

        public int Indentation { get; set; } = 8;

        public CharArrayFormatterOptions()
        { 
        }

        public CharArrayFormatterOptions(int blankLines, int indentation)
        {
            BlankLines = blankLines;
            Indentation = indentation;
        }
    }
}
