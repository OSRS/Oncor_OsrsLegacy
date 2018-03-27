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
using System.IO;

namespace Osrs.Net.Http.Handlers
{
    public sealed class FileExtensions
    {
        private bool hasWildcard = false;
        private readonly HashSet<string> extensions = new HashSet<string>();

        public int Count
        {
            get { return this.extensions.Count; }
        }

        public bool MatchesFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            if (this.hasWildcard)
                return true; //we match everything
            if (this.extensions.Contains(Path.GetExtension(filename).ToLowerInvariant()))
                return true;

            string fn = Path.GetFileName(filename).ToLowerInvariant(); //this may have more than one *.x.x
            foreach (string curF in this.extensions)
            {
                if (fn.EndsWith(curF, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public bool Contains(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return false;
            return this.extensions.Contains(ext.ToLowerInvariant());
        }

        public void Add(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return;
            if (ext == "*")
            {
                this.hasWildcard = true;
                return;
            }
            if (ext.Contains("*"))
                return; //no wildcards here
            this.extensions.Add(ext.ToLowerInvariant());
        }

        public void Remove(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext == "*") //wildcard
                    this.hasWildcard = false;
                else if (ext.Contains("*"))
                    return; //can't deal with a wildcard in the ext.
                else if (this.extensions.Contains(ext.ToLowerInvariant()))
                    this.extensions.Remove(ext.ToLowerInvariant());
            }
        }
    }
}
