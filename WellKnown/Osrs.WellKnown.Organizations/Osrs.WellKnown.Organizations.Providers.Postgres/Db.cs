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
using System.Data;
using System.Data.Common;

namespace Osrs.WellKnown.Organizations.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;
        internal static readonly Guid DataStoreIdentity = new Guid("{5914629D-DD2D-4F1F-A06F-1B199FE19B37}");

        internal const string CountAliasScheme = "SELECT COUNT(*) FROM oncor.\"OrganizationAliasSchemes\"";
        internal const string CountAlias = "SELECT COUNT(*) FROM oncor.\"OrganizationAliases\"";
        internal const string CountOrg = "SELECT COUNT(*) FROM oncor.\"Organizations\"";

        internal const string SelectAliasScheme = "SELECT \"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\" FROM oncor.\"OrganizationAliasSchemes\"";
        internal const string SelectAlias = "SELECT \"SystemId\", \"Id\", \"SchemeSystemId\", \"SchemeId\", \"Name\" FROM oncor.\"OrganizationAliases\"";
        internal const string SelectOrg = "SELECT \"SystemId\", \"Id\", \"Name\", \"Description\" FROM oncor.\"Organizations\"";

        internal const string SelectOrgById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string SelectAliasById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
        internal const string SelectAliasByScheme = " WHERE \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
        internal const string SelectAliasSchemeById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string SelectAliasSchemeByOwnerId = " WHERE \"OwnerSystemId\"=:sid AND \"OwnerId\"=:id";

        internal const string InsertAliasScheme = "INSERT INTO oncor.\"OrganizationAliasSchemes\"(\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\") VALUES (:sid, :id, :osid, :oid, :name, :desc)";
        internal const string InsertAlias = "INSERT INTO oncor.\"OrganizationAliases\"(\"SystemId\", \"Id\", \"SchemeSystemId\", \"SchemeId\", \"Name\") VALUES (:sid, :id, :ssid, :scid, :name)";
        internal const string InsertOrg = "INSERT INTO oncor.\"Organizations\"(\"SystemId\", \"Id\", \"Name\", \"Description\") VALUES (:sid, :id, :name, :desc)";

        internal const string UpdateAliasScheme = "UPDATE oncor.\"OrganizationAliasSchemes\" SET \"OwnerSystemId\"=:osid, \"OwnerId\"=:oid, \"Name\"=:name, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string UpdateAlias = "UPDATE oncor.\"OrganizationAliases\" SET \"Name\"=:name WHERE \"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid AND \"Name\"=:origName";
        internal const string UpdateOrg = "UPDATE oncor.\"Organizations\" SET \"Name\"=:name, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";

        internal const string DeleteAliasScheme = "DELETE FROM oncor.\"OrganizationAliasSchemes\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteAlias = "DELETE FROM oncor.\"OrganizationAliases\" WHERE ";
        internal const string DeleteOrg = "DELETE FROM oncor.\"Organizations\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";

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

    //Field Order:  SystemId, Id, Name, Description
    internal sealed class OrganizationBuilder : IBuilder<Organization>
    {
        internal static readonly OrganizationBuilder Instance = new OrganizationBuilder();

        public Organization Build(DbDataReader reader)
        {
            return new Organization(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), DbReaderUtils.GetString(reader, 2), DbReaderUtils.GetString(reader, 3));
        }
    }

    //Field Order:  SystemId, Id, SchemeSystemId, SchemeId, Name
    internal sealed class OrganizationAliasBuilder : IBuilder<OrganizationAlias>
    {
        internal static readonly OrganizationAliasBuilder Instance = new OrganizationAliasBuilder();

        public OrganizationAlias Build(DbDataReader reader)
        {
            return new OrganizationAlias(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)),
                new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader, 3)), DbReaderUtils.GetString(reader, 4));
        }
    }

    //Field Order:  SystemId, Id, OrgSystemId, OrgId, Name, Description
    internal sealed class OrganizationAliasSchemeBuilder : IBuilder<OrganizationAliasScheme>
    {
        internal static readonly OrganizationAliasSchemeBuilder Instance = new OrganizationAliasSchemeBuilder();

        public OrganizationAliasScheme Build(DbDataReader reader)
        {
            return new OrganizationAliasScheme(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), 
                new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader, 3)), DbReaderUtils.GetString(reader, 4), DbReaderUtils.GetString(reader, 5));
        }
    }
}
