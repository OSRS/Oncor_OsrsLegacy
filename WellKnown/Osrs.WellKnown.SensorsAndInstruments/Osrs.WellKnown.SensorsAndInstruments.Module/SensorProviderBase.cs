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
	public abstract class SensorProviderBase : ISensorProvider
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorCreatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorGetPermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorUpdatePermission);
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
					return perms.HasPermission(this.Context.User, InstrumentProviderFactoryBase.SensorDeletePermission);
				}
			}
			return false;
		}

		public abstract bool CanDelete(Sensor item);
		public abstract bool CanUpdate(Sensor item);
		public Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId)
		{
			return this.Create(id, instrumentId, ownerId, name, typeId, null, null, null);
		}
		public Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description)
		{
			return this.Create(id, instrumentId, ownerId, name, typeId, description, null, null);
		}
		public Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, CompoundIdentity manufId)
		{
			return this.Create(id, instrumentId, ownerId, name, typeId, null, null, manufId);
		}
		public Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string serialNumber, CompoundIdentity manufId)
		{
			return this.Create(id, instrumentId, ownerId, name, typeId, null, serialNumber, manufId);
		}
		public abstract Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId);
		public bool Delete(Sensor item)
		{
			if (item != null)
				return this.Delete(item.Identity);
			return false;
		}
		public abstract bool Delete(CompoundIdentity id);
		public abstract bool Exists(CompoundIdentity id);
		public bool Exists(string name)
		{
			return this.Exists(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract bool Exists(string name, StringComparison comparisonOption);
		public abstract IEnumerable<Sensor> Get();
		public IEnumerable<Sensor> Get(string name)
		{
			return this.Get(name, StringComparison.OrdinalIgnoreCase);
		}
		public abstract IEnumerable<Sensor> Get(string name, StringComparison comparisonOption);
		public abstract Sensor Get(CompoundIdentity id);
		public abstract IEnumerable<Sensor> Get(IEnumerable<CompoundIdentity> ids);
		public abstract IEnumerable<Sensor> GetFor(string serialNumber);
		public abstract IEnumerable<Sensor> GetFor(IEnumerable<string> serialNumber);
		public abstract IEnumerable<Sensor> GetForInstrument(CompoundIdentity instrumentId);
		public abstract IEnumerable<Sensor> GetForInstrument(IEnumerable<CompoundIdentity> instrumentId);
		public abstract IEnumerable<Sensor> GetForManufacturer(CompoundIdentity manufId);
		public abstract IEnumerable<Sensor> GetForManufacturer(IEnumerable<CompoundIdentity> manufId);
		public abstract IEnumerable<Sensor> GetForOwner(CompoundIdentity ownerId);
		public abstract IEnumerable<Sensor> GetForOwner(IEnumerable<CompoundIdentity> ownerId);
		public abstract IEnumerable<Sensor> GetForSensorType(CompoundIdentity typeId);
		public abstract IEnumerable<Sensor> GetForSensorType(IEnumerable<CompoundIdentity> typeId);
		public abstract bool Update(Sensor item);

		protected SensorProviderBase(UserSecurityContext context)
		{
			this.Context = context;
		}
	}
}
