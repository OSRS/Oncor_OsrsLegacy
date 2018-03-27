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
    public abstract class SampleEventProviderBase : ISampleEventProvider
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

        protected SampleEventProviderBase(UserSecurityContext context)
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
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.SampleEventGetPermission);
                }
            }
            return false;
        }
        public abstract IEnumerable<SamplingEvent> Get();

        public IEnumerable<SamplingEvent> Get(string name)
        {
            return Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<SamplingEvent> Get(string name, StringComparison comparisonOption);

        public abstract SamplingEvent Get(CompoundIdentity id);
        public abstract IEnumerable<SamplingEvent> Get(IEnumerable<CompoundIdentity> ids);
        public abstract IEnumerable<SamplingEvent> GetForOrg(CompoundIdentity principalOrgId);
        public abstract IEnumerable<SamplingEvent> GetForTrip(CompoundIdentity tripId);
        public IEnumerable<SamplingEvent> GetForTrip(FieldTrip trip)
        {
            if (trip != null)
                return GetForTrip(trip.Identity);
            return null;
        }
        public abstract IEnumerable<SamplingEvent> GetForTrip(IEnumerable<CompoundIdentity> tripIds);
        public abstract IEnumerable<SamplingEvent> GetForTrip(IEnumerable<FieldTrip> trips);

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract bool ExistsForOrg(CompoundIdentity principalOrgId);
        public abstract bool ExistsForTrip(CompoundIdentity tripId);
        public bool ExistsForTrip(FieldTrip trip)
        {
            if (trip != null)
                return ExistsForTrip(trip.Identity);
            return false;
        }

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.SampleEventUpdatePermission);
                }
            }
            return false;
        }
        public abstract bool CanUpdate(SamplingEvent item);
        public abstract bool Update(SamplingEvent item);

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.SampleEventDeletePermission);
                }
            }
            return false;
        }
        public abstract bool CanDelete(SamplingEvent item);
        public bool Delete(SamplingEvent item)
        {
            if (item != null)
                return Delete(item.Identity);
            return false;
        }
        public abstract bool Delete(CompoundIdentity id);

        public  bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, FieldActivityProviderFactoryBase.SampleEventCreatePermission);
                }
            }
            return false;
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId)
        {
            return Create(name, trip, principalOrgId, null, null);
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, DateTime startDate)
        {
            return Create(name, trip, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), null);
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange)
        {
            return Create(name, trip, principalOrgId, dateRange, null);
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, string description)
        {
            return Create(name, trip, principalOrgId, null, description);
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, DateTime startDate, string description)
        {
            return Create(name, trip, principalOrgId, new ValueRange<DateTime>(startDate, DateTime.MaxValue), description);
        }
        public SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description)
        {
            if (trip != null)
                return Create(name, trip.Identity, principalOrgId, dateRange, description);
            return null;
        }
        public abstract SamplingEvent Create(string name, CompoundIdentity tripId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
    }
}
