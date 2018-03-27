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
    public sealed class CachingRoleProvider : IRoleProvider
    {
        private readonly IRoleProvider inner;
        private readonly UserSecurityContext context;

        public bool CanCreate()
        {
            return this.inner.CanCreate();
        }

        public bool CanCreate(IUserIdentity user)
        {
            return this.inner.CanCreate(user);
        }

        public bool CanDelete()
        {
            return this.inner.CanDelete();
        }

        public bool CanDelete(IUserIdentity user)
        {
            return this.inner.CanDelete(user);
        }

        public bool CanModifyMembership()
        {
            return this.inner.CanModifyMembership();
        }

        public bool CanModifyMembership(Role role)
        {
            return this.inner.CanModifyMembership(role);
        }

        public bool CanModifyMembership(IUserIdentity user)
        {
            return this.inner.CanModifyMembership(user);
        }

        public bool CanModifyMembership(Role role, IUserIdentity user)
        {
            return this.inner.CanModifyMembership(role, user);
        }

        public bool AddToRole(Role role, IEnumerable<IUserIdentity> users)
        {
            if (this.inner.AddToRole(role, users))
            {
                foreach(IUserIdentity cur in users)
                {
                    if (cur != null)
                        RoleMembershipMemorySet.Instance.AddUserToRole(cur.Uid, role.Id);
                }
                return true;
            }
            return false;
        }

        public bool AddToRole(Role parent, IEnumerable<Role> children)
        {
            if (this.inner.AddToRole(parent, children))
            {
                foreach (Role cur in children)
                {
                    if (cur != null)
                        RoleMembershipMemorySet.Instance.AddRoleToRole(parent.Id, cur.Id);
                }
                return true;
            }
            return false;
        }

        public bool AddToRole(Role parent, Role child)
        {
            if (this.inner.AddToRole(parent, child))
            {
                RoleMembershipMemorySet.Instance.AddRoleToRole(parent.Id, child.Id);
                return true;
            }
            return false;
        }

        public bool AddToRole(Role role, IEnumerable<Permission> permission)
        {
            return this.AddToRole(role, permission, GrantType.Grant);
        }

        public bool AddToRole(Role role, Permission permission)
        {
            return this.AddToRole(role, permission, GrantType.Grant);
        }

        public bool AddToRole(Role role, IUserIdentity user)
        {
            if (this.inner.AddToRole(role, user))
            {
                RoleMembershipMemorySet.Instance.AddUserToRole(user.Uid, role.Id);
                return true;
            }
            return false;
        }

        public bool AddToRole(Role role, IEnumerable<Permission> permission, GrantType grantOrDeny)
        {
            if (this.inner.AddToRole(role, permission, grantOrDeny))
            {
                foreach(Permission cur in permission)
                {
                    if (cur != null)
                        RoleMembershipMemorySet.Instance.AddPermissionToRole(role.Id, cur.Id, grantOrDeny == GrantType.Grant);
                }
            }
            return false;
        }

        public bool AddToRole(Role role, Permission permission, GrantType grantOrDeny)
        {
            if (this.inner.AddToRole(role, permission, grantOrDeny))
            {
                RoleMembershipMemorySet.Instance.AddPermissionToRole(role.Id, permission.Id, grantOrDeny == GrantType.Grant);
                return true;
            }
            return false;
        }

        public Role Create(string name)
        {
            Role r = this.inner.Create(name);
            if (r != null)
                RoleMemorySet.Instance.RegisterRole(r);
            return r;
        }

        public bool Delete(IEnumerable<Role> roles)
        {
            if (this.inner.Delete(roles))
            {
                foreach(Role r in roles)
                {
                    if (r != null)
                        RoleMemorySet.Instance.UnregisterRole(r);
                }
                return true;
            }
            return false;
        }

        public bool Delete(Role role)
        {
            if (this.inner.Delete(role))
            {
                RoleMemorySet.Instance.UnregisterRole(role);
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role parent, Role child)
        {
            if (this.inner.DeleteFromRole(parent, child))
            {
                RoleMembershipMemorySet.Instance.RemoveRoleFromRole(parent.Id, child.Id);
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, Permission permission)
        {
            if (this.inner.DeleteFromRole(role, permission))
            {
                RoleMembershipMemorySet.Instance.RemovePermissionFromRole(role.Id, permission.Id);
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IEnumerable<Permission> permissions)
        {
            if (this.inner.DeleteFromRole(role, permissions))
            {
                foreach(Permission p in permissions)
                {
                    if (p != null)
                        RoleMembershipMemorySet.Instance.RemovePermissionFromRole(role.Id, p.Id);
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role parent, IEnumerable<Role> children)
        {
            if (this.inner.DeleteFromRole(parent, children))
            {
                foreach (Role p in children)
                {
                    if (p != null)
                        RoleMembershipMemorySet.Instance.RemoveRoleFromRole(parent.Id, p.Id);
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IEnumerable<IUserIdentity> users)
        {
            if (this.inner.DeleteFromRole(role, users))
            {
                foreach (IUserIdentity p in users)
                {
                    if (p != null)
                        RoleMembershipMemorySet.Instance.RemoveUserFromRole(role.Id, p.Uid);
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IUserIdentity user)
        {
            if (this.inner.DeleteFromRole(role, user))
            {
                RoleMembershipMemorySet.Instance.RemoveUserFromRole(user.Uid, role.Id);
                return true;
            }
            return false;
        }

        public bool Exists(Guid id)
        {
            return RoleMemorySet.Instance.Exists(id);
        }

        public bool Exists(string name)
        {
            return RoleMemorySet.Instance.Exists(name);
        }

        public Role Get(Guid id)
        {
            return RoleMemorySet.Instance.Get(id);
        }

        public Role Get(string name)
        {
            return RoleMemorySet.Instance.Get(name);
        }

        public IEnumerable<Role> GetChildRoles(Role role)
        {
            if (role != null)
            {
                return RoleMemorySet.Instance.GetRoles(RoleMembershipMemorySet.Instance.ChildRoles(role.Id));
            }
            return null;
        }

        public IEnumerable<Permission> GetEffectivePermissions(Role role)
        {
            if (role != null)
                return PermissionUtils.CompactToEffective(RoleMembershipMemorySet.Instance.GetEffectiveAssignments(role.Id));
            return null;
        }

        public IEnumerable<Role> GetParentRoles(Role role)
        {
            if (role != null)
                return RoleMemorySet.Instance.GetRoles(RoleMembershipMemorySet.Instance.ParentRoles(role.Id));
            return null;
        }

        public IEnumerable<PermissionAssignment> GetPermissionAssignments(Role role)
        {
            if (role!=null)
            {
                HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
                RoleMembershipMemorySet.Instance.Fill(perms, role.Id);
                return perms;
            }
            return null;
        }

        public IEnumerable<Permission> GetPermissions(Role role)
        {
            if (role != null)
            {
                HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
                RoleMembershipMemorySet.Instance.Fill(perms, role.Id);
                return PermissionUtils.CompactToEffective(perms);
            }
            return null;
        }

        public IEnumerable<Permission> GetPermissions(IUserIdentity user)
        {
            if (user!=null)
            {
                HashSet<Guid> roleids = RoleMembershipMemorySet.Instance.AllRolesForUser(user.Uid);
                if (roleids != null)
                    return PermissionUtils.CompactToEffective(RoleMembershipMemorySet.Instance.GetEffectiveAssignments(roleids));
            }
            return null;
        }

        public IEnumerable<Role> GetRoles()
        {
            return RoleMemorySet.Instance.GetRoles();
        }

        public IEnumerable<Role> GetRoles(IUserIdentity user)
        {
            if (user!=null)
            {
                return RoleMemorySet.Instance.GetRoles(RoleMembershipMemorySet.Instance.RolesForUser(user.Uid));
            }
            return null;
        }

        public IEnumerable<Guid> GetUsers(Role role)
        {
            if (role != null)
                return RoleMembershipMemorySet.Instance.UsersInRole(role.Id).ToList();
            return null;
        }

        public bool HasPermission(Permission permission)
        {
            return this.HasPermission(this.context.User, permission);
        }

        public bool HasPermission(IUserIdentity user, Permission permission)
        {
            if (user!=null && permission!=null)
            {
                IEnumerable<Permission> perms = this.GetPermissions(user);
                if (perms!=null)
                {
                    foreach(Permission p in perms)
                    {
                        if (permission.Equals(p))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool IsMemberOf(Role parent, Role child)
        {
            return IsMemberOf(parent, child, false);
        }

        public bool IsMemberOf(Role parent, Role child, bool cascade)
        {
            if (parent!=null && child!=null)
            {
                if (!parent.Id.Equals(child.Id))
                {
                    if (RoleMembershipMemorySet.Instance.ChildRoles(parent.Id).Contains(child.Id))
                        return true;

                    if (cascade)
                    {
                        HashSet<Guid> res = new HashSet<Guid>();
                        RoleMembershipMemorySet.Instance.GetParents(res, RoleMembershipMemorySet.Instance.ParentRoles(child.Id));
                        return res.Contains(parent.Id);
                    }
                }
                else
                    return true; //parent==child
            }
            return false;
        }

        public bool IsUserInRole(IUserIdentity user, Role role)
        {
            if (user!=null && role!=null)
            {
                HashSet<Guid> roles = RoleMembershipMemorySet.Instance.RolesForUser(user.Uid);
                if (roles != null)
                    return roles.Contains(role.Id);
            }
            return false;
        }

        public bool Update(Role role)
        {
            if (this.inner.Update(role))
            {
                RoleMemorySet.Instance.Update(role);
                return true;
            }
            return false;
        }

        internal CachingRoleProvider(IRoleProvider inner, UserSecurityContext context)
        {
            this.inner = inner;
            this.context = context;
        }
    }
}
