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

using Osrs.Runtime;
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.Projects
{
    public abstract class ProjectProviderFactoryBase : SubclassableSingletonBase<ProjectProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal static Permission ProjectCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "Project"), ProjectUtils.ProjectCreatePermissionId);
            }
        }

        protected internal static Permission ProjectGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Project"), ProjectUtils.ProjectGetPermissionId);
            }
        }
        protected internal static Permission ProjectUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "Project"), ProjectUtils.ProjectUpdatePermissionId);
            }
        }
        protected internal static Permission ProjectDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Project"), ProjectUtils.ProjectDeletePermissionId);
            }
        }

        protected internal static Permission ProjectStatusTypeCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeCreatePermissionId);
            }
        }

        protected internal static Permission ProjectStatusTypeGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeGetPermissionId);
            }
        }
        protected internal static Permission ProjectStatusTypeUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeUpdatePermissionId);
            }
        }
        protected internal static Permission ProjectStatusTypeDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeDeletePermissionId);
            }
        }

        protected internal abstract bool Initialize();

        protected bool InitializeOther(ProjectProviderFactoryBase other)
        {
            if (other != null)
                return other.Initialize();
            return false;
        }

        protected internal abstract ProjectProviderBase GetProvider(UserSecurityContext context);

        protected ProjectProviderBase GetProviderOther(ProjectProviderFactoryBase other, UserSecurityContext context)
        {
            if (other != null)
                return other.GetProvider(context);
            return null;
        }

        protected internal abstract ProjectStatusTypeProviderBase GetStatusTypeProvider(UserSecurityContext context);

        protected ProjectStatusTypeProviderBase GetStatusTypeProviderOther(ProjectProviderFactoryBase other, UserSecurityContext context)
        {
            if (other != null)
                return other.GetStatusTypeProvider(context);
            return null;
        }
    }
}
