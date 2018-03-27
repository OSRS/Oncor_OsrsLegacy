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
	public interface ITaxaDomainProvider
	{
		bool CanGet();
		IEnumerable<TaxaDomain> Get();
		IEnumerable<TaxaDomain> Get(string name);
		IEnumerable<TaxaDomain> Get(string name, StringComparison comparisonOption);

		TaxaDomain Get(CompoundIdentity id);

		bool Exists(CompoundIdentity id);
		bool Exists(string name);
		bool Exists(string name, StringComparison comparisonOption);

		bool CanUpdate();
		bool CanUpdate(TaxaDomain domain);
		bool Update(TaxaDomain domain);

		bool CanDelete();
		bool CanDelete(TaxaDomain domain);
		bool Delete(TaxaDomain domain);
		bool Delete(CompoundIdentity id);

		bool CanCreate();
		TaxaDomain Create(string name, CompoundIdentity taxonomyId);
		TaxaDomain Create(string name, CompoundIdentity taxonomyId, string description);
	}
}
