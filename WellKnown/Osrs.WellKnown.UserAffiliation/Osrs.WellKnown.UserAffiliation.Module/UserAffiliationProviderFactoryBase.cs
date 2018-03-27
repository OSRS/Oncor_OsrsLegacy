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

namespace Osrs.WellKnown.UserAffiliation
{
    public abstract class UserAffiliationProviderFactoryBase : SubclassableSingletonBase<UserAffiliationProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal static Permission UserAffiliationAddPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "UserAffiliation"), UserAffiliationUtils.AddPermissionId);
            }
        }

        protected internal static Permission UserAffiliationRemovePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "UserAffiliation"), UserAffiliationUtils.RemovePermissionId);
            }
        }

        protected internal static Permission UserAffiliationGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "UserAffiliation"), UserAffiliationUtils.GetPermissionId);
            }
        }

        protected internal abstract bool Initialize();

        protected internal abstract UserAffiliationProviderBase GetProvider(UserSecurityContext context);
    }
}
