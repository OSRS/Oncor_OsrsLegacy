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
using System.Collections.Generic;
using Osrs.Data;
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.Taxonomy
{
	public abstract class TaxaUnitProviderBase : ITaxaUnitProvider
	{
		protected UserSecurityContext Context
		{
			get;
		}

		private IRoleProvider prov;
		protected IRoleProvider AuthProvider
		{
			get
			{
				if (prov == null)
					prov = AuthorizationManager.Instance.GetRoleProvider(this.Context);
				return prov;
			}
		}

		public bool CanCreate()
		{
			IRoleProvider perms = this.AuthProvider;
			if(perms != null)
			{
				if(this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaUnitCreatePermission);
				}
			}
			return false;
		}

		public bool CanGet()
		{
			IRoleProvider perms = this.AuthProvider;
			if(perms != null)
			{
				if(this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaUnitGetPermission);
				}
			}
			return false;
		}

		public bool CanUpdate()
		{
			IRoleProvider perms = this.AuthProvider;
			if(perms != null)
			{
				if(this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaUnitUpdatePermission);
				}
			}
			return false;
		}

		public bool CanDelete()
		{
			IRoleProvider perms = this.AuthProvider;
			if(perms != null)
			{
				if(this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaUnitDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(TaxaUnit taxaUnit);
		public abstract bool CanUpdate(TaxaUnit taxaUnit);

		public TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId)
		{
			return this.Create(name, domainId, taxonomyId, unitTypeId, null, null);
		}
		public TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, string description)
		{
			return this.Create(name, domainId, taxonomyId, unitTypeId, null, description);
		}
		public TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, CompoundIdentity parentId)
		{
			return this.Create(name, domainId, taxonomyId, unitTypeId, parentId, null);
		}
		public abstract TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, CompoundIdentity parentId, string description);

		public bool Delete(TaxaUnit taxaUnit)
		{
			if (taxaUnit != null)
				return this.Delete(taxaUnit.Identity);
			return false;
		}
		public abstract bool Delete(CompoundIdentity id);

		public abstract bool Exists(CompoundIdentity id);
		public bool Exists(string name)
		{
			return this.Exists(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract bool Exists(string name, StringComparison comparisonOption);

		public IEnumerable<TaxaUnit> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<TaxaUnit> Get(string name, StringComparison comparisonOption);
		public abstract TaxaUnit Get(CompoundIdentity id);
		public abstract IEnumerable<TaxaUnit> Get(IEnumerable<CompoundIdentity> taxaUnitIds);

		public abstract IEnumerable<TaxaUnit> GetByTaxaDomain(CompoundIdentity domainId);
		public abstract IEnumerable<TaxaUnit> GetByTaxaUnitTypeId(CompoundIdentity taxaUnitTypeId);
		public abstract IEnumerable<TaxaUnit> GetByTaxonomy(CompoundIdentity taxonomyId);

		public IEnumerable<TaxaUnit> GetChildren(TaxaUnit parentUnit)
		{
			if(parentUnit != null)
				return this.GetChildren(parentUnit.Identity);
			return null;
		}
		public abstract IEnumerable<TaxaUnit> GetChildren(CompoundIdentity parentTaxaUnitId);
		public TaxaUnit GetParent(TaxaUnit taxaUnit)
		{
			if (taxaUnit != null)
				return this.GetParent(taxaUnit.Identity);
			return null;
		}
		public abstract TaxaUnit GetParent(CompoundIdentity taxaUnitId);

		public abstract bool HasChildren(TaxaUnit taxaUnit);
		public abstract bool HasParent(TaxaUnit taxaUnit);
		public abstract bool Update(TaxaUnit taxaUnit);

		protected  TaxaUnitProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
