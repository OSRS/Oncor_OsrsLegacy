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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class FormatWriter
    {
        public static string Format(JToken value)
        {
            if (value == null)
                return "null";
            return value.ToString(Formatting.None, (JsonConverter[])null);
        }
    }

    public sealed class JTable
    {
        private readonly object syncRoot = new object();
        private bool available = false;
        private int busy = 0;
        private readonly List<string> columns = new List<string>();
        private readonly List<List<JToken>> data = new List<List<JToken>>();

        public void SetColumns(IEnumerable<string> columnNames)
        {
            if (columnNames == null)
                return;
            Enumerable.ToArray<string>(columnNames);
        }

        public void SetColumns(string[] columnNames)
        {
            lock (this.syncRoot)
            {
                if (columnNames == null || columnNames.Length < 1 || this.columns.Count >= 1 && columnNames.Length != this.columns.Count)
                    return;
                this.available = false;
                if (this.columns.Count < 1)
                {
                    this.columns.AddRange((IEnumerable<string>)columnNames);
                }
                else
                {
                    for (int local_0 = 0; local_0 < this.columns.Count; ++local_0)
                        this.columns[local_0] = columnNames[local_0];
                }
                this.available = true;
            }
        }

        public void Reset()
        {
            lock (this.syncRoot)
            {
                this.available = false;
                while (this.busy > 0)
                    this.data.Clear(); //Thread.Sleep(5); //just for something to do
                this.columns.Clear();
                this.data.Clear();
                this.available = true;
            }
        }

        public void AddRow(List<JToken> data)
        {
            if (!this.available)
                return;
            if (this.columns.Count < 1)
                throw new ArgumentException("no columns defined");
            if (data != null && this.available)
            {
                ++this.busy;
                List<JToken> list = new List<JToken>();
                list.AddRange((IEnumerable<JToken>)data);
                if (list.Count == this.columns.Count)
                    this.data.Add(list);
                --this.busy;
            }
        }
    }
}
