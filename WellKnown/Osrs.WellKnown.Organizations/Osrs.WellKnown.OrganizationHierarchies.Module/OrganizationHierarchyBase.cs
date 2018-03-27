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

using Osrs.Data;
using Osrs.Runtime;
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.OrganizationHierarchies
{
    public abstract class OrganizationHierarchyBase : OrganizationHierarchy
    {
        protected UserSecurityContext Context
        {
            get;
        }

        private IRoleProvider prov;
        protected IRoleProvider AuthProvider
        {
            get
            {
                if (prov == null)
                    prov = AuthorizationManager.Instance.GetRoleProvider(this.Context);
                return prov;
            }
        }

        public override bool CanUpdateInfo()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyUpdatePermission);
                }
            }
            return false;
        }
        public override bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyMembershipGetPermission);
                }
            }
            return false;
        }
        public override bool CanAdd()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyMembershipAddPermission);
                }
            }
            return false;
        }
        public override bool CanMove()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyMembershipMovePermission);
                }
            }
            return false;
        }
        public override bool CanRemove()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyMembershipRemovePermission);
                }
            }
            return false;
        }

        protected OrganizationHierarchyBase(UserSecurityContext context, CompoundIdentity identity, CompoundIdentity owningOrgId, string name, string description) : base(identity, owningOrgId, name, description)
        {
            MethodContract.NotNull(context, nameof(context));
            this.Context = context;
        }
    }
}
