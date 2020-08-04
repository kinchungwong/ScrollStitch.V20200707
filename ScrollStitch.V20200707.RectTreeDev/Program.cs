using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.RectTreeDev
{
    public class Program
    {
        public static readonly string Banner = new string('-', 76);
        public static bool PrintVerbose = false;
        public static bool IsInteractive = Debugger.IsAttached;

        public static void PauseIfInteractive()
        {
            if (!IsInteractive)
            {
                return;
            }
            Console.WriteLine(Banner);
            Console.WriteLine("Paused. Press enter key to continue.");
            Console.ReadLine();
            Console.WriteLine(Banner);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Main(string[] args)
        {
            try
            {
                new Test_0001().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Banner);
                Console.WriteLine("Exception.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(Banner);
                Console.WriteLine("Execution finished. Press enter key to terminate.");
                Console.ReadLine();
                Console.WriteLine(Banner);
                return;
            }
            Console.WriteLine(Banner);
            Console.WriteLine("Finished. No uncaught exceptions.");
            Console.WriteLine(Banner);
        }
    }
}
