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
using System.Text;
using Osrs.Data;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;

namespace Osrs.WellKnown.Projects.Providers
{
    public sealed class PgProjectProvider : ProjectProviderBase
    {
        public override bool CanDelete(Project org)
        {
            return this.CanDelete();
        }

        public override bool CanUpdate(Project org)
        {
            return this.CanUpdate();
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool ExistsFor(CompoundIdentity principalOrgId)
        {
            if (principalOrgId != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + Db.SelectByPrinId;
                cmd.Parameters.AddWithValue("osid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", principalOrgId.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool ExistsFor(Project parentProject)
        {
            if (parentProject != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + Db.SelectByParentId;
                cmd.Parameters.AddWithValue("osid", parentProject.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", parentProject.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool InfoExists(Project project)
        {
            if (project != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", project.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", project.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool StatusExists(Project project)
        {
            if (project != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountProj + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", project.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", project.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<Project> Get()
        {
            if (this.CanGet())
                return new Enumerable<Project>(new EnumerableCommand<Project>(ProjectBuilder.Instance, Db.SelectProj, Db.ConnectionString));
            return null;
        }

        public override Project Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectProj + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                Project o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                            o = ProjectBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override IEnumerable<Project> Get(IEnumerable<CompoundIdentity> ids)
        {
            if (ids != null && this.CanGet())
            {
                Dictionary<Guid, HashSet<Guid>> systemIds = new Dictionary<Guid, HashSet<Guid>>();
                HashSet<Guid> oids;
                foreach (CompoundIdentity cur in ids)
                {
                    if (!cur.IsNullOrEmpty())
                    {
                        if (systemIds.ContainsKey(cur.DataStoreIdentity))
                            oids = systemIds[cur.DataStoreIdentity];
                        else
                        {
                            oids = new HashSet<Guid>();
                            systemIds[cur.DataStoreIdentity] = oids;
                        }
                        oids.Add(cur.Identity);
                    }
                }

                if (systemIds.Count > 0)
                {
                    StringBuilder where = new StringBuilder(" WHERE ");
                    foreach (KeyValuePair<Guid, HashSet<Guid>> cur in systemIds)
                    {
                        oids = cur.Value;
                        if (oids.Count > 0)
                        {
                            if (systemIds.Count > 1)
                                where.Append('(');
                            where.Append("\"SystemId\"='");
                            where.Append(cur.Key.ToString());
                            where.Append("' AND \"Id\" IN (");
                            foreach (Guid cid in oids)
                            {
                                where.Append('\'');
                                where.Append(cid.ToString());
                                where.Append("',");
                            }
                            where[where.Length - 1] = ')';
                            if (systemIds.Count > 1)
                                where.Append(") OR (");
                        }
                    }
                    if (where[where.Length - 1] == '(') //need to lop off the " OR ("
                        where.Length = where.Length - 5;

                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.SelectProj + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<Project> permissions = new List<Project>();
                    try
                    {
                        Project o;
                        while (rdr.Read())
                        {
                            o = ProjectBuilder.Instance.Build(rdr);
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
                else
                    return new Project[0]; //empty set
            }
            return null;
        }

        public override IEnumerable<Project> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectProj + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Project> permissions = new List<Project>();
                try
                {
                    Project o;
                    while (rdr.Read())
                    {
                        o = ProjectBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override IEnumerable<Project> GetFor(CompoundIdentity principalOrgId)
        {
            if (principalOrgId!=null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectProj + Db.SelectByPrinId;
                cmd.Parameters.AddWithValue("osid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", principalOrgId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Project> permissions = new List<Project>();
                try
                {
                    Project o;
                    while (rdr.Read())
                    {
                        o = ProjectBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override IEnumerable<Project> GetFor(Project parentProject)
        {
            if (parentProject!=null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectProj + Db.SelectByParentId;
                cmd.Parameters.AddWithValue("psid", parentProject.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("pid", parentProject.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Project> permissions = new List<Project>();
                try
                {
                    Project o;
                    while (rdr.Read())
                    {
                        o = ProjectBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override IEnumerable<ProjectInformation> GetInfo(Project project)
        {
            if (project!=null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectInfo + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", project.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", project.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<ProjectInformation> permissions = new List<ProjectInformation>();
                try
                {
                    ProjectInformation o;
                    while (rdr.Read())
                    {
                        o = ProjectInfoBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override IEnumerable<ProjectStatus> GetStatus(Project project)
        {
            if (project != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectStat + Db.SelectByProjectId;
                cmd.Parameters.AddWithValue("sid", project.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", project.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<ProjectStatus> permissions = new List<ProjectStatus>();
                try
                {
                    ProjectStatus o;
                    while (rdr.Read())
                    {
                        o = ProjecStatusBuilder.Instance.Build(rdr);
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
            return null;
        }

        public override bool AddInfo(ProjectInformation info)
        {
            if (info!=null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertInfo;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", info.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", info.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", info.Identity);
                    cmd.Parameters.AddWithValue("name", info.InformationText);
                    if (info.StartDate.HasValue)
                        cmd.Parameters.AddWithValue("start", info.StartDate);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("start", NpgsqlTypes.NpgsqlDbType.TimestampTZ));

                    if (info.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("end", info.EndDate);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("end", NpgsqlTypes.NpgsqlDbType.TimestampTZ));

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool AddStatus(ProjectStatus status)
        {
            if (status != null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertStat;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", status.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", status.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", status.Identity);
                    if (!string.IsNullOrEmpty(status.Text))
                        cmd.Parameters.AddWithValue("name", status.Text);
                    else
                        NpgSqlCommandUtils.GetNullInParam("name", NpgsqlTypes.NpgsqlDbType.Varchar);

                    cmd.Parameters.AddWithValue("when", status.StatusDate);

                    if (status.StatusTypeIdentity.IsNullOrEmpty())
                    {
                        NpgSqlCommandUtils.GetNullInParam("stsid", NpgsqlTypes.NpgsqlDbType.Uuid);
                        NpgSqlCommandUtils.GetNullInParam("stid", NpgsqlTypes.NpgsqlDbType.Uuid);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("stsid", status.StatusTypeIdentity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("stid", status.StatusTypeIdentity.Identity);
                    }

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Update(Project org)
        {
            if (org != null && this.CanUpdate(org))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateProj;
                    cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", org.Name);
                    cmd.Parameters.AddWithValue("osid", org.PrincipalOrganization.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", org.PrincipalOrganization.Identity);
                    if (org.ParentId.IsNullOrEmpty())
                    {
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("psid", NpgsqlTypes.NpgsqlDbType.Uuid));
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("psid", org.ParentId.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("pid", org.ParentId.Identity);
                    }
                    if (!string.IsNullOrEmpty(org.Description))
                        cmd.Parameters.AddWithValue("desc", org.Description);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));

                    Db.ExecuteNonQuery(cmd);

                    Db.ReplaceAffiliates(org);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool UpdateInfo(ProjectInformation info)
        {
            if (info != null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateInfo;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", info.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", info.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", info.Identity);
                    cmd.Parameters.AddWithValue("name", info.InformationText);
                    if (info.StartDate.HasValue)
                        cmd.Parameters.AddWithValue("start", info.StartDate);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("start", NpgsqlTypes.NpgsqlDbType.TimestampTZ));

                    if (info.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("end", info.EndDate);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("end", NpgsqlTypes.NpgsqlDbType.TimestampTZ));

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool UpdateStatus(ProjectStatus status)
        {
            if (status != null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateStat;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", status.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", status.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", status.Identity);
                    if (!string.IsNullOrEmpty(status.Text))
                        cmd.Parameters.AddWithValue("name", status.Text);
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("name", NpgsqlTypes.NpgsqlDbType.Varchar));

                    cmd.Parameters.AddWithValue("when", status.StatusDate);

                    if (status.StatusTypeIdentity.IsNullOrEmpty())
                    {
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("stsid", NpgsqlTypes.NpgsqlDbType.Uuid));
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("stid", NpgsqlTypes.NpgsqlDbType.Uuid));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("stsid", status.StatusTypeIdentity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("stid", status.StatusTypeIdentity.Identity);
                    }

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override Project Create(string name, CompoundIdentity principalOrgId, Project parentProject, string description)
        {
            if (!string.IsNullOrEmpty(name) && !principalOrgId.IsNullOrEmpty() && this.CanCreate())
            {
                try
                {
                    CompoundIdentity parentId = null; 
                    if (parentProject != null)
                        parentId = parentProject.Identity;

                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertProj;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("osid", principalOrgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", principalOrgId.Identity);
                    if (parentId.IsNullOrEmpty())
                    {
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("psid", NpgsqlTypes.NpgsqlDbType.Uuid));
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("psid", parentId.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("pid", parentId.Identity);
                    }

                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new Project(new CompoundIdentity(Db.DataStoreIdentity, id), name, principalOrgId, parentId, description);
                }
                catch
                { }
            }
            return null;
        }

        public override bool Delete(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteProj + Db.SelectById;
                    cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id.Identity);
                    Db.ExecuteNonQuery(cmd);

                    cmd.CommandText = Db.DeleteInfo + Db.SelectByProjectId;
                    Db.ExecuteNonQuery(cmd);

                    cmd.CommandText = Db.DeleteStat + Db.SelectByProjectId;
                    Db.ExecuteNonQuery(cmd);

                    cmd.CommandText = Db.DeleteAffil + Db.SelectByProjectId;
                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool DeleteInfo(ProjectInformation info)
        {
            if (info!=null && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteInfo + Db.SelectByProjectId + " AND" + Db.EntryId;
                    cmd.Parameters.AddWithValue("sid", info.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", info.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", info.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool DeleteStatus(ProjectStatus status)
        {
            if (status!=null && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteStat + Db.SelectByProjectId + " AND" + Db.EntryId;
                    cmd.Parameters.AddWithValue("sid", status.ProjectIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", status.ProjectIdentity.Identity);
                    cmd.Parameters.AddWithValue("eid", status.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgProjectProvider(UserSecurityContext context) : base(context)
        { }
    }
}
