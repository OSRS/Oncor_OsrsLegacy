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

using Osrs.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Osrs.Text;
using System.Collections;

namespace Osrs.Net.Http.Listener
{
    public sealed class ResponseHeaderWrapper : IHeaderDictionary
    {
        private readonly System.Collections.Specialized.NameValueCollection inner;

        internal ResponseHeaderWrapper(System.Net.HttpListenerContext ctx)
        {
            this.inner = ctx.Response.Headers;
        }

        public StringValues this[string key]
        {
            get
            {
                return this.inner[key];
            }

            set
            {
                this.inner[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.inner.AllKeys;
            }
        }

        public ICollection<StringValues> Values
        {
            get
            {
                List<StringValues> k = new List<StringValues>();
                foreach(string v in this.inner.Keys)
                {
                    k.Add(this.inner.GetValues(v));
                }
                return k.AsReadOnly();
            }
        }

        public void Add(KeyValuePair<string, StringValues> item)
        {
            this.inner.Add(item.Key, item.Value);
        }

        public void Add(string key, StringValues value)
        {
            this.inner.Add(key, value);
        }

        public void Clear()
        {
            this.inner.Clear();
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            return item.Value.Equals(this.inner[item.Key]);
        }

        public bool ContainsKey(string key)
        {
            string[] tmp = this.inner.GetValues(key);
            return tmp != null && tmp.Length > 0;
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return (IEnumerator < KeyValuePair < string, StringValues >> )this.inner.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            this.inner.Remove(item.Key);
            return true;
        }

        public bool Remove(string key)
        {
            this.inner.Remove(key);
            return true;
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            value = this.inner.GetValues(key);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
