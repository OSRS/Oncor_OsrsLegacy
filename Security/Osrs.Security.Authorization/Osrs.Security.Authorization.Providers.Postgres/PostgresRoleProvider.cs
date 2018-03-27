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

using Npgsql;
using Osrs.Data.Postgres;
using System;
using System.Collections.Generic;

namespace Osrs.Security.Authorization.Providers
{
    public sealed class PostgresRoleProvider : IRoleProvider
    {
        private readonly UserSecurityContext context;

        public bool Exists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.countSelectRole + Db.SelectRoleByName;
            cmd.Parameters.AddWithValue("name", name);
            return Db.Exists(cmd);
        }

        public bool Exists(Guid id)
        {
            if (Guid.Empty.Equals(id))
                return false;
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.countSelectRole + Db.SelectRoleById;
            cmd.Parameters.AddWithValue("id", id);
            return Db.Exists(cmd);
        }

        public Role Get(Guid id)
        {
            if (Guid.Empty.Equals(id))
                return null;
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRole + Db.SelectRoleById;
            cmd.Parameters.AddWithValue("id", id);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            Role o = null;
            if (rdr != null)
            {
                try
                {
                    rdr.Read();
                    o = Db.RoleBuilder.Build(rdr);
                    if (cmd.Connection.State == System.Data.ConnectionState.Open)
                        cmd.Connection.Close();
                }
                catch
                { }
                finally
                {
                    cmd.Dispose();
                }
            }
            return o;
        }

        public Role Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRole + Db.SelectRoleByName;
            cmd.Parameters.AddWithValue("name", name);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            Role o = null;
            if (rdr != null)
            {
                try
                {
                    rdr.Read();
                    o = Db.RoleBuilder.Build(rdr);
                    if (cmd.Connection.State == System.Data.ConnectionState.Open)
                        cmd.Connection.Close();
                }
                catch
                { }
                finally
                {
                    cmd.Dispose();
                }
            }
            return o;
        }

        public Role Create(string name)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    Guid id = Guid.NewGuid();
                    NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                    cmd.CommandText = Db.CreateRole;
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", name);

                    int ct = Db.ExecuteNonQuery(cmd); // 1?

                    return new Role(name, id);
                }
                catch
                { }
            }
            return null;
        }

        public bool Delete(IEnumerable<Role> roles)
        {
            if (roles!=null && this.CanDelete())
            {
                foreach(Role cur in roles)
                {
                    if (!Db.RoleBuilder.Delete(cur))
                        return false; //just in case something happens along the way
                }
            }
            return false;
        }

        public bool Delete(Role role)
        {
            if (role!=null && this.CanDelete())
            {
                return Db.RoleBuilder.Delete(role);
            }
            return false;
        }

        public bool Update(Role role)
        {
            if (role != null && (CanCreate() || CanDelete()))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                    cmd.CommandText = Db.UpdateRole;
                    cmd.Parameters.AddWithValue("id", role.Id);
                    cmd.Parameters.AddWithValue("name", role.Name);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool AddToRole(Role parent, Role child)
        {
            if (parent != null && child != null && !(parent.Id.Equals(child.Id)) && CanModifyMembership())
                return Db.RoleBuilder.Add(parent, child);
            return false;
        }

        public bool AddToRole(Role parent, IEnumerable<Role> children)
        {
            if (parent!=null && children!=null && CanModifyMembership())
            {
                foreach(Role cur in children)
                {
                    if (cur!=null && !(parent.Id.Equals(cur.Id)))
                    {
                        if (!Db.RoleBuilder.Add(parent, cur))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddToRole(Role role, IEnumerable<IUserIdentity> users)
        {
            if (role != null && users != null && CanModifyMembership())
            {
                foreach (IUserIdentity cur in users)
                {
                    if (cur != null)
                    {
                        if (!Db.RoleBuilder.Add(role, cur))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddToRole(Role role, IUserIdentity user)
        {
            if (role != null && user != null && CanModifyMembership())
            {
                return Db.RoleBuilder.Add(role, user);
            }
            return false;
        }

        public bool AddToRole(Role role, Permission permission)
        {
            return this.AddToRole(role, permission, GrantType.Grant);
        }

        public bool AddToRole(Role role, IEnumerable<Permission> permission)
        {
            return this.AddToRole(role, permission, GrantType.Grant);
        }

        public bool AddToRole(Role role, IEnumerable<Permission> permission, GrantType grantOrDeny)
        {
            if (role!=null && permission!=null && this.CanModifyMembership())
            {
                foreach (Permission cur in permission)
                {
                    if (cur != null)
                    {
                        if (!Db.RoleBuilder.Add(role, cur, grantOrDeny))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddToRole(Role role, Permission permission, GrantType grantOrDeny)
        {
            if (role != null && permission != null && this.CanModifyMembership())
            {
                return Db.RoleBuilder.Add(role, permission, grantOrDeny);
            }
            return false;
        }

        public bool CanCreate()
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.CreateRolePermissionId);
            return this.HasPermission(p);
        }

        public bool CanCreate(IUserIdentity user)
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.CreateRolePermissionId);
            return this.HasPermission(user, p);
        }

        public bool CanDelete()
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.DeleteRolePermissionId);
            return this.HasPermission(p);
        }

        public bool CanDelete(IUserIdentity user)
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.DeleteRolePermissionId);
            return this.HasPermission(user, p);
        }

        public bool CanModifyMembership()
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.ManageRolePermissionId);
            return this.HasPermission(p);
        }

        //TODO -- add fine grained security
        public bool CanModifyMembership(Role role)
        {
            return this.CanModifyMembership();
        }

        public bool CanModifyMembership(IUserIdentity user)
        {
            Permission p = AuthorizationManager.Instance.GetPermissionProvider(this.context).Get(PermissionUtils.ManageRolePermissionId);
            return this.HasPermission(user, p);
        }

        public bool CanModifyMembership(Role role, IUserIdentity user)
        {
            return this.CanModifyMembership(user);
        }

        public bool DeleteFromRole(Role role, Permission permission)
        {
            if (role!=null && permission!=null && this.CanModifyMembership())
            {
                return Db.RoleBuilder.Remove(role, permission);
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IEnumerable<Permission> permissions)
        {
            if (role != null && permissions != null && this.CanModifyMembership())
            {
                foreach (Permission cur in permissions)
                {
                    if (cur != null)
                    {
                        if (!Db.RoleBuilder.Remove(role, cur))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role parent, Role child)
        {
            if (parent != null && child != null && this.CanModifyMembership())
            {
                return Db.RoleBuilder.Remove(parent, child);
            }
            return false;
        }

        public bool DeleteFromRole(Role parent, IEnumerable<Role> children)
        {
            if (parent != null && children != null && this.CanModifyMembership())
            {
                foreach (Role cur in children)
                {
                    if (cur != null)
                    {
                        if (!Db.RoleBuilder.Remove(parent, cur))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IEnumerable<IUserIdentity> users)
        {
            if (role!=null && users!=null && this.CanModifyMembership())
            {
                foreach (IUserIdentity cur in users)
                {
                    if (cur != null)
                    {
                        if (!Db.RoleBuilder.Remove(role, cur))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool DeleteFromRole(Role role, IUserIdentity user)
        {
            if (role!=null && user!=null && this.CanModifyMembership())
            {
                return Db.RoleBuilder.Remove(role, user);
            }
            return false;
        }

        public IEnumerable<Permission> GetPermissions(IUserIdentity user)
        {
            if (user!=null)
            {
                HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
                Db.RoleBuilder.Fill(perms, Db.RoleBuilder.GetRoles(user));
                return PermissionUtils.CompactToEffective(perms);
            }
            return null;
        }

        //Directly assigned, no cascade
        public IEnumerable<PermissionAssignment> GetPermissionAssignments(Role role)
        {
            if (role != null)
            {
                IPermissionProvider prov = AuthorizationManager.Instance.GetPermissionProvider(this.context);
                if (prov != null)
                {
                    HashSet<PermissionAssignment> perm = new HashSet<PermissionAssignment>();
                    Db.RoleBuilder.Fill(perm, role, prov);
                    return perm;
                }
            }
            return null;
        }

        //Directly assigned, no cascade
        public IEnumerable<Permission> GetPermissions(Role role)
        {
            if (role != null)
            {
                IPermissionProvider prov=AuthorizationManager.Instance.GetPermissionProvider(this.context);
                if (prov != null)
                {
                    HashSet<PermissionAssignment> perm = new HashSet<PermissionAssignment>();
                    Db.RoleBuilder.Fill(perm, role, prov);
                    return PermissionUtils.CompactToEffective(perm);
                }
            }
            return null;
        }

        public IEnumerable<Permission> GetEffectivePermissions(Role role)
        {
            if (role != null)
            {
                return PermissionUtils.CompactToEffective(Db.RoleBuilder.GetPermissions(role));
            }
            return null;
        }

        public IEnumerable<Role> GetRoles()
        {
            return new Enumerable<Role>(new EnumerableCommand<Role>(Db.RoleBuilder, Db.SelectRole, Db.RoleConnectionString));
        }

        public IEnumerable<Role> GetRoles(IUserIdentity user)
        {
            if (user != null)
                return Db.RoleBuilder.GetRoles(user);
            return null;
        }

        public IEnumerable<Role> GetParentRoles(Role role)
        {
            if (role != null)
                return Db.RoleBuilder.GetParents(role);
            return null;
        }

        public IEnumerable<Role> GetChildRoles(Role role)
        {
            if (role != null)
                return Db.RoleBuilder.GetChildren(role);
            return null;
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
                    if (Db.RoleBuilder.GetChildren(parent).Contains(child))
                        return true;

                    if (cascade)
                        return Db.RoleBuilder.ExpandRoles(child).Contains(parent);
                }
                else
                    return true; //parent==child
            }
            return false;
        }

        public IEnumerable<Guid> GetUsers(Role role)
        {
            if (role != null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.SelectUserByRole;
                cmd.Parameters.AddWithValue("rid", role.Id);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                HashSet<Guid> uids = new HashSet<Guid>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            Guid id = Osrs.Data.DbReaderUtils.GetGuid(rdr, 1);
                            if (!Guid.Empty.Equals(id))
                                uids.Add(id);
                        }
                        if (cmd.Connection.State == System.Data.ConnectionState.Open)
                            cmd.Connection.Close();
                    }
                    catch
                    { }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
                return uids;
            }
            return null;
        }

        public bool HasPermission(Permission permission)
        {
            if (permission!=null && this.context.User != null)
            {
                return HasPermission(this.context.User, permission);
            }
            return false;
        }

        public bool HasPermission(IUserIdentity user, Permission permission)
        {
            if (user != null && permission != null)
            {
                HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
                Db.RoleBuilder.Fill(perms, Db.RoleBuilder.ExpandRoles(user));
                IEnumerable<Permission> p = PermissionUtils.CompactToEffective(perms);
                if (p!=null)
                {
                    foreach(Permission pp in p)
                    {
                        if (permission.Equals(pp))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool IsUserInRole(IUserIdentity user, Role role)
        {
            if (user != null && role != null)
            {
                return Db.RoleBuilder.GetRoles(user).Contains(role);
            }
            return false;
        }

        internal PostgresRoleProvider(UserSecurityContext context)
        {
            this.context = context;
        }
    }
}
