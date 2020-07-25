using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config.Data
{
    public class TestSetEnumeratedFiles
    {
        public TestSet TestSet { get; }

        public List<string> Files { get; private set; }

        public TestSetEnumeratedFiles(TestSet testSet)
        {
            TestSet = testSet;
        }

        public void EnumerateFiles()
        {
            if (Directory.Exists(TestSet.LocalPath))
            {
                Files = Directory.GetFiles(TestSet.LocalPath, "*.png").ToList();
                Files.Sort();
            }
        }
    }
}
