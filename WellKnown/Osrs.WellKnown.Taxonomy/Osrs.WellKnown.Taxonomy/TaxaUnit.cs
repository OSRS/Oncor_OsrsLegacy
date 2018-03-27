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
	public sealed class TaxaUnit : INamed, IDescribable, INamedEntity<CompoundIdentity>, IEquatable<TaxaUnit>
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

		private CompoundIdentity taxaDomainId;
		public CompoundIdentity TaxaDomainId
		{
			get { return this.taxaDomainId; }
			set
			{
				if (value != null)
					this.taxaDomainId = value;
			}
		}

		private CompoundIdentity taxonomyId;
		public CompoundIdentity TaxonomyId
		{
			get { return this.taxonomyId; }
		}

		private CompoundIdentity taxaUnitTypeId;
		public CompoundIdentity TaxaUnitTypeId
		{
			get { return this.taxaUnitTypeId; }
			set
			{
				if (value != null)
					this.taxaUnitTypeId = value;
			}
		}
		private CompoundIdentity parentId;
		public CompoundIdentity ParentId
		{
			get { return this.parentId; }
			set
			{
				this.parentId = value;
			}
		}

		public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
		{
			return this.Equals(other as TaxaUnit);
		}

		public bool Equals(TaxaUnit other)
		{
			if (other != null)
				return this.Identity.Equals(other.Identity);
			return false;
		}

		public TaxaUnit(CompoundIdentity id, string name, CompoundIdentity taxaDomainId, CompoundIdentity taxonomyId, CompoundIdentity taxaUnitTypeId):this(id, name, taxaDomainId, taxonomyId, taxaUnitTypeId, null, null)
		{ }

		public TaxaUnit(CompoundIdentity id, string name, CompoundIdentity taxaDomainId, CompoundIdentity taxonomyId, CompoundIdentity taxaUnitTypeId, CompoundIdentity parentId):this(id, name, taxaDomainId, taxonomyId, taxaUnitTypeId, parentId, null)
		{ }

		public TaxaUnit(CompoundIdentity id, string name, CompoundIdentity taxaDomainId, CompoundIdentity taxonomyId, CompoundIdentity taxaUnitTypeId, string description):this(id, name, taxaDomainId, taxonomyId, taxaUnitTypeId, null, description)
		{ }

		public TaxaUnit(CompoundIdentity id, string name, CompoundIdentity taxaDomainId, CompoundIdentity taxonomyId, CompoundIdentity taxaUnitTypeId, CompoundIdentity parentId, string description)
		{
			MethodContract.NotNull(id, nameof(id));
			MethodContract.NotNullOrEmpty(name, nameof(name));
			MethodContract.NotNull(taxaDomainId, nameof(taxaDomainId));
			MethodContract.NotNull(taxonomyId, nameof(taxonomyId));
			MethodContract.NotNull(taxaUnitTypeId, nameof(taxaUnitTypeId));

			this.Identity = id;
			this.name = name;
			this.taxaDomainId = taxaDomainId;
			this.taxonomyId = taxonomyId;
			this.taxaUnitTypeId = taxaUnitTypeId;
			this.parentId = parentId;
			this.Description = description;
		}
	}
}
