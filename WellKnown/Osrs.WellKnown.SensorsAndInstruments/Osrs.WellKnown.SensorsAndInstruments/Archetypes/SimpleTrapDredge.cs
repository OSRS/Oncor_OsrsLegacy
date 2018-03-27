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
    public sealed class SimpleTrapDredge : IArchetype
    {
        //NOTE -- this is a hack to use the same dsid as is in the pg provider -- all of these types of things should move to centralized config in a future version
        internal static readonly CompoundIdentity id = new CompoundIdentity(new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}"), new Guid("{415F2DD7-4E0E-4DA5-AAF7-09C26FCF4EE0}"));
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

        public SimpleTrapDredge(CompoundIdentity instrumentId, double openArea)
        {
            this.InstrumentId = instrumentId;
            this.OpenArea = openArea;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            if (other != null)
            {
                return this.Equals(other as SimpleTrapDredge);
            }
            return false;
        }

        public bool Equals(SimpleTrapDredge other)
        {
            if (other != null)
            {
                return this.InstrumentId.Equals(other.InstrumentId);
            }
            return false;
        }
    }
}
