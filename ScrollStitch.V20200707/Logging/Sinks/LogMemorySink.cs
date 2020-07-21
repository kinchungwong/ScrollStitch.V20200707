using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Logging.Sinks
{
    public class LogMemorySink
    {
        public static LogMemorySink DefaultInstance { get; } = new LogMemorySink();

        private ConcurrentQueue<(DateTime, string)> _queue = new ConcurrentQueue<(DateTime, string)>();

        public LogMemorySink()
        { 
        }

        public void Add(DateTime dateTime, string msg)
        {
            _queue.Enqueue((dateTime, msg));
        }

        public void PumpToConsole()
        {
            while (_queue.TryDequeue(out (DateTime dateTime, string msg) entry))
            {
                Console.WriteLine(entry.dateTime.ToString() + new string(' ', 8) + entry.msg);
            }
        }

        public static void DefaultAdd(DateTime dateTime, string msg)
        {
            DefaultInstance.Add(dateTime, msg);
        }

        public static void DefaultPumpToConsole()
        {
            DefaultInstance.PumpToConsole();
        }
    }
}
