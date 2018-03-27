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

namespace Osrs.WellKnown.FieldActivities
{
    public abstract class FieldTeamMemberRoleProviderBase : IFieldTeamMemberRoleProvider
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

        protected FieldTeamMemberRoleProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamMemberRoleCreatePermission);
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
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamMemberRoleDeletePermission);
                }
            }
            return false;
        }

        public abstract bool CanDelete(FieldTeamMemberRole item);

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamMemberRoleGetPermission);
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
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamMemberRoleUpdatePermission);
                }
            }
            return false;
        }

        public abstract bool CanUpdate(FieldTeamMemberRole item);

        public FieldTeamMemberRole Create(string name)
        {
            return Create(name, null);
        }

        public abstract FieldTeamMemberRole Create(string name, string description);

        public abstract bool Delete(CompoundIdentity id);

        public bool Delete(FieldTeamMemberRole item)
        {
            if (item != null)
                return this.Delete(item.Identity);
            return false;
        }

        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }

        public abstract bool Exists(CompoundIdentity id);

        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract IEnumerable<FieldTeamMemberRole> Get();

        public abstract IEnumerable<FieldTeamMemberRole> Get(IEnumerable<CompoundIdentity> ids);

        public abstract FieldTeamMemberRole Get(CompoundIdentity id);

        public abstract IEnumerable<FieldTeamMemberRole> Get(string name);

        public abstract IEnumerable<FieldTeamMemberRole> Get(string name, StringComparison comparisonOption);

        public abstract bool Update(FieldTeamMemberRole item);
    }
}
