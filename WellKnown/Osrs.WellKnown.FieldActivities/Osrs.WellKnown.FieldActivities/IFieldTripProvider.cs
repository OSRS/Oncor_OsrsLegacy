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
    public interface IFieldTripProvider
    {
        bool CanGet();
        IEnumerable<FieldTrip> Get();
        IEnumerable<FieldTrip> Get(string name);
        IEnumerable<FieldTrip> Get(string name, StringComparison comparisonOption);

        FieldTrip Get(CompoundIdentity id);
        IEnumerable<FieldTrip> Get(IEnumerable<CompoundIdentity> ids);
        IEnumerable<FieldTrip> GetForOrg(CompoundIdentity principalOrgId);
        IEnumerable<FieldTrip> GetForActivity(CompoundIdentity activityId);
        IEnumerable<FieldTrip> GetForActivity(FieldActivity activity);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool ExistsForOrg(CompoundIdentity principalOrgId);
        bool ExistsForActivity(CompoundIdentity activityId);
        bool ExistsForActivity(FieldActivity activity);

        bool CanUpdate();
        bool CanUpdate(FieldTrip item);
        bool Update(FieldTrip item);

        bool CanDelete();
        bool CanDelete(FieldTrip item);
        bool Delete(FieldTrip item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId);
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, DateTime startDate);
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange);
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, string description);
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
        FieldTrip Create(string name, FieldActivity activity, CompoundIdentity principalOrgId, DateTime startDate, string description);
        FieldTrip Create(string name, CompoundIdentity activityId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
    }
}
