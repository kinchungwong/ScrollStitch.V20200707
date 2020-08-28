using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Memory.Diagnostics
{
    using ScrollStitch.V20200707.Text;

    /// <summary>
    /// <para>
    /// Provides the memory usage statistics of the array pool.
    /// <br/>
    /// To get an instance of this class, call <see cref="ExactLengthArrayPool{T}.Unsafe_GetMemoryUsage()"/>.
    /// </para>
    /// 
    /// <para>
    /// Important: The gathering of usage statistics may interfere with the operations of the array pool. 
    /// Do not call this method while the array pool is in active use, especially in a multithreaded 
    /// execution environment.
    /// </para>
    /// </summary>
    /// 
    public class ArrayPoolMemoryUsage
    {
        internal struct Record
        {
            public int ArrayLength;
            public int InstanceCount;
            public long TotalElements;
            public long TotalBytes;
        }

        private string _className;
        private List<Record> _records = new List<Record>();
        private RecordFormatter _formatter;

        /// <summary>
        /// <para>
        /// Do not call.<br/>
        /// To get an instance of this class, call <see cref="ExactLengthArrayPool{T}.Unsafe_GetMemoryUsage()"/>.
        /// </para>
        /// </summary>
        /// 
        internal static ArrayPoolMemoryUsage Create<T>(
            ExactLengthArrayPool<T> arrayPool,
            KeyValuePair<int, ConcurrentBag<WeakReference<T[]>>>[] allBags)
        {
            string className = $"{nameof(ExactLengthArrayPool<T>)}<{typeof(T).Name}>";
            ArrayPoolMemoryUsage usage = new ArrayPoolMemoryUsage(className);
            foreach (var kvp in allBags)
            {
                WeakReference<T[]>[] weakRefArr = kvp.Value.ToArray();
                usage._Add(weakRefArr);
            }
            return usage;
        }

        /// <summary>
        /// <para>
        /// Do not call.<br/>
        /// To get an instance of this class, call <see cref="ExactLengthArrayPool{T}.Unsafe_GetMemoryUsage()"/>.
        /// </para>
        /// </summary>
        /// 
        /// <param name="className">
        /// The formatted class name for <see cref="ExactLengthArrayPool{T}"/> which is provided by 
        /// <see cref="Create{T}"/>.
        /// </param>
        /// 
        private ArrayPoolMemoryUsage(string className)
        {
            _className = className;
            string numToStringOrAsterisk(long value)
            {
                return (value > 0) ? value.ToString("N0") : "*";
            }
            _formatter = RecordFormatter.Create(
                _records,
                (s) =>
                s.AddField("ArrayLength", (r) => numToStringOrAsterisk(r.ArrayLength))
                .AddField("InstanceCount", (r) => numToStringOrAsterisk(r.InstanceCount))
                .AddField("TotalElements", (r) => numToStringOrAsterisk(r.TotalElements))
                .AddField("TotalBytes", (r) => numToStringOrAsterisk(r.TotalBytes)));
            _formatter.Indent = 4;
            _formatter.ColumnSpacing = 4;
            _formatter.ShowRowNumber = false;
        }

        public void Generate(IMultiLineTextOutput mlto)
        {
            mlto.AppendLine($"{_className} statistics");
            _formatter.Generate(mlto);
        }

        public void Generate(StringBuilder sb)
        {
            var mlto = new MultiLineTextOutput();
            Generate(mlto);
            mlto.ToStringBuilder(sb);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Generate(sb);
            return sb.ToString();
        }

        private void _Add<T>(WeakReference<T[]>[] weakArr)
        {
            int weakCount = weakArr.Length;
            int arrayLength = 0;
            int instanceCount = 0;
            long totalElements = 0;
            long totalBytes = 0;
            bool canComputeBytes = true;
            void UpdateTotalBytes(T[] arr)
            {
                if (!canComputeBytes)
                {
                    return;
                }
                try
                {
                    totalBytes += Buffer.ByteLength(arr);
                }
                catch
                {
                    // Buffer.ByteLength() is invalid for C# structs.
                    canComputeBytes = false;
                }
            }
            for (int weakIndex = 0; weakIndex < weakCount; ++weakIndex)
            {
                if (!weakArr[weakIndex].TryGetTarget(out T[] target))
                {
                    continue;
                }
                if (target is null ||
                    target.Length == 0)
                {
                    continue;
                }
                ++instanceCount;
                totalElements += target.Length;
                arrayLength = target.Length;
                UpdateTotalBytes(target);
            }
            if (!canComputeBytes)
            {
                totalBytes = -1;
            }
            if (arrayLength > 0 &&
                instanceCount > 0 &&
                totalElements > 0)
            {
                _records.Add(new Record()
                {
                    ArrayLength = arrayLength,
                    InstanceCount = instanceCount,
                    TotalElements = totalElements,
                    TotalBytes = totalBytes
                });
            }
        }
    }
}
