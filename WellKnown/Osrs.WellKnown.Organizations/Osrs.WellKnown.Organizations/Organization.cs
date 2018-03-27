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
    public sealed class Organization : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<Organization>
    {
        public CompoundIdentity Identity
        {
            get;
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
                if (!string.IsNullOrEmpty(name))
                    this.name = value;
            }
        }

        string INamed.Name
        {
            get
            {
                return this.Name;
            }
        }

        public string Description
        {
            get;
            set;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as Organization);
        }

        public bool Equals(Organization other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public Organization(CompoundIdentity id, string name):this(id, name, null)
        { }

        public Organization(CompoundIdentity id, string name, string description)
        {
            MethodContract.NotNull(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));

            this.Identity = id;
            this.name = name;
            this.Description = description;
        }
    }
}
