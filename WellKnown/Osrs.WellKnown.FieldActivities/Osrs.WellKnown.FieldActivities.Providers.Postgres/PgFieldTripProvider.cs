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

using Osrs.Security;
using System;
using System.Collections.Generic;
using Osrs.Data;
using Osrs.Numerics;
using Npgsql;
using Osrs.Data.Postgres;
using System.Text;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    public sealed class PgFieldTripProvider : FieldTripProviderBase
    {
        public override bool CanDelete(FieldTrip item)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(FieldTrip item)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override FieldTrip Create(string name, CompoundIdentity activityId, CompoundIdentity principalOrgId, ValueRange<DateTime> dateRange, string description)
        {
            if (!activityId.IsNullOrEmpty() && !principalOrgId.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertTrip;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);

                    cmd.Parameters.AddWithValue("fasid", activityId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("faid", activityId.Identity);

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

                    return new FieldTrip(new CompoundIdentity(Db.DataStoreIdentity, id), name, activityId, principalOrgId, dateRange, description);
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
                    cmd.CommandText = Db.DeleteTrip;
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

        public override bool Delete(FieldTrip item)
        {
            if (item != null && this.CanDelete(item))
            {
                return this.Delete(item.Identity);
            }
            return false;
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountTrips + Db.SelectById;
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
                cmd.CommandText = Db.CountTrips + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool ExistsForActivity(FieldActivity activity)
        {
            if (activity != null)
                return this.ExistsForActivity(activity.Identity);
            return false;
        }

        public override bool ExistsForActivity(CompoundIdentity activityId)
        {
            if (!activityId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"FieldActivitySystemId\"=:sid AND \"FieldActivityId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountTrips + where;
                cmd.Parameters.AddWithValue("sid", activityId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", activityId.Identity);
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
                cmd.CommandText = Db.CountTrips + where;
                cmd.Parameters.AddWithValue("sid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", principalOrgId.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<FieldTrip> Get()
        {
            if (this.CanGet())
                return new Enumerable<FieldTrip>(new EnumerableCommand<FieldTrip>(FieldTripBuilder.Instance, Db.SelectTrips, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<FieldTrip> Get(IEnumerable<CompoundIdentity> ids)
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
                    cmd.CommandText = Db.SelectTrips + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<FieldTrip> permissions = new List<FieldTrip>();
                    try
                    {
                        FieldTrip o;
                        while (rdr.Read())
                        {
                            o = FieldTripBuilder.Instance.Build(rdr);
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
                    return new FieldTrip[0]; //empty set
            }
            return null;
        }

        public override FieldTrip Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectTrips + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                FieldTrip o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = FieldTripBuilder.Instance.Build(rdr);
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

        public override IEnumerable<FieldTrip> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectTrips + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<FieldTrip> permissions = new List<FieldTrip>();
                try
                {
                    FieldTrip o;
                    while (rdr.Read())
                    {
                        o = FieldTripBuilder.Instance.Build(rdr);
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

        public override IEnumerable<FieldTrip> GetForActivity(FieldActivity activity)
        {
            if (activity != null)
                return this.GetForActivity(activity.Identity);
            return null;
        }

        public override IEnumerable<FieldTrip> GetForActivity(CompoundIdentity activityId)
        {
            if (!activityId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"FieldActivitySystemId\"=:sid AND \"FieldActivityId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectTrips + where;
                cmd.Parameters.AddWithValue("sid", activityId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", activityId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                FieldTrip o = null;
                List<FieldTrip> permissions = new List<FieldTrip>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = FieldTripBuilder.Instance.Build(rdr);
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

        public override IEnumerable<FieldTrip> GetForOrg(CompoundIdentity principalOrgId)
        {
            if (!principalOrgId.IsNullOrEmpty() && this.CanGet())
            {
                string where = " WHERE \"PrincipalOrgSystemId\"=:sid AND \"PrincipalOrgId\"=:id";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectTrips + where;
                cmd.Parameters.AddWithValue("sid", principalOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", principalOrgId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                FieldTrip o = null;
                List<FieldTrip> permissions = new List<FieldTrip>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = FieldTripBuilder.Instance.Build(rdr);
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

        public override bool Update(FieldTrip item)
        {
            if (item != null && this.CanUpdate(item))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateTrip;
                    cmd.Parameters.AddWithValue("sid", item.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", item.Identity.Identity);
                    cmd.Parameters.AddWithValue("fasid", item.FieldActivityId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("faid", item.FieldActivityId.Identity);
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

        internal PgFieldTripProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
