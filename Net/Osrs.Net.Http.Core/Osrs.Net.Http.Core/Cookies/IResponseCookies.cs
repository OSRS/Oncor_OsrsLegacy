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


namespace Osrs.Net.Http.Cookies
{
    /// <summary>
    /// A wrapper for the response Set-Cookie header.
    /// </summary>
    public interface IResponseCookies
    {
        /// <summary>
        /// Add a new cookie and value.
        /// </summary>
        /// <param name="key">Name of the new cookie.</param>
        /// <param name="value">Value of the new cookie.</param>
        void Append(string key, string value);

        /// <summary>
        /// Add a new cookie.
        /// </summary>
        /// <param name="key">Name of the new cookie.</param>
        /// <param name="value">Value of the new cookie.</param>
        /// <param name="options"><see cref="CookieOptions"/> included in the new cookie setting.</param>
        void Append(string key, string value, CookieOptions options);

        /// <summary>
        /// Sets an expired cookie.
        /// </summary>
        /// <param name="key">Name of the cookie to expire.</param>
        void Delete(string key);

        /// <summary>
        /// Sets an expired cookie.
        /// </summary>
        /// <param name="key">Name of the cookie to expire.</param>
        /// <param name="options">
        /// <see cref="CookieOptions"/> used to discriminate the particular cookie to expire. The
        /// <see cref="CookieOptions.Domain"/> and <see cref="CookieOptions.Path"/> values are especially important.
        /// </param>
        void Delete(string key, CookieOptions options);
    }
}
