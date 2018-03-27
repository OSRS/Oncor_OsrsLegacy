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
using System;
using System.Collections.Generic;

namespace Osrs.Security.Authorization.Providers
{
    public sealed class PostgresPermissionProvider : IPermissionProvider
    {
        private readonly UserSecurityContext context;

        public bool CanManagePermissions()
        {
            return AuthorizationManager.Instance.GetRoleProvider(this.context).HasPermission(this.Get(PermissionUtils.ManagePermissionsPermissionId));
        }

        public bool Exists(Guid id)
        {
            if (Guid.Empty.Equals(id))
                return false;
            NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
            cmd.CommandText = Db.countSelectPermission + Db.SelectPermissionById;
            cmd.Parameters.AddWithValue("id", id);
            return Db.Exists(cmd);
        }

        public bool Exists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
            cmd.CommandText = Db.countSelectPermission + Db.SelectPermissionByName;
            cmd.Parameters.AddWithValue("name", name);
            return Db.Exists(cmd);
        }

        public Permission Get(Guid id)
        {
            if (Guid.Empty.Equals(id))
                return null;
            NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
            cmd.CommandText = Db.SelectPermission + Db.SelectPermissionById;
            cmd.Parameters.AddWithValue("id", id);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            Permission o = null;
            if (rdr != null)
            {
                try
                {
                    rdr.Read();
                    o = Db.PermissionBuilder.Build(rdr);
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

        public Permission Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
            cmd.CommandText = Db.SelectPermission + Db.SelectPermissionByName;
            cmd.Parameters.AddWithValue("name", name);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            Permission o = null;
            if (rdr != null)
            {
                try
                {
                    rdr.Read();
                    o = Db.PermissionBuilder.Build(rdr);
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

        public IEnumerable<Permission> GetPermissions()
        {
            NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
            cmd.CommandText = Db.SelectPermission;
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            List<Permission> permissions = new List<Permission>();
            try
            {
                Permission o;
                while(rdr.Read())
                {
                    o = Db.PermissionBuilder.Build(rdr);
                    if (o != null)
                        permissions.Add(o);
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

            return permissions;
        }

        public bool RegisterPermission(Permission permission)
        {
            if (permission!=null && !Guid.Empty.Equals(permission.Id) && CanManagePermissions())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
                    cmd.CommandText = Db.RegisterPermission;
                    cmd.Parameters.AddWithValue("id", permission.Id);
                    cmd.Parameters.AddWithValue("name", permission.Name);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool UnregisterPermission(Permission permission)
        {
            if (permission != null && !IsBuiltIn(permission.Id))
            {
                if (CanManagePermissions())
                {
                    try
                    {
                        NpgsqlCommand cmd = Db.GetCmd(Db.PermissionConnectionString);
                        cmd.CommandText = Db.UnRegisterPermission;
                        cmd.Parameters.AddWithValue("id", permission.Id);

                        Db.ExecuteNonQuery(cmd);

                        return true;
                    }
                    catch
                    { }
                }
            }
            return false;
        }

        //Ensure built-in security permissions are not deletable
        private bool IsBuiltIn(Guid permissionId)
        {
            if (PermissionUtils.CreateRolePermissionId.Equals(permissionId))
                return true;
            if (PermissionUtils.DeleteRolePermissionId.Equals(permissionId))
                return true;
            if (PermissionUtils.ManagePermissionsPermissionId.Equals(permissionId))
                return true;
            if (PermissionUtils.ManageRolePermissionId.Equals(permissionId))
                return true;
            return false;
        }

        internal PostgresPermissionProvider(UserSecurityContext context)
        {
            this.context = context;
        }
    }
}
