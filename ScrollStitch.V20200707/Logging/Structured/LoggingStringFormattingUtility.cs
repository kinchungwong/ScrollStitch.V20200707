using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScrollStitch.V20200707.Logging.Structured
{
    using Data;
    using HashCode;
    using Bitwise;

    public static class LoggingStringFormattingUtility
    {
        #region private static
        private static Lazy<int> _lzSeed = new Lazy<int>(() => DateTime.Now.GetHashCode());
        #endregion

        public static string GetCurrentThreadBanner()
        {
            int managedId = Thread.CurrentThread.ManagedThreadId;
            int seed = _lzSeed.Value;
            int hashed = new HashCodeBuilder(typeof(LoggingStringFormattingUtility)).Ingest(seed, managedId).GetHashCode();
            byte[] bytes = BitConverter.GetBytes(hashed);
            string banner = Convert.ToBase64String(bytes);
            if (banner.Length > 8)
            {
                banner = banner.Substring(0, 8);
            }
            else if (banner.Length < 8)
            {
                banner += new string('-', (8 - banner.Length));
            }
            return banner;
        }
    }
}
