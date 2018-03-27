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
	public abstract class TaxonomyProviderBase : ITaxonomyProvider
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxonomyCreatePermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxonomyGetPermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxonomyUpdatePermission);
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
					return perms.HasPermission(this.Context.User, TaxonomyProviderFactoryBase.TaxonomyDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(Taxonomy taxonomy);
		public abstract bool CanUpdate(Taxonomy taxonomy);

		public Taxonomy Create(string name)
		{
			return this.Create(name, null);
		}
		public abstract Taxonomy Create(string name, string description);

		public bool Delete(Taxonomy taxonomy)
		{
			if (taxonomy != null)
				return this.Delete(taxonomy.Identity);
			return false;
		}
		public abstract bool Delete(CompoundIdentity id);

		public abstract bool Exists(CompoundIdentity id);
		public bool Exists(string name)
		{
			return this.Exists(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract bool Exists(string name, StringComparison comparisonOption);

		public abstract IEnumerable<Taxonomy> Get();
		public IEnumerable<Taxonomy> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<Taxonomy> Get(string name, StringComparison comparisonOption);
		public abstract Taxonomy Get(CompoundIdentity id);
		public abstract bool Update(Taxonomy taxonomy);

		protected TaxonomyProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
