using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.TestSets
{
    public class Run_20200403 : TestSetBase
    {
        public string Folder { get; } = @"C:\Users\kinch\Screenshots\Run_20200403";

        public Run_20200403()
        {
            _files = _FromDirectoryFiles(Folder);
        }

        public Run_20200403(int maxCount)
        {
            _files = _FromDirectoryFiles(Folder).Take(maxCount).ToList();
        }
    }
}
