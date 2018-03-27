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

using System;
using System.Collections.Generic;
using Osrs.Data;
using Osrs.Security.Authorization;
using Osrs.Security;

namespace Osrs.WellKnown.Projects
{
    public abstract class ProjectProviderBase : IProjectProvider
    {
        protected UserSecurityContext Context
        {
            get;
        }

        private IRoleProvider prov;
        protected IRoleProvider AuthProvider
        {
            get
            {
                if (prov == null)
                    prov = AuthorizationManager.Instance.GetRoleProvider(this.Context);
                return prov;
            }
        }

        public bool CanCreate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, ProjectProviderFactoryBase.ProjectCreatePermission);
                }
            }
            return false;
        }

        public bool CanDelete()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, ProjectProviderFactoryBase.ProjectDeletePermission);
                }
            }
            return false;
        }

        public abstract bool CanDelete(Project org);

        public bool CanGet()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, ProjectProviderFactoryBase.ProjectGetPermission);
                }
            }
            return false;
        }

        public bool CanUpdate()
        {
            IRoleProvider perms = this.AuthProvider;
            if (perms != null)
            {
                if (this.Context != null && this.Context.User != null)
                {
                    return perms.HasPermission(this.Context.User, ProjectProviderFactoryBase.ProjectUpdatePermission);
                }
            }
            return false;
        }

        public abstract bool CanUpdate(Project org);
        
        public abstract bool AddInfo(ProjectInformation info);
        public abstract bool AddStatus(ProjectStatus status);

        public Project Create(string name, CompoundIdentity principalOrgId)
        {
            return Create(name, principalOrgId, null, null);
        }
        public Project Create(string name, CompoundIdentity principalOrgId, string description)
        {
            return Create(name, principalOrgId, null, description);
        }
        public Project Create(string name, CompoundIdentity principalOrgId, Project parentProject)
        {
            return Create(name, principalOrgId, parentProject, null);
        }

        public abstract Project Create(string name, CompoundIdentity principalOrgId, Project parentProject, string description);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(Project org)
        {
            if (org != null)
                return Delete(org.Identity);
            return false;
        }

        public abstract bool DeleteInfo(ProjectInformation info);
        public abstract bool DeleteStatus(ProjectStatus status);

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return this.Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);
        public abstract bool ExistsFor(Project parentProject);
        public abstract bool ExistsFor(CompoundIdentity principalOrgId);

        public abstract IEnumerable<Project> Get();
        public abstract IEnumerable<Project> Get(IEnumerable<CompoundIdentity> ids);
        public abstract Project Get(CompoundIdentity id);

        public IEnumerable<Project> Get(string name)
        {
            return Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<Project> Get(string name, StringComparison comparisonOption);

        public abstract IEnumerable<Project> GetFor(Project parentProject);
        public abstract IEnumerable<Project> GetFor(CompoundIdentity principalOrgId);
        public abstract IEnumerable<ProjectInformation> GetInfo(Project project);
        public abstract IEnumerable<ProjectStatus> GetStatus(Project project);

        public abstract bool InfoExists(Project project);
        public abstract bool StatusExists(Project project);
        public abstract bool Update(Project org);
        public abstract bool UpdateInfo(ProjectInformation info);
        public abstract bool UpdateStatus(ProjectStatus status);

        protected ProjectProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}
