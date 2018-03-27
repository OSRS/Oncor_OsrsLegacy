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
using Osrs.Security;
using Osrs.Security.Authorization;
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.Taxonomy
{
	public abstract class TaxaCommonNameProviderBase : ITaxaCommonNameProvider
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
				if(prov == null)
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaCommonNameCreatePermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaCommonNameGetPermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaCommonNameUpdatePermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxaCommonNameDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(TaxaCommonName commonName);
		public abstract bool CanUpdate(TaxaCommonName commonName);

		public TaxaCommonName Create(string name)
		{
			return this.Create(name, null);
		}

		public abstract TaxaCommonName Create(string name, string description);

		public bool Delete(TaxaCommonName commonName)
		{
			if(commonName != null)
				return this.Delete(commonName.Identity);
			return false;
		}

		public abstract bool Delete(CompoundIdentity id);
		public abstract bool Exists(CompoundIdentity id);

		public bool Exists(string name)
		{
			return this.Exists(name, StringComparison.OrdinalIgnoreCase);
		}

		public abstract bool Exists(string name, StringComparison comparisonOption);

		public IEnumerable<TaxaCommonName> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}

		public abstract IEnumerable<TaxaCommonName> Get(string name, StringComparison comparisonOption);
		public abstract TaxaCommonName Get(CompoundIdentity id);
		public abstract bool Update(TaxaCommonName commonName);

		public abstract IEnumerable<TaxaCommonName> GetCommonNamesByTaxa(TaxaUnit taxa);
		public abstract IEnumerable<TaxaUnit> GetTaxaUnitsByCommonName(TaxaCommonName commonName);
		
		public bool Add(TaxaUnit taxaUnit, TaxaCommonName taxaCommonName)
		{
			return this.Add(taxaUnit.Identity, taxaCommonName.Identity);
		}
		public abstract bool Add(CompoundIdentity taxaUnitId, CompoundIdentity commonNameId);

		public bool Remove(TaxaUnit taxaUnit, TaxaCommonName taxaCommonName)
		{
			return this.Remove(taxaUnit.Identity, taxaCommonName.Identity);
		}
		public abstract bool Remove(CompoundIdentity taxaUnitId, CompoundIdentity commonNameId);

		protected TaxaCommonNameProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
