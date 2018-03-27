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
    public abstract class SiteProviderBase : ISiteProvider
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteCreatePermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteGetPermission);
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
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(Site item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, SiteProviderFactoryBase.SiteDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(Site item);

        public abstract IEnumerable<Site> Get();

        public abstract IEnumerable<Site> GetByOwner(CompoundIdentity owningOrgId);

        public IEnumerable<Site> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<Site> Get(string name, StringComparison comparisonOption);

        public abstract Site Get(CompoundIdentity id);

        public abstract bool Exists(CompoundIdentity id);
        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract bool HasParents(Site item);

        public abstract bool HasChildren(Site item);

        public IEnumerable<Site> GetParents(Site item)
        {
            if (item != null)
                return GetParents(item.Identity);
            return null;
        }

        public abstract IEnumerable<Site> GetParents(CompoundIdentity item);

        public IEnumerable<Site> GetChildren(Site parentSite)
        {
            if (parentSite != null)
                return GetChildren(parentSite.Identity);
            return null;
        }

        public abstract IEnumerable<Site> GetChildren(CompoundIdentity parentSite);

        public bool RemoveParent(Site child, Site parent)
        {
            if (child != null && parent != null)
                return RemoveParent(child.Identity, parent.Identity);
            return false;
        }

        public bool AddParent(Site child, Site parent)
        {
            if (child != null && parent != null)
                return AddParent(child.Identity, parent.Identity);
            return false;
        }

        public abstract bool RemoveParent(CompoundIdentity child, CompoundIdentity parent);

        public abstract bool AddParent(CompoundIdentity child, CompoundIdentity parent);

        public bool AddParent(IEnumerable<Site> children, Site parent)
        {
            if (children!=null && parent!=null)
            {
                return AddParent((IEnumerable<CompoundIdentity>)children, parent.Identity);
            }
            return false;
        }

        public abstract bool AddParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent);

        public bool RemoveParent(IEnumerable<Site> children, Site parent)
        {
            if (children != null && parent != null)
            {
                return RemoveParent((IEnumerable<CompoundIdentity>)children, parent.Identity);
            }
            return false;
        }

        public abstract bool RemoveParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent);

        public Site Create(CompoundIdentity owningOrgId, string name, Site parent)
        {
            Site s = this.Create(owningOrgId, name, (string)null);
            if (s!=null && parent!=null)
            {
                this.AddParent(s.Identity, parent.Identity);
            }
            return s;
        }

        public Site Create(CompoundIdentity owningOrgId, string name, string description, Site parent)
        {
            Site s = this.Create(owningOrgId, name, description);
            if (s != null && parent != null)
            {
                this.AddParent(s.Identity, parent.Identity);
            }
            return s;
        }

        public Site Create(CompoundIdentity owningOrgId, string name)
        {
            return this.Create(owningOrgId, name, (string)null);
        }

        public abstract Site Create(CompoundIdentity owningOrgId, string name, string description);

        public abstract bool Update(Site item);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(Site item)
        {
            if (item != null)
                return this.Delete(item.Identity);
            return false;
        }

        protected SiteProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}