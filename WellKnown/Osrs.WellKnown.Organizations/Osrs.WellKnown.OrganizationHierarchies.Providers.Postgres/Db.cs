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
using System.Data;

namespace Osrs.WellKnown.OrganizationHierarchies.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;
        internal static readonly Guid DataStoreIdentity = new Guid("{EC65C5A0-C608-432C-8EE5-198D5D32DA40}"); //TODO -- move this to db

        internal const string CountMem = "SELECT COUNT(*) FROM oncor.\"OrganizationHierarchyMembers\" WHERE ";
        internal const string SelectMem = "SELECT \"ParentSystemId\", \"ParentId\", \"ChildSystemId\", \"ChildId\" FROM oncor.\"OrganizationHierarchyMembers\" WHERE \"SystemId\"=:sid AND \"Id\"=:id ";
        internal const string SelectMemCh = "SELECT \"ChildSystemId\", \"ChildId\" FROM oncor.\"OrganizationHierarchyMembers\" WHERE \"SystemId\"=:sid AND \"Id\"=:id AND \"ParentSystemId\"=:psid AND \"ParentId\"=:pid";
        internal const string UpdateMem = "SELECT oncor.\"updateorghierarchymembers\"(:sid,:id,:psid,:pid,:csid,:cid)";
        internal const string DeleteMem = "DELETE FROM oncor.\"OrganizationHierarchyMembers\" WHERE \"SystemId\"=:sid AND \"Id\"=:id ";

        internal const string WhereElem = "AND \"ChildSystemId\"=:csid AND \"ChildId\"=:cid ";
        internal const string WhereElemParent = "AND \"ParentSystemId\"=:psid AND \"ParentId\"=:pid ";

        internal const string Count = "SELECT COUNT(*) FROM oncor.\"OrganizationHierarchies\" WHERE ";
        internal const string Select = "SELECT \"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\" FROM oncor.\"OrganizationHierarchies\" WHERE ";
        internal const string Insert = "INSERT INTO oncor.\"OrganizationHierarchies\"(\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\") VALUES (:sid, :id, :osid, :oid, :name, :desc)";
        internal const string Update = "UPDATE oncor.\"OrganizationHierarchies\" SET \"Name\"=:name, \"Description\"=:desc \"OwnerSystemId\"=:osid, \"OwnerId\"=:oid WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string Delete = "DELETE FROM oncor.\"OrganizationHierarchies\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";

        internal const string WhereId = "\"SystemId\"=:sid AND \"Id\"=:id";
        internal const string WhereName = "lower(\"Name\")=lower(:name)";

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
