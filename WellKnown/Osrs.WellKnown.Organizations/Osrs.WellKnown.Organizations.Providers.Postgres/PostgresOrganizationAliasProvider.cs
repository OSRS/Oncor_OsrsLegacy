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
    public sealed class PostgresOrganizationAliasProvider : OrganizationAliasProviderBase
    {
        public override bool CanDelete(OrganizationAlias scheme)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(OrganizationAlias alias)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + Db.SelectOrgById;
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
                cmd.CommandText = Db.CountAlias + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(CompoundIdentity id, OrganizationAliasScheme scheme)
        {
            if (!id.IsNullOrEmpty() && scheme!=null && !(scheme.Identity.IsNullOrEmpty()) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + Db.SelectAliasById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption)
        {
            if (scheme != null && !(scheme.Identity.IsNullOrEmpty()) && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasByScheme;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + where;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(Organization org, string name, StringComparison comparisonOption)
        {
            if (org != null && !(org.Identity.IsNullOrEmpty()) && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectOrgById;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + where;
                cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<OrganizationAlias> Get()
        {
            if (this.CanGet())
                return new Enumerable<OrganizationAlias>(new EnumerableCommand<OrganizationAlias>(OrganizationAliasBuilder.Instance, Db.SelectAlias, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<OrganizationAlias> Get(Organization org)
        {
            if (org != null && !org.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectOrgById;
                cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme)
        {
            if (scheme != null && !scheme.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectAliasByScheme;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAlias> Get(CompoundIdentity id, OrganizationAliasScheme scheme)
        {
            if (!id.IsNullOrEmpty() && scheme != null && !scheme.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectAliasById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAlias> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = " WHERE ";
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += "lower(\"Name\")=lower(:name)";
                else
                    where += "\"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAlias> Get(Organization org, string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectOrgById;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where;
                cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<OrganizationAlias> Get(OrganizationAliasScheme scheme, string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasByScheme;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<OrganizationAlias> schemes = new List<OrganizationAlias>();
                OrganizationAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = OrganizationAliasBuilder.Instance.Build(rdr);
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

        public override bool Update(OrganizationAlias org)
        {
            if (org != null && !org.Identity.IsNullOrEmpty() && !org.AliasSchemeIdentity.IsNullOrEmpty() && CanUpdate(org))
            {
                if (OrganizationUtils.IsDirty(org))
                {
                    try
                    {
                        NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                        cmd.CommandText = Db.UpdateAlias;
                        cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                        cmd.Parameters.AddWithValue("ssid", org.AliasSchemeIdentity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("scid", org.AliasSchemeIdentity.Identity);
                        cmd.Parameters.AddWithValue("name", org.Name);
                        cmd.Parameters.AddWithValue("origName", OrganizationUtils.OriginalName(org));

                        Db.ExecuteNonQuery(cmd);

                        return true;
                    }
                    catch
                    { }
                }
                else
                    return true; //nothing to change
            }
            return false;
        }

        public override OrganizationAlias Create(OrganizationAliasScheme scheme, Organization org, string name)
        {
            if (scheme != null && !scheme.Identity.IsNullOrEmpty() && org != null && !org.Identity.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertAlias;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", org.Identity.Identity);
                    cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", name);

                    Db.ExecuteNonQuery(cmd);

                    return new OrganizationAlias(org.Identity, scheme.Identity, name);
                }
                catch
                { }
            }
            return null;
        }

        public override bool Delete(OrganizationAliasScheme scheme)
        {
            if (scheme!=null && !scheme.Identity.IsNullOrEmpty() && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
                    cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Delete(CompoundIdentity orgId)
        {
            if (!orgId.IsNullOrEmpty() && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id";
                    cmd.Parameters.AddWithValue("sid", orgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", orgId.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Delete(CompoundIdentity id, CompoundIdentity schemeId)
        {
            if (!id.IsNullOrEmpty() && !schemeId.IsNullOrEmpty() && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid";
                    cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id.Identity);
                    cmd.Parameters.AddWithValue("ssid", schemeId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", schemeId.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Delete(OrganizationAlias alias)
        {
            if (alias!=null && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid AND \"Name\"=:n";
                    cmd.Parameters.AddWithValue("sid", alias.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", alias.Identity.Identity);
                    cmd.Parameters.AddWithValue("ssid", alias.AliasSchemeIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", alias.AliasSchemeIdentity.Identity);
                    cmd.Parameters.AddWithValue("n", alias.Name);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PostgresOrganizationAliasProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}