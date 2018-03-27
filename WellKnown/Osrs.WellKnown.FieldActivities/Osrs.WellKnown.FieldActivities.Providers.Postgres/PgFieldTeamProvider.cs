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
using System.Data;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    public sealed class PgFieldTeamProvider : FieldTeamProviderBase
    {
        public override bool Add(FieldTeam team, SamplingEvent item)
        {
            if (team != null && item != null && this.CanCreate())
            {
                return this.AddImpl(team.Identity, item.Identity, FieldActivityUtils.SamplingEventWktId);
            }
            return false;
        }

        public override bool Add(FieldTeam team, FieldTrip item)
        {
            if (team != null && item != null && this.CanCreate())
            {
                return this.AddImpl(team.Identity, item.Identity, FieldActivityUtils.FieldTripWktId);
            }
            return false;
        }

        public override bool Add(FieldTeam team, FieldActivity item)
        {
            if (team != null && item != null && this.CanCreate())
            {
                return this.AddImpl(team.Identity, item.Identity, FieldActivityUtils.FieldActivityWktId);
            }
            return false;
        }

        private bool AddImpl(CompoundIdentity team, CompoundIdentity item, Guid wktid)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.InsertFieldTeamAssociations;

                cmd.Parameters.AddWithValue("fsid", team.DataStoreIdentity);
                cmd.Parameters.AddWithValue("fid", team.Identity);
                cmd.Parameters.AddWithValue("esid", item.DataStoreIdentity);
                cmd.Parameters.AddWithValue("eid", item.Identity);
                cmd.Parameters.AddWithValue("wtid", wktid);

                Db.ExecuteNonQuery(cmd);

                return true; //count check or ignore if already existed
            }
            catch
            { }
            return false;
        }

        public override bool CanDelete(FieldTeam item)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(FieldTeam item)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Contains(FieldTeam team, SamplingEvent item)
        {
            if (team != null && item != null && this.CanGet())
            {
                return ContainsImpl(team.Identity, item.Identity, FieldActivityUtils.SamplingEventWktId);
            }
            return false;
        }

        public override bool Contains(FieldTeam team, FieldTrip item)
        {
            if (team != null && item != null && this.CanGet())
            {
                return ContainsImpl(team.Identity, item.Identity, FieldActivityUtils.FieldTripWktId);
            }
            return false;
        }

        public override bool Contains(FieldTeam team, FieldActivity item)
        {
            if (team != null && item !=null && this.CanGet())
            {
                return ContainsImpl(team.Identity, item.Identity, FieldActivityUtils.FieldActivityWktId);
            }
            return false;
        }

        private bool ContainsImpl(CompoundIdentity team, CompoundIdentity item, Guid wktid)
        {
            if (team != null && item != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountFieldTeamByAssociation;
                cmd.Parameters.AddWithValue("fsid", team.DataStoreIdentity);
                cmd.Parameters.AddWithValue("fid", team.Identity);
                cmd.Parameters.AddWithValue("esid", item.DataStoreIdentity);
                cmd.Parameters.AddWithValue("eid", item.Identity);
                cmd.Parameters.AddWithValue("wtid", wktid);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override FieldTeam Create(string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertFieldTeam;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new FieldTeam(new CompoundIdentity(Db.DataStoreIdentity, id), name, description);
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
                    this.DeleteMembers(id); //delete all connected members

                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteFieldTeam;
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
                cmd.CommandText = Db.CountFieldTeams + Db.SelectById;
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
                cmd.CommandText = Db.CountFieldTeams + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<FieldTeam> Get()
        {
            if (this.CanGet())
                return new Enumerable<FieldTeam>(new EnumerableCommand<FieldTeam>(FieldTeamBuilder.Instance, Db.SelectFieldTeams, Db.ConnectionString));
            return null;
        }

        private IEnumerable<FieldTeam> Get(CompoundIdentity eid, Guid wktid)
        {
            //:esid, :eid, :wtid
            NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
            cmd.CommandText = Db.SelectFieldTeamByAssociation;
            cmd.Parameters.AddWithValue("esid", eid.DataStoreIdentity);
            cmd.Parameters.AddWithValue("eid", eid.Identity);
            cmd.Parameters.AddWithValue("wtid", wktid);
            NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
            if (rdr != null)
            {
                try
                {
                    List<FieldTeam> m = new List<FieldTeam>();
                    FieldTeam tmp = null;
                    while (rdr.Read())
                    {
                        tmp = FieldTeamBuilder.Instance.Build(rdr);
                        if (tmp != null)
                            m.Add(tmp);
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
            return null;
        }

        public override IEnumerable<FieldTeam> Get(FieldActivity item)
        {
            if (item != null && this.CanGet())
            {
                return Get(item.Identity, FieldActivityUtils.FieldActivityWktId);
            }
            return null;
        }

        public override IEnumerable<FieldTeam> Get(FieldTrip item)
        {
            if (item != null && this.CanGet())
            {
                return Get(item.Identity, FieldActivityUtils.FieldTripWktId);
            }
            return null;
        }

        public override IEnumerable<FieldTeam> Get(SamplingEvent item)
        {
            if (item != null && this.CanGet())
            {
                return Get(item.Identity, FieldActivityUtils.SamplingEventWktId);
            }
            return null;
        }

        public override IEnumerable<FieldTeam> Get(IEnumerable<CompoundIdentity> ids)
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
                    cmd.CommandText = Db.SelectFieldTeams + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<FieldTeam> permissions = new List<FieldTeam>();
                    try
                    {
                        FieldTeam o;
                        while (rdr.Read())
                        {
                            o = FieldTeamBuilder.Instance.Build(rdr);
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
                    return new FieldTeam[0]; //empty set
            }
            return null;
        }

        public override FieldTeam Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectFieldTeams + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                FieldTeam o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = FieldTeamBuilder.Instance.Build(rdr);
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

        public override IEnumerable<FieldTeam> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectFieldTeams + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<FieldTeam> permissions = new List<FieldTeam>();
                try
                {
                    FieldTeam o;
                    while (rdr.Read())
                    {
                        o = FieldTeamBuilder.Instance.Build(rdr);
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

        private bool Remove(CompoundIdentity ftid, CompoundIdentity eid, Guid wktid)
        {
            if (!ftid.IsNullOrEmpty() && !eid.IsNullOrEmpty())
            {
                //:fsid, :fid, :esid, :eid, :wtid
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteFieldTeamAssociations + Db.WhereFtaWkt + "AND " + Db.WhereFtaFT + "AND " + Db.WhereFtaPer;
                    cmd.Parameters.AddWithValue("fsid", ftid.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("fid", ftid.Identity);
                    cmd.Parameters.AddWithValue("esid", eid.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("eid", eid.Identity);
                    cmd.Parameters.AddWithValue("wtid", wktid);

                    Db.ExecuteNonQuery(cmd);
                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Remove(FieldTeam team, SamplingEvent item)
        {
            if (team!=null && item != null)
            {
                return Remove(team.Identity, item.Identity, FieldActivityUtils.SamplingEventWktId);
            }
            return false;
        }

        public override bool Remove(FieldTeam team, FieldTrip item)
        {
            if (team != null && item != null)
            {
                return Remove(team.Identity, item.Identity, FieldActivityUtils.FieldTripWktId);
            }
            return false;
        }

        public override bool Remove(FieldTeam team, FieldActivity item)
        {
            if (team != null && item != null)
            {
                return Remove(team.Identity, item.Identity, FieldActivityUtils.FieldActivityWktId);
            }
            return false;
        }

        public override bool Update(FieldTeam item)
        {
            if (item != null && this.CanUpdate(item))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateFieldTeam;
                    cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", item.Name);
                    cmd.Parameters.AddWithValue("desc", item.Description);

                    Db.ExecuteNonQuery(cmd);

                    return this.UpdateMembers(item);
                }
                catch
                { }
            }
            return false;
        }

        internal static void FillMembers(FieldTeam item)
        {
            //\"FieldTeamSystemId\", \"FieldTeamId\", \"PersonSystemId\", \"PersonId\", \"FieldTeamMemberRoleSystemId\", \"FieldTeamMemberRoleId\"
            if (item!=null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectFieldTeamMembers;
                cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                if (rdr != null)
                {
                    try
                    {
                        FieldTeamMembers m = item.Members;
                        while (rdr.Read())
                        {
                            m.Add(new FieldTeamMember(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 2), DbReaderUtils.GetGuid(rdr, 3)), new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 4), DbReaderUtils.GetGuid(rdr, 5))));
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
        }

        private bool DeleteMembers(CompoundIdentity id)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.DeleteFieldTeamMembers;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);

                Db.ExecuteNonQuery(cmd);
                return true;
            }
            catch
            { }

            return false;
        }

        private bool UpdateMembers(FieldTeam item)
        {
            bool clear = this.DeleteMembers(item.Identity);
            if (clear && item.Members.Count>0)
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertFieldTeamMembers;

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    foreach (FieldTeamMember cur in item.Members)
                    {
                        if (cur!=null && !cur.FieldTeamMemberRoleId.IsNullOrEmpty() && !cur.PersonId.IsNullOrEmpty())
                        try
                        {
                            //:sid, :id, :psid, :pid, :rsid, :rid)
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                            cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                            cmd.Parameters.AddWithValue("psid", cur.PersonId.DataStoreIdentity);
                            cmd.Parameters.AddWithValue("pid", cur.PersonId.Identity);
                            cmd.Parameters.AddWithValue("rsid", cur.FieldTeamMemberRoleId.DataStoreIdentity);
                            cmd.Parameters.AddWithValue("rid", cur.FieldTeamMemberRoleId.Identity);

                            cmd.ExecuteNonQuery();
                        }
                        catch
                        { }

                    }

                    try
                    {
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
                    }
                    catch
                    { }

                    return true;
                }
                catch
                { }
            }
            return clear;
        }

        internal PgFieldTeamProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
