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
using System.Collections.Generic;

namespace Osrs.WellKnown.SensorsAndInstruments
{
	//This is currently an immediate need solution to get archetypes in place.
	//There will be no Create/Update/Delete for archetypes - they are fixed and "installed" types
	//An archetype is a fixed structure to define specific properties about an entity - each archetype has different fields from other archetypes
	//Each instrument can be of any specific archetype registered against the instrument type that instrument is an instance of... 
	//    an instrument that is of the "widget" instrument type can have an instance of any archetype supported by the "widget" instrument type
	public sealed class KnownArchetype
	{
		public CompoundIdentity Id
		{
			get;
			private set;
		}

		private string name;
		public string Name
		{
			get { return this.name; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					this.name = value;
			}
		}

		public string Description
		{
			get;
			set;
		}

		internal KnownArchetype(CompoundIdentity id, string name, string description)
		{
			MethodContract.Assert(!id.IsNullOrEmpty(), nameof(id));
			MethodContract.NotNullOrEmpty(name, nameof(name));

			this.Id = id;
			this.name = name;
			this.Description = description;
		}
	}

	public sealed class InstrumentArchetypeInstance
	{
		public CompoundIdentity InstrumentId
		{
			get;
			private set;
		}

		public CompoundIdentity ArchetypeId
		{
			get;
			private set;
		}

		private string data;
		public string Data
		{
			get { return this.data; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					this.data = value;
			}
		}

		public InstrumentArchetypeInstance(CompoundIdentity instrumentId, CompoundIdentity archetypeId, string data)
		{
			MethodContract.Assert(!InstrumentId.IsNullOrEmpty(), nameof(instrumentId));
			MethodContract.Assert(!archetypeId.IsNullOrEmpty(), nameof(archetypeId));
			MethodContract.NotNullOrEmpty(data, nameof(data));

			this.InstrumentId = instrumentId;
			this.ArchetypeId = archetypeId;
			this.Data = data;
		}
	}

	public abstract class InstrumentKnownArchetypes
	{
		//Methods for InstrumentTypeKnownArchetypes
		public abstract bool AddInstrumentTypeArchetype(CompoundIdentity archetypeId, CompoundIdentity instrumentTypeId);
		public abstract bool RemoveInstrumentTypeArchetype(CompoundIdentity archetypeId, CompoundIdentity instrumentTypeId);
		public abstract bool RemoveArchetype(CompoundIdentity archetypeId);
		public abstract bool RemoveInstrumentType(CompoundIdentity instrumentTypeId);

		public abstract IEnumerable<Tuple<CompoundIdentity, CompoundIdentity>> GetInstrumentTypeKnownArchetypes();
		public abstract IEnumerable<CompoundIdentity> ArchetypesForInstrumentType(CompoundIdentity instrumentTypeId);
		public abstract IEnumerable<CompoundIdentity> InstrumentTypesForArchetype(CompoundIdentity archetypeId);

		public IEnumerable<KnownArchetype> GetKnownArchetypes()
		{
			return knownArchetypes;
		}

		private static readonly List<KnownArchetype> knownArchetypes = new List<KnownArchetype>();

		protected InstrumentKnownArchetypes()
		{
			if (knownArchetypes.Count<1)
			{
				knownArchetypes.Add(new KnownArchetype(Archetypes.SimpleTrapDredge.id, "Trap or Dredge",  "Simple trap, dredge, or other open capture instrument"));
				knownArchetypes.Add(new KnownArchetype(Archetypes.StandardMeshNet.id, "Seine / rectangular net", "Simple seine style rectangular mesh net"));
				knownArchetypes.Add(new KnownArchetype(Archetypes.StandardPlanktonNet.id, "Plankton style net", "Standard net with open mesh and cod opening, generally a plankton net"));
				knownArchetypes.Add(new KnownArchetype(Archetypes.WingedBagNet.id, "Winged Bag Seine", "Simple seine style net with bag and wings, possibly with different mesh sizes"));
			}
		}

		//Methods for InstrumentArchetypeInstances
		public IArchetype AddInstrumentArchetype(CompoundIdentity instrumentId, CompoundIdentity archetypeId)
		{
			if (instrumentId != null && archetypeId != null)
			{
				if (archetypeId.Equals(Archetypes.SimpleTrapDredge.id))
					return this.AddSimpleTrapDredge(instrumentId);
				else if (archetypeId.Equals(Archetypes.StandardMeshNet.id))
					return this.AddStandardMeshNet(instrumentId);
				else if (archetypeId.Equals(Archetypes.StandardPlanktonNet.id))
					return this.AddStandardPlanktonNet(instrumentId);
				else if (archetypeId.Equals(Archetypes.WingedBagNet.id))
					return this.AddWingedBagNet(instrumentId);
			}
			return null;
		}

		public abstract Archetypes.SimpleTrapDredge AddSimpleTrapDredge(CompoundIdentity instrumentId);
		public abstract Archetypes.StandardMeshNet AddStandardMeshNet(CompoundIdentity instrumentId);
		public abstract Archetypes.StandardPlanktonNet AddStandardPlanktonNet(CompoundIdentity instrumentId);
		public abstract Archetypes.WingedBagNet AddWingedBagNet(CompoundIdentity instrumentId);

		public abstract bool Exists(CompoundIdentity instrumentId, CompoundIdentity archetypeId);
		public abstract IArchetype Get(CompoundIdentity instrumentId);
		public abstract bool Update(IArchetype a);
		public abstract bool Delete(CompoundIdentity instrumentId);
		
		public string GetArchetypeType(CompoundIdentity archetypeId)
		{
			if (archetypeId != null)
			{
				if (archetypeId.Equals(Archetypes.SimpleTrapDredge.id))
					return "SimpleTrapDredge";
				else if (archetypeId.Equals(Archetypes.StandardMeshNet.id))
					return "StandardMeshNet";
				else if (archetypeId.Equals(Archetypes.StandardPlanktonNet.id))
					return "StandardPlanktonNet";
				else if (archetypeId.Equals(Archetypes.WingedBagNet.id))
					return "WingedBagNet";
			}
			return null;
		}
	}

	public interface IArchetype : IIdentifiableEntity<CompoundIdentity>
	{ }
}
