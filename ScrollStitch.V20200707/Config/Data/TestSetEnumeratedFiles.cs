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
        public TestClassConfig TestClassConfig { get; }

        public List<TestSet> TestSets { get; }

        public string CurrentTestSetName { get; }

        public IntegerExpandList CurrentTestSetItems { get; }

        public TestSet TestSet { get; }

        public List<string> Files { get; private set; }

        public TestSetEnumeratedFiles(TestClassConfig testClassConfig)
        {
            TestClassConfig = testClassConfig;
            TestSets = TestClassConfig.TestSets;
            var cts = TestClassConfig.CurrentTestSet;
            CurrentTestSetName = cts.TestSetName;
            CurrentTestSetItems = cts.Items;
            TestSet = TestSets.Find((ts) => ts.Name.Equals(CurrentTestSetName));
        }

        public List<string> EnumerateFiles()
        {
            if (!Directory.Exists(TestSet.LocalPath))
            {
                Files = new List<string>();
            }
            else
            {
                Files = Directory.GetFiles(TestSet.LocalPath, "*.png").ToList();
                Files.Sort();
            }
            if ((CurrentTestSetItems?.Items?.Count ?? 0) > 0)
            {
                var selectedFiles = new List<string>();
                foreach (int index in CurrentTestSetItems.Enumerate())
                {
                    selectedFiles.Add(Files[index]);
                }
                Files = selectedFiles;
            }
            return Files;
        }
    }
}
