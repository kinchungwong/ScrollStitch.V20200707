using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.TestSets
{
    public abstract class TestSetBase
        : ITestSet
    {
        protected List<string> _files;

        public string this[int index] => ((IReadOnlyList<string>)_files)[index];

        public int Count => ((IReadOnlyCollection<string>)_files).Count;

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)_files).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_files).GetEnumerator();
        }

        protected TestSetBase()
        {
        }

        protected static List<string> _FromDirectoryFiles(string dirPath)
        {
            var list = System.IO.Directory.GetFiles(dirPath, "*.png").ToList();
            list.Sort();
            return list;
        }

        protected static List<string> _PositiveFilter(IEnumerable<string> files, IList<string> positiveSubstrings)
        {
            var filtered = new List<string>();
            foreach (var file in files)
            {
                bool hasMatch = false;
                foreach (string pattern in positiveSubstrings)
                {
                    if (file.Contains(pattern))
                    {
                        hasMatch = true;
                        break;
                    }
                }
                if (hasMatch)
                {
                    filtered.Add(file);
                }
            }
            return filtered;
        }
    }
}
