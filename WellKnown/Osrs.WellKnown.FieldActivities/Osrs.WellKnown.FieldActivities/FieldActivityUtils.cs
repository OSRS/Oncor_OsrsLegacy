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

namespace Osrs.WellKnown.FieldActivities
{
    public static class FieldActivityUtils
    {
        //NOTE These are WKTIDs that need to eventually make it to the type registry
        public static readonly Guid FieldActivityWktId = new Guid("{42DBD76B-A427-4E58-8781-50F2E7C735D8}");
        public static readonly Guid FieldTripWktId = new Guid("{EB0D01F4-D9BC-44D2-B20A-2C08F125A380}");
        public static readonly Guid FieldTeamWktId = new Guid("{042D0762-4387-4121-A2DA-8534EAA31889}");
        public static readonly Guid FieldTeamMemberRoleWktId = new Guid("{E30C58A4-5F55-4132-A988-BB827CE77BD6}");
        public static readonly Guid SamplingEventWktId = new Guid("{08E08F4F-BFA0-45C8-96C7-5BDEE882FFEF}");

        public static readonly Guid FieldActivityCreatePermissionId = new Guid("{9FFEC187-A343-4AE3-B541-054E559DD562}");
        public static readonly Guid FieldActivityGetPermissionId = new Guid("{59F0E95A-B8D9-4EFA-A94D-F4A2E7020F59}");
        public static readonly Guid FieldActivityUpdatePermissionId = new Guid("{06365E08-4FB3-47F1-844A-98BEA454F62D}");
        public static readonly Guid FieldActivityDeletePermissionId = new Guid("{A8E4CFEA-913C-4906-8E55-7C72E9C5F2F4}");

        public static readonly Guid FieldTripCreatePermissionId = new Guid("{A64EC1AA-2BB8-4DD9-B402-2F63FA14A98E}");
        public static readonly Guid FieldTripGetPermissionId = new Guid("{7079A265-C5FC-4679-83C5-7F023032C67B}");
        public static readonly Guid FieldTripUpdatePermissionId = new Guid("{9BF66C46-2CFA-415D-B0A2-1028CF6865E5}");
        public static readonly Guid FieldTripDeletePermissionId = new Guid("{E9A25800-78BF-4A60-9717-E4E7533E5168}");

        public static readonly Guid FieldTeamCreatePermissionId = new Guid("{B3130E20-0A7A-4D3B-AF51-3B0266B2A89D}");
        public static readonly Guid FieldTeamGetPermissionId = new Guid("{2F73FFF0-EBEF-448A-AAB1-E56D42296BB5}");
        public static readonly Guid FieldTeamUpdatePermissionId = new Guid("{18E3AAA0-1FA9-490E-BE1C-CC8A7E78DF94}");
        public static readonly Guid FieldTeamDeletePermissionId = new Guid("{444BA84D-D9F7-4562-88BB-677F95C0F0CE}");

        public static readonly Guid FieldTeamMemberRoleCreatePermissionId = new Guid("{56EC3EA6-0027-4EA2-9162-8B200D432CC0}");
        public static readonly Guid FieldTeamMemberRoleGetPermissionId = new Guid("{CFADA7B8-04B8-46A0-816C-C63AC499D3D5}");
        public static readonly Guid FieldTeamMemberRoleUpdatePermissionId = new Guid("{CB25105F-CCAC-4823-8931-DCFEBF82C664}");
        public static readonly Guid FieldTeamMemberRoleDeletePermissionId = new Guid("{4CDDCF27-7533-4E5A-8933-C0D050AC51F9}");

        public static readonly Guid SampleEventCreatePermissionId = new Guid("{594A65ED-210C-435E-B83A-6B205EF533D4}");
        public static readonly Guid SampleEventGetPermissionId = new Guid("{70AAA0A8-28FD-42C0-9173-7437858ADD45}");
        public static readonly Guid SampleEventUpdatePermissionId = new Guid("{64C550B2-CF4B-44C3-823E-DC035D5FB46E}");
        public static readonly Guid SampleEventDeletePermissionId = new Guid("{494F0328-4D2D-4AE2-8BED-9BD43FD73FF8}");
    }
}
