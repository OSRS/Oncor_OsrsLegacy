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
	public abstract class SensorTypeProviderBase : ISensorTypeProvider
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorTypeCreatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorTypeGetPermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorTypeUpdatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorTypeDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(SensorType org);
		public abstract bool CanUpdate(SensorType org);
		public SensorType Create(string name)
		{
			return this.Create(name, null, null);
		}
		public SensorType Create(string name, string description)
		{
			return this.Create(name, description, null);
		}
		public SensorType Create(string name, CompoundIdentity parentId)
		{
			return this.Create(name, null, parentId);
		}
		public abstract SensorType Create(string name, string description, CompoundIdentity parentId);
		public bool Delete(SensorType org)
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
		public abstract IEnumerable<SensorType> Get();
		public IEnumerable<SensorType> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<SensorType> Get(string name, StringComparison comparisonOption);
		public abstract SensorType Get(CompoundIdentity id);
		public abstract IEnumerable<SensorType> Get(IEnumerable<CompoundIdentity> ids);
		public abstract IEnumerable<SensorType> GetChildren(CompoundIdentity id);
		public abstract IEnumerable<SensorType> GetFor(CompoundIdentity instrumentFamilyId);
		public abstract IEnumerable<SensorType> GetFor(IEnumerable<CompoundIdentity> instrumentFamilyId);
		public abstract bool Update(SensorType org);

		protected SensorTypeProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
