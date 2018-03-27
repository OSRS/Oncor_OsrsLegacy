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

namespace Osrs.WellKnown.UserAffiliation
{
    public static class UserAffiliationUtils
    {
        private static readonly Guid addPermissionId = new Guid("{6121A4E6-A636-4025-A3F8-4B61B730093A}");
        public static Guid AddPermissionId
        {
            get { return addPermissionId; }
        }

        private static readonly Guid removePermissionId = new Guid("{07254D0A-1C08-4490-8D08-3F0B9575981D}");
        public static Guid RemovePermissionId
        {
            get { return removePermissionId; }
        }

        private static readonly Guid getPermissionId = new Guid("{EFCE3C06-608A-4190-9E65-4043B9AD0935}");
        public static Guid GetPermissionId
        {
            get { return getPermissionId; }
        }
    }
}
