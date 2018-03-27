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

using System.Collections.Generic;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.Organizations;
using System;
using Osrs.Data;
using Osrs.Security.Identity;

namespace Osrs.WellKnown.UserAffiliation
{
    public abstract class UserAffiliationProviderBase : IUserAffiliationProvider
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

        private IIdentityProvider idProv;
        protected IIdentityProvider IdProvider
        {
            get
            {
                if (idProv == null)
                    idProv = IdentityManager.Instance.GetProvider(this.Context);
                return idProv;
            }
        }

        private IOrganizationProvider orgProv;
        protected IOrganizationProvider OrgProvider
        {
            get
            {
                if (orgProv == null)
                    orgProv = OrganizationManager.Instance.GetOrganizationProvider(this.Context);
                return orgProv;
            }
        }

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, UserAffiliationProviderFactoryBase.UserAffiliationGetPermission);
                }
            }
            return false;
        }
        public abstract bool CanGet(Organization org);
        public abstract bool CanGet(IUserIdentity user);

        public bool CanAdd()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, UserAffiliationProviderFactoryBase.UserAffiliationAddPermission);
                }
            }
            return false;
        }
        public abstract bool CanAdd(Organization org);
        public abstract bool CanAdd(IUserIdentity user);

        public bool CanRemove()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, UserAffiliationProviderFactoryBase.UserAffiliationRemovePermission);
                }
            }
            return false;
        }
        public abstract bool CanRemove(Organization org);
        public abstract bool CanRemove(IUserIdentity user);

        public bool HasAffiliation(Organization org)
        {
            if (this.Context.User != null && !Guid.Empty.Equals(this.Context.User.Uid))
                return HasAffiliation(this.Context.User, org);
            return false;
        }

        public abstract bool HasAffiliation(IUserIdentity user, Organization org);

        public IEnumerable<IUserIdentity> Get(Organization org)
        {
            IEnumerable<Guid> ids = GetIds(org); //note, this should be empty if CanGet(org) is true and null if CanGet(org) is false
            if (ids != null)
            {
                List<IUserIdentity> users = new List<IUserIdentity>();
                IUserIdentity tmp;
                IIdentityProvider iProv = IdProvider;
                foreach (Guid id in ids)
                {
                    tmp = iProv.Get(id);
                    if (tmp != null)
                        users.Add(tmp);
                }
                return users;
            }
            return null;
        }

        public abstract IEnumerable<Guid> GetIds(Organization org);

        public IEnumerable<Organization> Get(IUserIdentity user)
        {
            IEnumerable<CompoundIdentity> ids = GetIds(user); //note, this should be empty if CanGet(org) is true and null if CanGet(org) is false
            if (ids != null)
            {
                List<Organization> users = new List<Organization>();
                Organization tmp;
                IOrganizationProvider iProv = OrgProvider;
                foreach (CompoundIdentity id in ids)
                {
                    tmp = iProv.Get(id);
                    if (tmp != null)
                        users.Add(tmp);
                }
                return users;
            }
            return null;
        }

        public abstract IEnumerable<CompoundIdentity> GetIds(IUserIdentity user);
        public abstract bool Add(IUserIdentity user, Organization org);
        public abstract bool Add(IEnumerable<IUserIdentity> user, Organization org);
        public abstract bool Add(IUserIdentity user, IEnumerable<Organization> org);
        public abstract bool Remove(IUserIdentity user, Organization org);
        public abstract bool Remove(IEnumerable<IUserIdentity> user, Organization org);
        public abstract bool Remove(IUserIdentity user, IEnumerable<Organization> org);

        protected UserAffiliationProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}