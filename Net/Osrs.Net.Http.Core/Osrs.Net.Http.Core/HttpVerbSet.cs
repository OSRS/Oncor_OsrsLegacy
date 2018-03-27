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
using System.Collections.Generic;

namespace Osrs.Net.Http
{
    public class HttpVerbSet:IEnumerable<string>
    {
        public static HttpVerbSet StandardVerbs
        {
            get
            {
                HttpVerbSet tmp = new HttpVerbSet();
                tmp.allowedVerbs.Add(HttpVerbs.GET.ToString());
                tmp.allowedVerbs.Add(HttpVerbs.POST.ToString());
                tmp.allowedVerbs.Add(HttpVerbs.PUT.ToString());
                tmp.allowedVerbs.Add(HttpVerbs.DELETE.ToString());
                return tmp;
            }
        }

        private readonly HashSet<string> allowedVerbs = new HashSet<string>();

        public int Count
        {
            get { return this.allowedVerbs.Count; }
        }

        public bool Contains(HttpVerbs verb)
        {
            return this.Contains(verb.ToString());
        }

        public bool Contains(string verb)
        {
            foreach (string cur in this.allowedVerbs)
            {
                if (cur.Equals(verb, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public void Add(HttpVerbs verb)
        {
            this.allowedVerbs.Add(verb.ToString());
        }

        public void Add(string verb)
        {
            if (!string.IsNullOrEmpty(verb))
                this.allowedVerbs.Add(verb);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.allowedVerbs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
