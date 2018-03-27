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

namespace Osrs.WellKnown.Sites
{
    public interface ISiteAliasSchemeProvider
    {
        bool CanGet();
        IEnumerable<SiteAliasScheme> Get();
        IEnumerable<SiteAliasScheme> Get(string name);
        IEnumerable<SiteAliasScheme> Get(string name, StringComparison comparisonOption);
        IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId);
        IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId, string name);
        IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption);

        SiteAliasScheme Get(CompoundIdentity id);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool Exists(CompoundIdentity owningOrgId, string name);
        bool Exists(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(SiteAliasScheme org);
        bool Update(SiteAliasScheme org);

        bool CanDelete();
        bool CanDelete(SiteAliasScheme org);
        bool Delete(SiteAliasScheme org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        SiteAliasScheme Create(CompoundIdentity owningOrgId, string name);
        SiteAliasScheme Create(CompoundIdentity owningOrgId, string name, string description);
    }
}
