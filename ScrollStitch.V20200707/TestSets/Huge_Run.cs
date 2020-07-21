using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.TestSets
{
    public class Huge_Run : TestSetBase
    {
        public string Folder { get; } = @"C:\Users\kinch\Screenshots\Huge_Run";

        public Huge_Run()
        {
            _files = _FromDirectoryFiles(Folder);
        }

        public Huge_Run(int maxCount)
        {
            _files = _FromDirectoryFiles(Folder).Take(maxCount).ToList();
        }

        public class Subset_49243 : TestSetBase
        {
            public static readonly string[] FilterList = new string[]
            {
                @"17.01.48.828pm",
                @"17.01.49.243pm",
                @"17.01.49.635pm",
                @"17.01.50.057pm",
                @"17.01.50.489pm",
            };

            public Subset_49243()
            {
                var _base = new Huge_Run();
                _files = _PositiveFilter(_base._files, FilterList);
            }
        }
    }
}
