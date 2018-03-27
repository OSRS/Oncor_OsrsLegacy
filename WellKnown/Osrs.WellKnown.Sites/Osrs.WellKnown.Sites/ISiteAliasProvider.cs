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
    public interface ISiteAliasProvider
    {
        bool CanGet();
        IEnumerable<SiteAlias> Get();
        IEnumerable<SiteAlias> Get(Site site);
        IEnumerable<SiteAlias> Get(SiteAliasScheme scheme);
        IEnumerable<SiteAlias> Get(string name);
        IEnumerable<SiteAlias> Get(Site site, string name);
        IEnumerable<SiteAlias> Get(SiteAliasScheme scheme, string name);
        IEnumerable<SiteAlias> Get(string name, StringComparison comparisonOption);
        IEnumerable<SiteAlias> Get(Site site, string name, StringComparison comparisonOption);
        IEnumerable<SiteAlias> Get(SiteAliasScheme scheme, string name, StringComparison comparisonOption);

        IEnumerable<SiteAlias> Get(CompoundIdentity id, SiteAliasScheme scheme);

        bool Exists(CompoundIdentity id);
        bool Exists(CompoundIdentity id, SiteAliasScheme scheme);
        bool Exists(string name);
        bool Exists(SiteAliasScheme scheme, string name);
        bool Exists(Site site, string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool Exists(SiteAliasScheme scheme, string name, StringComparison comparisonOption);
        bool Exists(Site site, string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(SiteAlias alias);
        bool Update(SiteAlias org);

        bool CanDelete();
        bool CanDelete(SiteAlias scheme);
        /// <summary>
        /// Delete all aliases within scheme
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        bool Delete(SiteAliasScheme scheme);

        /// <summary>
        /// Delete all aliases for org
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        bool Delete(CompoundIdentity orgId);


        bool Delete(SiteAlias alias);

        /// <summary>
        /// Delete specific alias within scheme
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        bool Delete(CompoundIdentity id, SiteAliasScheme scheme);

        bool CanCreate();
        SiteAlias Create(SiteAliasScheme scheme, Site site, string name);
    }
}
