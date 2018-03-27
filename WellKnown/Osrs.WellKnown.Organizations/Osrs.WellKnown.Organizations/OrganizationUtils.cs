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

namespace Osrs.WellKnown.Organizations
{
    public static class OrganizationUtils
    {
        public static readonly Guid OrganizationCreatePermissionId = new Guid("{7DB9E478-AAAC-43C2-A194-10F572BF21E9}");
        public static readonly Guid OrganizationGetPermissionId = new Guid("{B28FB47F-B4DE-4D77-962A-301A282F3927}");
        public static readonly Guid OrganizationUpdatePermissionId = new Guid("{F5063772-E938-4904-A756-2AEDD9EB3F88}");
        public static readonly Guid OrganizationDeletePermissionId = new Guid("{6721AFC2-E129-4685-91AC-D8DE24096E66}");

        public static readonly Guid OrganizationAliasCreatePermissionId = new Guid("{836C44D0-A327-4950-BB01-F6E22D1D9D6D}");
        public static readonly Guid OrganizationAliasGetPermissionId = new Guid("{22E893D2-9CA2-4FD6-B111-C104EA0B2F80}");
        public static readonly Guid OrganizationAliasUpdatePermissionId = new Guid("{9A702BB8-73DE-4C3E-9A60-7CDAC3557C0B}");
        public static readonly Guid OrganizationAliasDeletePermissionId = new Guid("{0C39D844-9419-4C9D-BB62-D1B39296AAAB}");

        public static readonly Guid OrganizationAliasSchemeCreatePermissionId = new Guid("{BB95770C-6FA6-4395-AB49-6AEAC677EF9B}");
        public static readonly Guid OrganizationAliasSchemeGetPermissionId = new Guid("{7A7066BB-936A-4058-A095-5C004CC360A2}");
        public static readonly Guid OrganizationAliasSchemeUpdatePermissionId = new Guid("{9B61F24D-6C01-44EA-A596-995A83708288}");
        public static readonly Guid OrganizationAliasSchemeDeletePermissionId = new Guid("{1336DBA9-D996-46A2-9C0B-31EB3C0F26D8}");

        public static bool IsDirty(OrganizationAlias item)
        {
            if (item != null)
                return item.isDirty;
            return false;
        }

        public static string OriginalName(OrganizationAlias item)
        {
            if (item != null)
                return item.origName;
            return null;
        }
    }
}
