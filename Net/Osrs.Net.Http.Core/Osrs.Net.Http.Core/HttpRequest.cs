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
    /// Represents the incoming side of an individual HTTP request.
    /// </summary>
    public abstract class HttpRequest
    {
        /// <summary>
        /// Gets the <see cref="HttpContext"/> this request;
        /// </summary>
        public abstract HttpContext HttpContext { get; }

        /// <summary>
        /// Gets or set the HTTP method.
        /// </summary>
        /// <returns>The HTTP method.</returns>
        public abstract string Method { get; set; }

        /// <summary>
        /// Gets or set the HTTP request scheme.
        /// </summary>
        /// <returns>The HTTP request scheme.</returns>
        public abstract string Scheme { get; set; }

        /// <summary>
        /// Returns true if the RequestScheme is https.
        /// </summary>
        /// <returns>true if this request is using https; otherwise, false.</returns>
        public abstract bool IsHttps { get; set; }

        /// <summary>
        /// Gets or set the Host header. May include the port.
        /// </summary>
        /// <return>The Host header.</return>
        public abstract HostString Host { get; set; }

        /// <summary>
        /// Gets or set the RequestPathBase.
        /// </summary>
        /// <returns>The RequestPathBase.</returns>
        public abstract PathString PathBase { get; set; }

        /// <summary>
        /// Gets or set the request path from RequestPath.
        /// </summary>
        /// <returns>The request path from RequestPath.</returns>
        public abstract PathString Path { get; set; }

        /// <summary>
        /// Gets or set the raw query string used to create the query collection in Request.Query.
        /// </summary>
        /// <returns>The raw query string.</returns>
        public abstract QueryString QueryString { get; set; }

        /// <summary>
        /// Gets the query value collection parsed from Request.QueryString.
        /// </summary>
        /// <returns>The query value collection parsed from Request.QueryString.</returns>
        public abstract IQueryCollection Query { get; set; }

        /// <summary>
        /// Gets or set the RequestProtocol.
        /// </summary>
        /// <returns>The RequestProtocol.</returns>
        public abstract string Protocol { get; set; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        /// <returns>The request headers.</returns>
        public abstract IHeaderDictionary Headers { get; }

        /// <summary>
        /// Gets the collection of Cookies for this request.
        /// </summary>
        /// <returns>The collection of Cookies for this request.</returns>
        public abstract IRequestCookieCollection Cookies { get; set; }

        /// <summary>
        /// Gets or sets the Content-Length header
        /// </summary>
        public abstract long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header.
        /// </summary>
        /// <returns>The Content-Type header.</returns>
        public abstract string ContentType { get; set; }

        /// <summary>
        /// Gets or set the RequestBody Stream.
        /// </summary>
        /// <returns>The RequestBody Stream.</returns>
        public abstract Stream Body { get; set; }
    }
}
