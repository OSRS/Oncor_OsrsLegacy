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

namespace Osrs.WellKnown.Projects
{
    public interface IProjectProvider
    {
        bool CanGet();
        IEnumerable<Project> Get();
        IEnumerable<Project> Get(string name);
        IEnumerable<Project> Get(string name, StringComparison comparisonOption);

        Project Get(CompoundIdentity id);
        IEnumerable<Project> Get(IEnumerable<CompoundIdentity> ids);
        IEnumerable<Project> GetFor(CompoundIdentity principalOrgId);
        IEnumerable<Project> GetFor(Project parentProject);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);
        bool ExistsFor(CompoundIdentity principalOrgId);
        bool ExistsFor(Project parentProject);

        IEnumerable<ProjectStatus> GetStatus(Project project);
        IEnumerable<ProjectInformation> GetInfo(Project project);

        bool StatusExists(Project project);
        bool InfoExists(Project project);

        bool CanUpdate();
        bool CanUpdate(Project org);
        bool Update(Project org);

        bool AddInfo(ProjectInformation info);
        bool UpdateInfo(ProjectInformation info);
        bool DeleteInfo(ProjectInformation info);

        bool AddStatus(ProjectStatus status);
        bool UpdateStatus(ProjectStatus status);
        bool DeleteStatus(ProjectStatus status);

        bool CanDelete();
        bool CanDelete(Project org);
        bool Delete(Project org);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        Project Create(string name, CompoundIdentity principalOrgId);
        Project Create(string name, CompoundIdentity principalOrgId, Project parentProject);
        Project Create(string name, CompoundIdentity principalOrgId, string description);
        Project Create(string name, CompoundIdentity principalOrgId, Project parentProject, string description);
    }
}
