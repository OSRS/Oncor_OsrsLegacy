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

namespace Osrs.WellKnown.Projects.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;
        internal static readonly Guid DataStoreIdentity = new Guid("{9D4B5A7E-8A34-4E2C-9119-6D4DCECF0B6C}");

        internal const string CountProj = "SELECT COUNT(*) FROM oncor.\"Projects\"";
        internal const string CountInfo = "SELECT COUNT(*) FROM oncor.\"ProjectInfos\"";
        internal const string CountStat = "SELECT COUNT(*) FROM oncor.\"ProjectStatuses\"";
        internal const string CountStatTyp = "SELECT COUNT(*) FROM oncor.\"ProjectStatusTypes\"";

        internal const string SelectProj = "SELECT \"SystemId\", \"Id\", \"Name\", \"OrgSystemId\", \"OrgId\", \"ParentSystemId\", \"ParentId\", \"Description\" FROM oncor.\"Projects\"";
        internal const string SelectInfo = "SELECT \"ProjectSystemId\", \"ProjectId\", \"Id\", \"StartDate\", \"EndDate\", \"InfoText\" FROM oncor.\"ProjectInfos\"";
        internal const string SelectStat = "SELECT \"ProjectSystemId\", \"ProjectId\", \"Id\", \"StatusDate\", \"StatusTypeSystemId\", \"StatusTypeId\", \"Text\" FROM oncor.\"ProjectStatuses\"";
        internal const string SelectStatTyp = "SELECT \"SystemId\", \"Id\", \"Name\", \"Description\" FROM oncor.\"ProjectStatusTypes\"";
        internal const string SelectAffil = "SELECT \"OrgSystemId\", \"OrgId\" FROM oncor.\"ProjectAffiliates\"";

        internal const string SelectById = " WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string SelectByProjectId = " WHERE \"ProjectSystemId\"=:sid AND \"ProjectId\"=:id";
        internal const string SelectByPrinId = " WHERE \"OrgSystemId\"=:osid AND \"OrgId\"=:oid";
        internal const string SelectByParentId = " WHERE \"ParentSystemId\"=:psid AND \"ParentId\"=:pid";
        internal const string EntryId = " \"Id\"=:eid";

        internal const string InsertProj = "INSERT INTO oncor.\"Projects\"(\"SystemId\", \"Id\", \"Name\", \"OrgSystemId\", \"OrgId\", \"ParentSystemId\", \"ParentId\", \"Description\") VALUES (:sid, :id, :name, :osid, :oid, :psid, :pid, :desc)";
        internal const string InsertInfo = "INSERT INTO oncor.\"ProjectInfos\"(\"ProjectSystemId\", \"ProjectId\", \"Id\", \"StartDate\", \"EndDate\", \"InfoText\") VALUES (:sid, :id, :eid, :start, :end, :name)";
        internal const string InsertStat = "INSERT INTO oncor.\"ProjectStatuses\"(\"ProjectSystemId\", \"ProjectId\", \"Id\", \"StatusDate\", \"StatusTypeSystemId\", \"StatusTypeId\", \"Text\") VALUES (:sid, :id, :eid, :when, :stsid, :stid, :name)";
        internal const string InsertStatTyp = "INSERT INTO oncor.\"ProjectStatusTypes\"(\"SystemId\", \"Id\", \"Name\", \"Description\") VALUES (:sid, :id, :name, :desc)";
        internal const string InsertAffil = "INSERT INTO oncor.\"ProjectAffiliates\"(\"ProjectSystemId\", \"ProjectId\", \"OrgSystemId\", \"OrgId\") VALUES (:sid, :id, :osid, :oid)";

        internal const string UpdateProj = "UPDATE oncor.\"Projects\" SET \"Name\"=:name, \"OrgSystemId\"=:osid, \"OrgId\"=:oid, \"ParentSystemId\"=:psid, \"ParentId\"=:pid, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";
        internal const string UpdateInfo = "UPDATE oncor.\"ProjectInfos\" SET \"StartDate\"=:start, \"EndDate\"=:end, \"InfoText\"=:name WHERE \"ProjectSystemId\"=:sid AND \"ProjectId\"=:id AND \"Id\"=eid";
        internal const string UpdateStat = "UPDATE oncor.\"ProjectStatuses\" SET \"StatusDate\"=:when, \"StatusTypeSystemId\"=:stsid, \"StatusTypeId\"=:stid, \"Text\"=:name WHERE \"ProjectSystemId\"=:sid AND \"ProjectId\"=:id AND \"Id\"=:eid";
        internal const string UpdateStatTyp = "UPDATE oncor.\"ProjectStatusTypes\" SET \"Name\"=:name, \"Description\"=:desc WHERE \"SystemId\"=:sid AND \"Id\"=:id";

        internal const string DeleteProj = "DELETE FROM oncor.\"Projects\"";
        internal const string DeleteInfo = "DELETE FROM oncor.\"ProjectInfos\"";
        internal const string DeleteStat = "DELETE FROM oncor.\"ProjectStatuses\"";
        internal const string DeleteStatTyp = "DELETE FROM oncor.\"ProjectStatusTypes\"";
        internal const string DeleteAffil = "DELETE FROM oncor.\"ProjectAffiliates\"";

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

        internal static void LoadAffiliates(Project p)
        {
            if (p != null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAffil + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", p.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", p.Identity.Identity);

                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                ProjectAffiliates aff = p.Affiliates;
                while (rdr.Read())
                {
                    try
                    {
                        aff.Add(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)));
                    }
                    catch
                    { }
                }
                rdr.Dispose();
                cmd.Dispose();
            }
        }

        internal static void ReplaceAffiliates(Project p)
        {
            if (p!=null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.DeleteAffil + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", p.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", p.Identity.Identity);
                Db.ExecuteNonQuery(cmd);
                cmd.Dispose();

                ProjectAffiliates affils = p.Affiliates;
                if (affils.Count > 0)
                {
                    cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertAffil;

                    cmd.Connection.Open();

                    foreach(CompoundIdentity cur in affils)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("sid", p.Identity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("id", p.Identity.Identity);
                        cmd.Parameters.AddWithValue("osid", cur.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("oid", cur.Identity);

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
        }
    }

    //Field Order:  SystemId, Id, Name, PrincipalSystemId, PrincipalId, ParentSystemId, ParentId, Description
    internal sealed class ProjectBuilder : IBuilder<Project>
    {
        internal static readonly ProjectBuilder Instance = new ProjectBuilder();

        public Project Build(DbDataReader reader)
        {
            CompoundIdentity parentId = null;
            Guid parentSysId = DbReaderUtils.GetGuid(reader, 5);
            if (!Guid.Empty.Equals(parentSysId))
                parentId = new CompoundIdentity(parentSysId, DbReaderUtils.GetGuid(reader, 6));

            Project p = new Project(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), 
                DbReaderUtils.GetString(reader, 2), new CompoundIdentity(DbReaderUtils.GetGuid(reader, 3), DbReaderUtils.GetGuid(reader, 4)),
                parentId, DbReaderUtils.GetString(reader, 7));

            Db.LoadAffiliates(p);
            return p;
        }
    }

    //Field Order:  ProjectSystemId, ProjectId, Id, StartDate, EndDate, InfoText
    internal sealed class ProjectInfoBuilder : IBuilder<ProjectInformation>
    {
        internal static readonly ProjectInfoBuilder Instance = new ProjectInfoBuilder();

        public ProjectInformation Build(DbDataReader reader)
        {
            return new ProjectInformation(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), DbReaderUtils.GetGuid(reader, 2),
                DbReaderUtils.GetString(reader, 3), DbReaderUtils.GetNullableDateTime(reader, 4), DbReaderUtils.GetNullableDateTime(reader, 5));
        }
    }

    //Field Order:  ProjectSystemId, ProjectId, Id, StatusDate, StatusTypeSystemId, StatusTypeId, Text
    internal sealed class ProjecStatusBuilder : IBuilder<ProjectStatus>
    {
        internal static readonly ProjecStatusBuilder Instance = new ProjecStatusBuilder();

        public ProjectStatus Build(DbDataReader reader)
        {
            CompoundIdentity statTypeId = null;
            if (!DBNull.Value.Equals(reader[4]))
                statTypeId = new CompoundIdentity(DbReaderUtils.GetGuid(reader, 4), DbReaderUtils.GetGuid(reader, 5));

            return new ProjectStatus(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), DbReaderUtils.GetGuid(reader, 2),
                statTypeId, DbReaderUtils.GetString(reader, 6), DbReaderUtils.GetDate(reader, 3));
        }
    }

    //Field Order:  SystemId, Id, Name, Description
    internal sealed class ProjecStatusTypeBuilder : IBuilder<ProjectStatusType>
    {
        internal static readonly ProjecStatusTypeBuilder Instance = new ProjecStatusTypeBuilder();

        public ProjectStatusType Build(DbDataReader reader)
        {
            return new ProjectStatusType(new CompoundIdentity(DbReaderUtils.GetGuid(reader, 0), DbReaderUtils.GetGuid(reader, 1)), DbReaderUtils.GetString(reader, 2), DbReaderUtils.GetString(reader, 3));
        }
    }
}
