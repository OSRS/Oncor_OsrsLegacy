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

namespace Osrs.WellKnown.OrganizationHierarchies
{
    public abstract class OrganizationHierarchyProviderFactoryBase : SubclassableSingletonBase<OrganizationHierarchyProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal static Permission OrganizationHierarchyCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyCreatePermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyDeletePermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyGetPermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyUpdatePermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyMembershipAddPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipAddPermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyMembershipGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipGetPermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyMembershipMovePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipMovePermissionId);
            }
        }

        protected internal static Permission OrganizationHierarchyMembershipRemovePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipRemovePermissionId);
            }
        }

        protected internal abstract bool Initialize();

        protected internal abstract OrganizationHierarchyProviderBase GetProvider(UserSecurityContext context);
    }
}
