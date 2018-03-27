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
using Osrs.Runtime;
using System;

namespace Osrs.WellKnown.Organizations
{
    public sealed class OrganizationAlias : INamed, IEquatable<OrganizationAlias>
    {
        internal readonly string origName;
        internal bool isDirty
        {
            get { return this.origName != this.name; }
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

        public CompoundIdentity Identity
        {
            get;
        }

        public CompoundIdentity AliasSchemeIdentity
        {
            get;
        }

        public bool OrganizationEquals(Organization other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public bool OrganizationEquals(OrganizationAlias other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public bool Equals(OrganizationAlias other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity) && this.AliasSchemeIdentity.Equals(other.AliasSchemeIdentity);
            return false;
        }

        public OrganizationAlias(CompoundIdentity orgId, CompoundIdentity schemeId, string name)
        {
            MethodContract.NotNull(orgId, nameof(orgId));
            MethodContract.NotNull(schemeId, nameof(schemeId));
            MethodContract.NotNullOrEmpty(name, nameof(name));

            this.Identity = orgId;
            this.AliasSchemeIdentity = schemeId;
            this.name = name;
            this.origName = name;
        }
    }
}
