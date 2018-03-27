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
using Osrs.Numerics;

namespace Osrs.WellKnown.FieldActivities
{
    public abstract class FieldActivityProviderBase : IFieldActivityProvider
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

        protected FieldActivityProviderBase(UserSecurityContext context)
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
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldActivityGetPermission);
                }
            }
            return false;
        }

        public abstract IEnumerable<FieldActivity> Get();

        public IEnumerable<FieldActivity> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }

        public abstract IEnumerable<FieldActivity> Get(string name, StringComparison comparisonOption);

        public abstract FieldActivity Get(CompoundIdentity id);

        public abstract IEnumerable<FieldActivity> Get(IEnumerable<CompoundIdentity> ids);

        public abstract IEnumerable<FieldActivity> GetForOrg(CompoundIdentity principalOrgId);

        public abstract IEnumerable<FieldActivity> GetForProject(CompoundIdentity projectId);

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return Exists(name, StringComparison.OrdinalIgnoreCase);
        }

        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract bool ExistsForOrg(CompoundIdentity principalOrgId);

        public abstract bool ExistsForProject(CompoundIdentity projectId);

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldActivityUpdatePermission);
                }
            }
            return false;
        }

        public abstract bool CanUpdate(FieldActivity item);

        public abstract bool Update(FieldActivity item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldActivityDeletePermission);
                }
            }
            return false;
        }

        public abstract bool CanDelete(FieldActivity item);

        public abstract bool Delete(FieldActivity item);

        public abstract bool Delete(CompoundIdentity id);

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldActivityCreatePermission);
                }
            }
            return false;
        }

        public FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId)
        {
            return Create(name, projectId, principalOrgId, null, null);
        }

        public FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, DateTime startDate)
        {
            return Create(name, projectId, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), null);
        }

        public FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange)
        {
            return Create(name, projectId, principalOrgId, dateRange, null);
        }

        public FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, string description)
        {
            return Create(name, projectId, principalOrgId, null, description);
        }

        public abstract FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);

        public FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, DateTime startDate, string description)
        {
            return Create(name, projectId, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), description);
        }
    }
}
