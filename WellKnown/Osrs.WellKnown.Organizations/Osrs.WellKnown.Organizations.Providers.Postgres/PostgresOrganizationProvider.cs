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

namespace Osrs.WellKnown.Organizations.Providers
{
    public sealed class PostgresOrganizationProvider : OrganizationProviderBase
    {
        public override bool CanDelete(Organization org)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(Organization org)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountOrg + Db.SelectOrgById;
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
                cmd.CommandText = Db.CountOrg + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<Organization> Get()
        {
            if (this.CanGet())
                return new Enumerable<Organization>(new EnumerableCommand<Organization>(OrganizationBuilder.Instance, Db.SelectOrg, Db.ConnectionString));
            return null;
        }

        public override Organization Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectOrg + Db.SelectOrgById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                Organization o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = OrganizationBuilder.Instance.Build(rdr);
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

        public override IEnumerable<Organization> Get(IEnumerable<CompoundIdentity> ids)
        {
            if (ids!=null && this.CanGet())
            {
                Dictionary<Guid, HashSet<Guid>> systemIds = new Dictionary<Guid, HashSet<Guid>>();
                HashSet<Guid> oids;
                foreach(CompoundIdentity cur in ids)
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
                    cmd.CommandText = Db.SelectOrg + where.ToString();
                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    List<Organization> permissions = new List<Organization>();
                    try
                    {
                        Organization o;
                        while (rdr.Read())
                        {
                            o = OrganizationBuilder.Instance.Build(rdr);
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
                    return new Organization[0]; //empty set
            }
            return null;
        }

        public override IEnumerable<Organization> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectOrg + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Organization> permissions = new List<Organization>();
                try
                {
                    Organization o;
                    while (rdr.Read())
                    {
                        o = OrganizationBuilder.Instance.Build(rdr);
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

        public override bool Update(Organization org)
        {
            if (org != null && this.CanUpdate(org))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateOrg;
                    cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", org.Name);
                    cmd.Parameters.AddWithValue("desc", org.Description);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override Organization Create(string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertOrg;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new Organization(new CompoundIdentity(Db.DataStoreIdentity, id), name, description);
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
                    cmd.CommandText = Db.DeleteOrg;
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

        internal PostgresOrganizationProvider(UserSecurityContext context) : base(context)
        { }
    }
}