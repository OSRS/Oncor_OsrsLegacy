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
using System;

namespace Osrs.WellKnown.SensorsAndInstruments.Archetypes
{
    public sealed class StandardPlanktonNet : IArchetype
    {
        //NOTE -- this is a hack to use the same dsid as is in the pg provider -- all of these types of things should move to centralized config in a future version
        internal static readonly CompoundIdentity id = new CompoundIdentity(new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}"), new Guid("{E9505346-6BD4-4049-937E-C93034A7052E}"));
        public CompoundIdentity Identity
        {
            get
            {
                return id;
            }
        }

        public CompoundIdentity InstrumentId
        {
            get;
            private set;
        }

        public double OpenArea
        {
            get;
            set;
        }

        public double MeshSize
        {
            get;
            set;
        }

        public double CodSize
        {
            get;
            set;
        }

        public StandardPlanktonNet(CompoundIdentity instrumentId, double openArea, double meshSize, double codSize)
        {
            this.InstrumentId = instrumentId;
            this.OpenArea = openArea;
            this.MeshSize = meshSize;
            this.CodSize = codSize;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            if (other != null)
            {
                return this.Equals(other as StandardPlanktonNet);
            }
            return false;
        }

        public bool Equals(StandardPlanktonNet other)
        {
            if (other != null)
            {
                return this.InstrumentId.Equals(other.InstrumentId);
            }
            return false;
        }
    }
}
