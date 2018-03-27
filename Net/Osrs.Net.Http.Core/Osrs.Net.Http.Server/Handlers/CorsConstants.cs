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


namespace Osrs.Net.Http.Handlers
{
    /// <summary>
    /// CORS-related constants.
    /// </summary>
    public static class CorsConstants
    {
        /// <summary>
        /// The HTTP method for the CORS preflight request.
        /// </summary>
        public const string PreflightHttpMethod = "OPTIONS";

        /// <summary>
        /// The Vary response header.
        /// </summary>
        public const string VaryByOrigin = "Vary";

        /// <summary>
        /// The Origin request header.
        /// </summary>
        public const string Origin = "Origin";

        /// <summary>
        /// The value for the Access-Control-Allow-Origin response header to allow all origins.
        /// </summary>
        public const string AnyOrigin = "*";

        /// <summary>
        /// The Access-Control-Request-Method request header.
        /// </summary>
        public const string AccessControlRequestMethod = "Access-Control-Request-Method";

        /// <summary>
        /// The Access-Control-Request-Headers request header.
        /// </summary>
        public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";

        /// <summary>
        /// The Access-Control-Allow-Origin response header.
        /// </summary>
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>
        /// The Access-Control-Allow-Headers response header.
        /// </summary>
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>
        /// The Access-Control-Expose-Headers response header.
        /// </summary>
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";

        /// <summary>
        /// The Access-Control-Allow-Methods response header.
        /// </summary>
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";

        /// <summary>
        /// The Access-Control-Allow-Credentials response header.
        /// </summary>
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";

        /// <summary>
        /// The Access-Control-Max-Age response header.
        /// </summary>
        public const string AccessControlMaxAge = "Access-Control-Max-Age";

        internal static readonly string[] SimpleRequestHeaders =
        {
            "Origin",
            "Accept",
            "Accept-Language",
            "Content-Language",
        };

        internal static readonly string[] SimpleResponseHeaders =
        {
            "Cache-Control",
            "Content-Language",
            "Content-Type",
            "Expires",
            "Last-Modified",
            "Pragma"
        };

        internal static readonly string[] SimpleMethods =
        {
            "GET",
            "HEAD",
            "POST"
        };
    }
}
