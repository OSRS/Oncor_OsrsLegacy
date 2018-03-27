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


namespace Osrs.Net.Http
{
    internal class HostStringHelper
    {
        // Allowed Characters:
        // A-Z, a-z, 0-9, ., 
        // -, %, [, ], : 
        // Above for IPV6
        private static bool[] SafeHostStringChars = {
            false, false, false, false, false, false, false, false,     // 0x00 - 0x07
            false, false, false, false, false, false, false, false,     // 0x08 - 0x0F
            false, false, false, false, false, false, false, false,     // 0x10 - 0x17
            false, false, false, false, false, false, false, false,     // 0x18 - 0x1F
            false, false, false, false, false, true,  false, false,     // 0x20 - 0x27
            false, false, false, false, false, true,  true,  false,     // 0x28 - 0x2F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x30 - 0x37
            true,  true,  true,  false, false, false, false, false,     // 0x38 - 0x3F
            false, true,  true,  true,  true,  true,  true,  true,      // 0x40 - 0x47
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x48 - 0x4F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x50 - 0x57
            true,  true,  true,  true,  false, true,  false, false,     // 0x58 - 0x5F
            false, true,  true,  true,  true,  true,  true,  true,      // 0x60 - 0x67
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x68 - 0x6F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x70 - 0x77
            true,  true,  true,  false, false, false, false, false,     // 0x78 - 0x7F
        };

        public static bool IsSafeHostStringChar(char c)
        {
            return c < SafeHostStringChars.Length && SafeHostStringChars[c];
        }
    }

    internal class PathStringHelper
    {
        private static bool[] ValidPathChars = {
            false, false, false, false, false, false, false, false,     // 0x00 - 0x07
            false, false, false, false, false, false, false, false,     // 0x08 - 0x0F
            false, false, false, false, false, false, false, false,     // 0x10 - 0x17
            false, false, false, false, false, false, false, false,     // 0x18 - 0x1F
            false, true,  false, false, true,  false, true,  true,      // 0x20 - 0x27
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x28 - 0x2F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x30 - 0x37
            true,  true,  true,  true,  false, true,  false, false,     // 0x38 - 0x3F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x40 - 0x47
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x48 - 0x4F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x50 - 0x57
            true,  true,  true,  false, false, false, false, true,      // 0x58 - 0x5F
            false, true,  true,  true,  true,  true,  true,  true,      // 0x60 - 0x67
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x68 - 0x6F
            true,  true,  true,  true,  true,  true,  true,  true,      // 0x70 - 0x77
            true,  true,  true,  false, false, false, true,  false,     // 0x78 - 0x7F
        };

        public static bool IsValidPathChar(char c)
        {
            return c < ValidPathChars.Length && ValidPathChars[c];
        }
    }
}
