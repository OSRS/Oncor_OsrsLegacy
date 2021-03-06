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
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Osrs.Net.Http.Headers
{
    public class EntityTagHeaderValue
    {
        // Note that the ETag header does not allow a * but we're not that strict: We allow both '*' and ETag values in a single value.
        // We can't guarantee that a single parsed value will be used directly in an ETag header.
        private static readonly HttpHeaderParser<EntityTagHeaderValue> SingleValueParser
            = new GenericHeaderParser<EntityTagHeaderValue>(false, GetEntityTagLength);
        // Note that if multiple ETag values are allowed (e.g. 'If-Match', 'If-None-Match'), according to the RFC
        // the value must either be '*' or a list of ETag values. It's not allowed to have both '*' and a list of
        // ETag values. We're not that strict: We allow both '*' and ETag values in a list. If the server sends such
        // an invalid list, we want to be able to represent it using the corresponding header property.
        private static readonly HttpHeaderParser<EntityTagHeaderValue> MultipleValueParser
            = new GenericHeaderParser<EntityTagHeaderValue>(true, GetEntityTagLength);

        private static EntityTagHeaderValue AnyType;

        private string _tag;
        private bool _isWeak;

        private EntityTagHeaderValue()
        {
            // Used by the parser to create a new instance of this type.
        }

        public EntityTagHeaderValue(string tag)
            : this(tag, false)
        {
        }

        public EntityTagHeaderValue(string tag, bool isWeak)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentException("An empty string is not allowed.", nameof(tag));
            }

            int length = 0;
            if (!isWeak && string.Equals(tag, "*", StringComparison.Ordinal))
            {
                // * is valid, but W/* isn't.
                _tag = tag;
            }
            else if ((HttpRuleParser.GetQuotedStringLength(tag, 0, out length) != HttpParseResult.Parsed) ||
                (length != tag.Length))
            {
                // Note that we don't allow 'W/' prefixes for weak ETags in the 'tag' parameter. If the user wants to
                // add a weak ETag, he can set 'isWeak' to true.
                throw new FormatException("Invalid ETag name");
            }

            _tag = tag;
            _isWeak = isWeak;
        }

        public static EntityTagHeaderValue Any
        {
            get
            {
                if (AnyType == null)
                {
                    AnyType = new EntityTagHeaderValue();
                    AnyType._tag = "*";
                    AnyType._isWeak = false;
                }
                return AnyType;
            }
        }

        public string Tag
        {
            get { return _tag; }
        }

        public bool IsWeak
        {
            get { return _isWeak; }
        }

        public override string ToString()
        {
            if (_isWeak)
            {
                return "W/" + _tag;
            }
            return _tag;
        }

        /// <summary>
        /// Check against another <see cref="EntityTagHeaderValue"/> for equality.
        /// This equality check should not be used to determine if two values match under the RFC specifications (https://tools.ietf.org/html/rfc7232#section-2.3.2).
        /// </summary>
        /// <param name="obj">The other value to check against for equality.</param>
        /// <returns>
        /// <c>true</c> if the strength and tag of the two values match,
        /// <c>false</c> if the other value is null, is not an <see cref="EntityTagHeaderValue"/>, or if there is a mismatch of strength or tag between the two values.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as EntityTagHeaderValue;

            if (other == null)
            {
                return false;
            }

            // Since the tag is a quoted-string we treat it case-sensitive.
            return _isWeak == other._isWeak && string.Equals(_tag, other._tag, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            // Since the tag is a quoted-string we treat it case-sensitive.
            return _tag.GetHashCode() ^ _isWeak.GetHashCode();
        }

        /// <summary>
        /// Compares against another <see cref="EntityTagHeaderValue"/> to see if they match under the RFC specifications (https://tools.ietf.org/html/rfc7232#section-2.3.2).
        /// </summary>
        /// <param name="other">The other <see cref="EntityTagHeaderValue"/> to compare against.</param>
        /// <param name="useStrongComparison"><c>true</c> to use a strong comparison, <c>false</c> to use a weak comparison</param>
        /// <returns>
        /// <c>true</c> if the <see cref="EntityTagHeaderValue"/> match for the given comparison type,
        /// <c>false</c> if the other value is null or the comparison failed.
        /// </returns>
        public bool Compare(EntityTagHeaderValue other, bool useStrongComparison)
        {
            if (other == null)
            {
                return false;
            }

            if (useStrongComparison)
            {
                return !IsWeak && !other.IsWeak && string.Equals(Tag, other.Tag, StringComparison.Ordinal);
            }
            else
            {
                return string.Equals(Tag, other.Tag, StringComparison.Ordinal);
            }
        }

        public static EntityTagHeaderValue Parse(string input)
        {
            var index = 0;
            return SingleValueParser.ParseValue(input, ref index);
        }

        public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
        {
            var index = 0;
            return SingleValueParser.TryParseValue(input, ref index, out parsedValue);
        }

        public static IList<EntityTagHeaderValue> ParseList(IList<string> inputs)
        {
            return MultipleValueParser.ParseValues(inputs);
        }

        public static IList<EntityTagHeaderValue> ParseStrictList(IList<string> inputs)
        {
            return MultipleValueParser.ParseStrictValues(inputs);
        }

        public static bool TryParseList(IList<string> inputs, out IList<EntityTagHeaderValue> parsedValues)
        {
            return MultipleValueParser.TryParseValues(inputs, out parsedValues);
        }

        public static bool TryParseStrictList(IList<string> inputs, out IList<EntityTagHeaderValue> parsedValues)
        {
            return MultipleValueParser.TryParseStrictValues(inputs, out parsedValues);
        }

        internal static int GetEntityTagLength(string input, int startIndex, out EntityTagHeaderValue parsedValue)
        {
            Contract.Requires(startIndex >= 0);

            parsedValue = null;

            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }

            // Caller must remove leading whitespaces. If not, we'll return 0.
            var isWeak = false;
            var current = startIndex;

            var firstChar = input[startIndex];
            if (firstChar == '*')
            {
                // We have '*' value, indicating "any" ETag.
                parsedValue = Any;
                current++;
            }
            else
            {
                // The RFC defines 'W/' as prefix, but we'll be flexible and also accept lower-case 'w'.
                if ((firstChar == 'W') || (firstChar == 'w'))
                {
                    current++;
                    // We need at least 3 more chars: the '/' character followed by two quotes.
                    if ((current + 2 >= input.Length) || (input[current] != '/'))
                    {
                        return 0;
                    }
                    isWeak = true;
                    current++; // we have a weak-entity tag.
                    current = current + HttpRuleParser.GetWhitespaceLength(input, current);
                }

                var tagStartIndex = current;
                var tagLength = 0;
                if (HttpRuleParser.GetQuotedStringLength(input, current, out tagLength) != HttpParseResult.Parsed)
                {
                    return 0;
                }

                parsedValue = new EntityTagHeaderValue();
                if (tagLength == input.Length)
                {
                    // Most of the time we'll have strong ETags without leading/trailing whitespaces.
                    Contract.Assert(startIndex == 0);
                    Contract.Assert(!isWeak);
                    parsedValue._tag = input;
                    parsedValue._isWeak = false;
                }
                else
                {
                    parsedValue._tag = input.Substring(tagStartIndex, tagLength);
                    parsedValue._isWeak = isWeak;
                }

                current = current + tagLength;
            }
            current = current + HttpRuleParser.GetWhitespaceLength(input, current);

            return current - startIndex;
        }
    }
}
