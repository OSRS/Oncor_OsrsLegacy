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
    public abstract class SiteAliasSchemeProviderBase : ISiteAliasSchemeProvider
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasSchemeCreatePermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasSchemeGetPermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasSchemeUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(SiteAliasScheme item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteAliasSchemeDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(SiteAliasScheme item);

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public bool Exists(CompoundIdentity owningOrgId, string name)
        {
            return Exists(owningOrgId, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption);

        public abstract IEnumerable<SiteAliasScheme> Get();

        public abstract SiteAliasScheme Get(CompoundIdentity id);

        public IEnumerable<SiteAliasScheme> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SiteAliasScheme> Get(string name, StringComparison comparisonOption);

        public abstract IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId);
        public IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId, string name)
        {
            return GetByOwner(owningOrgId, name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption);

        public abstract bool Update(SiteAliasScheme item);

        public SiteAliasScheme Create(CompoundIdentity owningOrgId, string name)
        {
            return Create(owningOrgId, name, null);
        }
        public abstract SiteAliasScheme Create(CompoundIdentity owningOrgId, string name, string description);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(SiteAliasScheme item)
        {
            if (item != null)
                return this.Delete(item.Identity);
            return false;
        }

        protected SiteAliasSchemeProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}