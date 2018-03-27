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
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.Data;

namespace Osrs.WellKnown.SensorsAndInstruments
{
	public abstract class InstrumentFamilyProviderBase : IInstrumentFamilyProvider
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
			if (perms != null)
			{
				if (this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentFamilyCreatePermission);
				}
			}
			return false;
		}

		public bool CanGet()
		{
			IRoleProvider perms = this.AuthProvider;
			if (perms != null)
			{
				if (this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentFamilyGetPermission);
				}
			}
			return false;
		}

		public bool CanUpdate()
		{
			IRoleProvider perms = this.AuthProvider;
			if (perms != null)
			{
				if (this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentFamilyUpdatePermission);
				}
			}
			return false;
		}

		public bool CanDelete()
		{
			IRoleProvider perms = this.AuthProvider;
			if (perms != null)
			{
				if (this.Context != null && this.Context.User != null)
				{
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentFamilyDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(InstrumentFamily org);
		public abstract bool CanUpdate(InstrumentFamily org);
		public InstrumentFamily Create(string name)
		{
			return this.Create(name, null, null);
		}
		public InstrumentFamily Create(string name, string description)
		{
			return this.Create(name, description, null);
		}
		public InstrumentFamily Create(string name, CompoundIdentity parentId)
		{
			return this.Create(name, null, parentId);
		}
		public abstract InstrumentFamily Create(string name, string description, CompoundIdentity parentId);
		public bool Delete(InstrumentFamily org)
		{
			if (org != null)
				return this.Delete(org.Identity);
			return false;
		}
		public abstract bool Delete(CompoundIdentity id);
		public abstract bool Exists(CompoundIdentity id);
		public bool Exists(string name)
		{
			return this.Exists(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract bool Exists(string name, StringComparison comparisonOption);
		public abstract IEnumerable<InstrumentFamily> Get();
		public IEnumerable<InstrumentFamily> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<InstrumentFamily> Get(string name, StringComparison comparisonOption);
		public abstract InstrumentFamily Get(CompoundIdentity id);
		public abstract IEnumerable<InstrumentFamily> Get(IEnumerable<CompoundIdentity> ids);
		public abstract IEnumerable<InstrumentFamily> GetChildren(CompoundIdentity id);
		public abstract bool Update(InstrumentFamily org);

		protected InstrumentFamilyProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
