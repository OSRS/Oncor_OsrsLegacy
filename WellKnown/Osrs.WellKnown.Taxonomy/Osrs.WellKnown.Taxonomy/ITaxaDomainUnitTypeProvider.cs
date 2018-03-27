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

namespace Osrs.WellKnown.Taxonomy
{
	public interface ITaxaDomainUnitTypeProvider
	{
		bool CanGet();
		IEnumerable<TaxaUnitType> GetTaxaUnitTypeByDomain(TaxaDomain domain);
		IEnumerable<TaxaUnitType> GetDescendants(TaxaDomain domain, TaxaUnitType taxaUnitType);
		//IEnumerable<TaxaUnitType> GetAncestors(TaxaUnitType taxaUnitType);

		TaxaUnitType GetParent(TaxaDomain domain, TaxaUnitType child);
		IEnumerable<TaxaUnitType> GetChildren(TaxaDomain domain, TaxaUnitType parent);

		bool AddChild(CompoundIdentity domainId, CompoundIdentity parentId, CompoundIdentity childId);
		bool RemoveChild(CompoundIdentity domainId, CompoundIdentity parentId, CompoundIdentity childId);

		bool CanDelete();

		//Implementation must check that taxonomy ids must match
		bool IsParentOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child);
		bool IsChildOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child);
		bool IsDescendantOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child);
		bool IsAncestorOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child);

		bool CanUpdate();
		bool CanCreate();
	}
}
