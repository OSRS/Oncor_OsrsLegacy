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
    internal static class PermissionMemorySet
    {
        private static readonly Dictionary<Guid, Permission> permissions = new Dictionary<Guid, Permission>();
        private static readonly Dictionary<string, Guid> permissionNames = new Dictionary<string, Guid>();

        internal static IEnumerable<Permission> GetPermissions()
        {
            return permissions.Values;
        }

        internal static IEnumerable<Permission> GetPermissions(IEnumerable<Guid> permissionIds)
        {
            if (permissionIds != null)
            {
                List<Permission> tmp = new List<Permission>();
                foreach (Guid cur in permissionIds)
                {
                    if (permissions.ContainsKey(cur))
                        tmp.Add(permissions[cur]);
                }
                return tmp;
            }
            return null;
        }

        internal static Permission Get(string name)
        {
            try
            {
                if (permissionNames.ContainsKey(name))
                {
                    return permissions[permissionNames[name]];
                }
            }
            catch
            { }
            return null;
        }
        internal static Permission Get(Guid id)
        {
            try
            {
                if (permissions.ContainsKey(id))
                {
                    return permissions[id];
                }
            }
            catch
            { }
            return null;
        }

        internal static bool Exists(string name)
        {
            return permissionNames.ContainsKey(name);
        }
        internal static bool Exists(Guid id)
        {
            return permissions.ContainsKey(id);
        }

        internal static bool RegisterPermission(Permission permission)
        {
            try
            {
                permissions.Add(permission.Id, permission);
                permissionNames.Add(permission.Name, permission.Id);
                return true;
            }
            catch
            { }
            return false;
        }
        internal static bool UnregisterPermission(Permission permission)
        {
            try
            {
                if (permissions.Remove(permission.Id))
                {
                    RoleMembershipMemorySet.Instance.RemovePermission(permission.Id);
                    return permissionNames.Remove(permission.Name);
                }
            }
            catch
            { }
            return false;
        }
    }
}
