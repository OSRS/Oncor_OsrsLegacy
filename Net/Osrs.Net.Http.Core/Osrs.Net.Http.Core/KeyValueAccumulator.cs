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

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Osrs.Text;
using System;
using System.Collections.Generic;

namespace Osrs.Net.Http
{
    public struct KeyValueAccumulator
    {
        private Dictionary<string, StringValues> _accumulator;
        private Dictionary<string, List<string>> _expandingAccumulator;

        public void Append(string key, string value)
        {
            if (_accumulator == null)
            {
                _accumulator = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            }

            StringValues values;
            if (_accumulator.TryGetValue(key, out values))
            {
                if (values.Count == 0)
                {
                    // Marker entry for this key to indicate entry already in expanding list dictionary
                    _expandingAccumulator[key].Add(value);
                }
                else if (values.Count == 1)
                {
                    // Second value for this key
                    _accumulator[key] = new string[] { values[0], value };
                }
                else
                {
                    // Third value for this key
                    // Add zero count entry and move to data to expanding list dictionary
                    _accumulator[key] = default(StringValues);

                    if (_expandingAccumulator == null)
                    {
                        _expandingAccumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    }

                    // Already 3 entries so use starting allocated as 8; then use List's expansion mechanism for more
                    var list = new List<string>(8);
                    var array = values.ToArray();

                    list.Add(array[0]);
                    list.Add(array[1]);
                    list.Add(value);

                    _expandingAccumulator[key] = list;
                }
            }
            else
            {
                // First value for this key
                _accumulator[key] = new StringValues(value);
            }

            ValueCount++;
        }

        public bool HasValues => ValueCount > 0;

        public int KeyCount => _accumulator?.Count ?? 0;

        public int ValueCount { get; private set; }

        public Dictionary<string, StringValues> GetResults()
        {
            if (_expandingAccumulator != null)
            {
                // Coalesce count 3+ multi-value entries into _accumulator dictionary
                foreach (var entry in _expandingAccumulator)
                {
                    _accumulator[entry.Key] = new StringValues(entry.Value.ToArray());
                }
            }

            return _accumulator ?? new Dictionary<string, StringValues>(0, StringComparer.OrdinalIgnoreCase);
        }
    }
}
