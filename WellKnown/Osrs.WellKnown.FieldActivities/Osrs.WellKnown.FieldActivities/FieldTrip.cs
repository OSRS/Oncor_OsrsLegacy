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
    public sealed class FieldTrip : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<FieldTrip>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        private CompoundIdentity actId;
        public CompoundIdentity FieldActivityId
        {
            get { return actId; }
            set
            {
                if (!value.IsNullOrEmpty())
                    this.actId = value;
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

        public FieldTrip(CompoundIdentity id, string name, CompoundIdentity fieldActivityId, CompoundIdentity principalOrgId, ValueRange<DateTime> range, string description)
        {
            MethodContract.NotNullOrEmpty(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            MethodContract.NotNullOrEmpty(fieldActivityId, nameof(fieldActivityId));
            MethodContract.NotNullOrEmpty(principalOrgId, nameof(principalOrgId));

            this.Identity = id;
            this.name = name;
            this.actId = fieldActivityId;
            this.orgId = principalOrgId;
            this.dates = range;
            this.Description = description;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as FieldTrip);
        }

        public bool Equals(FieldTrip other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }
    }
}
