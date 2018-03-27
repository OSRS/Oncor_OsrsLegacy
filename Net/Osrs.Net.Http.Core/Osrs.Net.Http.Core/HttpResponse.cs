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

using Osrs.Net.Http.Cookies;
using Osrs.Net.Http.Headers;
using System.IO;

namespace Osrs.Net.Http
{
    /// <summary>
    /// Represents the outgoing side of an individual HTTP request.
    /// </summary>
    public abstract class HttpResponse
    {
        /// <summary>
        /// Gets the <see cref="HttpContext"/> for this request.
        /// </summary>
        public abstract HttpContext HttpContext { get; }

        /// <summary>
        /// Gets or sets the HTTP response code.
        /// </summary>
        public abstract int StatusCode { get; set; }

        /// <summary>
        /// Gets the response headers.
        /// </summary>
        public abstract IHeaderDictionary Headers { get; }

        /// <summary>
        /// Gets the response body <see cref="Stream"/>.
        /// </summary>
        public abstract Stream Body { get; set; }

        /// <summary>
        /// Gets or sets the value for the <c>Content-Length</c> response header.
        /// </summary>
        public abstract long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the value for the <c>Content-Type</c> response header.
        /// </summary>
        public abstract string ContentType { get; set; }

        /// <summary>
        /// Gets an object that can be used to manage cookies for this response.
        /// </summary>
        public abstract IResponseCookies Cookies { get; }
    }
}
