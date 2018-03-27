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
using Osrs.Data;
using Osrs.Security;
using Npgsql;
using Osrs.WellKnown.Organizations;
using Osrs.Data.Postgres;

namespace Osrs.WellKnown.OrganizationHierarchies.Providers
{
    public sealed class PgOrgHierarchyProvider : OrganizationHierarchyProviderBase
    {
        protected override bool CanDeleteImpl(CompoundIdentity hierarchyId)
        {
            return this.CanDelete();
        }

        public override bool Exists(string hierarchyName)
        {
            if (!string.IsNullOrEmpty(hierarchyName) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Count + Db.WhereName;
                cmd.Parameters.AddWithValue("name", hierarchyName);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override bool Exists(CompoundIdentity hierarchyId)
        {
            if (hierarchyId != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Count + Db.WhereId;
                cmd.Parameters.AddWithValue("sid", hierarchyId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", hierarchyId.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        //\"SystemId\", \"Id\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"Description\"
        public override OrganizationHierarchy Get(string hierarchyName)
        {
            if (!string.IsNullOrEmpty(hierarchyName) && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.WhereName;
                cmd.Parameters.AddWithValue("name", hierarchyName);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                OrganizationHierarchy o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                        {
                            o = new PgOrgHierarchy(this.Context, new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)),
                                new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 2), DbReaderUtils.GetGuid(rdr, 3)), DbReaderUtils.GetString(rdr, 4), DbReaderUtils.GetString(rdr, 5));
                            if (cmd.Connection.State == System.Data.ConnectionState.Open)
                                cmd.Connection.Close();
                        }
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

        public override OrganizationHierarchy Get(CompoundIdentity hierarchyId)
        {
            if (!hierarchyId.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.WhereId;
                cmd.Parameters.AddWithValue("sid", hierarchyId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", hierarchyId.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                OrganizationHierarchy o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                        {
                            o = new PgOrgHierarchy(this.Context, new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)),
                                new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 2), DbReaderUtils.GetGuid(rdr, 3)), DbReaderUtils.GetString(rdr, 4), DbReaderUtils.GetString(rdr, 5));
                            if (cmd.Connection.State == System.Data.ConnectionState.Open)
                                cmd.Connection.Close();
                        }
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

        public override OrganizationHierarchy Create(string hierarchyName, Organization owningOrg)
        {
            if (owningOrg != null && !owningOrg.Identity.IsNullOrEmpty() && !string.IsNullOrEmpty(hierarchyName) && this.CanCreate())
            {
                if (!this.Exists(hierarchyName))
                {
                    try
                    {
                        NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                        cmd.CommandText = Db.Insert;
                        Guid id = Guid.NewGuid();
                        cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Parameters.AddWithValue("osid", owningOrg.Identity.DataStoreIdentity);
                        cmd.Parameters.AddWithValue("oid", owningOrg.Identity.Identity);
                        cmd.Parameters.AddWithValue("name", hierarchyName);
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));

                        Db.ExecuteNonQuery(cmd);

                        return new PgOrgHierarchy(this.Context, new CompoundIdentity(Db.DataStoreIdentity, id), owningOrg.Identity, hierarchyName, null);
                    }
                    catch
                    { }
                }
            }
            return null;
        }

        public override bool Delete(OrganizationHierarchy hierarchy)
        {
            if (hierarchy != null)
                return Delete(hierarchy.Identity);
            return false;
        }

        public override bool Delete(CompoundIdentity hierarchyId)
        {
            if (!hierarchyId.IsNullOrEmpty() && this.CanDelete(hierarchyId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Delete;
                    cmd.Parameters.AddWithValue("sid", hierarchyId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", hierarchyId.Identity);

                    Db.ExecuteNonQuery(cmd);

                    cmd.CommandText = Db.DeleteMem; //don't forget to clear the actual records

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override OrganizationHierarchy GetReporting()
        {
            return this.Get(new CompoundIdentity(Db.DataStoreIdentity, OrganizationHierarchyUtils.ReportingHierarchyId));
        }

        internal PgOrgHierarchyProvider(UserSecurityContext context):base(context)
        { }
    }
}
