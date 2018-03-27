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

namespace Osrs.Security.Authorization.Providers
{
    public sealed class CachingPermissionProvider : IPermissionProvider
    {
        private readonly IPermissionProvider inner;

        public bool CanManagePermissions()
        {
            return this.inner.CanManagePermissions();
        }

        public bool Exists(Guid id)
        {
            return PermissionMemorySet.Exists(id);
        }

        public bool Exists(string name)
        {
            return PermissionMemorySet.Exists(name);
        }

        public Permission Get(Guid id)
        {
            return PermissionMemorySet.Get(id);
        }

        public Permission Get(string name)
        {
            return PermissionMemorySet.Get(name);
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return PermissionMemorySet.GetPermissions();
        }

        public bool RegisterPermission(Permission permission)
        {
            if (this.inner.RegisterPermission(permission))
            {
                PermissionMemorySet.RegisterPermission(permission);
                return true;
            }
            return false;
        }

        public bool UnregisterPermission(Permission permission)
        {
            if (this.inner.UnregisterPermission(permission))
            {
                PermissionMemorySet.UnregisterPermission(permission);
                return true;
            }
            return false;
        }

        internal CachingPermissionProvider(IPermissionProvider inner)
        {
            this.inner = inner;
        }
    }
}
