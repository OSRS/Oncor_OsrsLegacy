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

namespace Osrs.Data
{
    public abstract class ValuePair : IEquatable<ValuePair>
    {
        public abstract object KeyObject
        {
            get;
        }

        public abstract object ValueObject
        {
            get;
        }

        public abstract bool Equals(ValuePair other);
    }

    public class ValuePair<T> : ValuePair<T, T>
    {
        public ValuePair(T key, T value)
            : base(key, value)
        { }
    }

    public class ValuePair<K, V> : ValuePair, IEquatable<ValuePair<K, V>>
    {
        private readonly K key;
        public K Key
        {
            get { return this.key; }
        }

        private readonly V value;
        public V Value
        {
            get { return this.value; }
        }

        public ValuePair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            if (this.key != null)
                sb.Append("Key", this.key.ToString());
            else
                sb.Append("Key", (string)null);

            if (this.value != null)
                sb.Append("Value", this.value.ToString());
            else
                sb.Append("Value", (string)null);

            return sb.ToString();
        }

        public virtual bool Equals(ValuePair<K, V> other)
        {
            if (other == null)
                return false;
            if (this.key != null)
            {
                if (other.key == null)
                    return false;
            }
            else if (other.key != null)
                return false;

            if (this.value != null)
            {
                if (other.value == null)
                    return false;
            }
            else if (other.value != null)
                return false;

            return this.key.Equals(other.key) && this.value.Equals(other.value);
        }

        public override bool Equals(ValuePair other)
        {
            return this.Equals(other as ValuePair<K, V>);
        }

        public override object KeyObject
        {
            get { return this.key; }
        }

        public override object ValueObject
        {
            get { return this.value; }
        }
    }

    public class OrderableValuePair<K,V> : ValuePair<K,V>, IComparable<OrderableValuePair<K,V>>, IEquatable<OrderableValuePair<K,V>>
        where K : IComparable<K>, IEquatable<K>
    {
        public OrderableValuePair(K key, V value) : base(key, value)
        { }

        public int CompareTo(OrderableValuePair<K, V> other)
        {
            return this.Key.CompareTo(other.Key);
        }

        public bool Equals(OrderableValuePair<K, V> other)
        {
            return this.Key.Equals(other.Key);
        }
    }
}
