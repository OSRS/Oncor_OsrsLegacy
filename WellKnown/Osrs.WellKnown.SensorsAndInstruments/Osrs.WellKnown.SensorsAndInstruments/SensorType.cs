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
using System.Collections;
using System.Collections.Generic;

namespace Osrs.WellKnown.SensorsAndInstruments
{
    public sealed class SensorType : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<SensorType>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        /// <summary>
        /// NOTE: this is implicitly created as an empty set and must be filled after create, then updated
        /// </summary>
        public SensorTypeInstrumentFamilies InstrumentFamilies
        {
            get;
        } = new SensorTypeInstrumentFamilies();

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

        public CompoundIdentity ParentId
        {
            get;
            set;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as SensorType);
        }

        public bool Equals(SensorType other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public SensorType(CompoundIdentity id, string name):this(id, name, null, null)
        { }

        public SensorType(CompoundIdentity id, string name, string description):this(id, name, null, null)
        { }

        public SensorType(CompoundIdentity id, string name, CompoundIdentity parentId) : this(id, name, null, null)
        { }

        public SensorType(CompoundIdentity id, string name, string description, CompoundIdentity parentId)
        {
            MethodContract.Assert(!id.IsNullOrEmpty(), nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));

            this.Identity = id;
            this.name = name;
            this.Description = description;
            this.ParentId = parentId;
        }

        public static implicit operator CompoundIdentity(SensorType item)
        {
            if (item != null)
                return item.Identity;
            return null;
        }
    }

    public sealed class SensorTypeInstrumentFamilies : IEnumerable<CompoundIdentity>
    {
        public CompoundIdentity SensorTypeId;
        private HashSet<CompoundIdentity> members = new HashSet<CompoundIdentity>();

        public int Count
        {
            get { return this.members.Count; }
        }

        public bool Add(CompoundIdentity member)
        {
            if (member != null)
            {
                if (!this.Contains(member))
                {
                    this.members.Add(member);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(CompoundIdentity member)
        {
            if (member != null)
            {
                return this.members.Remove(member);
            }
            return false;
        }

        public bool Contains(CompoundIdentity member)
        {
            if (member != null)
            {
                foreach (CompoundIdentity cur in this.members)
                {
                    if (member.Equals(cur))
                        return true;
                }
            }
            return false;
        }

        public IEnumerator<CompoundIdentity> GetEnumerator()
        {
            return this.members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.members.GetEnumerator();
        }
    }
}
