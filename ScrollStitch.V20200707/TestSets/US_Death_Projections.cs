using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.TestSets
{
    public class US_Death_Projections : TestSetBase
    {
        public string Folder { get; } = @"C:\Users\kinch\Screenshots\Run_20200407_US_Deaths_Projections";

        public US_Death_Projections()
        {
            _files = _FromDirectoryFiles(Folder);
        }

        public US_Death_Projections(int maxCount)
        {
            _files = _FromDirectoryFiles(Folder).Take(maxCount).ToList();
        }
    }
}
