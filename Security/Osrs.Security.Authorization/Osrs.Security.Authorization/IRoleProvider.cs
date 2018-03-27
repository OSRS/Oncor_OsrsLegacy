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

namespace Osrs.Security.Authorization
{
    public interface IRoleProvider
    {
        /// <summary>
        /// Checks if the current user has the specified permission
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasPermission(Permission permission);

        /// <summary>
        /// Checks if the specified user has the specified permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasPermission(IUserIdentity user, Permission permission);
        
        /// <summary>
        /// Gets all permissions the specified user is granted
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<Permission> GetPermissions(IUserIdentity user);

        bool CanCreate();
        bool CanCreate(IUserIdentity user);
        Role Create(string name);

        Role Get(string name);
        Role Get(Guid id);

        bool Exists(string name);
        bool Exists(Guid id);

        bool CanDelete();
        bool CanDelete(IUserIdentity user);
        bool Delete(Role role);
        bool Delete(IEnumerable<Role> roles);

        IEnumerable<Role> GetRoles();
        IEnumerable<Role> GetRoles(IUserIdentity user);
        IEnumerable<Role> GetParentRoles(Role role);
        IEnumerable<Role> GetChildRoles(Role role);
        bool IsMemberOf(Role parent, Role child);
        bool IsMemberOf(Role parent, Role child, bool cascade);

        bool IsUserInRole(IUserIdentity user, Role role);
        IEnumerable<Guid> GetUsers(Role role);

        IEnumerable<Permission> GetPermissions(Role role);
        IEnumerable<PermissionAssignment> GetPermissionAssignments(Role role);
        IEnumerable<Permission> GetEffectivePermissions(Role role);

        bool Update(Role role);

        bool CanModifyMembership();
        bool CanModifyMembership(IUserIdentity user);
        bool CanModifyMembership(Role role);
        bool CanModifyMembership(Role role, IUserIdentity user);

        bool AddToRole(Role role, IUserIdentity user);
        bool AddToRole(Role role, IEnumerable<IUserIdentity> users);

        bool AddToRole(Role parent, Role child);
        bool AddToRole(Role parent, IEnumerable<Role> children);

        bool AddToRole(Role role, Permission permission);
        bool AddToRole(Role role, IEnumerable<Permission> permission);
        bool AddToRole(Role role, Permission permission, GrantType grantOrDeny);
        bool AddToRole(Role role, IEnumerable<Permission> permission, GrantType grantOrDeny);

        bool DeleteFromRole(Role role, IUserIdentity user);
        bool DeleteFromRole(Role role, IEnumerable<IUserIdentity> users);
        bool DeleteFromRole(Role parent, Role child);
        bool DeleteFromRole(Role parent, IEnumerable<Role> children);
        bool DeleteFromRole(Role role, Permission permission);
        bool DeleteFromRole(Role role, IEnumerable<Permission> permissions);
    }
}
