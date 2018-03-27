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
using System.Diagnostics.Contracts;
using System.Globalization;

namespace Osrs.Net.Http.Headers
{
    public static class HeaderUtilities
    {
        private const string QualityName = "q";
        internal const string BytesUnit = "bytes";

        public static bool IsNullOrEmpty(this IHeaderDictionary headers, string key)
        {
            if (headers.ContainsKey(key))
            {
                return StringValues.IsNullOrEmpty(headers[key]);
            }
            return true;
        }

        internal static void SetQuality(IList<NameValueHeaderValue> parameters, double? value)
        {
            Contract.Requires(parameters != null);

            var qualityParameter = NameValueHeaderValue.Find(parameters, QualityName);
            if (value.HasValue)
            {
                // Note that even if we check the value here, we can't prevent a user from adding an invalid quality
                // value using Parameters.Add(). Even if we would prevent the user from adding an invalid value
                // using Parameters.Add() he could always add invalid values using HttpHeaders.AddWithoutValidation().
                // So this check is really for convenience to show users that they're trying to add an invalid
                // value.
                if ((value < 0) || (value > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                var qualityString = ((double)value).ToString("0.0##", NumberFormatInfo.InvariantInfo);
                if (qualityParameter != null)
                {
                    qualityParameter.Value = qualityString;
                }
                else
                {
                    parameters.Add(new NameValueHeaderValue(QualityName, qualityString));
                }
            }
            else
            {
                // Remove quality parameter
                if (qualityParameter != null)
                {
                    parameters.Remove(qualityParameter);
                }
            }
        }

        internal static double? GetQuality(IList<NameValueHeaderValue> parameters)
        {
            Contract.Requires(parameters != null);

            var qualityParameter = NameValueHeaderValue.Find(parameters, QualityName);
            if (qualityParameter != null)
            {
                // Note that the RFC requires decimal '.' regardless of the culture. I.e. using ',' as decimal
                // separator is considered invalid (even if the current culture would allow it).
                double qualityValue;
                if (double.TryParse(qualityParameter.Value, NumberStyles.AllowDecimalPoint,
                    NumberFormatInfo.InvariantInfo, out qualityValue))
                {
                    return qualityValue;
                }
            }
            return null;
        }

        internal static void CheckValidToken(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("An empty string is not allowed.", parameterName);
            }

            if (HttpRuleParser.GetTokenLength(value, 0) != value.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid token '{0}.", value));
            }
        }

        internal static void CheckValidQuotedString(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("An empty string is not allowed.", parameterName);
            }

            int length;
            if ((HttpRuleParser.GetQuotedStringLength(value, 0, out length) != HttpParseResult.Parsed) ||
                (length != value.Length)) // no trailing spaces allowed
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid quoted string '{0}'.", value));
            }
        }

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y)
        {
            return AreEqualCollections(x, y, null);
        }

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y, IEqualityComparer<T> comparer)
        {
            if (x == null)
            {
                return (y == null) || (y.Count == 0);
            }

            if (y == null)
            {
                return (x.Count == 0);
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            if (x.Count == 0)
            {
                return true;
            }

            // We have two unordered lists. So comparison is an O(n*m) operation which is expensive. Usually
            // headers have 1-2 parameters (if any), so this comparison shouldn't be too expensive.
            var alreadyFound = new bool[x.Count];
            var i = 0;
            foreach (var xItem in x)
            {
                Contract.Assert(xItem != null);

                i = 0;
                var found = false;
                foreach (var yItem in y)
                {
                    if (!alreadyFound[i])
                    {
                        if (((comparer == null) && xItem.Equals(yItem)) ||
                            ((comparer != null) && comparer.Equals(xItem, yItem)))
                        {
                            alreadyFound[i] = true;
                            found = true;
                            break;
                        }
                    }
                    i++;
                }

                if (!found)
                {
                    return false;
                }
            }

            // Since we never re-use a "found" value in 'y', we expecte 'alreadyFound' to have all fields set to 'true'.
            // Otherwise the two collections can't be equal and we should not get here.
            Contract.Assert(Contract.ForAll(alreadyFound, value => { return value; }),
                "Expected all values in 'alreadyFound' to be true since collections are considered equal.");

            return true;
        }

        internal static int GetNextNonEmptyOrWhitespaceIndex(
            string input,
            int startIndex,
            bool skipEmptyValues,
            out bool separatorFound)
        {
            Contract.Requires(input != null);
            Contract.Requires(startIndex <= input.Length); // it's OK if index == value.Length.

            separatorFound = false;
            var current = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);

            if ((current == input.Length) || (input[current] != ','))
            {
                return current;
            }

            // If we have a separator, skip the separator and all following whitespaces. If we support
            // empty values, continue until the current character is neither a separator nor a whitespace.
            separatorFound = true;
            current++; // skip delimiter.
            current = current + HttpRuleParser.GetWhitespaceLength(input, current);

            if (skipEmptyValues)
            {
                while ((current < input.Length) && (input[current] == ','))
                {
                    current++; // skip delimiter.
                    current = current + HttpRuleParser.GetWhitespaceLength(input, current);
                }
            }

            return current;
        }

        internal static bool TryParseInt32(string value, out int result)
        {
            return int.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result);
        }

        public static bool TryParseInt64(string value, out long result)
        {
            return long.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result);
        }

        public static string FormatInt64(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool TryParseDate(string input, out DateTimeOffset result)
        {
            return HttpRuleParser.TryStringToDate(input, out result);
        }

        public static string FormatDate(DateTimeOffset dateTime)
        {
            return HttpRuleParser.DateToString(dateTime);
        }

        public static string RemoveQuotes(string input)
        {
            if (!string.IsNullOrEmpty(input) && input.Length >= 2 && input[0] == '"' && input[input.Length - 1] == '"')
            {
                input = input.Substring(1, input.Length - 2);
            }
            return input;
        }

        internal static void ThrowIfReadOnly(bool isReadOnly)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("The object cannot be modified because it is read-only.");
            }
        }

        /// <summary>
        /// Add new values. Each item remains a separate array entry.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to use.</param>
        /// <param name="key">The header name.</param>
        /// <param name="value">The header value.</param>
        public static void Append(this IHeaderDictionary headers, string key, StringValues value)
        {
            ParsingHelpers.AppendHeaderUnmodified(headers, key, value);
        }

        /// <summary>
        /// Quotes any values containing comas, and then coma joins all of the values with any existing values.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to use.</param>
        /// <param name="key">The header name.</param>
        /// <param name="values">The header values.</param>
        public static void AppendCommaSeparatedValues(this IHeaderDictionary headers, string key, params string[] values)
        {
            ParsingHelpers.AppendHeaderJoined(headers, key, values);
        }

        /// <summary>
        /// Get the associated values from the collection separated into individual values.
        /// Quoted values will not be split, and the quotes will be removed.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to use.</param>
        /// <param name="key">The header name.</param>
        /// <returns>the associated values from the collection separated into individual values, or StringValues.Empty if the key is not present.</returns>
        public static string[] GetCommaSeparatedValues(this IHeaderDictionary headers, string key)
        {
            return ParsingHelpers.GetHeaderSplit(headers, key);
        }

        /// <summary>
        /// Quotes any values containing comas, and then coma joins all of the values.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to use.</param>
        /// <param name="key">The header name.</param>
        /// <param name="values">The header values.</param>
        public static void SetCommaSeparatedValues(this IHeaderDictionary headers, string key, params string[] values)
        {
            ParsingHelpers.SetHeaderJoined(headers, key, values);
        }
    }
}
