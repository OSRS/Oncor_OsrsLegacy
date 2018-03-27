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
using Osrs.Data;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;
using System.Text;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    public sealed class PgFieldTeamMemberRoleProvider : FieldTeamMemberRoleProviderBase
    {
        public override bool CanDelete(FieldTeamMemberRole item)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(FieldTeamMemberRole item)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override FieldTeamMemberRole Create(string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertFTMRole;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new FieldTeamMemberRole(new CompoundIdentity(Db.DataStoreIdentity, id), name, description);
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
                    cmd.CommandText = Db.DeleteFTMRole;
                    cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountFTMRoles + Db.SelectById;
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
                cmd.CommandText = Db.CountFTMRoles + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<FieldTeamMemberRole> Get()
        {
            if (this.CanGet())
                return new Enumerable<FieldTeamMemberRole>(new EnumerableCommand<FieldTeamMemberRole>(FieldTeamMemberRoleBuilder.Instance, Db.SelectFTMRoles, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<FieldTeamMemberRole> Get(string name)
        {
            return this.Get(name, StringComparison.OrdinalIgnoreCase);
        }

        public override FieldTeamMemberRole Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectFTMRoles + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                FieldTeamMemberRole o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = FieldTeamMemberRoleBuilder.Instance.Build(rdr);
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

        public override IEnumerable<FieldTeamMemberRole> Get(IEnumerable<CompoundIdentity> ids)
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
                    cmd.CommandText = Db.SelectFTMRoles + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<FieldTeamMemberRole> permissions = new List<FieldTeamMemberRole>();
                    try
                    {
                        FieldTeamMemberRole o;
                        while (rdr.Read())
                        {
                            o = FieldTeamMemberRoleBuilder.Instance.Build(rdr);
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
                    return new FieldTeamMemberRole[0]; //empty set
            }
            return null;
        }

        public override IEnumerable<FieldTeamMemberRole> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectFTMRoles + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<FieldTeamMemberRole> permissions = new List<FieldTeamMemberRole>();
                try
                {
                    FieldTeamMemberRole o;
                    while (rdr.Read())
                    {
                        o = FieldTeamMemberRoleBuilder.Instance.Build(rdr);
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

        public override bool Update(FieldTeamMemberRole item)
        {
            if (item != null && this.CanUpdate(item))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateFTMRole;
                    cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", item.Name);
                    cmd.Parameters.AddWithValue("desc", item.Description);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgFieldTeamMemberRoleProvider(UserSecurityContext context) : base(context)
        { }
    }
}
