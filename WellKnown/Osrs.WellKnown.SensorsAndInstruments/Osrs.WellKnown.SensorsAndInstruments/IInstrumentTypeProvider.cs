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
    public interface IInstrumentTypeProvider
    {
        bool CanGet();
        IEnumerable<InstrumentType> Get();
        IEnumerable<InstrumentType> Get(string name);
        IEnumerable<InstrumentType> Get(string name, StringComparison comparisonOption);

        InstrumentType Get(CompoundIdentity id);
        IEnumerable<InstrumentType> Get(IEnumerable<CompoundIdentity> ids);

        IEnumerable<InstrumentType> GetChildren(CompoundIdentity id); //since there's only one parent, we don't need a separate method, just use: provider.Get(aInstrumentType.ParentId) 

        IEnumerable<InstrumentType> GetFor(CompoundIdentity instrumentFamilyId);

        IEnumerable<InstrumentType> GetFor(IEnumerable<CompoundIdentity> instrumentFamilyIds);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(InstrumentType org);
        bool Update(InstrumentType org);

        bool CanDelete();
        bool CanDelete(InstrumentType org);
        bool Delete(InstrumentType org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        InstrumentType Create(string name, CompoundIdentity familyId);
        InstrumentType Create(string name, CompoundIdentity familyId, string description);
        InstrumentType Create(string name, CompoundIdentity familyId, CompoundIdentity parentId);
        InstrumentType Create(string name, CompoundIdentity familyId, string description, CompoundIdentity parentId);
    }
}
