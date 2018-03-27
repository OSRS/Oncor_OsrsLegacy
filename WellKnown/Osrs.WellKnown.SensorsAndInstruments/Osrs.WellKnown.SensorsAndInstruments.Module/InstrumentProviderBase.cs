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
	public abstract class InstrumentProviderBase : IInstrumentProvider
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentCreatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentGetPermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentUpdatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.InstrumentDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(Instrument org);
		public abstract bool CanUpdate(Instrument org);
		public Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId)
		{
			return this.Create(ownerId, name, typeId, null, null, null);
		}
		public Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description)
		{
			return this.Create(ownerId, name, typeId, description, null, null);
		}
		public Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, CompoundIdentity manufId)
		{
			return this.Create(ownerId, name, typeId, null, null, manufId);
		}
		public Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string serialNumber, CompoundIdentity manufId)
		{
			return this.Create(ownerId, name, typeId, null, serialNumber, manufId);
		}
		public abstract Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId);
		public bool Delete(Instrument org)
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
		public abstract IEnumerable<Instrument> Get();
		public IEnumerable<Instrument> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<Instrument> Get(string name, StringComparison comparisonOption);
		public abstract Instrument Get(CompoundIdentity id);
		public abstract IEnumerable<Instrument> Get(IEnumerable<CompoundIdentity> ids);
		public abstract IEnumerable<Instrument> GetForInstrumentType(CompoundIdentity typeId);
		public abstract IEnumerable<Instrument> GetForInstrumentType(IEnumerable<CompoundIdentity> typeId);
		public abstract IEnumerable<Instrument> GetForManufacturer(CompoundIdentity manufId);
		public abstract IEnumerable<Instrument> GetForManufacturer(IEnumerable<CompoundIdentity> manufId);
		public abstract IEnumerable<Instrument> GetForOwner(CompoundIdentity ownerId);
		public abstract IEnumerable<Instrument> GetForOwner(IEnumerable<CompoundIdentity> ownerId);
		public abstract bool Update(Instrument org);

		protected InstrumentProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
