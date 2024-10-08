﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Data
{
    using HashCode;

    public struct Range
        : IEquatable<Range>
    {
        public static Range Nothing { get; } = new Range(0, 0);

        public int Start { get; }
        public int Stop { get; }
        public int Count => Stop - Start;
        public bool IsEmpty => (Stop <= Start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Range(int start, int stop)
        {
            Start = start;
            Stop = stop;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<int> func)
        {
            for (int k = Start; k < Stop; ++k)
            {
                func(k);
            }
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<Range>().Ingest(Start, Stop).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Range other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Range other)
        {
            return Start == other.Start && Stop == other.Stop;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Range r1, Range r2)
        {
            return r1.Equals(r2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Range r1, Range r2)
        {
            return !r1.Equals(r2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int value)
        {
            return (value >= Start && value < Stop);
        }

        public override string ToString()
        {
            return $"(Range start={Start} stop={Stop})";
        }
    }
}
