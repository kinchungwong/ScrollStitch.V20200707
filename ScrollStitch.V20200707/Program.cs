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
                captureLoop.ScreenshotBaseFolder = @"C:\Users\kinch\Screenshots\2020-07-17";
                //captureLoop.ScreenshotOutputFolder = null;
                captureLoop.ConfigureFromInteractiveConsole();
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
