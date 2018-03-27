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
using Osrs.Runtime.ObjectPooling;
using Osrs.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Net.Http.Cookies
{
    /// <summary>
    /// A wrapper for the response Set-Cookie header.
    /// </summary>
    public class ResponseCookies : IResponseCookies
    {
        private readonly ObjectPool<StringBuilder> _builderPool;

        /// <summary>
        /// Create a new wrapper.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> for the response.</param>
        /// <param name="builderPool">The <see cref="ObjectPool{T}"/>, if available.</param>
        public ResponseCookies(IHeaderDictionary headers, ObjectPool<StringBuilder> builderPool)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Headers = headers;
            _builderPool = builderPool;
        }

        private IHeaderDictionary Headers { get; set; }

        /// <inheritdoc />
        public void Append(string key, string value)
        {
            var setCookieHeaderValue = new SetCookieHeaderValue(
                Uri.EscapeDataString(key),
                Uri.EscapeDataString(value))
            {
                Path = "/"
            };

            string cookieValue;
            if (_builderPool == null)
            {
                cookieValue = setCookieHeaderValue.ToString();
            }
            else
            {
                var stringBuilder = _builderPool.Get();
                try
                {
                    setCookieHeaderValue.AppendToStringBuilder(stringBuilder);
                    cookieValue = stringBuilder.ToString();
                }
                finally
                {
                    _builderPool.Return(stringBuilder);
                }
            }

            Headers[HeaderNames.SetCookie] = StringValues.Concat(Headers[HeaderNames.SetCookie], cookieValue);
        }

        /// <inheritdoc />
        public void Append(string key, string value, CookieOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var setCookieHeaderValue = new SetCookieHeaderValue(
                Uri.EscapeDataString(key),
                Uri.EscapeDataString(value))
            {
                Domain = options.Domain,
                Path = options.Path,
                Expires = options.Expires,
                Secure = options.Secure,
                HttpOnly = options.HttpOnly,
            };

            string cookieValue;
            if (_builderPool == null)
            {
                cookieValue = setCookieHeaderValue.ToString();
            }
            else
            {
                var stringBuilder = _builderPool.Get();
                try
                {
                    setCookieHeaderValue.AppendToStringBuilder(stringBuilder);
                    cookieValue = stringBuilder.ToString();
                }
                finally
                {
                    _builderPool.Return(stringBuilder);
                }
            }

            Headers[HeaderNames.SetCookie] = StringValues.Concat(Headers[HeaderNames.SetCookie], cookieValue);
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            Delete(key, new CookieOptions() { Path = "/" });
        }

        /// <inheritdoc />
        public void Delete(string key, CookieOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var encodedKeyPlusEquals = Uri.EscapeDataString(key) + "=";
            bool domainHasValue = !string.IsNullOrEmpty(options.Domain);
            bool pathHasValue = !string.IsNullOrEmpty(options.Path);

            Func<string, string, CookieOptions, bool> rejectPredicate;
            if (domainHasValue)
            {
                rejectPredicate = (value, encKeyPlusEquals, opts) =>
                    value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase) &&
                        value.IndexOf($"domain={opts.Domain}", StringComparison.OrdinalIgnoreCase) != -1;
            }
            else if (pathHasValue)
            {
                rejectPredicate = (value, encKeyPlusEquals, opts) =>
                    value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase) &&
                        value.IndexOf($"path={opts.Path}", StringComparison.OrdinalIgnoreCase) != -1;
            }
            else
            {
                rejectPredicate = (value, encKeyPlusEquals, opts) => value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase);
            }

            var existingValues = Headers[HeaderNames.SetCookie];
            if (!StringValues.IsNullOrEmpty(existingValues))
            {
                var values = existingValues.ToArray();
                var newValues = new List<string>();

                for (var i = 0; i < values.Length; i++)
                {
                    if (!rejectPredicate(values[i], encodedKeyPlusEquals, options))
                    {
                        newValues.Add(values[i]);
                    }
                }

                Headers[HeaderNames.SetCookie] = new StringValues(newValues.ToArray());
            }

            Append(key, string.Empty, new CookieOptions
            {
                Path = options.Path,
                Domain = options.Domain,
                Expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });
        }
    }
}
