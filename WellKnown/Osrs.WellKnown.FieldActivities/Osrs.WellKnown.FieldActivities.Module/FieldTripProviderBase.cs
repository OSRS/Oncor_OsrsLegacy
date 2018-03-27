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
    public abstract class FieldTripProviderBase : IFieldTripProvider
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

        protected FieldTripProviderBase(UserSecurityContext context)
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
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTripGetPermission);
                }
            }
            return false;
        }

        public abstract IEnumerable<FieldTrip> Get();
        public IEnumerable<FieldTrip> Get(string name)
        {
            return Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<FieldTrip> Get(string name, StringComparison comparisonOption);

        public abstract FieldTrip Get(CompoundIdentity id);
        public abstract IEnumerable<FieldTrip> Get(IEnumerable<CompoundIdentity> ids);
        public abstract IEnumerable<FieldTrip> GetForOrg(CompoundIdentity principalOrgId);
        public abstract IEnumerable<FieldTrip> GetForActivity(CompoundIdentity activityId);
        public abstract IEnumerable<FieldTrip> GetForActivity(FieldActivity activity);
        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract bool ExistsForOrg(CompoundIdentity principalOrgId);
        public abstract bool ExistsForActivity(CompoundIdentity activityId);
        public abstract bool ExistsForActivity(FieldActivity activity);

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTripUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(FieldTrip item);
        public abstract bool Update(FieldTrip item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTripDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(FieldTrip item);
        public abstract bool Delete(FieldTrip item);
        public abstract bool Delete(CompoundIdentity id);

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.FieldTripCreatePermission);
                }
            }
            return false;
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId)
        {
            return Create(name, activity, principalOrgId, null, null);
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, DateTime startDate)
        {
            return Create(name, activity, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), null);
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange)
        {
            return Create(name, activity, principalOrgId, dateRange, null);
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, string description)
        {
            return Create(name, activity, principalOrgId, null, description);
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description)
        {
            if (activity != null)
                return Create(name, activity.Identity, principalOrgId, dateRange, null);
            else
                return null;
        }
        public FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, DateTime startDate, string description)
        {
            return Create(name, activity, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), description);
        }
        public abstract FieldTrip Create(string name, CompoundIdentity activityId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
    }
}
