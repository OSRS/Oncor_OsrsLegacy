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

using Osrs.Net.Http.Headers;
using Osrs.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Net.Http
{
    internal static class ParsingHelpers
    {
        public static StringValues GetHeader(IHeaderDictionary headers, string key)
        {
            StringValues value;
            return headers.TryGetValue(key, out value) ? value : StringValues.Empty;
        }

        public static StringValues GetHeaderSplit(IHeaderDictionary headers, string key)
        {
            var values = GetHeaderUnmodified(headers, key);
            return new StringValues(GetHeaderSplitImplementation(values).ToArray());
        }

        private static IEnumerable<string> GetHeaderSplitImplementation(StringValues values)
        {
            foreach (var segment in new HeaderSegmentCollection(values))
            {
                if (segment.Data.HasValue)
                {
                    yield return DeQuote(segment.Data.Value);
                }
            }
        }

        public static StringValues GetHeaderUnmodified(IHeaderDictionary headers, string key)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            StringValues values;
            return headers.TryGetValue(key, out values) ? values : StringValues.Empty;
        }

        public static void SetHeaderJoined(IHeaderDictionary headers, string key, StringValues value)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (StringValues.IsNullOrEmpty(value))
            {
                headers.Remove(key);
            }
            else
            {
                headers[key] = string.Join(",", value.Select((s) => QuoteIfNeeded(s)));
            }
        }

        // Quote items that contain comas and are not already quoted.
        private static string QuoteIfNeeded(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                value.Contains(',') &&
                (value[0] != '"' || value[value.Length - 1] != '"'))
            {
                return $"\"{value}\"";
            }
            return value;
        }

        private static string DeQuote(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"'))
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }

        public static void SetHeaderUnmodified(IHeaderDictionary headers, string key, StringValues? values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!values.HasValue || StringValues.IsNullOrEmpty(values.Value))
            {
                headers.Remove(key);
            }
            else
            {
                headers[key] = values.Value;
            }
        }

        public static void AppendHeaderJoined(IHeaderDictionary headers, string key, params string[] values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (values == null || values.Length == 0)
            {
                return;
            }

            string existing = GetHeader(headers, key);
            if (existing == null)
            {
                SetHeaderJoined(headers, key, values);
            }
            else
            {
                headers[key] = existing + "," + string.Join(",", values.Select(value => QuoteIfNeeded(value)));
            }
        }

        public static void AppendHeaderUnmodified(IHeaderDictionary headers, string key, StringValues values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (values.Count == 0)
            {
                return;
            }

            var existing = GetHeaderUnmodified(headers, key);
            SetHeaderUnmodified(headers, key, StringValues.Concat(existing, values));
        }
    }
}
