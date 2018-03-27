//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using Osrs.Text;
using System;

namespace Osrs
{
    public sealed class DateTimeInstant : IEquatable<DateTimeInstant>, IComparable<DateTimeInstant>
    {
        private const long ticksPerMillisecond = 10000;

        private static readonly ulong maxDateTimeMillis = (ulong)(DateTime.MaxValue.Ticks / ticksPerMillisecond);
        private static readonly DateTime anchor = new DateTime(1600, 01, 01, 00, 00, 00, DateTimeKind.Utc);
        private static readonly long anchorTicks = anchor.Ticks;

        private readonly ulong millis;
        public ulong Value
        {
            get { return this.millis; }
        }

        public static DateTimeInstant Now
        {
            get { return new DateTimeInstant(DateTime.UtcNow, DateTimeKind.Utc); }
        }

        public DateTimeInstant(DateTimeInstant other)
        {
            if (other != null)
                this.millis = other.millis;
        }

        public DateTimeInstant(ulong value)
        {
            this.millis = value;
        }

        public DateTimeInstant(DateTime item) : this(item, DateTimeKind.Utc)
        { }

        public DateTimeInstant(DateTime item, DateTimeKind impliedKind)
        {
            if (item.Kind == DateTimeKind.Utc)
                this.millis = GetMillis(item.Ticks);
            else if (item.Kind == DateTimeKind.Local)
                this.millis = GetMillis(item.ToUniversalTime().Ticks);
            else
            {
                if (impliedKind == DateTimeKind.Utc || impliedKind == DateTimeKind.Unspecified)
                    this.millis = GetMillis(item.Ticks);
                else
                    this.millis = GetMillis(new DateTime(item.Ticks, impliedKind).ToUniversalTime().Ticks);
            }
        }

        public static explicit operator DateTime?(DateTimeInstant item)
        {
            if (item != null)
            {
                if (item.millis > maxDateTimeMillis)
                    return null;
                return new DateTime((anchorTicks + ((long)item.millis * ticksPerMillisecond)), DateTimeKind.Utc);
            }
            return null;
        }

        public static implicit operator DateTimeInstant(DateTime item)
        {
            return new DateTimeInstant(item);
        }

        private static ulong GetMillis(long ticks)
        {
            double deltaMillis = new TimeSpan(ticks - anchorTicks).TotalMilliseconds;
            if (deltaMillis < 0)
                return 0; //min date
            return (ulong)deltaMillis;
        }

        public int CompareTo(DateTimeInstant other)
        {
            if (other != null)
                return this.millis.CompareTo(other.millis);
            return 1;
        }

        public bool Equals(DateTimeInstant other)
        {
            if (other != null)
                return this.millis == other.millis;
            return false;
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append(this.millis);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (Object.Equals(null, obj))
                return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.millis.GetHashCode();
        }

        public static bool operator !=(DateTimeInstant a, DateTimeInstant b)
        {
            if (!Object.ReferenceEquals(null, a))
                return !a.Equals(b);
            return !Object.ReferenceEquals(null, b);
        }

        public static bool operator == (DateTimeInstant a, DateTimeInstant b)
        {
            if (!Object.ReferenceEquals(null, a))
                return a.Equals(b);
            return !Object.ReferenceEquals(null, b);
        }
    }
}
