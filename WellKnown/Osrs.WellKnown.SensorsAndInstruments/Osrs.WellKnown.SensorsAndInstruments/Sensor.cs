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

namespace Osrs.WellKnown.SensorsAndInstruments
{
    public sealed class Sensor : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<Sensor>
    {
        /// <summary>
        /// Mandatory, immutable, created by provider at create
        /// </summary>
        public CompoundIdentity Identity
        {
            get;
        }

        private string name;
        /// <summary>
        /// Mandatory, mutable, not null or empty
        /// </summary>
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

        /// <summary>
        /// Optional, null or empty allowed
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Optional, null or empty allowed
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Optional, null or empty allowed
        /// </summary>
        public CompoundIdentity ManufacturerId
        {
            get;
            set;
        }

        private CompoundIdentity typeId;
        /// <summary>
        /// Mandatory, mutable, not null or empty
        /// </summary>
        public CompoundIdentity SensorTypeIdentity
        {
            get { return this.typeId; }
            set
            {
                if (value != null)
                    this.typeId = value;
            }
        }

        private CompoundIdentity owner;
        /// <summary>
        /// Mandatory, mutable, not null or empty
        /// </summary>
        public CompoundIdentity OwningOrganizationIdentity
        {
            get { return this.owner; }
            set
            {
                if (value != null)
                    this.owner = value;
            }
        }

        private CompoundIdentity instrument;
        /// <summary>
        /// Mandatory, mutable, not null or empty
        /// </summary>
        public CompoundIdentity InstrumentIdentity
        {
            get { return this.instrument; }
            set
            {
                if (value != null)
                    this.instrument = value;
            }
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as Sensor);
        }

        public bool Equals(Sensor other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public Sensor(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId) :this(id, instrumentId, ownerId, name, typeId, null, null, null)
        { }

        public Sensor(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description):this(id, instrumentId, ownerId, name, typeId, description, null, null)
        { }

        public Sensor(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, CompoundIdentity manufId) : this(id, instrumentId, ownerId, name, typeId, null, null, manufId)
        { }

        public Sensor(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string serialNumber, CompoundIdentity manufId) : this(id, instrumentId, ownerId, name, typeId, null, serialNumber, manufId)
        { }

        public Sensor(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId)
        {
            MethodContract.Assert(!id.IsNullOrEmpty(), nameof(id));
            MethodContract.Assert(!instrumentId.IsNullOrEmpty(), nameof(instrumentId));
            MethodContract.Assert(!ownerId.IsNullOrEmpty(), nameof(ownerId));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            MethodContract.Assert(!typeId.IsNullOrEmpty(), nameof(typeId));

            this.Identity = id;
            this.instrument = instrumentId;
            this.owner = ownerId;
            this.name = name;
            this.typeId = typeId;
            this.Description = description;
            this.SerialNumber = serialNumber;
            this.ManufacturerId = manufId;
        }

        public static implicit operator CompoundIdentity(Sensor item)
        {
            if (item != null)
                return item.Identity;
            return null;
        }
    }
}
