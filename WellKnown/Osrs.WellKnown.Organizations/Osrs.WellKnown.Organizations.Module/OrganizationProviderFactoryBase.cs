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

namespace Osrs.WellKnown.Organizations
{
    public abstract class OrganizationProviderFactoryBase : SubclassableSingletonBase<OrganizationProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal static Permission OrganizationCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Organization"), OrganizationUtils.OrganizationCreatePermissionId);
            }
        }
        protected internal static Permission OrganizationGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Organization"), OrganizationUtils.OrganizationGetPermissionId);
            }
        }
        protected internal static Permission OrganizationUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Organization"), OrganizationUtils.OrganizationUpdatePermissionId);
            }
        }
        protected internal static Permission OrganizationDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Organization"), OrganizationUtils.OrganizationDeletePermissionId);
            }
        }

        protected internal static Permission OrganizationAliasCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAlias"), OrganizationUtils.OrganizationAliasCreatePermissionId);
            }
        }
        protected internal static Permission OrganizationAliasGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAlias"), OrganizationUtils.OrganizationAliasGetPermissionId);
            }
        }
        protected internal static Permission OrganizationAliasUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAlias"), OrganizationUtils.OrganizationAliasUpdatePermissionId);
            }
        }
        protected internal static Permission OrganizationAliasDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAlias"), OrganizationUtils.OrganizationAliasDeletePermissionId);
            }
        }

        protected internal static Permission OrganizationAliasSchemeCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeCreatePermissionId);
            }
        }
        protected internal static Permission OrganizationAliasSchemeGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeGetPermissionId);
            }
        }
        protected internal static Permission OrganizationAliasSchemeUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeUpdatePermissionId);
            }
        }
        protected internal static Permission OrganizationAliasSchemeDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeDeletePermissionId);
            }
        }

        protected internal abstract bool Initialize();

        protected internal abstract OrganizationProviderBase GetOrganizationProvider(UserSecurityContext context);

        protected internal abstract OrganizationAliasSchemeProviderBase GetOrganizationAliasSchemeProvider(UserSecurityContext context);

        protected internal abstract OrganizationAliasProviderBase GetOrganizationAliasProvider(UserSecurityContext context);
    }
}
