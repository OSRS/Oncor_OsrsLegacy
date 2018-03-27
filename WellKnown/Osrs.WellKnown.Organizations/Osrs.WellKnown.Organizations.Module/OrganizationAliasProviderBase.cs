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
    public abstract class OrganizationAliasProviderBase : IOrganizationAliasProvider
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasCreatePermission);
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasGetPermission);
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
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(OrganizationAlias alias);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, OrganizationProviderFactoryBase.OrganizationAliasDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(OrganizationAlias scheme);

        public abstract bool Exists(CompoundIdentity id);
        public abstract bool Exists(CompoundIdentity id, OrganizationAliasScheme scheme);
        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);
        public bool Exists(Organization org, string name)
        {
            return this.Exists(org, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(Organization org, string name, StringComparison comparisonOption);
        public bool Exists(OrganizationAliasScheme scheme, string name)
        {
            return this.Exists(scheme, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption);

        public abstract IEnumerable<OrganizationAlias> Get();

        public abstract IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme);
        public abstract IEnumerable<OrganizationAlias> Get(Organization org);

        public IEnumerable<OrganizationAlias> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<OrganizationAlias> Get(string name, StringComparison comparisonOption);

        public IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme, string name)
        {
            return this.Get(scheme, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption);

        public IEnumerable<OrganizationAlias> Get(Organization org, string name)
        {
            return this.Get(org, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<OrganizationAlias> Get(Organization org, string name, StringComparison comparisonOption);

        public abstract IEnumerable<OrganizationAlias> Get(CompoundIdentity id, OrganizationAliasScheme scheme);

        public abstract OrganizationAlias Create(OrganizationAliasScheme scheme, Organization org, string name);

        public abstract bool Update(OrganizationAlias org);

        public abstract bool Delete(OrganizationAlias alias);
        public abstract bool Delete(CompoundIdentity orgId);
        public abstract bool Delete(OrganizationAliasScheme scheme);
        public bool Delete(CompoundIdentity id, OrganizationAliasScheme scheme)
        {
            if (id!=null && scheme!=null)
            {
                return this.Delete(id, scheme.Identity);
            }
            return false;
        }
        public abstract bool Delete(CompoundIdentity id, CompoundIdentity schemeId);

        protected OrganizationAliasProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}
