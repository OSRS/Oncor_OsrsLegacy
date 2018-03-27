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
    public interface ISensorProvider
    {
        bool CanGet();
        IEnumerable<Sensor> Get();
        IEnumerable<Sensor> Get(string name);
        IEnumerable<Sensor> Get(string name, StringComparison comparisonOption);

        Sensor Get(CompoundIdentity id);
        IEnumerable<Sensor> Get(IEnumerable<CompoundIdentity> ids);

        IEnumerable<Sensor> GetFor(string serialNumber);

        IEnumerable<Sensor> GetFor(IEnumerable<string> serialNumber);

        IEnumerable<Sensor> GetForOwner(CompoundIdentity ownerId);

        IEnumerable<Sensor> GetForOwner(IEnumerable<CompoundIdentity> ownerId);

        IEnumerable<Sensor> GetForManufacturer(CompoundIdentity manufId);

        IEnumerable<Sensor> GetForManufacturer(IEnumerable<CompoundIdentity> manufId);

        IEnumerable<Sensor> GetForSensorType(CompoundIdentity typeId);

        IEnumerable<Sensor> GetForSensorType(IEnumerable<CompoundIdentity> typeId);

        IEnumerable<Sensor> GetForInstrument(CompoundIdentity instrumentId);

        IEnumerable<Sensor> GetForInstrument(IEnumerable<CompoundIdentity> instrumentId);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(Sensor item);
        bool Update(Sensor item);

        bool CanDelete();
        bool CanDelete(Sensor item);
        bool Delete(Sensor item);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId);

        Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description);

        Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, CompoundIdentity manufId);

        Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string serialNumber, CompoundIdentity manufId);

        Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId);
    }
}
