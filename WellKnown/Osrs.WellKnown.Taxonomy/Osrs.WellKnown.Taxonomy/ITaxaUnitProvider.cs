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
	public interface ITaxaUnitProvider
	{
		bool CanGet();
		IEnumerable<TaxaUnit> Get(string name);
		IEnumerable<TaxaUnit> Get(string name, StringComparison comparisonOption);
		IEnumerable<TaxaUnit> GetByTaxaUnitTypeId(CompoundIdentity taxaUnitTypeId);
		IEnumerable<TaxaUnit> GetByTaxaDomain(CompoundIdentity domainId);
		IEnumerable<TaxaUnit> GetByTaxonomy(CompoundIdentity taxonomyId);

		TaxaUnit Get(CompoundIdentity id);
		IEnumerable<TaxaUnit> Get(IEnumerable<CompoundIdentity> taxaUnitIds);

		bool HasParent(TaxaUnit taxaUnit);
		bool HasChildren(TaxaUnit taxaUnit);
		TaxaUnit GetParent(TaxaUnit taxaUnit);
		TaxaUnit GetParent(CompoundIdentity taxaUnitId);
		IEnumerable<TaxaUnit> GetChildren(TaxaUnit parentUnit);
		IEnumerable<TaxaUnit> GetChildren(CompoundIdentity parentTaxaUnitId);

		bool Exists(CompoundIdentity id);
		bool Exists(string name);
		bool Exists(string name, StringComparison comparisonOption);

		bool CanUpdate();
		bool CanUpdate(TaxaUnit taxaUnit);
		bool Update(TaxaUnit taxaUnit);

		bool CanDelete();
		bool CanDelete(TaxaUnit taxaUnit);
		bool Delete(TaxaUnit taxaUnit);
		bool Delete(CompoundIdentity id);

		bool CanCreate();
		TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId);
		TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, string description);
		TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, CompoundIdentity parentId);
		TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, CompoundIdentity parentId, string description);
	}
}
