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
using System;

namespace Osrs.Collections.Generic
{
    public interface IKeyValuePair<K, V>
        where K : IEquatable<K>
    {
        K Key
        {
            get;
        }

        V Value
        {
            get;
        }
    }

    public sealed class KeyValuePair<K, V> : IKeyValuePair<K, V>
        where K : IEquatable<K>
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

        public KeyValuePair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public static implicit operator KeyValuePair<K, V>(System.Collections.Generic.KeyValuePair<K, V> other)
        {
            return new KeyValuePair<K, V>(other.Key, other.Value);
        }

        public static explicit operator System.Collections.Generic.KeyValuePair<K, V>(KeyValuePair<K, V> other)
        {
            if (other != null)
                return new System.Collections.Generic.KeyValuePair<K, V>(other.key, other.value);
            return new System.Collections.Generic.KeyValuePair<K, V>(default(K), default(V));
        }
    }
}
