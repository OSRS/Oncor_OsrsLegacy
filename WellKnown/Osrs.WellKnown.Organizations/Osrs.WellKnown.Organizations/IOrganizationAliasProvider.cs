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
    public interface IOrganizationAliasProvider
    {
        bool CanGet();
        IEnumerable<OrganizationAlias> Get();
        IEnumerable<OrganizationAlias> Get(Organization org);
        IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme);
        IEnumerable<OrganizationAlias> Get(string name);
        IEnumerable<OrganizationAlias> Get(Organization org, string name);
        IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme, string name);
        IEnumerable<OrganizationAlias> Get(string name, StringComparison comparisonOption);
        IEnumerable<OrganizationAlias> Get(Organization org, string name, StringComparison comparisonOption);
        IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption);

        IEnumerable<OrganizationAlias> Get(CompoundIdentity id, OrganizationAliasScheme scheme);

        bool Exists(CompoundIdentity id);
        bool Exists(CompoundIdentity id, OrganizationAliasScheme scheme);
        bool Exists(string name);
        bool Exists(OrganizationAliasScheme scheme, string name);
        bool Exists(Organization org, string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool Exists(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption);
        bool Exists(Organization org, string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(OrganizationAlias alias);
        bool Update(OrganizationAlias org);

        bool CanDelete();
        bool CanDelete(OrganizationAlias scheme);
        /// <summary>
        /// Delete all aliases within scheme
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        bool Delete(OrganizationAliasScheme scheme);

        /// <summary>
        /// Delete all aliases for org
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        bool Delete(CompoundIdentity orgId);


        bool Delete(OrganizationAlias alias);

        /// <summary>
        /// Delete specific alias within scheme
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        bool Delete(CompoundIdentity id, OrganizationAliasScheme scheme);

        bool CanCreate();
        OrganizationAlias Create(OrganizationAliasScheme scheme, Organization org, string name);
    }
}
