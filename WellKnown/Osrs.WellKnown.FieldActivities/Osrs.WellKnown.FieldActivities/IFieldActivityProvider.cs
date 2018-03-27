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
using Osrs.Numerics;
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.FieldActivities
{
    public interface IFieldActivityProvider
    {
        bool CanGet();
        IEnumerable<FieldActivity> Get();
        IEnumerable<FieldActivity> Get(string name);
        IEnumerable<FieldActivity> Get(string name, StringComparison comparisonOption);

        FieldActivity Get(CompoundIdentity id);
        IEnumerable<FieldActivity> Get(IEnumerable<CompoundIdentity> ids);
        IEnumerable<FieldActivity> GetForOrg(CompoundIdentity principalOrgId);
        IEnumerable<FieldActivity> GetForProject(CompoundIdentity projectId);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool ExistsForOrg(CompoundIdentity principalOrgId);
        bool ExistsForProject(CompoundIdentity projectId);

        bool CanUpdate();
        bool CanUpdate(FieldActivity item);
        bool Update(FieldActivity item);

        bool CanDelete();
        bool CanDelete(FieldActivity item);
        bool Delete(FieldActivity item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId);
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, DateTime startDate);
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange);
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, string description);
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
        FieldActivity Create(string name, CompoundIdentity projectId, CompoundIdentity principalOrgId, DateTime startDate, string description);
    }
}
