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
using Osrs.Numerics;
using Osrs.Runtime;
using System;

namespace Osrs.WellKnown.FieldActivities
{
    public sealed class SamplingEvent : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<SamplingEvent>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        private CompoundIdentity tripId;
        public CompoundIdentity FieldTripId
        {
            get { return tripId; }
            set
            {
                if (!value.IsNullOrEmpty())
                    this.tripId = value;
            }
        }

        private CompoundIdentity orgId;
        public CompoundIdentity PrincipalOrgId
        {
            get { return orgId; }
            set
            {
                if (!value.IsNullOrEmpty())
                    this.orgId = value;
            }
        }

        private ValueRange<DateTime> dates;
        public ValueRange<DateTime> DateRange
        {
            get
            {
                return this.dates;
            }
            set
            {
                if (value != null)
                    this.dates = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.name = value;
            }
        }

        string INamed.Name
        {
            get
            {
                return this.name;
            }
        }

        public string Description
        {
            get;
            set;
        }

        public SamplingEvent(CompoundIdentity id, string name, CompoundIdentity fieldTripId, CompoundIdentity principalOrgId, ValueRange<DateTime> range, string description)
        {
            MethodContract.NotNullOrEmpty(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            MethodContract.NotNullOrEmpty(fieldTripId, nameof(fieldTripId));
            MethodContract.NotNullOrEmpty(principalOrgId, nameof(principalOrgId));

            this.Identity = id;
            this.name = name;
            this.tripId = fieldTripId;
            this.orgId = principalOrgId;
            this.dates = range;
            this.Description = description;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as SamplingEvent);
        }

        public bool Equals(SamplingEvent other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }
    }
}
