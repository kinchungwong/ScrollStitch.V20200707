using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Logging.Structured
{
    public class LogThread
    {
        public int ManagedThreadId { get; }
        public int Indent { get; set; }

        //public Stack<LogTask> TaskStack

        public LogThread(int managedId)
        {
            ManagedThreadId = managedId;
            Indent = 0;
        }
    }
}
