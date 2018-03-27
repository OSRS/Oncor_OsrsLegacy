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
    public interface ISampleEventProvider
    {
        bool CanGet();
        IEnumerable<SamplingEvent> Get();
        IEnumerable<SamplingEvent> Get(string name);
        IEnumerable<SamplingEvent> Get(string name, StringComparison comparisonOption);

        SamplingEvent Get(CompoundIdentity id);
        IEnumerable<SamplingEvent> Get(IEnumerable<CompoundIdentity> ids);
        IEnumerable<SamplingEvent> GetForOrg(CompoundIdentity principalOrgId);
        IEnumerable<SamplingEvent> GetForTrip(CompoundIdentity tripId);
        IEnumerable<SamplingEvent> GetForTrip(FieldTrip trip);
        IEnumerable<SamplingEvent> GetForTrip(IEnumerable<CompoundIdentity> tripIds);
        IEnumerable<SamplingEvent> GetForTrip(IEnumerable<FieldTrip> trips);
        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool ExistsForOrg(CompoundIdentity principalOrgId);
        bool ExistsForTrip(CompoundIdentity tripId);
        bool ExistsForTrip(FieldTrip trip);

        bool CanUpdate();
        bool CanUpdate(SamplingEvent item);
        bool Update(SamplingEvent item);

        bool CanDelete();
        bool CanDelete(SamplingEvent item);
        bool Delete(SamplingEvent item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId);
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, DateTime startDate);
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange);
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, string description);
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
        SamplingEvent Create(string name, FieldTrip trip, CompoundIdentity principalOrgId, DateTime startDate, string description);
        SamplingEvent Create(string name, CompoundIdentity tripId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description);
    }
}
