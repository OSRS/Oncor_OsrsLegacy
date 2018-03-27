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

namespace Osrs.WellKnown.SensorsAndInstruments
{
    public interface ISensorTypeProvider
    {
        bool CanGet();
        IEnumerable<SensorType> Get();
        IEnumerable<SensorType> Get(string name);
        IEnumerable<SensorType> Get(string name, StringComparison comparisonOption);

        IEnumerable<SensorType> GetChildren(CompoundIdentity id); //since there's only one parent, we don't need a separate method, just use: provider.Get(aSensorType.ParentId) 

        IEnumerable<SensorType> GetFor(CompoundIdentity instrumentFamilyId);
        IEnumerable<SensorType> GetFor(IEnumerable<CompoundIdentity> instrumentFamilyId);

        SensorType Get(CompoundIdentity id);
        IEnumerable<SensorType> Get(IEnumerable<CompoundIdentity> ids);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(SensorType org);
        bool Update(SensorType org);

        bool CanDelete();
        bool CanDelete(SensorType org);
        bool Delete(SensorType org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        SensorType Create(string name);
        SensorType Create(string name, string description);
        SensorType Create(string name, CompoundIdentity parentId);
        SensorType Create(string name, string description, CompoundIdentity parentId);
    }
}
