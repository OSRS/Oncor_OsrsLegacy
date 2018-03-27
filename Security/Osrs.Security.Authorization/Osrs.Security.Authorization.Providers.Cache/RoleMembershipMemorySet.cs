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
    internal sealed class RoleMembershipMemorySet
    {
        private static RoleMembershipMemorySet instance = null;
        internal static RoleMembershipMemorySet Instance
        {
            get { return instance; }
        }

        private readonly RoleRoleMembership roleMemberships = new RoleRoleMembership();
        private readonly UserRoleMembership userRoles = new UserRoleMembership();
        private readonly RolePermissionMembership rolePermissions = new RolePermissionMembership();

        //recursive
        internal void GetChildren(HashSet<Guid> result, HashSet<Guid> roles)
        {
            if (roles != null)
            {
                foreach (Guid cur in roles)
                {
                    if (!result.Contains(cur))
                    {
                        result.Add(cur);
                        GetChildren(result, ChildRoles(cur));
                    }
                }
            }
        }

        //recursive
        internal void GetParents(HashSet<Guid> result, HashSet<Guid> roles)
        {
            if (roles!=null)
            {
                foreach(Guid cur in roles)
                {
                    if (!result.Contains(cur))
                    {
                        result.Add(cur);
                        GetParents(result, ParentRoles(cur));
                    }
                }
            }
        }

        internal void Fill(HashSet<PermissionAssignment> perms, Guid roleId)
        {
            rolePermissions.Fill(perms, roleId);
        }

        internal HashSet<PermissionAssignment> GetEffectiveAssignments(Guid roleId)
        {
            HashSet<Guid> allRoles = roleMemberships.Parents(roleId);
            allRoles.Add(roleId);

            HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
            rolePermissions.Fill(perms, allRoles);
            return perms;
        }

        internal HashSet<PermissionAssignment> GetEffectiveAssignments(HashSet<Guid> roleIds)
        {
            HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
            rolePermissions.Fill(perms, roleIds);
            return perms;
        }

        internal HashSet<Guid> UsersInRole(Guid roleId)
        {
            return userRoles.Users(roleId);
        }

        internal HashSet<Guid> AllRolesForUser(Guid userId)
        {
            HashSet<Guid> tmp = RolesForUser(userId);
            if (tmp != null)
            {
                HashSet<Guid> result = new HashSet<Guid>();
                GetParents(result, tmp);
                return result;
            }
            return null;
        }

        internal HashSet<Guid> RolesForUser(Guid userId)
        {
            return userRoles.Roles(userId);
        }

        internal HashSet<Guid> ChildRoles(Guid roleId)
        {
            return roleMemberships.Children(roleId);
        }

        internal HashSet<Guid> ParentRoles(Guid roleId)
        {
            return roleMemberships.Parents(roleId);
        }

        internal void RemoveUserFromRole(Guid userId, Guid roleId)
        {
            userRoles.RemoveFromRole(userId, roleId);
        }

        internal void RemoveRoleFromRole(Guid parentRole, Guid childRole)
        {
            roleMemberships.Remove(parentRole, childRole);
        }

        internal void RemovePermissionFromRole(Guid roleId, Guid permissionId, bool isGrant)
        {
            rolePermissions.Remove(roleId, permissionId, isGrant);
        }

        internal void RemovePermissionFromRole(Guid roleId, Guid permissionId)
        {
            rolePermissions.Remove(roleId, permissionId);
        }

        internal void AddRoleToRole(Guid parentRole, Guid childRole)
        {
            roleMemberships.Add(parentRole, childRole);
        }

        internal void AddUserToRole(Guid userId, Guid roleId)
        {
            userRoles.AddToRole(userId, roleId);
        }

        internal void AddPermissionToRole(Guid roleId, Guid permissionId, bool isGrant)
        {
            rolePermissions.Add(roleId, permissionId, isGrant);
        }

        internal void RemoveRole(Guid roleId)
        {
            rolePermissions.RemoveRole(roleId);
            userRoles.RemoveRole(roleId);
            roleMemberships.Remove(roleId);
        }

        internal void RemoveUser(Guid userId)
        {
            userRoles.RemoveUser(userId);
        }

        internal void RemovePermission(Guid permissionId)
        {
            rolePermissions.RemovePermission(permissionId);
        }

        internal static bool Reset(IRoleProvider prov)
        {
            try
            {
                RoleMembershipMemorySet tmp = new RoleMembershipMemorySet();
                foreach (Role p in RoleMemorySet.Instance.GetRoles())
                {
                    IEnumerable<Role> kids = prov.GetChildRoles(p);
                    if (kids != null)
                    {
                        foreach (Role r in kids)
                        {
                            tmp.AddRoleToRole(p.Id, r.Id);
                        }
                    }

                    IEnumerable<Guid> users = prov.GetUsers(p);
                    if (users != null)
                    {
                        foreach (Guid us in users)
                        {
                            tmp.AddUserToRole(us, p.Id);
                        }
                    }

                    IEnumerable<PermissionAssignment> pas = prov.GetPermissionAssignments(p);
                    if (pas != null)
                    {
                        foreach (PermissionAssignment pa in pas)
                        {
                            tmp.AddPermissionToRole(p.Id, pa.Permission.Id, pa.GrantType == GrantType.Grant);
                        }
                    }
                }
                instance = tmp;
                return true;
            }
            catch
            { }
            return false;
        }
    }

    internal sealed class UserRoleMembership
    {
        private readonly Dictionary<Guid, HashSet<Guid>> userRoles = new Dictionary<Guid, HashSet<Guid>>();  //Dictionary<userid, HashSet<roleid>>
        private readonly Dictionary<Guid, HashSet<Guid>> roleUsers = new Dictionary<Guid, HashSet<Guid>>();  //Dictionary<roleid, HashSet<userid>>

        internal HashSet<Guid> Users(Guid roleId)
        {
            if (roleUsers.ContainsKey(roleId))
                return roleUsers[roleId];
            return null;
        }

        internal HashSet<Guid> Roles(Guid userId)
        {
            if (userRoles.ContainsKey(userId))
                return userRoles[userId];
            return null;
        }

        internal bool AddToRole(Guid userId, Guid roleId)
        {
            if (!userRoles.ContainsKey(userId))
                userRoles[userId] = new HashSet<Guid>();
            userRoles[userId].Add(roleId);

            if (!roleUsers.ContainsKey(roleId))
                roleUsers[roleId] = new HashSet<Guid>();
            roleUsers[roleId].Add(userId);

            return true;
        }

        internal bool RemoveFromRole(Guid userId, Guid roleId)
        {
            try
            {
                if (userRoles.ContainsKey(userId))
                {
                    HashSet<Guid> tmp = userRoles[userId];
                    if (tmp.Count > 1)
                        tmp.Remove(roleId);
                    else
                        userRoles.Remove(userId);
                }
                if (roleUsers.ContainsKey(roleId))
                    roleUsers[roleId].Remove(userId);
                return true;
            }
            catch
            { }
            return false;
        }

        internal void RemoveRole(Guid roleId)
        {
            try
            {
                if (roleUsers.ContainsKey(roleId))
                {
                    HashSet<Guid> userids = roleUsers[roleId];  //these are the specific items with this role
                    roleUsers.Remove(roleId);
                    //now we have to remove the reverse in total
                    foreach(Guid cur in userids)
                    {
                        if (this.userRoles.ContainsKey(cur))
                            this.userRoles.Remove(cur);
                    }
                }
            }
            catch
            { }
        }

        internal void RemoveUser(Guid userId)
        {
            if (this.userRoles.ContainsKey(userId))
            {
                HashSet<Guid> roles = this.userRoles[userId];
                this.userRoles.Remove(userId);
                foreach(Guid cur in roles)
                {
                    if (this.roleUsers.ContainsKey(cur))
                        this.roleUsers.Remove(cur);
                }
            }
        }

        internal UserRoleMembership()
        { }
    }

    internal sealed class RoleRoleMembership
    {
        private readonly Dictionary<Guid, HashSet<Guid>> Members = new Dictionary<Guid, HashSet<Guid>>();

        internal HashSet<Guid> Children(Guid roleId)
        {
            if (Members.ContainsKey(roleId))
                return Members[roleId];
            return null;
        }

        internal HashSet<Guid> Parents(Guid roleId)
        {
            //ooph - gotta go looking
            HashSet<Guid> tmp = new HashSet<Guid>();
            foreach(KeyValuePair<Guid, HashSet<Guid>> cur in Members)
            {
                if (cur.Value.Contains(roleId))
                    tmp.Add(cur.Key);
            }
            return tmp;
        }

        internal bool Add(Guid role, Guid child)
        {
            if (!this.Members.ContainsKey(role))
                this.Members.Add(role, new HashSet<Guid>());
            this.Members[role].Add(child);
            return true;
        }

        internal bool Add(Guid role, HashSet<Guid> children)
        {
            if (children != null && children.Count > 0)
            {
                if (!this.Members.ContainsKey(role))
                    this.Members.Add(role, new HashSet<Guid>());
                HashSet<Guid> tmp = this.Members[role];
                foreach (Guid cur in children)
                {
                    tmp.Add(cur);
                }
                return true;
            }
            return false;
        }

        internal void Remove(Guid role)
        {
            if (this.Members.ContainsKey(role))
                this.Members.Remove(role);
        }

        internal bool Remove(Guid role, Guid child)
        {
            if (!this.Members.ContainsKey(role))
                return true;
            this.Members[role].Remove(child);
            return true;
        }

        internal bool Remove(Guid role, HashSet<Guid> children)
        {
            if (children != null && children.Count > 0)
            {
                if (!this.Members.ContainsKey(role))
                    return true; 
                HashSet<Guid> tmp = this.Members[role];
                foreach (Guid cur in children)
                {
                    tmp.Remove(cur);
                }
                if (tmp.Count<1)
                {
                    this.Members.Remove(role);
                }
                return true;
            }
            return false;
        }

        internal RoleRoleMembership()
        { }
    }

    internal sealed class RolePermissionNode
    {
        private readonly HashSet<Guid> grants = new HashSet<Guid>();
        private readonly HashSet<Guid> denies = new HashSet<Guid>();

        internal int Count
        {
            get { return this.grants.Count + this.denies.Count; }
        }

        internal bool HasGrant(Guid permissionId)
        {
            return this.grants.Contains(permissionId);
        }

        internal bool HasDeny(Guid permissionId)
        {
            return this.denies.Contains(permissionId);
        }

        internal void Fill(HashSet<PermissionAssignment> perms)
        {
            foreach (Guid cur in denies)
            {
                perms.Add(new PermissionAssignment(PermissionMemorySet.Get(cur), GrantType.Deny));
            }
            foreach (Guid cur in grants)
            {
                perms.Add(new PermissionAssignment(PermissionMemorySet.Get(cur), GrantType.Grant));
            }
        }

        internal void Add(Guid permissionId, bool isGrant)
        {
            if (isGrant)
            {
                this.denies.Remove(permissionId);
                this.grants.Add(permissionId);
            }
            else
            {
                this.grants.Remove(permissionId);
                this.denies.Add(permissionId);
            }
        }

        internal void Remove(Guid permissionId, bool isGrant)
        {
            if (isGrant)
            {
                this.grants.Remove(permissionId);
            }
            else
            {
                this.denies.Remove(permissionId);
            }
        }

        internal void Remove(Guid permissionId)
        {
            this.grants.Remove(permissionId);
            this.denies.Remove(permissionId);
        }

        internal RolePermissionNode() { }
    }

    internal sealed class RolePermissionMembership
    {
        private readonly Dictionary<Guid, RolePermissionNode> rolePermissions = new Dictionary<Guid, RolePermissionNode>();

        internal bool HasGrant(Guid roleId, Guid permissionId)
        {
            if (this.rolePermissions.ContainsKey(roleId))
                return this.rolePermissions[roleId].HasGrant(permissionId);
            return false;
        }

        internal bool HasDeny(Guid roleId, Guid permissionId)
        {
            if (this.rolePermissions.ContainsKey(roleId))
                return this.rolePermissions[roleId].HasDeny(permissionId);
            return false;
        }

        internal void Fill(HashSet<PermissionAssignment> perms, Guid roleId)
        {
            if (rolePermissions.ContainsKey(roleId))
            {
                rolePermissions[roleId].Fill(perms);
            }
        }

        internal void Fill(HashSet<PermissionAssignment> perms, HashSet<Guid> roleIds)
        {
            foreach(Guid roleId in roleIds)
            {
                if (rolePermissions.ContainsKey(roleId))
                {
                    rolePermissions[roleId].Fill(perms);
                }
            }
        }

        internal void Add(Guid roleId, Guid permissionId, bool isGrant)
        {
            if (!this.rolePermissions.ContainsKey(roleId))
                this.rolePermissions.Add(roleId, new RolePermissionNode());
            this.rolePermissions[roleId].Add(permissionId, isGrant);
        }

        internal void Remove(Guid roleId, Guid permissionId, bool isGrant)
        {
            if (!this.rolePermissions.ContainsKey(roleId))
                return;
            RolePermissionNode tmp = this.rolePermissions[roleId];
            tmp.Remove(permissionId, isGrant);
            if (tmp.Count < 1)
                this.rolePermissions.Remove(roleId);

        }

        internal void Remove(Guid roleId, Guid permissionId)
        {
            if (rolePermissions.ContainsKey(roleId))
            {
                rolePermissions[roleId].Remove(permissionId);
            }
        }

        internal void RemoveRole(Guid roleId)
        {
            if (this.rolePermissions.ContainsKey(roleId))
            {
                this.rolePermissions.Remove(roleId);
            }
        }

        internal void RemovePermission(Guid permissionId)
        {
            foreach(RolePermissionNode cur in rolePermissions.Values)
            {
                cur.Remove(permissionId);
            }
        }

        internal RolePermissionMembership()
        { }
    }
}
