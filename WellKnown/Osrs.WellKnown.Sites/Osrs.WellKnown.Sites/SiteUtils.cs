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

namespace Osrs.WellKnown.Sites
{
    public static class SiteUtils
    {
        public static readonly Guid SiteCreatePermissionId = new Guid("{57268A5D-C61A-40FF-A6F0-4C2C95F1FF82}");
        public static readonly Guid SiteGetPermissionId = new Guid("{285890CC-C05F-43D0-B4B2-C9A8EE716BE1}");
        public static readonly Guid SiteUpdatePermissionId = new Guid("{8D0E2AFD-19C6-44E7-B65E-771EB2FC2B4B}");
        public static readonly Guid SiteDeletePermissionId = new Guid("{012BD9F3-23BA-42F3-9D61-98A400666C22}");

        public static readonly Guid SiteAliasCreatePermissionId = new Guid("{98D300CF-9289-44F7-A837-1427476EE6B4}");
        public static readonly Guid SiteAliasGetPermissionId = new Guid("{737C843F-326E-41AE-AAFB-C14157530D14}");
        public static readonly Guid SiteAliasUpdatePermissionId = new Guid("{F73CB987-469F-474D-9725-6862EB089D21}");
        public static readonly Guid SiteAliasDeletePermissionId = new Guid("{32F370C1-F7E5-4BA5-B464-FBDA5BE05D4E}");

        public static readonly Guid SiteAliasSchemeCreatePermissionId = new Guid("{A6C1019C-CA28-4D8F-AA88-85B045CECFB5}");
        public static readonly Guid SiteAliasSchemeGetPermissionId = new Guid("{CAC9B2D9-56FE-486A-BAD6-411B0912A17A}");
        public static readonly Guid SiteAliasSchemeUpdatePermissionId = new Guid("{E044A099-79C6-439D-83DC-2853DB46C8E5}");
        public static readonly Guid SiteAliasSchemeDeletePermissionId = new Guid("{C0C83939-FC50-4A82-88D6-BC56B044D1D3}");

        public static bool IsDirty(SiteAlias alias)
        {
            if (alias != null)
                return alias.isDirty;
            return false;
        }

        public static string OriginalName(SiteAlias alias)
        {
            if (alias != null)
                return alias.origName;
            return null;
        }
    }
}
