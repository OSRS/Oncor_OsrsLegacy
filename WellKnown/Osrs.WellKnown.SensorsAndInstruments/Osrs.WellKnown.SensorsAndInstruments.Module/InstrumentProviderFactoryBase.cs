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

using Osrs.Runtime;
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.SensorsAndInstruments
{
	public abstract class InstrumentProviderFactoryBase : SubclassableSingletonBase<InstrumentProviderFactoryBase>
	{
		protected internal UserSecurityContext LocalContext
		{
			get;
			internal set;
		}

		protected internal abstract bool Initialize();

		protected internal static Permission InstrumentTypeCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentType"), InstrumentUtils.InstrumentTypeCreatePermissionId);
			}
		}
		protected internal static Permission InstrumentTypeGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentType"), InstrumentUtils.InstrumentTypeGetPermissionId);
			}
		}
		protected internal static Permission InstrumentTypeUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentType"), InstrumentUtils.InstrumentTypeUpdatePermissionId);
			}
		}
		protected internal static Permission InstrumentTypeDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentType"), InstrumentUtils.InstrumentTypeDeletePermissionId);
			}
		}

		protected internal static Permission InstrumentFamilyCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyCreatePermissionId);
			}
		}
		protected internal static Permission InstrumentFamilyGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyGetPermissionId);
			}
		}
		protected internal static Permission InstrumentFamilyUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyUpdatePermissionId);
			}
		}
		protected internal static Permission InstrumentFamilyDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyDeletePermissionId);
			}
		}

		protected internal static Permission InstrumentCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Instrument"), InstrumentUtils.InstrumentCreatePermissionId);
			}
		}
		protected internal static Permission InstrumentGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Instrument"), InstrumentUtils.InstrumentGetPermissionId);
			}
		}
		protected internal static Permission InstrumentUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Instrument"), InstrumentUtils.InstrumentUpdatePermissionId);
			}
		}
		protected internal static Permission InstrumentDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Instrument"), InstrumentUtils.InstrumentDeletePermissionId);
			}
		}

		protected internal static Permission SensorTypeCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "SensorType"), InstrumentUtils.SensorTypeCreatePermissionId);
			}
		}
		protected internal static Permission SensorTypeGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SensorType"), InstrumentUtils.SensorTypeGetPermissionId);
			}
		}
		protected internal static Permission SensorTypeUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "SensorType"), InstrumentUtils.SensorTypeUpdatePermissionId);
			}
		}
		protected internal static Permission SensorTypeDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SensorType"), InstrumentUtils.SensorTypeDeletePermissionId);
			}
		}

		protected internal static Permission SensorCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Sensor"), InstrumentUtils.SensorCreatePermissionId);
			}
		}
		protected internal static Permission SensorGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Sensor"), InstrumentUtils.SensorGetPermissionId);
			}
		}
		protected internal static Permission SensorUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Sensor"), InstrumentUtils.SensorUpdatePermissionId);
			}
		}
		protected internal static Permission SensorDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Sensor"), InstrumentUtils.SensorDeletePermissionId);
			}
		}

		protected internal static Permission InstrumentKnownArchetypeCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeCreatePermissionId);
			}
		}
		protected internal static Permission InstrumentKnownArchetypeGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeGetPermissionId);
			}
		}
		protected internal static Permission InstrumentKnownArchetypeUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeUpdatePermissionId);
			}
		}
		protected internal static Permission InstrumentKnownArchetypeDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeDeletePermissionId);
			}
		}

		protected internal abstract InstrumentFamilyProviderBase GetInstrumentFamilyProvider(UserSecurityContext context);
		protected internal abstract InstrumentTypeProviderBase GetInstrumentTypeProvider(UserSecurityContext context);
		protected internal abstract InstrumentProviderBase GetInstrumentProvider(UserSecurityContext context);
		protected internal abstract SensorTypeProviderBase GetSensorTypeProvider(UserSecurityContext context);
		protected internal abstract SensorProviderBase GetSensorProvider(UserSecurityContext context);
		protected internal abstract InstrumentKnownArchetypeProviderBase GetKnownArchetypesProvider(UserSecurityContext context);
	}
}
