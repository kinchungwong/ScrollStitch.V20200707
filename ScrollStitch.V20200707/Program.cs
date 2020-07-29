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
                // ======
                // Trigger config parsing.
                // Must happen at beginning of Application Main.
                // Not having a valid config will cause everything, including self-care 
                // (e.g. error logging) to fail, or failure to cleanly shutdown.
                // ======
                var _ = Config.TestClassConfig.DefaultInstance; 
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Exception during configuration parsing.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Execution finished. Press enter key to terminate.");
                Console.ReadLine();
                return;
            }
            if (true) 
            {
                // TODO this will be moved inside the class.
                string keyName = "Imaging.RowAccess.CroppedBitmapRowAccess.DefaultSettings.RandomizeOutOfBoundValues";
                var kvp = Config.TestClassConfig.DefaultInstance.DevelopmentSwitches.Find((_) => _.Key.Equals(keyName));
                if (!(kvp.Key is null) &&
                    bool.TryParse(kvp.Value, out bool boolValue))
                {
                    Imaging.RowAccess.CroppedBitmapRowAccess.DefaultSettings.RandomizeOutOfBoundValues = boolValue;
                }
            }
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
