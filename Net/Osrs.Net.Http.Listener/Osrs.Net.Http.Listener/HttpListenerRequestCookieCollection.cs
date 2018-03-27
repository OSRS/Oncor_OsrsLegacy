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

using System.Collections;
using System.Collections.Generic;

namespace Osrs.Net.Http.Listener
{
    public class HttpListenerRequestCookieCollection : Osrs.Net.Http.Cookies.IRequestCookieCollection
    {
        private System.Net.HttpListenerRequest inner;
        private Dictionary<string, string> cookies = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                if (this.cookies.ContainsKey(key))
                    return this.cookies[key];
                return string.Empty;
            }
        }

        public int Count
        {
            get
            {
                return this.cookies.Count;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.cookies.Keys;
            }
        }

        public bool ContainsKey(string key)
        {
            return this.cookies.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.cookies.GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            return this.cookies.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal HttpListenerRequestCookieCollection(System.Net.HttpListenerRequest ctx)
        {
            this.inner = ctx;
            foreach(KeyValuePair<string, System.Net.Cookie> cur in this.inner.Cookies)
            {
                this.cookies[cur.Key] = cur.Value.ToString();
            }
        }
    }
}
