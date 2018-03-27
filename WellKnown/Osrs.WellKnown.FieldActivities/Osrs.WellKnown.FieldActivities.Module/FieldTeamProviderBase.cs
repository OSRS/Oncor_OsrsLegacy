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

using Osrs.Security;
using Osrs.Security.Authorization;
using System;
using System.Collections.Generic;
using Osrs.Data;

namespace Osrs.WellKnown.FieldActivities
{
    public abstract class FieldTeamProviderBase : IFieldTeamProvider
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

        protected FieldTeamProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamGetPermission);
                }
            }
            return false;
        }

        public abstract IEnumerable<FieldTeam> Get();

        public IEnumerable<FieldTeam> Get(string name)
        {
            return Get(name, StringComparison.OrdinalIgnoreCase);
        }

        public abstract IEnumerable<FieldTeam> Get(string name, StringComparison comparisonOption);

        public abstract FieldTeam Get(CompoundIdentity id);
        public abstract IEnumerable<FieldTeam> Get(IEnumerable<CompoundIdentity> ids);
        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamUpdatePermission);
                }
            }
            return false;
        }

        public abstract bool CanUpdate(FieldTeam item);

        public abstract bool Update(FieldTeam item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamDeletePermission);
                }
            }
            return false;
        }

        public abstract bool CanDelete(FieldTeam item);

        public bool Delete(FieldTeam item)
        {
            if (item != null)
                return Delete(item.Identity);
            return false;
        }
        public abstract bool Delete(CompoundIdentity id);

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTeamCreatePermission);
                }
            }
            return false;
        }

        public FieldTeam Create(string name)
        {
            return Create(name, null);
        }
        public abstract FieldTeam Create(string name, string description);

        public abstract IEnumerable<FieldTeam> Get(FieldActivity item);
        public abstract IEnumerable<FieldTeam> Get(FieldTrip item);
        public abstract IEnumerable<FieldTeam> Get(SamplingEvent item);

        public abstract bool Contains(FieldTeam team, FieldActivity item);
        public abstract bool Contains(FieldTeam team, FieldTrip item);
        public abstract bool Contains(FieldTeam team, SamplingEvent item);

        public bool CanAddRemove()
        {
            return this.CanGet();
        }
        public bool CanAddRemove(FieldActivity item)
        {
            if (item!=null)
                return this.CanAddRemove() && FieldActivityManager.Instance.GetFieldActivityProvider(this.Context).CanUpdate(item);
            return false;
        }
        public bool CanAddRemove(FieldTrip item)
        {
            if (item != null)
                return this.CanAddRemove() && FieldActivityManager.Instance.GetFieldTripProvider(this.Context).CanUpdate(item);
            return false;
        }

        public bool CanAddRemove(SamplingEvent item)
        {
            if (item != null)
                return this.CanAddRemove() && FieldActivityManager.Instance.GetSampleEventProvider(this.Context).CanUpdate(item);
            return false;
        }

        public abstract bool Add(FieldTeam team, FieldActivity item);
        public abstract bool Add(FieldTeam team, FieldTrip item);
        public abstract bool Add(FieldTeam team, SamplingEvent item);

        public abstract bool Remove(FieldTeam team, FieldActivity item);
        public abstract bool Remove(FieldTeam team, FieldTrip item);
        public abstract bool Remove(FieldTeam team, SamplingEvent item);
    }
}
