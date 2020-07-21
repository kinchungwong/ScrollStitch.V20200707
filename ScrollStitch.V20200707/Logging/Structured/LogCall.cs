using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace ScrollStitch.V20200707.Logging.Structured
{
    public class LogCall 
        : IDisposable
    {
        private MethodInfo _methodInfo;
        private int _entryThreadId;
        private int _entryIndent;

        public LogCall(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            _entryThreadId = Thread.CurrentThread.ManagedThreadId;
            var logThread = LogThreadManager.DefaultInstance.GetLogThread();
            _entryIndent = logThread.Indent;
        }

        public void Dispose()
        { 

        }
    }
}
