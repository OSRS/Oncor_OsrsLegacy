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

namespace Osrs.WellKnown.Sites.Providers
{
    //TODO -- update all for temporal
    public sealed class PgSiteAliasProvider : SiteAliasProviderBase
    {
        public override bool CanDelete(SiteAlias scheme)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(SiteAlias alias)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + Db.SelectSiteById + " AND" + Db.WhereCurrent;
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
                cmd.CommandText = Db.CountAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(CompoundIdentity id, SiteAliasScheme scheme)
        {
            if (!id.IsNullOrEmpty() && scheme != null && !(scheme.Identity.IsNullOrEmpty()) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + Db.SelectAliasById + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(SiteAliasScheme scheme, string name, StringComparison comparisonOption)
        {
            if (scheme != null && !(scheme.Identity.IsNullOrEmpty()) && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasByScheme;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(Site site, string name, StringComparison comparisonOption)
        {
            if (site != null && !(site.Identity.IsNullOrEmpty()) && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectSiteById;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<SiteAlias> Get()
        {
            if (this.CanGet())
                return new Enumerable<SiteAlias>(new EnumerableCommand<SiteAlias>(SiteAliasBuilder.Instance, Db.SelectAlias + " WHERE " + Db.WhereCurrent, Db.ConnectionString));
            return null;
        }

        public override IEnumerable<SiteAlias> Get(Site site)
        {
            if (site != null && !site.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectSiteById + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAlias> Get(SiteAliasScheme scheme)
        {
            if (scheme != null && !scheme.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectAliasByScheme + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAlias> Get(CompoundIdentity id, SiteAliasScheme scheme)
        {
            if (!id.IsNullOrEmpty() && scheme != null && !scheme.Identity.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + Db.SelectAliasById + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAlias> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = " WHERE ";
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += "lower(\"Name\")=lower(:name)";
                else
                    where += "\"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAlias> Get(Site site, string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectSiteById;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAlias> Get(SiteAliasScheme scheme, string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasByScheme;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAlias + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAlias> schemes = new List<SiteAlias>();
                SiteAlias o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasBuilder.Instance.Build(rdr);
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

        public override SiteAlias Create(SiteAliasScheme scheme, Site org, string name)
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
                    cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return new SiteAlias(org.Identity, scheme.Identity, name);
                }
                catch
                { }
            }
            return null;
        }

        public override bool Delete(SiteAlias alias)
        {
            if (alias!=null && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid AND \"Name\"=:n AND" + Db.WhereCurrent;
                    cmd.Parameters.AddWithValue("sid", alias.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", alias.Identity.Identity);
                    cmd.Parameters.AddWithValue("ssid", alias.AliasSchemeIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", alias.AliasSchemeIdentity.Identity);
                    cmd.Parameters.AddWithValue("n", alias.Name);
                    cmd.Parameters.AddWithValue("end", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Delete(SiteAliasScheme scheme)
        {
            if (scheme != null && !scheme.Identity.IsNullOrEmpty() && this.CanDelete())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.DeleteAlias + "\"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid AND" + Db.WhereCurrent;
                    cmd.Parameters.AddWithValue("ssid", scheme.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", scheme.Identity.Identity);
                    cmd.Parameters.AddWithValue("end", DateTime.UtcNow);

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
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id AND" + Db.WhereCurrent;
                    cmd.Parameters.AddWithValue("sid", orgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", orgId.Identity);
                    cmd.Parameters.AddWithValue("end", DateTime.UtcNow);

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
                    cmd.CommandText = Db.DeleteAlias + "\"SystemId\"=:sid AND \"Id\"=:id AND \"SchemeSystemId\"=:ssid AND \"SchemeId\"=:scid AND" + Db.WhereCurrent;
                    cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id.Identity);
                    cmd.Parameters.AddWithValue("ssid", schemeId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("scid", schemeId.Identity);
                    cmd.Parameters.AddWithValue("end", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Update(SiteAlias site)
        {
            if (site != null && !site.Identity.IsNullOrEmpty() && !site.AliasSchemeIdentity.IsNullOrEmpty() && CanUpdate(site))
            {
                if (SiteUtils.IsDirty(site))
                {
                    try
                    {
                        NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                        cmd.CommandText = Db.UpdateAlias;
                        cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                        cmd.Parameters.AddWithValue("ssid", site.AliasSchemeIdentity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("scid", site.AliasSchemeIdentity.Identity);
                        cmd.Parameters.AddWithValue("name", site.Name);
                        cmd.Parameters.AddWithValue("oldName", SiteUtils.OriginalName(site));
                        cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                        Db.ExecuteNonQuery(cmd);

                        return true;
                    }
                    catch
                    { }
                }
                else
                    return true; //no change to the alias
            }
            return false;
        }

        internal PgSiteAliasProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
