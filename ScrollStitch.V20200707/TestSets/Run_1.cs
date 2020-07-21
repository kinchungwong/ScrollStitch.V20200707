using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ScrollStitch.V20200707.TestSets
{
    public class Run_1 : TestSetBase
    {
        public string Folder { get; } = @"C:\Users\kinch\Screenshots\Run_1";

        public Run_1()
        {
            _files = _FromDirectoryFiles(Folder);
        }

        public Run_1(int maxCount)
        {
            _files = _FromDirectoryFiles(Folder).Take(maxCount).ToList();
        }
    }
}
