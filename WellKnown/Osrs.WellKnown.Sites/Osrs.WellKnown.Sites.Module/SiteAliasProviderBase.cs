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
using Osrs.Security;
using Osrs.Security.Authorization;
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.Sites
{
    public abstract class SiteAliasProviderBase : ISiteAliasProvider
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasCreatePermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasGetPermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(SiteAlias alias);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(SiteAlias scheme);

        public abstract bool Exists(CompoundIdentity id);
        public abstract bool Exists(CompoundIdentity id, SiteAliasScheme scheme);
        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);
        public bool Exists(Site site, string name)
        {
            return this.Exists(site, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(Site site, string name, StringComparison comparisonOption);
        public bool Exists(SiteAliasScheme scheme, string name)
        {
            return this.Exists(scheme, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(SiteAliasScheme scheme, string name, StringComparison comparisonOption);

        public abstract IEnumerable<SiteAlias> Get();

        public abstract IEnumerable<SiteAlias> Get(SiteAliasScheme scheme);
        public abstract IEnumerable<SiteAlias> Get(Site site);

        public IEnumerable<SiteAlias> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SiteAlias> Get(string name, StringComparison comparisonOption);

        public IEnumerable<SiteAlias> Get(SiteAliasScheme scheme, string name)
        {
            return this.Get(scheme, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SiteAlias> Get(SiteAliasScheme scheme, string name, StringComparison comparisonOption);

        public IEnumerable<SiteAlias> Get(Site site, string name)
        {
            return this.Get(site, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SiteAlias> Get(Site site, string name, StringComparison comparisonOption);

        public abstract IEnumerable<SiteAlias> Get(CompoundIdentity id, SiteAliasScheme scheme);

        public abstract SiteAlias Create(SiteAliasScheme scheme, Site site, string name);

        public abstract bool Update(SiteAlias site);

        public abstract bool Delete(SiteAlias alias);
        public abstract bool Delete(CompoundIdentity siteId);
        public abstract bool Delete(SiteAliasScheme scheme);
        public bool Delete(CompoundIdentity id, SiteAliasScheme scheme)
        {
            if (id != null && scheme != null)
            {
                return this.Delete(id, scheme.Identity);
            }
            return false;
        }
        public abstract bool Delete(CompoundIdentity id, CompoundIdentity schemeId);

        protected SiteAliasProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}