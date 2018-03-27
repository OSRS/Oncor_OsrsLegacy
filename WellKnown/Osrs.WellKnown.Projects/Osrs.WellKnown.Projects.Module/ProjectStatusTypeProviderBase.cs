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
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.Projects
{
    public abstract class ProjectStatusTypeProviderBase : IProjectStatusTypeProvider
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
        public abstract bool CanDelete(ProjectStatusType statusType);

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

        public abstract bool CanUpdate(ProjectStatusType statusType);

        public abstract bool Exists(CompoundIdentity id);

        public bool Exists(string name)
        {
            return Exists(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract bool Exists(string name, StringComparison comparisonOption);

        public abstract IEnumerable<ProjectStatusType> Get();
        public abstract ProjectStatusType Get(CompoundIdentity id);

        public IEnumerable<ProjectStatusType> Get(string name)
        {
            return Get(name, StringComparison.OrdinalIgnoreCase);
        }
        public abstract IEnumerable<ProjectStatusType> Get(string name, StringComparison comparisonOption);

        public abstract bool Update(ProjectStatusType statusType);

        public ProjectStatusType Create(string name)
        {
            return Create(name, null);
        }
        public abstract ProjectStatusType Create(string name, string description);

        public abstract bool Delete(CompoundIdentity id);
        public bool Delete(ProjectStatusType statusType)
        {
            if (statusType != null)
                return Delete(statusType.Identity);
            return false;
        }

        protected ProjectStatusTypeProviderBase(UserSecurityContext context)
        {
            this.Context = context;
        }
    }
}
