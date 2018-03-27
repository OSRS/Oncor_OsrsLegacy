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
    /// <summary>
    /// Various extension methods for <see cref="ContentDispositionHeaderValue"/> for identifying the type of the disposition header
    /// </summary>
    public static class ContentDispositionHeaderValueIdentityExtensions
    {
        /// <summary>
        /// Checks if the content disposition header is a file disposition
        /// </summary>
        /// <param name="header">The header to check</param>
        /// <returns>True if the header is file disposition, false otherwise</returns>
        public static bool IsFileDisposition(this ContentDispositionHeaderValue header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            return header.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(header.FileName) || !string.IsNullOrEmpty(header.FileNameStar));
        }

        /// <summary>
        /// Checks if the content disposition header is a form disposition
        /// </summary>
        /// <param name="header">The header to check</param>
        /// <returns>True if the header is form disposition, false otherwise</returns>
        public static bool IsFormDisposition(this ContentDispositionHeaderValue header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            return header.DispositionType.Equals("form-data")
               && string.IsNullOrEmpty(header.FileName) && string.IsNullOrEmpty(header.FileNameStar);
        }
    }
}
