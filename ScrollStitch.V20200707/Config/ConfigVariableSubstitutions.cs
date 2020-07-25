using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Config
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// A class for applying variable substitutions to config string fields.
    /// </summary>
    public class ConfigVariableSubstitutions
    {
        private static Lazy<ConfigVariableSubstitutions> _StaticLazy = 
            new Lazy<ConfigVariableSubstitutions>(
                () => new ConfigVariableSubstitutions());

        public static ConfigVariableSubstitutions DefaultInstance => _StaticLazy.Value;

        #region private
        private Dictionary<string, string> _strings = new Dictionary<string, string>();
        private Dictionary<string, Func<string>> _funcs = new Dictionary<string, Func<string>>();
        #endregion

        public ConfigVariableSubstitutions()
        {
            AddBulitins();
        }

        public void AddBulitins()
        {
            Add("StartDate", BuiltIns.StartDate);
            Add("StartTime", BuiltIns.StartTime);
            Add("StartDateTime", BuiltIns.StartDateTime);
            Add("StartTimeMsecs", BuiltIns.StartTimeMsecs);
            Add("StartDateTimeMsecs", BuiltIns.StartDateTimeMsecs);
            Add("StartDateTimeYMDHM", BuiltIns.StartDateTimeYMDHM);
            Add("CurrentDate", BuiltIns.CurrentDate);
            Add("CurrentTime", BuiltIns.CurrentTime);
            Add("CurrentDateTime", BuiltIns.CurrentDateTime);
            Add("CurrentTimeMsecs", BuiltIns.CurrentTimeMsecs);
            Add("CurrentDateTimeMsecs", BuiltIns.CurrentDateTimeMsecs);
            Add("UserProfile", BuiltIns.UserProfile);
            Add("AppData", BuiltIns.AppData);
            Add("UserName", BuiltIns.UserName);
            Add("Temp", BuiltIns.Temp);
            Add("RandomFileName", BuiltIns.RandomFileName);
        }

        public void Add(string varName, string value)
        {
            _strings.Add(varName, value);
        }

        public void Add(string varName, Func<string> valueFunc)
        {
            _funcs.Add(varName, valueFunc);
        }

        public string Process(string input)
        {
            var proc = new VariableProcessor(this, input);
            proc.Process();
            return proc.Result;
        }

        private class VariableProcessor
        {
            internal ConfigVariableSubstitutions Host { get; }

            internal string OriginalInput { get; }

            internal string Intermediate { get; private set; }

            internal string Result { get; private set; }

            private int InternalProcessLimit = 100;

            internal VariableProcessor(ConfigVariableSubstitutions host, string originalInput)
            {
                Host = host;
                OriginalInput = originalInput;
            }

            internal void Process()
            {
                Intermediate = OriginalInput;
                int internalProcessCount = 0;
                while (true)
                {
                    bool shouldContinue = _InternalProcess();
                    ++internalProcessCount;
                    if (!shouldContinue)
                    {
                        break;
                    }
                    if (internalProcessCount >= InternalProcessLimit)
                    {
                        _DebugBreakOrThrow();
                    }
                }
                Result = Intermediate;
            }

            internal bool _InternalProcess()
            {
                var deferredLiteralSubs = new List<(Range, string)>();
                // ====== TODO ======
                // This is a stub.
                // ======
                int nextStart = 0;
                while (true)
                {
                    int idx = Intermediate.IndexOf("$(", nextStart);
                    if (idx < 0)
                    {
                        break;
                    }
                    int idx2 = idx + 2;
                    while (idx2 < Intermediate.Length)
                    {
                        char c = Intermediate[idx2];
                        if (!Text.IntegerBaseFormatter.Constants.Base09AZaz.Contains(c))
                        {
                            break;
                        }
                        ++idx2;
                    }
                    if (idx2 >= Intermediate.Length)
                    {
                        // mismatch: missing close parentheses 
                        _DebugBreakOrThrow();
                    }
                    if (Intermediate[idx2] != ')')
                    {
                        // may be modifiers, currently not implemented
                        // may be nested name, currently not implemented
                        // may be malformed, invalid
                        _DebugBreakOrThrow();
                    }
                    string varName = Intermediate.Substring(idx + 2, idx2 - idx - 2);
                    if (Host._strings.TryGetValue(varName, out string literalValue))
                    {
                        deferredLiteralSubs.Add((new Range(idx, idx2 + 1), literalValue));
                    }
                    else if (Host._funcs.TryGetValue(varName, out Func<string> literalFunc))
                    {
                        deferredLiteralSubs.Add((new Range(idx, idx2 + 1), literalFunc()));
                    }
                    nextStart = idx2 + 1;
                }
                while (deferredLiteralSubs.Count > 0)
                {
                    var lastSub = deferredLiteralSubs[deferredLiteralSubs.Count - 1];
                    deferredLiteralSubs.RemoveAt(deferredLiteralSubs.Count - 1);
                    Intermediate =
                        Intermediate.Substring(0, lastSub.Item1.Start) +
                        lastSub.Item2 +
                        Intermediate.Substring(lastSub.Item1.Stop);
                }
                return false;
            }

            internal static void _DebugBreakOrThrow()
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else 
                {
                    throw new Exception();
                }
            }
        }

        #region Built-ins
        public static class BuiltIns
        {
            private static Lazy<DateTime> LazyTimestamp = new Lazy<DateTime>(() => System.DateTime.Now);

            public static DateTime Timestamp => LazyTimestamp.Value;

            public static string StartDate()
            {
                return Timestamp.ToString("yyyy.MM.dd");
            }

            public static string StartTime()
            {
                return Timestamp.ToString("HH.mm.ss");
            }

            public static string StartDateTime()
            {
                return Timestamp.ToString("yyyy.MM.dd_HH.mm.ss");
            }

            public static string StartTimeMsecs()
            {
                return Timestamp.ToString("HH.mm.ss.fff");
            }

            public static string StartDateTimeMsecs()
            {
                return Timestamp.ToString("yyyy.MM.dd_HH.mm.ss.fff");
            }

            public static string StartDateTimeYMDHM()
            {
                return Timestamp.ToString("yyyy.MM.dd_HH.mm");
            }

            public static string CurrentDate()
            {
                return System.DateTime.Now.ToString("yyyy.MM.dd");
            }

            public static string CurrentTime()
            {
                return System.DateTime.Now.ToString("HH.mm.ss");
            }

            public static string CurrentDateTime()
            {
                return System.DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss");
            }

            public static string CurrentTimeMsecs()
            {
                return System.DateTime.Now.ToString("HH.mm.ss.fff");
            }

            public static string CurrentDateTimeMsecs()
            {
                return System.DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss.fff");
            }

            public static string UserProfile()
            {
                return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            public static string AppData()
            {
                return System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }

            public static string UserName()
            {
                var name = Environment.UserName;
                int idx = name.LastIndexOf('\\');
                if (idx >= 0)
                {
                    name = name.Substring(idx + 1);
                }
                return name;
            }

            public static string Temp()
            {
                return System.IO.Path.GetTempPath();
            }

            public static string RandomFileName()
            {
                return System.IO.Path.GetRandomFileName();
            }
        }
        #endregion
    }
}
