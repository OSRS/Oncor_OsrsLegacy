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

using Osrs.Runtime;
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.Sites
{
    public abstract class SiteProviderFactoryBase : SubclassableSingletonBase<SiteProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal static Permission SiteCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Site"), SiteUtils.SiteCreatePermissionId);
            }
        }
        protected internal static Permission SiteGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Site"), SiteUtils.SiteGetPermissionId);
            }
        }
        protected internal static Permission SiteUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Site"), SiteUtils.SiteUpdatePermissionId);
            }
        }
        protected internal static Permission SiteDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Site"), SiteUtils.SiteDeletePermissionId);
            }
        }

        protected internal static Permission SiteAliasCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAlias"), SiteUtils.SiteAliasCreatePermissionId);
            }
        }
        protected internal static Permission SiteAliasGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAlias"), SiteUtils.SiteAliasGetPermissionId);
            }
        }
        protected internal static Permission SiteAliasUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAlias"), SiteUtils.SiteAliasUpdatePermissionId);
            }
        }
        protected internal static Permission SiteAliasDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAlias"), SiteUtils.SiteAliasDeletePermissionId);
            }
        }

        protected internal static Permission SiteAliasSchemeCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeCreatePermissionId);
            }
        }
        protected internal static Permission SiteAliasSchemeGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeGetPermissionId);
            }
        }
        protected internal static Permission SiteAliasSchemeUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeUpdatePermissionId);
            }
        }
        protected internal static Permission SiteAliasSchemeDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeDeletePermissionId);
            }
        }

        protected internal abstract bool Initialize();

        protected internal abstract SiteProviderBase GetSiteProvider(UserSecurityContext context);

        protected internal abstract SiteAliasSchemeProviderBase GetSiteAliasSchemeProvider(UserSecurityContext context);

        protected internal abstract SiteAliasProviderBase GetSiteAliasProvider(UserSecurityContext context);
    }
}