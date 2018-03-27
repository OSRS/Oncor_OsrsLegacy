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
using Osrs.Numerics.Spatial.Geometry;
using Osrs.Runtime;
using System;

namespace Osrs.WellKnown.Sites
{
    public sealed class Site : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<Site>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        private CompoundIdentity owner;
        public CompoundIdentity OwningOrganizationIdentity
        {
            get { return this.owner; }
            set
            {
                if (value != null)
                    this.owner = value;
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

        public IGeometry2<double> Location
        {
            get;
            set;
        }

        public Point2<double> LocationMark
        {
            get;
            set;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as Site);
        }

        public bool Equals(Site other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public Site(CompoundIdentity id, CompoundIdentity owner, string name):this(id, owner, name, null)
        { }

        public Site(CompoundIdentity id, CompoundIdentity owner, string name, string description)
        {
            MethodContract.Assert(!id.IsNullOrEmpty(), nameof(id));
            MethodContract.NotNull(!owner.IsNullOrEmpty(), nameof(owner));
            MethodContract.NotNullOrEmpty(name, nameof(name));

            this.Identity = id;
            this.owner = owner;
            this.name = name;
            this.Description = description;
        }

        public static implicit operator CompoundIdentity(Site site)
        {
            if (site != null)
                return site.Identity;
            return null;
        }
    }
}
