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
    internal sealed class DirectoryMapper
    {
        internal static readonly char[] seps = new char[] { Path.DirectorySeparatorChar };
        internal readonly string BasePath;
        internal readonly Dir Dirs;

        internal string MapPath(string pathPart)
        {
            if (pathPart[0] == Path.DirectorySeparatorChar)
                pathPart = pathPart.Substring(1);//strip off leading '/'

            string[] parts = pathPart.Split(seps);
            return this.Dirs.MapPath(parts, 0);
        }

        internal DirectoryMapper(string basePath)
        {
            this.BasePath = basePath;
            this.Dirs = new Dir(this.BasePath);
        }
    }

    internal sealed class Dir
    {
        private readonly HashSet<string> files = new HashSet<string>();

        //key is the part, value is the Dir
        private readonly Dictionary<string, Dir> dirs = new Dictionary<string, Dir>();

        //path is the fully qualified, case sensitive path
        private readonly string path;

        internal string MapPath(string[] pathParts, int curIndex)
        {
            int next = curIndex + 1;
            string curPart = pathParts[curIndex];

            if (pathParts.Length == next) //this is the last part, so we're looking for a file
            {
                foreach (string curFile in this.files)
                {
                    if (curFile.Equals(curPart, StringComparison.OrdinalIgnoreCase)) //we matched the filename
                        return Path.Combine(this.path, curFile);
                }

                foreach (string curFile in Directory.EnumerateFiles(this.path))
                {
                    string fileName = Path.GetFileName(curFile);

                    if (fileName.Equals(curPart, StringComparison.OrdinalIgnoreCase)) //we matched the filename
                    {
                        this.files.Add(fileName);
                        return curFile;
                    }
                }
                return null; //no match to be found
            }

            //not the terminal entry, so we're looking for folders
            curPart = pathParts[curIndex];
            foreach (KeyValuePair<string, Dir> curDir in this.dirs)
            {
                if (curDir.Key == curPart)
                    return curDir.Value.MapPath(pathParts, next);
            }

            foreach (string curDir in Directory.EnumerateDirectories(this.path))
            {
                string lastPart = Path.GetFileName(curDir);
                if (curPart.Equals(lastPart, StringComparison.OrdinalIgnoreCase))
                {
                    Dir tmp;
                    if (this.dirs.ContainsKey(lastPart))
                    {
                        tmp = this.dirs[lastPart];
                    }
                    else
                    {
                        tmp = new Dir(curDir);
                        this.dirs[lastPart] = tmp;
                    }
                    return tmp.MapPath(pathParts, next);
                }
            }

            return null;
        }

        internal void Scavenge()
        {
            if (!Directory.Exists(this.path))
            {
                this.files.Clear();
                this.dirs.Clear();
                return;
            }

            List<string> removes = new List<string>();
            foreach (string curFil in this.files)
            {
                if (!File.Exists(curFil))
                    removes.Add(curFil);
            }
            foreach (string curFil in removes)
                this.files.Remove(curFil);
            removes.Clear();

            foreach (KeyValuePair<string, Dir> curDir in this.dirs)
            {
                if (!Directory.Exists(curDir.Value.path))
                {
                    removes.Add(curDir.Key);
                    curDir.Value.files.Clear();
                    curDir.Value.dirs.Clear();
                }
                else
                    curDir.Value.Scavenge();
            }
            foreach (string curDir in removes)
                this.dirs.Remove(curDir);
            removes.Clear();

            foreach (string curDir in Directory.EnumerateDirectories(this.path))
            {
                string lastPart = Path.GetFileName(curDir).ToLowerInvariant();
                if (!this.dirs.ContainsKey(lastPart))
                {
                    Dir tmp = new Dir(curDir);
                    this.dirs[lastPart] = tmp;
                    tmp.Scavenge();
                }
            }
            foreach (string curFil in Directory.EnumerateFiles(this.path))
            {
                string fileName = Path.GetFileName(curFil).ToLowerInvariant();
                this.files.Add(fileName);
            }
        }

        internal Dir(string path)
        {
            this.path = path;
        }
    }
}
