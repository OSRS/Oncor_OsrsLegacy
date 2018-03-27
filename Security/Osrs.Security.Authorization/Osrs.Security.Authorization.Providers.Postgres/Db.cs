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
using System.Data;

namespace Osrs.Security.Authorization.Providers
{
    internal static class Db
    {
        internal static string PermissionConnectionString;
        internal static string RoleConnectionString;

        internal const string countSelectPermission = "SELECT COUNT(*) FROM osrs.\"SecurityPermissions\"";
        internal const string countSelectRole = "SELECT COUNT(*) FROM osrs.\"SecurityRoles\"";

        internal const string SelectPermission = "SELECT \"Id\", \"Name\" FROM osrs.\"SecurityPermissions\"";
        internal const string SelectPermissionById = " WHERE \"Id\"=:id";
        internal const string SelectPermissionByName = " WHERE \"Name\"=:name";

        internal const string RegisterPermission = "INSERT INTO osrs.\"SecurityPermissions\" (\"Id\", \"Name\") VALUES (:id,:name)";
        internal const string UnRegisterPermission = "DELETE FROM osrs.\"SecurityPermissions\" WHERE \"Id\"=:id";

        internal const string RemoveRole_Permission = "DELETE FROM osrs.\"SecurityRoleMemberPermissions\" WHERE \"RoleId\"=:id";
        internal const string RemoveRole_User = "DELETE FROM osrs.\"SecurityRoleMemberUsers\" WHERE \"RoleId\"=:id";
        internal const string RemovePermission_Roles = "DELETE FROM osrs.\"SecurityRoleMemberPermissions\" WHERE \"PermissionId\"=:id";
        internal const string RemoveUser_Roles = "DELETE FROM osrs.\"SecurityRoleMemberUsers\" WHERE \"UserId\"=:id";
        internal const string RemoveRole_Roles = "DELETE FROM osrs.\"SecurityRoleMemberRoles\" WHERE \"ParentId\"=:id OR \"ChildId\"=:id";
        internal const string RemovePermissionFromRole = "DELETE FROM osrs.\"SecurityRoleMemberPermissions\" WHERE \"RoleId\"=:rid AND \"PermissionId\"=:id";
        internal const string RemoveUserFromRole = "DELETE FROM osrs.\"SecurityRoleMemberUsers\" WHERE \"RoleId\"=:rid AND \"UserId\"=:id";
        internal const string RemoveRoleFromRole = "DELETE FROM osrs.\"SecurityRoleMemberRoles\" WHERE \"ParentId\"=:pid AND \"ChildId\"=:cid";

        internal const string SelectRole = "SELECT \"Id\", \"Name\" FROM osrs.\"SecurityRoles\"";
        internal const string SelectRoleById = " WHERE \"Id\"=:id";
        internal const string SelectRoleByName = " WHERE \"Name\"=:name";
        internal const string SelectRoleParents = "SELECT \"SecurityRoleMemberRoles\".\"ChildId\", \"SecurityRoles\".\"Id\", \"SecurityRoles\".\"Name\" FROM osrs.\"SecurityRoles\", osrs.\"SecurityRoleMemberRoles\" WHERE \"SecurityRoleMemberRoles\".\"ChildId\" = :cid AND \"SecurityRoleMemberRoles\".\"ParentId\" = \"SecurityRoles\".\"Id\"";
        internal const string SelectRoleChildren = "SELECT \"SecurityRoleMemberRoles\".\"ParentId\", \"SecurityRoles\".\"Id\", \"SecurityRoles\".\"Name\" FROM osrs.\"SecurityRoles\", osrs.\"SecurityRoleMemberRoles\" WHERE \"SecurityRoleMemberRoles\".\"ParentId\" = :pid AND \"SecurityRoleMemberRoles\".\"ChildId\" = \"SecurityRoles\".\"Id\"";
        internal const string SelectRoleUser = "SELECT \"SecurityRoles\".\"Id\", \"SecurityRoles\".\"Name\" FROM osrs.\"SecurityRoles\", osrs.\"SecurityRoleMemberUsers\" WHERE \"SecurityRoleMemberUsers\".\"UserId\" =:uid AND \"SecurityRoleMemberUsers\".\"RoleId\" = \"SecurityRoles\".\"Id\"";

        internal const string SelectUserByRole = "SELECT \"RoleId\", \"UserId\" FROM osrs.\"SecurityRoleMemberUsers\" WHERE \"RoleId\"=:rid";

        internal const string SelectRolePermissions = "SELECT \"RoleId\", \"PermissionId\", \"IsGrant\" FROM osrs.\"SecurityRoleMemberPermissions\"";
        internal const string SelectRoleByRoleId = " WHERE \"RoleId\"=:rid";

        internal const string UpdateRole = "UPDATE osrs.\"SecurityRoles\" SET \"Name\"=:name WHERE \"Id\"=:id";
        internal const string CreateRole = "INSERT INTO osrs.\"SecurityRoles\" (\"Id\", \"Name\") VALUES (:id,:name)";
        internal const string DeleteRole = "DELETE FROM osrs.\"SecurityRoles\" WHERE \"Id\"=:id";

        internal const string InsertRoleMemberRole = "INSERT INTO osrs.\"SecurityRoleMemberRoles\" (\"ParentId\", \"ChildId\") VALUES (:pid,:cid)";
        internal const string InsertRoleMemberUser = "INSERT INTO osrs.\"SecurityRoleMemberUsers\" (\"RoleId\", \"UserId\") VALUES (:rid,:uid)";
        internal const string InsertRoleMemberPermission = "INSERT INTO osrs.\"SecurityRoleMemberPermissions\" (\"RoleId\", \"PermissionId\", \"IsGrant\") VALUES (:rid,:pid,:isG) ON CONFLICT (\"RoleId\", \"PermissionId\") DO UPDATE SET \"IsGrant\" = :isG";

        internal static PermissionBuilder PermissionBuilder = new PermissionBuilder();
        internal static RoleBuilder RoleBuilder = new RoleBuilder();

        internal static NpgsqlConnection GetCon(string conString)
        {
            try
            {
                NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder(conString);
                if (sb.Timeout == 15) //default
                    sb.Timeout = 60;
                if (sb.CommandTimeout == 30) //default
                    sb.CommandTimeout = 60;
                sb.Pooling = false;
                NpgsqlConnection conn = new NpgsqlConnection(sb.ToString());
                return conn;
            }
            catch
            { }
            return null;
        }

        internal static NpgsqlCommand GetCmd(NpgsqlConnection con)
        {
            if (con == null)
                return null;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                if (cmd != null)
                {
                    cmd.Connection = con;
                    return cmd;
                }
            }
            catch
            { }
            return null;
        }

        internal static NpgsqlCommand GetCmd(string conString)
        {
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = GetCon(conString);
                return cmd;
            }
            catch
            { }
            return null;
        }

        internal static int ExecuteNonQuery(NpgsqlCommand cmd)
        {
            int res = int.MinValue;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                res = cmd.ExecuteNonQuery();
            }
            catch
            { }

            try
            {
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
            }
            catch
            { }

            return res;
        }

        internal static NpgsqlDataReader ExecuteReader(NpgsqlCommand cmd)
        {
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                return cmd.ExecuteReader();
            }
            catch
            { }
            return null;
        }

        internal static void Close(NpgsqlConnection con)
        {
            try
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
            catch
            { }
        }

        internal static void Close(NpgsqlCommand cmd)
        {
            if (cmd != null && cmd.Connection != null)
                Close(cmd.Connection);
        }

        internal static bool Exists(NpgsqlCommand cmd)
        {
            NpgsqlDataReader rdr = null;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                rdr = ExecuteReader(cmd);
                rdr.Read();

                try
                {
                    long ct = (long)(rdr[0]);
                    if (cmd.Connection.State == System.Data.ConnectionState.Open)
                        cmd.Connection.Close();

                    return ct > 0L;
                }
                catch
                { }
            }
            catch
            {
                Close(cmd);
            }
            finally
            {
                cmd.Dispose();
            }
            return false;
        }
    }
}
