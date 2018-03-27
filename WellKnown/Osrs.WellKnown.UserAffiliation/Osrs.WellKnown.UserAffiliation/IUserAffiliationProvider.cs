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
using Osrs.Security;
using Osrs.WellKnown.Organizations;
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.UserAffiliation
{
    public interface IUserAffiliationProvider
    {
        bool CanGet();
        bool CanGet(Organization org);
        bool CanGet(IUserIdentity user);
        bool CanAdd();
        bool CanAdd(Organization org);
        bool CanAdd(IUserIdentity user);
        bool CanRemove();
        bool CanRemove(Organization org);
        bool CanRemove(IUserIdentity user);

        /// <summary>
        /// For the current user context, is there an affiliation
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        bool HasAffiliation(Organization org);

        bool HasAffiliation(IUserIdentity user, Organization org);

        IEnumerable<IUserIdentity> Get(Organization org);
        IEnumerable<Guid> GetIds(Organization org);
        IEnumerable<Organization> Get(IUserIdentity user);
        IEnumerable<CompoundIdentity> GetIds(IUserIdentity user);

        bool Add(IUserIdentity user, Organization org);
        bool Add(IEnumerable<IUserIdentity> user, Organization org);
        bool Add(IUserIdentity user, IEnumerable<Organization> org);

        bool Remove(IUserIdentity user, Organization org);
        bool Remove(IEnumerable<IUserIdentity> user, Organization org);
        bool Remove(IUserIdentity user, IEnumerable<Organization> org);
    }
}
