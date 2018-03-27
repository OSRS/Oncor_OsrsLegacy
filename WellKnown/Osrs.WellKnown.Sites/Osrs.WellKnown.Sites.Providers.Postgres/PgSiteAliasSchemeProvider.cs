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
    //TODO -- Update all for temporal
    public sealed class PgSiteAliasSchemeProvider : SiteAliasSchemeProviderBase
    {
        public override bool CanDelete(SiteAliasScheme org)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(SiteAliasScheme org)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAliasScheme + Db.SelectAliasSchemeById + " AND" + Db.WhereCurrent;
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
                cmd.CommandText = Db.CountAliasScheme + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption)
        {
            if (!owningOrgId.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasSchemeByOwnerId;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountAliasScheme + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", owningOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owningOrgId.Identity);
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<SiteAliasScheme> Get()
        {
            if (this.CanGet())
                return new Enumerable<SiteAliasScheme>(new EnumerableCommand<SiteAliasScheme>(SiteAliasSchemeBuilder.Instance, Db.SelectAliasScheme + " WHERE " + Db.WhereCurrent, Db.ConnectionString));
            return null;
        }

        public override SiteAliasScheme Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + Db.SelectAliasSchemeById + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                SiteAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = SiteAliasSchemeBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAliasScheme> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = " WHERE ";
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += "lower(\"Name\")=lower(:name)";
                else
                    where += "\"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAliasScheme> schemes = new List<SiteAliasScheme>();
                SiteAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasSchemeBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId)
        {
            if (!owningOrgId.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + Db.SelectAliasSchemeByOwnerId + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", owningOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owningOrgId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAliasScheme> schemes = new List<SiteAliasScheme>();
                SiteAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasSchemeBuilder.Instance.Build(rdr);
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

        public override IEnumerable<SiteAliasScheme> GetByOwner(CompoundIdentity owningOrgId, string name, StringComparison comparisonOption)
        {
            if (!owningOrgId.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = Db.SelectAliasSchemeByOwnerId;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += " AND lower(\"Name\")=lower(:name)";
                else
                    where += " AND \"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectAliasScheme + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", owningOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", owningOrgId.Identity);
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<SiteAliasScheme> schemes = new List<SiteAliasScheme>();
                SiteAliasScheme o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteAliasSchemeBuilder.Instance.Build(rdr);
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

        public override SiteAliasScheme Create(CompoundIdentity owningOrgId, string name, string description)
        {
            if (!owningOrgId.IsNullOrEmpty() && !string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertAliasScheme;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("osid", owningOrgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", owningOrgId.Identity);
                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);
                    cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return new SiteAliasScheme(new CompoundIdentity(Db.DataStoreIdentity, id), owningOrgId, name, description);
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
                    cmd.CommandText = Db.DeleteAliasScheme + " AND" + Db.WhereCurrent;
                    cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id.Identity);
                    cmd.Parameters.AddWithValue("end", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Update(SiteAliasScheme scheme)
        {
            if (scheme != null && !scheme.Identity.IsNullOrEmpty() && CanUpdate(scheme))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateAliasScheme;
                    cmd.Parameters.AddWithValue("sid", scheme.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", scheme.Identity.Identity);
                    cmd.Parameters.AddWithValue("osid", scheme.OwningOrganizationIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", scheme.OwningOrganizationIdentity.Identity);
                    cmd.Parameters.AddWithValue("name", scheme.Name);
                    if (string.IsNullOrEmpty(scheme.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", scheme.Description);
                    cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgSiteAliasSchemeProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
