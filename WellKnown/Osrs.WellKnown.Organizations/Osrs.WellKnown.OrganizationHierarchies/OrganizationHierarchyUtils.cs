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

namespace Osrs.WellKnown.OrganizationHierarchies
{
    public static class OrganizationHierarchyUtils
    {
        public static readonly Guid ReportingHierarchyId = new Guid("{39A2033A-EC1F-4D06-818D-786B562F8428}");

        public static readonly Guid OrganizationHierarchyCreatePermissionId = new Guid("{15F826D3-467C-4EA4-B8B7-EA037F9F0952}");
        public static readonly Guid OrganizationHierarchyGetPermissionId = new Guid("{C350E569-CDF5-4784-B030-4D3CCE37CF2F}");
        public static readonly Guid OrganizationHierarchyUpdatePermissionId = new Guid("{92339B3E-E3A0-4FA1-A16A-0063855CE017}");
        public static readonly Guid OrganizationHierarchyDeletePermissionId = new Guid("{30D140B4-BE02-41EC-8BD0-187704785476}");

        public static readonly Guid OrganizationHierarchyMembershipAddPermissionId = new Guid("{530C4160-581F-49AF-9D7E-A8A2BCDCBD3F}");
        public static readonly Guid OrganizationHierarchyMembershipGetPermissionId = new Guid("{3E12B7A4-64CF-44DE-9325-CED97E43CF5E}");
        public static readonly Guid OrganizationHierarchyMembershipMovePermissionId = new Guid("{4F5F50E8-2AD2-4E5B-B1C8-DD526CF62BCB}");
        public static readonly Guid OrganizationHierarchyMembershipRemovePermissionId = new Guid("{35F2128D-63EE-4C44-BE8C-0C69158F0EEB}");
    }
}
