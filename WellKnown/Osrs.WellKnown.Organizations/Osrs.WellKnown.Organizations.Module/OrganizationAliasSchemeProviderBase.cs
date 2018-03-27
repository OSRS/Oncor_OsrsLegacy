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
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.Organizations
{
    public abstract class OrganizationAliasSchemeProviderBase : IOrganizationAliasSchemeProvider
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasSchemeCreatePermission);
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasSchemeGetPermission);
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasSchemeUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(OrganizationAliasScheme org);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasSchemeDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(OrganizationAliasScheme org);

        public OrganizationAliasScheme Create(Organization owner, string name)
        {
            return this.Create(owner, name, null);
        }

        public abstract OrganizationAliasScheme Create(Organization owner, string name, string description);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(OrganizationAliasScheme org)
        {
            if (org != null)
                return this.Delete(org.Identity);
            return false;
        }

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public bool Exists(Organization owner, string name)
        {
            return this.Exists(owner, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(Organization owner, string name, StringComparison comparisonOption);

        public abstract IEnumerable<OrganizationAliasScheme> Get();

        public abstract OrganizationAliasScheme Get(CompoundIdentity id);

        public abstract IEnumerable<OrganizationAliasScheme> Get(Organization owner);

        public IEnumerable<OrganizationAliasScheme> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<OrganizationAliasScheme> Get(string name, StringComparison comparisonOption);

        public IEnumerable<OrganizationAliasScheme> Get(Organization owner, string name)
        {
            return this.Get(owner, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<OrganizationAliasScheme> Get(Organization owner, string name, StringComparison comparisonOption);

        public abstract bool Update(OrganizationAliasScheme org);

        protected OrganizationAliasSchemeProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}
