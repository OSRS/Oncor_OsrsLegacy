﻿//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
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

namespace Osrs.WellKnown.Projects
{
    public interface IProjectStatusTypeProvider
    {
        bool CanGet();
        IEnumerable<ProjectStatusType> Get();
        IEnumerable<ProjectStatusType> Get(string name);
        IEnumerable<ProjectStatusType> Get(string name, StringComparison comparisonOption);

        ProjectStatusType Get(CompoundIdentity id);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(ProjectStatusType statusType);
        bool Update(ProjectStatusType statusType);

        bool CanDelete();
        bool CanDelete(ProjectStatusType statusType);
        bool Delete(ProjectStatusType statusType);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        ProjectStatusType Create(string name);
        ProjectStatusType Create(string name, string description);
    }
}
