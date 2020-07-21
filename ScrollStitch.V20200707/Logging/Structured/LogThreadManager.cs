using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Logging.Structured
{
    public class LogThreadManager
    {
        #region private static
        private static Lazy<LogThreadManager> _stcLzInstance =
            new Lazy<LogThreadManager>(() => new LogThreadManager());
        #endregion

        #region private
        private ConcurrentDictionary<int, LogThread> _infos =
            new ConcurrentDictionary<int, LogThread>();
        #endregion

        public static LogThreadManager DefaultInstance => _stcLzInstance.Value;

        public LogThread GetLogThread(int managedId)
        {
            return _infos.GetOrAdd(managedId, (int id) => new LogThread(id));
        }

        public LogThread GetLogThread()
        {
            int managedId = Thread.CurrentThread.ManagedThreadId;
            return GetLogThread(managedId);
        }
    }
}
