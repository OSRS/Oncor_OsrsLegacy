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

namespace Osrs.WellKnown.Taxonomy
{
	public abstract class TaxonomyProviderFactoryBase : SubclassableSingletonBase<TaxonomyProviderFactoryBase>
	{
		protected internal UserSecurityContext LocalContext
		{
			get;
			internal set;
		}

		protected internal static Permission TaxaCommonNameCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameCreatePermissionId);
			}
		}
		protected internal static Permission TaxaCommonNameGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameGetPermissionId);
			}
		}
		protected internal static Permission TaxaCommonNameUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameUpdatePermissionId);
			}
		}
		protected internal static Permission TaxaCommonNameDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameDeletePermissionId);
			}
		}

		protected internal static Permission TaxaUnitCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaUnit"), TaxonomyUtils.TaxaUnitCreatePermissionId);
			}
		}
		protected internal static Permission TaxaUnitGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaUnit"), TaxonomyUtils.TaxaUnitGetPermissionId);
			}
		}
		protected internal static Permission TaxaUnitUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaUnit"), TaxonomyUtils.TaxaUnitUpdatePermissionId);
			}
		}
		protected internal static Permission TaxaUnitDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaUnit"), TaxonomyUtils.TaxaUnitDeletePermissionId);
			}
		}

		protected internal static Permission TaxonomyCreatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Taxonomy"), TaxonomyUtils.TaxonomyCreatePermissionId);
			}
		}
		protected internal static Permission TaxonomyGetPermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Taxonomy"), TaxonomyUtils.TaxonomyGetPermissionId);
			}
		}
		protected internal static Permission TaxonomyUpdatePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Taxonomy"), TaxonomyUtils.TaxonomyUpdatePermissionId);
			}
		}
		protected internal static Permission TaxonomyDeletePermission
		{
			get
			{
				return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Taxonomy"), TaxonomyUtils.TaxonomyDeletePermissionId);
			}
		}

		protected internal abstract bool Initialize();

		protected internal abstract TaxaCommonNameProviderBase GetTaxaCommonNameProvider(UserSecurityContext context);

		protected internal abstract TaxaDomainProviderBase GetTaxaDomainProvider(UserSecurityContext context);

		protected internal abstract TaxaUnitProviderBase GetTaxaUnitProvider(UserSecurityContext context);

		protected internal abstract TaxaUnitTypeProviderBase GetTaxaUnitTypeProvider(UserSecurityContext context);

		protected internal abstract TaxonomyProviderBase GetTaxonomyProvider(UserSecurityContext context);

		protected internal abstract TaxaDomainUnitTypeProviderBase GetTaxaDomainUnitTypeProvider(UserSecurityContext context);
	}
}
