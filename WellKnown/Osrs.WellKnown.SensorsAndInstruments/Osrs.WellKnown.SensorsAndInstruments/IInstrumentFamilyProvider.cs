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
    public interface IInstrumentFamilyProvider
    {
        bool CanGet();
        IEnumerable<InstrumentFamily> Get();
        IEnumerable<InstrumentFamily> Get(string name);
        IEnumerable<InstrumentFamily> Get(string name, StringComparison comparisonOption);

        InstrumentFamily Get(CompoundIdentity id);
        IEnumerable<InstrumentFamily> Get(IEnumerable<CompoundIdentity> ids);

        IEnumerable<InstrumentFamily> GetChildren(CompoundIdentity id); //since there's only one parent, we don't need a separate method, just use: provider.Get(aInstrumentFamily.ParentId) 

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(InstrumentFamily org);
        bool Update(InstrumentFamily org);

        bool CanDelete();
        bool CanDelete(InstrumentFamily org);
        bool Delete(InstrumentFamily org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        InstrumentFamily Create(string name);
        InstrumentFamily Create(string name, string description);
        InstrumentFamily Create(string name, CompoundIdentity parentId);
        InstrumentFamily Create(string name, string description, CompoundIdentity parentId);
    }
}
