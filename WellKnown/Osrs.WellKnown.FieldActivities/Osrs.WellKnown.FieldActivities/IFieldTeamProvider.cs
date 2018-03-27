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
    public interface IFieldTeamProvider
    {
        bool CanGet();
        IEnumerable<FieldTeam> Get();
        IEnumerable<FieldTeam> Get(string name);
        IEnumerable<FieldTeam> Get(string name, StringComparison comparisonOption);

        FieldTeam Get(CompoundIdentity id);
        IEnumerable<FieldTeam> Get(IEnumerable<CompoundIdentity> ids);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(FieldTeam item);
        bool Update(FieldTeam item);

        bool CanDelete();
        bool CanDelete(FieldTeam item);
        bool Delete(FieldTeam item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        FieldTeam Create(string name);
        FieldTeam Create(string name, string description);

        bool Contains(FieldTeam team, FieldActivity item);
        bool Contains(FieldTeam team, FieldTrip item);
        bool Contains(FieldTeam team, SamplingEvent item);

        IEnumerable<FieldTeam> Get(FieldActivity item);
        IEnumerable<FieldTeam> Get(FieldTrip item);
        IEnumerable<FieldTeam> Get(SamplingEvent item);

        bool CanAddRemove();
        bool CanAddRemove(FieldActivity item);
        bool CanAddRemove(FieldTrip item);
        bool CanAddRemove(SamplingEvent item);

        bool Add(FieldTeam team, FieldActivity item);
        bool Add(FieldTeam team, FieldTrip item);
        bool Add(FieldTeam team, SamplingEvent item);

        bool Remove(FieldTeam team, FieldActivity item);
        bool Remove(FieldTeam team, FieldTrip item);
        bool Remove(FieldTeam team, SamplingEvent item);
    }
}
