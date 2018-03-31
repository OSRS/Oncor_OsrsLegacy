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
using System.IO;

namespace Osrs.Runtime.Hosting.AppHosting
{
    public sealed class ServiceListFile : IEnumerable<string>
    {
        private readonly HashSet<string> items;

        private ServiceListFile(HashSet<string> items)
        {
            this.items = items;
        }

        public static ServiceListFile Open(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            if (!File.Exists(filename))
                return null;

            FileStream fs = File.OpenRead(filename);
            if (fs == null)
                return null;
            TextReader rdr = new StreamReader(fs);

            HashSet<string> items = new HashSet<string>(); //automatically reject duplicates
            string line = rdr.ReadLine();
            while (line != null)
            {
                if (!string.Empty.Equals(line))
                {
                    if (!Directory.Exists(line)) //might have provided the exe name
                    {
                        try
                        {
                            line = Path.GetDirectoryName(line);
                        }
                        catch
                        {
                            line = "";
                        }
                    }
                    if (Directory.Exists(line))
                    {
                        if (!File.Exists(Path.Combine(line, "AppHost.exe")))
                        {
                            try
                            {
                                File.Copy(Path.Combine(Path.GetDirectoryName(System.AppContext.BaseDirectory), "AppHost.exe"), Path.Combine(line, "AppHost.exe"));
                            }
                            catch
                            { }
                        }
                        if (File.Exists(Path.Combine(line, "AppHost.exe")))
                        {
                            items.Add(line);
                        }
                    }
                }
                line = rdr.ReadLine();
            }
            rdr.Dispose();
            if (items.Count>0)
                return new ServiceListFile(items);
            return null;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
