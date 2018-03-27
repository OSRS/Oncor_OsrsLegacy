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
using Osrs.Data.Validation;
using Osrs.Text;
using System;
using System.Diagnostics;

namespace Osrs.Numerics
{
    public class ValueRange<T> : IEquatable<ValueRange<T>>, IValidator<T>
        where T : IComparable<T>
    {
        private readonly bool minInclusive;
        public bool MinInclusive
        {
            [DebuggerStepThrough]
            get { return this.minInclusive; }
        }

        private readonly T min;
        public T Min
        {
            [DebuggerStepThrough]
            get { return this.min; }
        }

        private readonly bool maxInclusive;
        public bool MaxInclusive
        {
            [DebuggerStepThrough]
            get { return this.maxInclusive; }
        }

        private readonly T max;
        public T Max
        {
            [DebuggerStepThrough]
            get { return this.max; }
        }

        [DebuggerStepThrough]
        public ValueRange(T min, T max) : this(min, true, max, true)
        { }

        [DebuggerStepThrough]
        public ValueRange(T min, bool minInclusive, T max, bool maxInclusive)
        {
            int res = min.CompareTo(max);

            if (res > 0)
                throw new ArgumentException(); //max can't be smaller than min
            if (res == 0) //degenerate case - a singularity
            {
                if (!(minInclusive && maxInclusive))
                    throw new ArgumentException(); //they're equal, but not both inclusive
            }

            this.min = min;
            this.max = max;
            this.minInclusive = minInclusive;
            this.maxInclusive = maxInclusive;
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            if (this.min != null)
                sb.Append("Min", this.min.ToString());
            else
                sb.Append("Min", (string)null);

            if (this.max != null)
                sb.Append("Max", this.max.ToString());
            else
                sb.Append("Max", (string)null);

            return sb.ToString();
        }

        [DebuggerStepThrough]
        public virtual bool Equals(ValueRange<T> other)
        {
            if (other == null)
                return false;
            if (this.min != null)
            {
                if (other.min == null)
                    return false;
            }
            else if (other.min != null)
                return false;

            if (this.max != null)
            {
                if (other.max == null)
                    return false;
            }
            else if (other.max != null)
                return false;

            return this.min.Equals(other.min) && this.max.Equals(other.max);
        }

        [DebuggerStepThrough]
        public bool IsValid(T item)
        {
            if (item != null)
            {
                int cur = this.min.CompareTo(item);
                if (cur > 0)
                    return false;
                else if (cur < 0 || this.minInclusive) //handles == case through inclusivity
                {
                    cur = this.max.CompareTo(item);
                    if (cur < 0)
                        return false;
                    else if (cur > 0 || this.maxInclusive) //handles == case through inclusivity
                        return true;
                }
            }
            return false;
        }

        [DebuggerStepThrough]
        public bool IsValidObject(object item)
        {
            return this.IsValid(item.As<T>());
        }
    }
}
