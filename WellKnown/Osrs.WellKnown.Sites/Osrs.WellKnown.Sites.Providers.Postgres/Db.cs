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
using Osrs.Numerics.Spatial.Geometry;
using System;
using System.Data;
using System.Data.Common;

namespace Osrs.WellKnown.Sites.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;
        internal static readonly Guid DataStoreIdentity = new Guid("{D927CA09-85F4-40E3-B5A8-41E83BF63D2D}"); //TODO -- move this to system module

        internal const string WhereCurrent = " \"EndDate\" IS NULL ";
        internal const string During = " (\"EndDate\" IS NULL AND \"StartDate\"<:end) OR (\"EndDate\">=:start AND \"StartDate\"<=:end) ";

        internal const string CountAliasScheme = "SELECT COUNT(*) FROM oncor.\"SiteAliasSchemes\"";
        internal const string CountAlias = "SELECT COUNT(*) FROM oncor.\"SiteAliases\"";
        internal const string CountSite = "SELECT COUNT(*) FROM oncor.\"Sites\"";
        internal const string CountHierarchy = "SELECT COUNT(*) FROM oncor.\"SiteHierarchy\"";

        internal const string SelectAliasScheme = "SELECT \"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\" FROM oncor.\"SiteAliasSchemes\"";
        internal const string SelectAlias = "SELECT \"SystemId\", \"Id\", \"SchemeSystemId\", \"SchemeId\", \"Name\", \"StartDate\", \"EndDate\" FROM oncor.\"SiteAliases\"";
        internal const string SelectSite = "SELECT \"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"Geom\", \"AltGeom\", \"StartDate\", \"EndDate\" FROM oncor.\"Sites\"";
        internal const string SelectHierarchy = "SELECT \"ParentSystemId\", \"ParentId\", \"ChildSystemId\", \"ChildId\", \"StartDate\", \"EndDate\" FROM oncor.\"SiteHierarchy\"";

        internal const string SelectSiteById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string SelectSiteByOwnerId = " WHERE \"OwnerSystemId\"=:osid AND \"OwnerId\"=:oid";
        internal const string SelectAliasById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
        internal const string SelectAliasByScheme = " WHERE \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
        internal const string SelectAliasSchemeById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string SelectAliasSchemeByOwnerId = " WHERE \"OwnerSystemId\"=:sid AND \"OwnerId\"=:id";
        internal const string SelectHierarchyByParent = " WHERE \"ParentSystemId\"=:sid AND \"ParentId\"=:id";
        internal const string SelectHierarchyByChild = " WHERE \"ChildSystemId\"=:sid AND \"ChildId\"=:id";

        internal const string InsertAliasScheme = "INSERT INTO oncor.\"SiteAliasSchemes\"(\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"StartDate\") VALUES (:sid, :id, :osid, :oid, :name, :desc, :start)";
        internal const string InsertAlias = "INSERT INTO oncor.\"SiteAliases\"(\"SystemId\", \"Id\", \"SchemeSystemId\", \"SchemeId\", \"Name\", \"StartDate\") VALUES (:sid, :id, :ssid, :scid, :name, :start)";
        internal const string InsertSite = "INSERT INTO oncor.\"Sites\"(\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"StartDate\") VALUES (:sid, :id, :osid, :oid, :name, :desc, :start)";
        internal const string InsertHierarchy = "INSERT INTO oncor.\"SiteHierarchy\"(\"ParentSystemId\", \"ParentId\", \"ChildSystemId\", \"ChildId\", \"StartDate\") VALUES (:psid, :pid, :csid, :cid, :start)";

        internal const string UpdateAliasScheme = "SELECT oncor.\"updatesitealiasscheme\"(:sid, :id, :osid, :oid, :name, :desc, :start)";
        internal const string UpdateAlias = "SELECT oncor.\"updatesitealias\"(:sid, :id, :ssid, :scid, :name, :oldName, :start)";
        internal const string UpdateSite = "SELECT oncor.\"updatesite\"(:sid, :id, :osid, :oid, :name, :desc, :loc, :poi, :start)";
        internal const string UpdateParent = "SELECT oncor.\"updatesitehierarchy\"(:psid, :pid, :csid, :cid, :start)";
        internal const string DeleteParent = "UPDATE oncor.\"SiteHierarchy\" SET \"EndDate\"=:startdate WHERE \"ParentSystemId\"=:psid AND \"ParentId\"=:pid AND \"ChildSystemId\"=:csid AND \"ChildId\"=:cid AND \"EndDate\" IS NULL";

        internal const string DeleteAliasScheme = "UPDATE oncor.\"SiteAliasSchemes\" SET \"EndDate\"=:end WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteAlias = "UPDATE oncor.\"SiteAliases\" SET \"EndDate\"=:end WHERE ";
        internal const string DeleteSite = "UPDATE oncor.\"Sites\" SET \"EndDate\"=:end WHERE \"SystemId\"=:sid AND \"Id\"=:id";

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

    internal sealed class SiteBuilder : IBuilder<Site>
    {
        internal static readonly SiteBuilder Instance = new SiteBuilder();

        //\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"Geom\", \"AltGeom\", \"StartDate\", \"EndDate\"
        public Site Build(DbDataReader reader)
        {
            Site tmp = new Site(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), 
                new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader,3)),
                DbReaderUtils.GetString(reader, 4), DbReaderUtils.GetString(reader, 5));

            if (!DBNull.Value.Equals(reader[6]))
                tmp.Location = Osrs.Numerics.Spatial.Postgres.NpgSpatialUtils.ToGeom((NpgsqlTypes.PostgisGeometry)reader[6]);

            if (!DBNull.Value.Equals(reader[7]))
                tmp.LocationMark = Osrs.Numerics.Spatial.Postgres.NpgSpatialUtils.ToGeom((NpgsqlTypes.PostgisGeometry)reader[7]) as Point2<double>;

            return tmp;
        }
    }

    internal sealed class SiteAliasBuilder : IBuilder<SiteAlias>
    {
        internal static readonly SiteAliasBuilder Instance = new SiteAliasBuilder();

        //\"SystemId\", \"Id\", \"SchemeSystemId\", \"SchemeId\", \"Name\", \"StartDate\", \"EndDate\"
        public SiteAlias Build(DbDataReader reader)
        {
            return new SiteAlias(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)),
                new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader, 3)), 
                DbReaderUtils.GetString(reader, 4));
        }
    }

    internal sealed class SiteAliasSchemeBuilder : IBuilder<SiteAliasScheme>
    {
        internal static readonly SiteAliasSchemeBuilder Instance = new SiteAliasSchemeBuilder();

        //\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\"
        public SiteAliasScheme Build(DbDataReader reader)
        {
            return new SiteAliasScheme(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)),
                new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader, 3)), 
                DbReaderUtils.GetString(reader, 4), DbReaderUtils.GetString(reader, 5));
        }
    }
}
