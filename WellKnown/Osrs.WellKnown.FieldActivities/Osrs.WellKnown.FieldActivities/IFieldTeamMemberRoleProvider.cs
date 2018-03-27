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

namespace Osrs.WellKnown.FieldActivities
{
    public interface IFieldTeamMemberRoleProvider
    {
        bool CanGet();
        IEnumerable<FieldTeamMemberRole> Get();
        IEnumerable<FieldTeamMemberRole> Get(string name);
        IEnumerable<FieldTeamMemberRole> Get(string name, StringComparison comparisonOption);

        FieldTeamMemberRole Get(CompoundIdentity id);
        IEnumerable<FieldTeamMemberRole> Get(IEnumerable<CompoundIdentity> ids);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(FieldTeamMemberRole item);
        bool Update(FieldTeamMemberRole item);

        bool CanDelete();
        bool CanDelete(FieldTeamMemberRole item);
        bool Delete(FieldTeamMemberRole item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        FieldTeamMemberRole Create(string name);
        FieldTeamMemberRole Create(string name, string description);
    }
}
