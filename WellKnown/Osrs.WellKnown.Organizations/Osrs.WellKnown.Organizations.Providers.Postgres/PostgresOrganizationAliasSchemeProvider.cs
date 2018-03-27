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

namespace Osrs.WellKnown.Organizations.Providers
{
    public sealed class PostgresOrganizationAliasSchemeProvider : OrganizationAliasSchemeProviderBase
    {
        public override bool CanDelete(OrganizationAliasScheme org)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(OrganizationAliasScheme org)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAliasScheme + Db.SelectAliasSchemeById;
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
                cmd.CommandText = Db.CountAliasScheme + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(Organization owner, string name, StringComparison comparisonOption)
        {
            if (owner!=null && !owner.Identity.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasSchemeByOwnerId;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAliasScheme + where;
                cmd.Parameters.AddWithValue("sid", owner.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owner.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<OrganizationAliasScheme> Get()
        {
            if (this.CanGet())
                return new Enumerable<OrganizationAliasScheme>(new EnumerableCommand<OrganizationAliasScheme>(OrganizationAliasSchemeBuilder.Instance, Db.SelectAliasScheme, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<OrganizationAliasScheme> Get(Organization owner)
        {
            if (owner != null && !owner.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + Db.SelectAliasSchemeByOwnerId;
                cmd.Parameters.AddWithValue("sid", owner.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owner.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAliasScheme> schemes = new List<OrganizationAliasScheme>();
                OrganizationAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasSchemeBuilder.Instance.Build(rdr);
                            if (o != null)
                                schemes.Add(o);
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
                return schemes;
            }
            return null;
        }

        public override OrganizationAliasScheme Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + Db.SelectAliasSchemeById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                OrganizationAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = OrganizationAliasSchemeBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAliasScheme> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = " WHERE ";
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += "lower(\"Name\")=lower(:name)";
                else
                    where += "\"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAliasScheme> schemes = new List<OrganizationAliasScheme>();
                OrganizationAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasSchemeBuilder.Instance.Build(rdr);
                            if (o != null)
                                schemes.Add(o);
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
                return schemes;
            }
            return null;
        }

        public override IEnumerable<OrganizationAliasScheme> Get(Organization owner, string name, StringComparison comparisonOption)
        {
            if (owner != null && !owner.Identity.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasSchemeByOwnerId;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + where;
                cmd.Parameters.AddWithValue("sid", owner.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owner.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAliasScheme> schemes = new List<OrganizationAliasScheme>();
                OrganizationAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasSchemeBuilder.Instance.Build(rdr);
                            if (o != null)
                                schemes.Add(o);
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
                return schemes;
            }
            return null;
        }

        public override bool Update(OrganizationAliasScheme org)
        {
            if (org!=null && !org.Identity.IsNullOrEmpty() && CanUpdate(org))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateAliasScheme;
                    cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                    cmd.Parameters.AddWithValue("osid", org.OwningOrganizationIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", org.OwningOrganizationIdentity.Identity);
                    cmd.Parameters.AddWithValue("name", org.Name);
                    if (string.IsNullOrEmpty(org.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", org.Description);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override OrganizationAliasScheme Create(Organization owner, string name, string description)
        {
            if (owner!=null && !owner.Identity.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertAliasScheme;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("osid", owner.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", owner.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new OrganizationAliasScheme(new CompoundIdentity(Db.DataStoreIdentity, id), owner.Identity, name, description);
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
                    cmd.CommandText = Db.DeleteAliasScheme;
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

        internal PostgresOrganizationAliasSchemeProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}