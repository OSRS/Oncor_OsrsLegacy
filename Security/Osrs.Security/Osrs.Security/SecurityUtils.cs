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

using System;

namespace Osrs.Security
{
    public static class SecurityUtils
    {
        private static readonly Guid identityRoot = new Guid("{9D272AFE-3BEB-49D6-847F-A1000C958C20}");
        public static Guid AdminIdentity
        {
            get { return identityRoot; }
        }

        private static readonly Guid roleRoot = new Guid("{286DFF3F-001D-4B52-B585-F68188D6D56D}");
        public static Guid AdminRole
        {
            get { return roleRoot; }
        }
    }
}
