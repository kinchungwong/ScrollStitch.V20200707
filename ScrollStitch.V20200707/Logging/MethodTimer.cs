using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Logging
{
    public class MethodTimer
        : IDisposable
    {
        public static string Indent { get; set; } = new string(' ', 8);
        public static bool PrintOnEntry { get; set; } = false;
        public static bool PrintOnExit { get; set; } = false;
        public static bool PrintTimingOnExit { get; set; } = true;
        public static long SuppressTimingBelow { get; set; } = 20;

        public string MethodName { get; }
        private Stopwatch _sw;

        public MethodTimer([CallerMemberName] string methodName = "?")
        {
            MethodName = methodName;
            if (PrintOnEntry)
            {
                Sinks.LogMemorySink.DefaultAdd(DateTime.Now, $"{Indent}{MethodName} enter");
            }
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw.Stop();
            long msecs = _sw.ElapsedMilliseconds;
            if (PrintOnExit && !PrintTimingOnExit)
            {
                Sinks.LogMemorySink.DefaultAdd(DateTime.Now, $"{Indent}{MethodName} exit");
            }
            else if (PrintOnExit ||
                (PrintTimingOnExit && msecs >= SuppressTimingBelow))
            {
                Sinks.LogMemorySink.DefaultAdd(DateTime.Now, $"{Indent}{MethodName} exit {msecs} msecs");
            }
        }
    }
}
