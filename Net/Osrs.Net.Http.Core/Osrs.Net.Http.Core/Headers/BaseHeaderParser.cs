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

namespace Osrs.Net.Http.Headers
{
    internal abstract class BaseHeaderParser<T> : HttpHeaderParser<T>
    {
        protected BaseHeaderParser(bool supportsMultipleValues)
            : base(supportsMultipleValues)
        {
        }

        protected abstract int GetParsedValueLength(string value, int startIndex, out T parsedValue);

        public sealed override bool TryParseValue(string value, ref int index, out T parsedValue)
        {
            parsedValue = default(T);

            // If multiple values are supported (i.e. list of values), then accept an empty string: The header may
            // be added multiple times to the request/response message. E.g.
            //  Accept: text/xml; q=1
            //  Accept:
            //  Accept: text/plain; q=0.2
            if (string.IsNullOrEmpty(value) || (index == value.Length))
            {
                return SupportsMultipleValues;
            }

            var separatorFound = false;
            var current = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, index, SupportsMultipleValues,
                out separatorFound);

            if (separatorFound && !SupportsMultipleValues)
            {
                return false; // leading separators not allowed if we don't support multiple values.
            }

            if (current == value.Length)
            {
                if (SupportsMultipleValues)
                {
                    index = current;
                }
                return SupportsMultipleValues;
            }

            T result;
            var length = GetParsedValueLength(value, current, out result);

            if (length == 0)
            {
                return false;
            }

            current = current + length;
            current = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, current, SupportsMultipleValues,
                out separatorFound);

            // If we support multiple values and we've not reached the end of the string, then we must have a separator.
            if ((separatorFound && !SupportsMultipleValues) || (!separatorFound && (current < value.Length)))
            {
                return false;
            }

            index = current;
            parsedValue = result;
            return true;
        }
    }
}
