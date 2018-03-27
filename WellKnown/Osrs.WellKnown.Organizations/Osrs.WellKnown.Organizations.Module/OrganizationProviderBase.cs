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
using System.Collections.Generic;
using Osrs.Data;
using Osrs.Security.Authorization;
using Osrs.Security;

namespace Osrs.WellKnown.Organizations
{
    public abstract class OrganizationProviderBase : IOrganizationProvider
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
            if (perms!=null)
            {
                if (this.Context!=null && this.Context.User!=null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationCreatePermission);
                }
            }
            return false;
        }

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationGetPermission);
                }
            }
            return false;
        }

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(Organization org);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(Organization org);

        public abstract IEnumerable<Organization> Get();
        public abstract Organization Get(CompoundIdentity id);
        public abstract IEnumerable<Organization> Get(IEnumerable<CompoundIdentity> ids);
        public IEnumerable<Organization> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<Organization> Get(string name, StringComparison comparisonOption);

        public abstract bool Exists(CompoundIdentity id);
        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public Organization Create(string name)
        {
            return this.Create(name, null);
        }
        public abstract Organization Create(string name, string description);

        public abstract bool Update(Organization org);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(Organization org)
        {
            if (org != null)
                return this.Delete(org.Identity);
            return false;
        }

        protected OrganizationProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}
