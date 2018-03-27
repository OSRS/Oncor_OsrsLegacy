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

using Osrs.Data;
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.Organizations
{
    public interface IOrganizationAliasSchemeProvider
    {
        bool CanGet();
        IEnumerable<OrganizationAliasScheme> Get();
        IEnumerable<OrganizationAliasScheme> Get(Organization owner);
        IEnumerable<OrganizationAliasScheme> Get(string name);
        IEnumerable<OrganizationAliasScheme> Get(Organization owner, string name);
        IEnumerable<OrganizationAliasScheme> Get(string name, StringComparison comparisonOption);
        IEnumerable<OrganizationAliasScheme> Get(Organization owner, string name, StringComparison comparisonOption);

        OrganizationAliasScheme Get(CompoundIdentity id);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(Organization owner, string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool Exists(Organization owner, string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(OrganizationAliasScheme org);
        bool Update(OrganizationAliasScheme org);

        bool CanDelete();
        bool CanDelete(OrganizationAliasScheme org);
        bool Delete(OrganizationAliasScheme org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        OrganizationAliasScheme Create(Organization owner, string name);
        OrganizationAliasScheme Create(Organization owner, string name, string description);
    }
}
