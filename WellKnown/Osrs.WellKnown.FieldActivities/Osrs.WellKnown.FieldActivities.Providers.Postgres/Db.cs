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
using Osrs.Numerics;
using System;
using System.Data;
using System.Data.Common;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;
        internal static readonly Guid DataStoreIdentity = new Guid("{80000567-299B-4992-B3E8-93E735EE9986}");

        internal const string SelectById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";

        internal const string CountActivities = "SELECT COUNT(*) FROM oncor.\"FieldActivities\"";
        internal const string SelectActivities = "SELECT \"SystemId\", \"Id\", \"ProjectSystemId\", \"ProjectId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"StartDate\", \"EndDate\", \"Name\", \"Description\" FROM oncor.\"FieldActivities\"";
        internal const string UpdateActivity = "UPDATE oncor.\"FieldActivities\" SET \"ProjectSystemId\"=:psid, \"ProjectId\"=:pid, \"PrincipalOrgSystemId\"=:osid, \"PrincipalOrgId\"=:oid, \"Name\"=:name, \"Description\"=:desc, \"StartDate\"=:start, \"EndDate\"=:end WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteActivity = "DELETE FROM oncor.\"FieldActivities\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string InsertActivity = "INSERT INTO oncor.\"FieldActivities\"(\"SystemId\", \"Id\", \"ProjectSystemId\", \"ProjectId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"StartDate\", \"EndDate\", \"Name\", \"Description\") VALUES (:sid, :id, :psid, :pid, :osid, :oid, :start, :end, :name, :desc)";

        internal const string CountTrips = "SELECT COUNT(*) FROM oncor.\"FieldTrips\"";
        internal const string SelectTrips = "SELECT \"SystemId\", \"Id\", \"FieldActivitySystemId\", \"FieldActivityId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\" FROM oncor.\"FieldTrips\"";
        internal const string UpdateTrip = "UPDATE oncor.\"FieldTrips\" SET \"FieldActivitySystemId\"=:fasid, \"FieldActivityId\"=:faid, \"PrincipalOrgSystemId\"=:osid, \"PrincipalOrgId\"=:oid, \"Name\"=:name, \"Description\"=:desc, \"StartDate\"=:start, \"EndDate\"=:end WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteTrip = "DELETE FROM oncor.\"FieldTrips\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string InsertTrip = "INSERT INTO oncor.\"FieldTrips\"(\"SystemId\", \"Id\", \"FieldActivitySystemId\", \"FieldActivityId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\") VALUES (:sid,:id,:fasid,:faid,:osid,:oid,:name,:desc,:start,:end)";

        internal const string CountEvent = "SELECT COUNT(*) FROM oncor.\"SamplingEvents\"";
        internal const string SelectEvent = "SELECT \"SystemId\", \"Id\", \"FieldTripSystemId\", \"FieldTripId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\" FROM oncor.\"SamplingEvents\"";
        internal const string UpdateEvent = "UPDATE oncor.\"SamplingEvents\" SET \"FieldTripSystemId\"=:ftsid, \"FieldTripId\"=:ftid, \"PrincipalOrgSystemId\"=:osid, \"PrincipalOrgId\"=:oid, \"Name\"=:name, \"Description\"=:desc, \"StartDate\"=:start, \"EndDate\"=:end WHERE \"SystemId\"=:sid, \"Id\"=:id";
        internal const string DeleteEvent = "DELETE FROM oncor.\"SamplingEvents\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string InsertEvent = "INSERT INTO oncor.\"SamplingEvents\"(\"SystemId\", \"Id\", \"FieldTripSystemId\", \"FieldTripId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\") VALUES (:sid, :id, :ftsid, :ftid, :osid, :oid, :name, :desc, :start, :end)";

        internal const string CountFTMRoles = "SELECT COUNT(*) FROM oncor.\"FieldTeamMemberRoles\"";
        internal const string SelectFTMRoles = "SELECT \"SystemId\", \"Id\", \"Name\", \"Description\" FROM oncor.\"FieldTeamMemberRoles\"";
        internal const string UpdateFTMRole = "UPDATE oncor.\"FieldTeamMemberRoles\" SET \"Name\"=:name, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteFTMRole = "DELETE FROM oncor.\"FieldTeamMemberRoles\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string InsertFTMRole = "INSERT INTO oncor.\"FieldTeamMemberRoles\"(\"SystemId\", \"Id\", \"Name\", \"Description\") VALUES (:sid, :id, :name, :desc)";

        internal const string CountFieldTeams = "SELECT COUNT(*) FROM oncor.\"FieldTeams\"";
        internal const string SelectFieldTeams = "SELECT \"SystemId\", \"Id\", \"Name\", \"Description\" FROM oncor.\"FieldTeams\"";
        internal const string UpdateFieldTeam = "UPDATE oncor.\"FieldTeams\" SET \"Name\"=:name, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string DeleteFieldTeam = "DELETE FROM oncor.\"FieldTeams\" WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string InsertFieldTeam = "INSERT INTO oncor.\"FieldTeams\"(\"SystemId\", \"Id\", \"Name\", \"Description\") VALUES (:sid, :id, :name, :desc)";

        internal const string SelectFieldTeamMembers = "SELECT \"FieldTeamSystemId\", \"FieldTeamId\", \"PersonSystemId\", \"PersonId\", \"FieldTeamMemberRoleSystemId\", \"FieldTeamMemberRoleId\" FROM oncor.\"FieldTeamMembers\" WHERE \"FieldTeamSystemId\"=:sid AND \"FieldTeamId\"=:id";
        internal const string DeleteFieldTeamMembers = "DELETE FROM oncor.\"FieldTeamMembers\" WHERE \"FieldTeamSystemId\"=:sid AND \"FieldTeamId\"=:id";
        internal const string InsertFieldTeamMembers = "INSERT INTO oncor.\"FieldTeamMembers\"(\"FieldTeamSystemId\", \"FieldTeamId\", \"PersonSystemId\", \"PersonId\", \"FieldTeamMemberRoleSystemId\", \"FieldTeamMemberRoleId\") VALUES (:sid, :id, :psid, :pid, :rsid, :rid)";

        internal const string CountFieldTeamByAssociation = "SELECT COUNT(*) FROM oncor.\"FieldTeamAssociations\" WHERE \"FieldTeamSystemId\"=:fsid AND \"FieldTeamId\"=:fid AND \"EntitySystemId\"=:esid AND \"EntityId\"=:eid AND \"WellKnownTypeId\"=:wtid";
        internal const string SelectFieldTeamByAssociation = "SELECT \"FieldTeams\".\"SystemId\", \"FieldTeams\".\"Id\", \"FieldTeams\".\"Name\", \"FieldTeams\".\"Description\" FROM oncor.\"FieldTeamAssociations\", oncor.\"FieldTeams\" WHERE \"FieldTeamAssociations\".\"FieldTeamSystemId\" = \"FieldTeams\".\"SystemId\" AND \"FieldTeamAssociations\".\"FieldTeamId\" = \"FieldTeams\".\"Id\" AND \"FieldTeamAssociations\".\"EntitySystemId\"=:esid AND \"FieldTeamAssociations\".\"EntityId\"=:eid AND \"FieldTeamAssociations\".\"WellKnownTypeId\"=:wtid";
        internal const string SelectFieldTeamAssociations = "SELECT \"FieldTeamSystemId\", \"FieldTeamId\", \"EntitySystemId\", \"EntityId\", \"WellKnownTypeId\" FROM oncor.\"FieldTeamAssociations\" ";
        internal const string DeleteFieldTeamAssociations = "DELETE FROM oncor.\"FieldTeamAssociations\" WHERE ";
        internal const string InsertFieldTeamAssociations = "INSERT INTO oncor.\"FieldTeamAssociations\"(\"FieldTeamSystemId\", \"FieldTeamId\", \"EntitySystemId\", \"EntityId\", \"WellKnownTypeId\") VALUES (:fsid, :fid, :esid, :eid, :wtid)";
        internal const string WhereFtaFT = "\"FieldTeamSystemId\"=:fsid AND \"FieldTeamId\"=:fid ";
        internal const string WhereFtaPer = "\"EntitySystemId\"=:esid AND \"EntityId\"=:eid ";
        internal const string WhereFtaWkt = "\"WellKnownTypeId\"=:wtid ";

        public static ValueRange<DateTime> CleanRange(ValueRange<DateTime> item)
        {
            DateTime start;
            DateTime end;
            if (item.Min.Kind != DateTimeKind.Utc)
                start = item.Min.ToUniversalTime();
            else
                start = item.Min;

            if (item.Max.Kind != DateTimeKind.Utc)
                end = item.Max.ToUniversalTime();
            else
                end = item.Max;

            return new ValueRange<DateTime>(start, end);
        }

        private static NpgsqlTypes.NpgsqlDateTime minSafe = new NpgsqlTypes.NpgsqlDateTime(DateTime.MinValue);
        private static NpgsqlTypes.NpgsqlDateTime maxSafe = new NpgsqlTypes.NpgsqlDateTime(DateTime.MaxValue);
        private static DateTime MinValueUtc = new DateTime(0L, DateTimeKind.Utc);
        private static DateTime MaxValueUtc = new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);

        internal static DateTime SafeGetDate(DbDataReader rdr, int index, DateTime defVal)
        {
            try
            {
                if (!DBNull.Value.Equals(rdr[index]))
                {
                    NpgsqlTypes.NpgsqlDateTime tmp = (NpgsqlTypes.NpgsqlDateTime)rdr.GetProviderSpecificValue(index);
                    if (tmp <= minSafe)
                        return MinValueUtc;
                    else if (tmp >= maxSafe)
                        return MaxValueUtc;
                    return ((DateTime)tmp).ToUniversalTime();
                }
            }
            catch { }
            return defVal.ToUniversalTime();
        }

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
            catch(Exception e)
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

    //Field Order:  SystemId, Id, ProjectSysId, ProjectId, OrgSysId, OrgId, Start, End, Name, Description
    internal sealed class FieldActivityBuilder : IBuilder<FieldActivity>
    {
        internal static readonly FieldActivityBuilder Instance = new FieldActivityBuilder();

        public FieldActivity Build(DbDataReader reader)
        {
            Guid sid = DbReaderUtils.GetGuid(reader, 0);
            Guid id = DbReaderUtils.GetGuid(reader, 1);
            Guid psid = DbReaderUtils.GetGuid(reader, 2);
            Guid pid = DbReaderUtils.GetGuid(reader, 3);
            Guid osid = DbReaderUtils.GetGuid(reader, 4);
            Guid oid = DbReaderUtils.GetGuid(reader, 5);
            DateTime start = Db.SafeGetDate(reader, 6, DateTime.MinValue);
            DateTime end = Db.SafeGetDate(reader, 7, DateTime.MaxValue);
            string name = DbReaderUtils.GetString(reader, 8);
            string desc = DbReaderUtils.GetString(reader, 9);

            if (start > DateTime.MinValue)
                return new FieldActivity(new CompoundIdentity(sid, id), name, new CompoundIdentity(psid, pid), new CompoundIdentity(osid, oid), new Numerics.ValueRange<DateTime>(start, end), desc);
            else
                return new FieldActivity(new CompoundIdentity(sid, id), name, new CompoundIdentity(psid, pid), new CompoundIdentity(osid, oid), null, desc);
        }
    }

    //Field Order:  SystemId, Id, Name, Description
    internal sealed class FieldTeamBuilder : IBuilder<FieldTeam>
    {
        internal static readonly FieldTeamBuilder Instance = new FieldTeamBuilder();

        public FieldTeam Build(DbDataReader reader)
        {
            Guid sid = DbReaderUtils.GetGuid(reader, 0);
            Guid id = DbReaderUtils.GetGuid(reader, 1);
            string name = DbReaderUtils.GetString(reader, 2);
            string desc = DbReaderUtils.GetString(reader, 3);

            FieldTeam ft = new FieldTeam(new CompoundIdentity(sid, id), name, desc);
            PgFieldTeamProvider.FillMembers(ft);
            return ft;
        }
    }

    //Field Order:  SystemId, Id, Name, Description
    internal sealed class FieldTeamMemberRoleBuilder : IBuilder<FieldTeamMemberRole>
    {
        internal static readonly FieldTeamMemberRoleBuilder Instance = new FieldTeamMemberRoleBuilder();

        public FieldTeamMemberRole Build(DbDataReader reader)
        {
            Guid sid = DbReaderUtils.GetGuid(reader, 0);
            Guid id = DbReaderUtils.GetGuid(reader, 1);
            string name = DbReaderUtils.GetString(reader, 2);
            string desc = DbReaderUtils.GetString(reader, 3);

            return new FieldTeamMemberRole(new CompoundIdentity(sid, id), name, desc);
        }
    }


    //Field Order:  \"SystemId\", \"Id\", \"FieldActivitySystemId\", \"FieldActivityId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\"
    internal sealed class FieldTripBuilder : IBuilder<FieldTrip>
    {
        internal static readonly FieldTripBuilder Instance = new FieldTripBuilder();

        public FieldTrip Build(DbDataReader reader)
        {
            Guid sid = DbReaderUtils.GetGuid(reader, 0);
            Guid id = DbReaderUtils.GetGuid(reader, 1);
            Guid fasid = DbReaderUtils.GetGuid(reader, 2);
            Guid fid = DbReaderUtils.GetGuid(reader, 3);
            Guid osid = DbReaderUtils.GetGuid(reader, 4);
            Guid oid = DbReaderUtils.GetGuid(reader, 5);
            string name = DbReaderUtils.GetString(reader, 6);
            string desc = DbReaderUtils.GetString(reader, 7);
            DateTime start = Db.SafeGetDate(reader, 8, DateTime.MinValue);
            DateTime end = Db.SafeGetDate(reader, 9, DateTime.MaxValue);

            if (start > DateTime.MinValue)
                return new FieldTrip(new CompoundIdentity(sid, id), name, new CompoundIdentity(fasid, fid), new CompoundIdentity(osid, oid), new Numerics.ValueRange<DateTime>(start, end), desc);
            else
                return new FieldTrip(new CompoundIdentity(sid, id), name, new CompoundIdentity(fasid, fid), new CompoundIdentity(osid, oid), null, desc);
        }
    }

    //Field Order:  \"SystemId\", \"Id\", \"FieldTripSystemId\", \"FieldTripId\", \"PrincipalOrgSystemId\", \"PrincipalOrgId\", \"Name\", \"Description\", \"StartDate\", \"EndDate\"
    internal sealed class SamplingEventBuilder : IBuilder<SamplingEvent>
    {
        internal static readonly SamplingEventBuilder Instance = new SamplingEventBuilder();

        public SamplingEvent Build(DbDataReader reader)
        {
            Guid sid = DbReaderUtils.GetGuid(reader, 0);
            Guid id = DbReaderUtils.GetGuid(reader, 1);
            Guid ftsid = DbReaderUtils.GetGuid(reader, 2);
            Guid ftid = DbReaderUtils.GetGuid(reader, 3);
            Guid osid = DbReaderUtils.GetGuid(reader, 4);
            Guid oid = DbReaderUtils.GetGuid(reader, 5);
            //Guid stsid = DbReaderUtils.GetGuid(reader, 6);
            //Guid stid = DbReaderUtils.GetGuid(reader, 7);
            string name = DbReaderUtils.GetString(reader, 6);
            string desc = DbReaderUtils.GetString(reader, 7);
            DateTime start = Db.SafeGetDate(reader, 8, DateTime.MinValue);
            DateTime end = Db.SafeGetDate(reader, 9, DateTime.MaxValue);

            return new SamplingEvent(new CompoundIdentity(sid, id), name, new CompoundIdentity(ftsid, ftid), new CompoundIdentity(osid, oid), new Numerics.ValueRange<DateTime>(start, end), desc);
        }
    }
}
