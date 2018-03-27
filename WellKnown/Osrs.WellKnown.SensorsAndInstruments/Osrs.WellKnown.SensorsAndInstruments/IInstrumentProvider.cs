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
    public interface IInstrumentProvider
    {
        bool CanGet();
        IEnumerable<Instrument> Get();
        IEnumerable<Instrument> Get(string name);
        IEnumerable<Instrument> Get(string name, StringComparison comparisonOption);

        Instrument Get(CompoundIdentity id);
        IEnumerable<Instrument> Get(IEnumerable<CompoundIdentity> ids);

        IEnumerable<Instrument> GetForOwner(CompoundIdentity ownerId);

        IEnumerable<Instrument> GetForOwner(IEnumerable<CompoundIdentity> ownerId);

        IEnumerable<Instrument> GetForManufacturer(CompoundIdentity manufId);

        IEnumerable<Instrument> GetForManufacturer(IEnumerable<CompoundIdentity> manufId);

        IEnumerable<Instrument> GetForInstrumentType(CompoundIdentity typeId);

        IEnumerable<Instrument> GetForInstrumentType(IEnumerable<CompoundIdentity> typeId);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(Instrument org);
        bool Update(Instrument org);

        bool CanDelete();
        bool CanDelete(Instrument org);
        bool Delete(Instrument org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId);

        Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description);

        Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, CompoundIdentity manufId);

        Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string serialNumber, CompoundIdentity manufId);

        Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId);
    }
}
