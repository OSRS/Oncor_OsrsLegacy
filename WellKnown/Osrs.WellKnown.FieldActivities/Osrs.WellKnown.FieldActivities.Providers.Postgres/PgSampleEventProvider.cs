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
using Osrs.Numerics;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;
using System.Text;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    public sealed class PgSampleEventProvider : SampleEventProviderBase
    {
        public override bool CanDelete(SamplingEvent item)
        {
            return this.CanDelete();
        }

        public override bool CanUpdate(SamplingEvent item)
        {
            return this.CanUpdate();
        }

        public override SamplingEvent Create(string name, CompoundIdentity tripId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description)
        {
            if (!tripId.IsNullOrEmpty() && !principalOrgId.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertEvent;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.Parameters.AddWithValue("ftsid", tripId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("ftid", tripId.Identity);

                    cmd.Parameters.AddWithValue("osid", principalOrgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", principalOrgId.Identity);

                    if (dateRange != null)
                    {
                        dateRange = Db.CleanRange(dateRange);
                        cmd.Parameters.AddWithValue("start", dateRange.Min);
                        cmd.Parameters.AddWithValue("end", dateRange.Max);
                    }
                    else
                    {
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("start", NpgsqlTypes.NpgsqlDbType.TimestampTZ));
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("end", NpgsqlTypes.NpgsqlDbType.TimestampTZ));
                    }

                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new SamplingEvent(new CompoundIdentity(Db.DataStoreIdentity, id), name, tripId, principalOrgId, dateRange, description);
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
                    cmd.CommandText = Db.DeleteEvent;
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
                cmd.CommandText = Db.CountEvent + Db.SelectById;
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
                cmd.CommandText = Db.CountEvent + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool ExistsForOrg(CompoundIdentity principalOrgId)
        {
            if (!principalOrgId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"PrincipalOrgSystemId\"=:sid AND \"PrincipalOrgId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountEvent + where;
                cmd.Parameters.AddWithValue("sid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", principalOrgId.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool ExistsForTrip(CompoundIdentity tripId)
        {
            if (!tripId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"FieldTripSystemId\"=:sid AND \"FieldTripId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountEvent + where;
                cmd.Parameters.AddWithValue("sid", tripId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", tripId.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<SamplingEvent> Get()
        {
            if (this.CanGet())
                return new Enumerable<SamplingEvent>(new EnumerableCommand<SamplingEvent>(SamplingEventBuilder.Instance, Db.SelectEvent, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<SamplingEvent> Get(IEnumerable<CompoundIdentity> ids)
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
                    cmd.CommandText = Db.SelectEvent + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<SamplingEvent> permissions = new List<SamplingEvent>();
                    try
                    {
                        SamplingEvent o;
                        while (rdr.Read())
                        {
                            o = SamplingEventBuilder.Instance.Build(rdr);
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
                    return new SamplingEvent[0]; //empty set
            }
            return null;
        }

        public override SamplingEvent Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectEvent + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                SamplingEvent o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = SamplingEventBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SamplingEvent> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectEvent + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SamplingEvent> permissions = new List<SamplingEvent>();
                try
                {
                    SamplingEvent o;
                    while (rdr.Read())
                    {
                        o = SamplingEventBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SamplingEvent> GetForOrg(CompoundIdentity principalOrgId)
        {
            if (!principalOrgId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"PrincipalOrgSystemId\"=:sid AND \"PrincipalOrgId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectEvent + where;
                cmd.Parameters.AddWithValue("sid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", principalOrgId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                SamplingEvent o = null;
                List<SamplingEvent> permissions = new List<SamplingEvent>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SamplingEventBuilder.Instance.Build(rdr);
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
                }
                return permissions;
            }
            return null;
        }

        public override IEnumerable<SamplingEvent> GetForTrip(IEnumerable<FieldTrip> trips)
        {
            if (trips != null)
            {
                HashSet<CompoundIdentity> tmp = new HashSet<CompoundIdentity>();
                foreach(FieldTrip cur in trips)
                {
                    if (cur != null)
                        tmp.Add(cur.Identity);
                }
                return this.GetForTrip(tmp);
            }
            return null;
        }

        public override IEnumerable<SamplingEvent> GetForTrip(IEnumerable<CompoundIdentity> tripIds)
        {
            if (tripIds != null && this.CanGet())
            {
                Dictionary<Guid, HashSet<Guid>> systemIds = new Dictionary<Guid, HashSet<Guid>>();
                HashSet<Guid> oids;
                foreach (CompoundIdentity cur in tripIds)
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
                            where.Append("\"FieldTripSystemId\"='");
                            where.Append(cur.Key.ToString());
                            where.Append("' AND \"FieldTripId\" IN (");
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
                    cmd.CommandText = Db.SelectEvent + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<SamplingEvent> permissions = new List<SamplingEvent>();
                    try
                    {
                        SamplingEvent o;
                        while (rdr.Read())
                        {
                            o = SamplingEventBuilder.Instance.Build(rdr);
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
                    return new SamplingEvent[0]; //empty set
            }
            return null;
        }

        public override IEnumerable<SamplingEvent> GetForTrip(CompoundIdentity tripId)
        {
            if (!tripId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"FieldTripSystemId\"=:sid AND \"FieldTripId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectEvent + where;
                cmd.Parameters.AddWithValue("sid", tripId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", tripId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                SamplingEvent o = null;
                List<SamplingEvent> permissions = new List<SamplingEvent>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SamplingEventBuilder.Instance.Build(rdr);
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
                }
                return permissions;
            }
            return null;
        }

        public override bool Update(SamplingEvent item)
        {
            if (item != null && this.CanUpdate(item))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateEvent;
                    cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                    cmd.Parameters.AddWithValue("ftsid", item.FieldTripId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("ftid", item.FieldTripId.Identity);
                    cmd.Parameters.AddWithValue("osid", item.PrincipalOrgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", item.PrincipalOrgId.Identity);
                    cmd.Parameters.AddWithValue("name", item.Name);
                    if (string.IsNullOrEmpty(item.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", item.Description);
                    if (item.DateRange != null)
                    {
                        item.DateRange = Db.CleanRange(item.DateRange);
                        cmd.Parameters.AddWithValue("start", item.DateRange.Min);
                        cmd.Parameters.AddWithValue("end", item.DateRange.Max);
                    }
                    else
                    {
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("start", NpgsqlTypes.NpgsqlDbType.TimestampTZ));
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("end", NpgsqlTypes.NpgsqlDbType.TimestampTZ));
                    }

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgSampleEventProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
