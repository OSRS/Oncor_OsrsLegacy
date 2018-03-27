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

using System;

namespace Osrs.Net.Http.Headers
{
    internal sealed class GenericHeaderParser<T> : BaseHeaderParser<T>
    {
        internal delegate int GetParsedValueLengthDelegate(string value, int startIndex, out T parsedValue);

        private GetParsedValueLengthDelegate _getParsedValueLength;

        internal GenericHeaderParser(bool supportsMultipleValues, GetParsedValueLengthDelegate getParsedValueLength)
            : base(supportsMultipleValues)
        {
            if (getParsedValueLength == null)
            {
                throw new ArgumentNullException(nameof(getParsedValueLength));
            }

            _getParsedValueLength = getParsedValueLength;
        }

        protected override int GetParsedValueLength(string value, int startIndex, out T parsedValue)
        {
            return _getParsedValueLength(value, startIndex, out parsedValue);
        }
    }
}
