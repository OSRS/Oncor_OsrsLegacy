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
using Osrs.WellKnown.Organizations;

namespace Osrs.WellKnown.OrganizationHierarchies
{
    public abstract class OrganizationHierarchyProviderBase : IOrganizationHierarchyProvider
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

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyCreatePermission);
                }
            }
            return false;
        }

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyDeletePermission);
                }
            }
            return false;
        }

        public bool CanDelete(CompoundIdentity hierarchyId)
        {
            if (!hierarchyId.IsNullOrEmpty())
            {
                if (!OrganizationHierarchyUtils.ReportingHierarchyId.Equals(hierarchyId)) //ensure we cannot ever allow delete of reporting hierarchy - but we do allow rename
                    return CanDeleteImpl(hierarchyId);
            }
            return false;
        }

        protected abstract bool CanDeleteImpl(CompoundIdentity hierarchyId);

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationHierarchyProviderFactoryBase.OrganizationHierarchyGetPermission);
                }
            }
            return false;
        }

        public abstract OrganizationHierarchy Create(string hierarchyName, Organization owningOrg);
        public abstract bool Delete(CompoundIdentity hierarchyId);
        public abstract bool Delete(OrganizationHierarchy hierarchy);
        public abstract bool Exists(CompoundIdentity hierarchyId);
        public abstract bool Exists(string hierarchyName);
        public abstract OrganizationHierarchy Get(CompoundIdentity hierarchyId);
        public abstract OrganizationHierarchy Get(string hierarchyName);
        public abstract OrganizationHierarchy GetReporting();

        protected OrganizationHierarchyProviderBase(UserSecurityContext context)
        {
            MethodContract.NotNull(context, nameof(context));
            this.Context = context;
        }
    }
}
