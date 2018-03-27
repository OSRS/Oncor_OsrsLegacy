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
using Osrs.Data;
using System.Collections.Generic;

namespace Osrs.Collections
{
    public sealed class NamedEntityBag<T> : IEnumerable<T> where T : INamed
    {
        private Dictionary<string, T> items = new Dictionary<string, T>();

        public T this[string name]
        {
            get
            {
                if (this.items.ContainsKey(name))
                    return this.items[name];
                return default(T);
            }
        }

        public NamedEntityBag()
        {
        }

        public bool Exists(string name)
        {
            return this.items.ContainsKey(name);
        }

        public bool Exists(T item)
        {
            if (item == null)
                return false;
            return this.items.ContainsKey(item.Name);
        }

        public void Add(T item)
        {
            if (item == null)
                return;
            if (this.items.ContainsKey(item.Name))
                return;
            this.items.Add(item.Name, item);
        }

        public void Remove(T item)
        {
            if (item == null)
                return;
            if (this.items.ContainsKey(item.Name))
                this.items.Remove(item.Name);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.items.Values.GetEnumerator();
        }
    }
}
