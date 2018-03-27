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

namespace Osrs.Net.Http.Cookies
{
    /// <summary>
    /// Options used to create a new cookie.
    /// </summary>
    public class CookieOptions
    {
        /// <summary>
        /// Creates a default cookie with a path of '/'.
        /// </summary>
        public CookieOptions()
        {
            Path = "/";
        }

        /// <summary>
        /// Gets or sets the domain to associate the cookie with.
        /// </summary>
        /// <returns>The domain to associate the cookie with.</returns>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the cookie path.
        /// </summary>
        /// <returns>The cookie path.</returns>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time for the cookie.
        /// </summary>
        /// <returns>The expiration date and time for the cookie.</returns>
        public DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to transmit the cookie using Secure Sockets Layer (SSL)�that is, over HTTPS only.
        /// </summary>
        /// <returns>true to transmit the cookie only over an SSL connection (HTTPS); otherwise, false.</returns>
        public bool Secure { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a cookie is accessible by client-side script.
        /// </summary>
        /// <returns>true if a cookie must not be accessible by client-side script; otherwise, false.</returns>
        public bool HttpOnly { get; set; }
    }
}
