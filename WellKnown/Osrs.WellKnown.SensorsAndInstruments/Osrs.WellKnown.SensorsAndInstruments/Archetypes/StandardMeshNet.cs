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

using System;
using Osrs.Data;

namespace Osrs.WellKnown.SensorsAndInstruments.Archetypes
{
    public sealed class StandardMeshNet : IArchetype
    {
        //NOTE -- this is a hack to use the same dsid as is in the pg provider -- all of these types of things should move to centralized config in a future version
        internal static readonly CompoundIdentity id = new CompoundIdentity(new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}"), new Guid("{7CCB4507-3649-40DB-A7E7-FBF7860D66CE}"));
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

        public double Length
        {
            get;
            set;
        }

        public double Depth
        {
            get;
            set;
        }

        public double MeshSize
        {
            get;
            set;
        }

        public StandardMeshNet(CompoundIdentity instrumentId, double length, double depth, double meshSize)
        {
            this.InstrumentId = instrumentId;
            this.Length = length;
            this.Depth = depth;
            this.MeshSize = meshSize;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            if (other!=null)
            {
                return this.Equals(other as StandardMeshNet);
            }
            return false;
        }

        public bool Equals(StandardMeshNet other)
        {
            if (other!=null)
            {
                return this.InstrumentId.Equals(other.InstrumentId);
            }
            return false;
        }
    }
}
