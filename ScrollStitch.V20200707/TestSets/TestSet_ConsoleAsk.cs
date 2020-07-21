using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.TestSets
{
    public class TestSet_ConsoleAsk 
        : TestSetBase
    {
        public TestSet_ConsoleAsk(string defaultFolder = null)
            : base()
        {
            if (string.IsNullOrWhiteSpace(defaultFolder))
            {
                Console.WriteLine("Enter the input folder path:");
                Console.Write("> ");
                string folder = Console.ReadLine();
                _files = _FromDirectoryFiles(folder);
            }
            else
            {
                Console.WriteLine("Enter the input folder path,");
                Console.WriteLine("Or, to use the default path (" + defaultFolder + "), just hit enter:");
                Console.Write("> ");
                string folder = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(folder))
                {
                    folder = defaultFolder;
                }
                _files = _FromDirectoryFiles(folder);
            }
        }
    }
}
