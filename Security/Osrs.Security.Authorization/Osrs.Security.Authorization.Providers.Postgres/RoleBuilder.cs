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
using Osrs.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Osrs.Security.Authorization.Providers
{
    internal sealed class RoleBuilder : IBuilder<Role>
    {
        public Role Build(DbDataReader reader)
        {
            Guid id = DbReaderUtils.GetGuid(reader, 0);
            string name = DbReaderUtils.GetString(reader, 1);
            return new Role(name, id);
        }

        internal HashSet<PermissionAssignment> GetPermissions(Role role)
        {
            HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
            HashSet<Role> loc = GetParents(role);
            loc.Add(role);
            Fill(perms, loc);
            return perms;
        }

        internal HashSet<PermissionAssignment> GetPermissions(HashSet<Role> roles)
        {
            HashSet<Role> results = new HashSet<Role>();
            GetParents(results, roles); //will auto-add all roles
            HashSet<PermissionAssignment> perms = new HashSet<PermissionAssignment>();
            Fill(perms, results);
            return perms;
        }

        internal void Fill(HashSet<PermissionAssignment> perms, HashSet<Role> roles)
        {
            IPermissionProvider prov = AuthorizationManager.Instance.GetPermissionProvider();
            if (prov != null)
            {
                foreach (Role cur in roles)
                {
                    Fill(perms, cur, prov);
                }
            }
        }

        internal void Fill(HashSet<PermissionAssignment> perms, Role role, IPermissionProvider prov)
        {
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRolePermissions + Db.SelectRoleByRoleId;
            cmd.Parameters.AddWithValue("rid", role.Id);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            if (rdr != null)
            {
                try
                {
                    while (rdr.Read())
                    {
                        Guid pid = DbReaderUtils.GetGuid(rdr, 1);
                        bool isGrant = DbReaderUtils.GetBoolean(rdr, 2);
                        Permission p = prov.Get(pid);
                        if (p != null)
                        {
                            PermissionAssignment pa = new PermissionAssignment(p, isGrant ? GrantType.Grant : GrantType.Deny);
                            perms.Add(pa);
                        }
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
        }

        //get all roles this role is associated with directly or indirectly
        internal HashSet<Role> ExpandRoles(Role role)
        {
            HashSet<Role> results = new HashSet<Role>();
            GetParents(results, GetParents(role)); //walk up the stack of roles from the ones assigned to the user
            return results;
        }

        //get all roles this user is associated with directly or indirectly
        internal HashSet<Role> ExpandRoles(IUserIdentity user)
        {
            HashSet<Role> results = new HashSet<Role>();
            GetParents(results, GetRoles(user)); //walk up the stack of roles from the ones assigned to the user
            return results;
        }

        //gets all directly assigned roles for this user
        internal HashSet<Role> GetRoles(IUserIdentity user)
        {
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRoleUser;
            cmd.Parameters.AddWithValue("uid", user.Uid);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            HashSet<Role> rs = new HashSet<Role>();
            Role o;
            if (rdr != null)
            {
                try
                {
                    while (rdr.Read())
                    {
                        o = Db.RoleBuilder.Build(rdr);
                        if (o != null)
                            rs.Add(o);
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
            return rs;
        }

        internal void GetChildren(HashSet<Role> result, HashSet<Role> roles)
        {
            if (roles != null)
            {
                foreach (Role cur in roles)
                {
                    if (!result.Contains(cur))
                    {
                        result.Add(cur);
                        GetChildren(result, GetChildren(cur));
                    }
                }
            }
        }

        internal void GetParents(HashSet<Role> result, HashSet<Role> roles)
        {
            if (roles != null)
            {
                foreach (Role cur in roles)
                {
                    if (!result.Contains(cur))
                    {
                        result.Add(cur);
                        GetParents(result, GetParents(cur));
                    }
                }
            }
        }

        //gets all directly assigned roles (children) for this role
        internal HashSet<Role> GetChildren(Role role)
        {
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRoleChildren;
            cmd.Parameters.AddWithValue("pid", role.Id);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            HashSet<Role> rs = new HashSet<Role>();
            Role o;
            if (rdr != null)
            {
                try
                {
                    while (rdr.Read())
                    {
                        o = Db.RoleBuilder.Build(rdr);
                        if (o != null)
                            rs.Add(o);
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
            return rs;
        }

        //gets all roles directly assigned (parents) for this role
        internal HashSet<Role> GetParents(Role role)
        {
            NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
            cmd.CommandText = Db.SelectRoleParents;
            cmd.Parameters.AddWithValue("cid", role.Id);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            HashSet<Role> rs = new HashSet<Role>();
            Role o;
            if (rdr != null)
            {
                try
                {
                    while (rdr.Read())
                    {
                        o = Db.RoleBuilder.Build(rdr);
                        if (o != null)
                            rs.Add(o);
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
            return rs;
        }

        internal bool Add(Role parent, Role child)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.InsertRoleMemberRole; //:pid,:cid
                cmd.Parameters.AddWithValue("pid", parent.Id);
                cmd.Parameters.AddWithValue("cid", child.Id);

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Add(Role role, IUserIdentity user)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.InsertRoleMemberUser; //:rid,:uid
                cmd.Parameters.AddWithValue("rid", role.Id);
                cmd.Parameters.AddWithValue("uid", user.Uid);

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Add(Role role, Permission p, GrantType grant)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.InsertRoleMemberPermission; //:rid,:pid,:isG
                cmd.Parameters.AddWithValue("rid", role.Id);
                cmd.Parameters.AddWithValue("pid", p.Id);
                cmd.Parameters.AddWithValue("isG", (grant == GrantType.Grant));

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Remove(Role parent, Role child)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveRoleFromRole; //:pid,:cid
                cmd.Parameters.AddWithValue("pid", parent.Id);
                cmd.Parameters.AddWithValue("cid", child.Id);

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Remove(Role role, IUserIdentity user)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveUserFromRole; //:rid,:id
                cmd.Parameters.AddWithValue("rid", role.Id);
                cmd.Parameters.AddWithValue("id", user.Uid);

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Remove(Role role, Permission p)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemovePermissionFromRole; //:rid,:id
                cmd.Parameters.AddWithValue("rid", role.Id);
                cmd.Parameters.AddWithValue("id", p.Id);

                int ct = Db.ExecuteNonQuery(cmd); // 1?

                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Delete(Role role)
        {
            //this is a local call, so we don't bother with checks
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.DeleteRole;
                cmd.Parameters.AddWithValue("id", role.Id);

                Db.ExecuteNonQuery(cmd);
                DeleteUsers(role);
                DeletePermissions(role);
                DeleteChildren(role);

                //need to handle cascading
                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Delete(Permission p)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemovePermission_Roles;
                cmd.Parameters.AddWithValue("id", p.Id);

                Db.ExecuteNonQuery(cmd);
                //need to handle cascading
                return true;
            }
            catch
            { }
            return false;
        }

        internal bool Delete(IUserIdentity u)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveUser_Roles;
                cmd.Parameters.AddWithValue("id", u.Uid);

                Db.ExecuteNonQuery(cmd);
                //need to handle cascading
                return true;
            }
            catch
            { }
            return false;
        }

        private bool DeleteUsers(Role role)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveRole_User;
                cmd.Parameters.AddWithValue("id", role.Id);

                Db.ExecuteNonQuery(cmd);
                //need to handle cascading
                return true;
            }
            catch
            { }
            return false;
        }

        private bool DeleteChildren(Role role)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveRole_Roles;
                cmd.Parameters.AddWithValue("id", role.Id);

                Db.ExecuteNonQuery(cmd);
                return true;
            }
            catch
            { }
            return false;
        }

        private bool DeletePermissions(Role role)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.RoleConnectionString);
                cmd.CommandText = Db.RemoveRole_Permission;
                cmd.Parameters.AddWithValue("id", role.Id);

                Db.ExecuteNonQuery(cmd);
                //need to handle cascading
                return true;
            }
            catch
            { }
            return false;
        }
    }
}
