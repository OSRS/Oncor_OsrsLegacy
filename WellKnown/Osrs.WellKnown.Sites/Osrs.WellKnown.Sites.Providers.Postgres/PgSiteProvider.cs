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
    public sealed class PgSiteProvider : SiteProviderBase
    {
        public override bool CanDelete(Site org)
        {
            return this.CanDelete(); //TODO -- add fine grained security
        }

        public override bool CanUpdate(Site org)
        {
            return this.CanUpdate(); //TODO -- add fine grained security
        }

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountSite + Db.SelectSiteById + " AND" + Db.WhereCurrent;
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
                cmd.CommandText = Db.CountSite + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<Site> Get()
        {
            if (this.CanGet())
                return new Enumerable<Site>(new EnumerableCommand<Site>(SiteBuilder.Instance, Db.SelectSite + " WHERE " + Db.WhereCurrent, Db.ConnectionString));
            return null;
        }

        public override Site Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectSite + Db.SelectSiteById + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                Site o = null;
                if (rdr != null)
                {
                    try
                    {
                        rdr.Read();
                        o = SiteBuilder.Instance.Build(rdr);
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

        public override IEnumerable<Site> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                string where;
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where = " WHERE lower(\"Name\")=lower(:name)";
                else
                    where = " WHERE \"Name\"=:name";
                cmd.CommandText = Db.SelectSite + where + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Site> permissions = new List<Site>();
                try
                {
                    Site o;
                    while (rdr.Read())
                    {
                        o = SiteBuilder.Instance.Build(rdr);
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

        public override IEnumerable<Site> GetByOwner(CompoundIdentity owningOrgId)
        {
            if (!owningOrgId.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectSite + Db.SelectSiteByOwnerId + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("osid", owningOrgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", owningOrgId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Site> schemes = new List<Site>();
                Site o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = SiteBuilder.Instance.Build(rdr);
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

        public override Site Create(CompoundIdentity owningOrgId, string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertSite;
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

                    return new Site(new CompoundIdentity(Db.DataStoreIdentity, id), owningOrgId, name, description);
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
                    cmd.CommandText = Db.DeleteSite + " AND" + Db.WhereCurrent;
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

        public override bool Update(Site site)
        {
            if (site != null && this.CanUpdate(site))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateSite;
                    cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                    cmd.Parameters.AddWithValue("osid", site.OwningOrganizationIdentity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", site.OwningOrganizationIdentity.Identity);
                    cmd.Parameters.AddWithValue("name", site.Name);
                    if (string.IsNullOrEmpty(site.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", site.Description);

                    if (site.Location != null)
                    {
                        NpgsqlTypes.PostgisGeometry geom = Osrs.Numerics.Spatial.Postgres.NpgSpatialUtils.ToPGis(site.Location);
                        if (geom!=null)
                            cmd.Parameters.AddWithValue("loc", geom);
                        else
                            cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("loc", NpgsqlTypes.NpgsqlDbType.Geometry));
                    }
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("loc", NpgsqlTypes.NpgsqlDbType.Geometry));

                    if (site.LocationMark != null)
                    {
                        NpgsqlTypes.PostgisPoint geom = Osrs.Numerics.Spatial.Postgres.NpgSpatialUtils.ToPGis(site.LocationMark);
                        if (geom!=null)
                            cmd.Parameters.AddWithValue("poi", geom);
                        else
                            cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("poi", NpgsqlTypes.NpgsqlDbType.Geometry));
                    }
                    else
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("poi", NpgsqlTypes.NpgsqlDbType.Geometry));

                    cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool HasParents(Site site)
        {
            if (site != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountHierarchy + Db.SelectHierarchyByChild + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool HasChildren(Site site)
        {
            if (site != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountHierarchy + Db.SelectHierarchyByParent + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<Site> GetParents(CompoundIdentity site)
        {
            if (!site.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectHierarchy + Db.SelectHierarchyByChild + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", site.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", site.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Site> schemes = new List<Site>();
                Site o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = this.Get(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)));
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

        public override IEnumerable<Site> GetChildren(CompoundIdentity parentSite)
        {
            if (!parentSite.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectHierarchy + Db.SelectHierarchyByParent + " AND" + Db.WhereCurrent;
                cmd.Parameters.AddWithValue("sid", parentSite.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", parentSite.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<Site> schemes = new List<Site>();
                Site o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = this.Get(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 2), DbReaderUtils.GetGuid(rdr, 3)));
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

        public override bool AddParent(CompoundIdentity child, CompoundIdentity parent)
        {
            if (child != null && parent!=null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);

                    cmd.CommandText = Db.UpdateParent;
                    cmd.Parameters.AddWithValue("psid", parent.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("pid", parent.Identity);
                    cmd.Parameters.AddWithValue("csid", child.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("cid", child.Identity);
                    cmd.Parameters.AddWithValue("start", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool RemoveParent(CompoundIdentity child, CompoundIdentity parent)
        {
            if (child != null && parent != null && this.CanUpdate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);

                    cmd.CommandText = Db.DeleteParent;
                    cmd.Parameters.AddWithValue("psid", parent.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("pid", parent.Identity);
                    cmd.Parameters.AddWithValue("csid", child.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("cid", child.Identity);
                    cmd.Parameters.AddWithValue("startdate", DateTime.UtcNow);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool AddParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent)
        {
            if (parent!=null && children!= null && this.CanUpdate())
            {
                bool res = true;
                foreach (CompoundIdentity cur in children)
                {
                    if (!AddParent(cur, parent))
                        res = false;
                }
                return res;
            }
            return false;
        }

        public override bool RemoveParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent)
        {
            if (parent != null && children != null && this.CanUpdate())
            {
                bool res = true;
                foreach (CompoundIdentity cur in children)
                {
                    if (!RemoveParent(cur, parent))
                        res = false;
                }
                return res;
            }
            return false;
        }

        internal PgSiteProvider(UserSecurityContext context) : base(context)
        {
        }
    }
}
