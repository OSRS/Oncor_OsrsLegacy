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
using Osrs.Runtime;

namespace Osrs.WellKnown.Taxonomy
{
	public sealed class TaxaUnitType : INamed, IDescribable, INamedEntity<CompoundIdentity>, IEquatable<TaxaUnitType>
	{
		public CompoundIdentity Identity
		{
			get;
		}

		private CompoundIdentity taxonId;
		public CompoundIdentity TaxonomyId
		{
			get { return this.taxonId; }
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
			return this.Equals(other as TaxaUnitType);
		}

		public bool Equals(TaxaUnitType other)
		{
			if (other != null)
				return this.Identity.Equals(other.Identity);
			return false;
		}

		public TaxaUnitType(CompoundIdentity id, string name, CompoundIdentity taxonomyId):this(id, name, taxonomyId, null)
		{ }

		public TaxaUnitType(CompoundIdentity id, string name, CompoundIdentity taxonomyId, string description)
		{
			MethodContract.NotNull(id, nameof(id));
			MethodContract.NotNull(taxonomyId, nameof(taxonomyId));
			MethodContract.NotNullOrEmpty(name, nameof(name));

			this.Identity = id;
			this.taxonId = taxonomyId;
			this.name = name;
			this.Description = description;
		}
	}
}
