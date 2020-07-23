using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707
{
    class Program
    {
        static Lazy<string> LazyOutputFolder = new Lazy<string>(
            () =>
            {
                var cfgVarSub = Config.ConfigVariableSubstitutions.DefaultInstance;
                cfgVarSub.AddBulitins();
                string outputFolderPattern = @"$(UserProfile)\Screenshots\$(StartDate)\$(StartDateTimeYMDHM)_$(RandomFileName)";
                string outputFolder = cfgVarSub.Process(outputFolderPattern);
                return outputFolder;
            });

        static string OutputFolder => LazyOutputFolder.Value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Main(string[] args)
        {
            try
            {
#if true
                new TestClass_20200708_1112am(args).Run();
#endif
#if false
                var captureLoop = new Main_ScreenCaptureLoop();
                //captureLoop.ScreenshotBaseFolder = @"C:\Users\kinch\Screenshots";
                //captureLoop.ConfigureFromInteractiveConsole();
                captureLoop.ScreenshotOutputFolder = OutputFolder;
                captureLoop.RunCaptureLoop();
#endif
                Console.WriteLine("Finished. No uncaught exceptions.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Exception.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Execution finished. Press enter key to terminate.");
                Console.ReadLine();
            }
        }
    }
}
