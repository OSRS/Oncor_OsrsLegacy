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
using Osrs.Security;
using Osrs.WellKnown.Organizations;
using Osrs.Data;
using Npgsql;

namespace Osrs.WellKnown.UserAffiliation.Providers
{
    public sealed class PgUserAffiliationProvider : UserAffiliationProviderBase
    {
        public override bool CanAdd(IUserIdentity user)
        {
            return this.CanAdd(); //TODO -- add fine grained security
        }

        public override bool CanAdd(Organization org)
        {
            return this.CanAdd(); //TODO -- add fine grained security
        }

        public override bool CanGet(IUserIdentity user)
        {
            return this.CanGet(); //TODO -- add fine grained security
        }

        public override bool CanGet(Organization org)
        {
            return this.CanGet(); //TODO -- add fine grained security
        }

        public override bool CanRemove(IUserIdentity user)
        {
            return this.CanRemove(); //TODO -- add fine grained security
        }

        public override bool CanRemove(Organization org)
        {
            return this.CanRemove(); //TODO -- add fine grained security
        }

        public override bool HasAffiliation(IUserIdentity user, Organization org)
        {
            if (user != null && org !=null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectCount;
                cmd.Parameters.AddWithValue("uid", user.Uid);
                cmd.Parameters.AddWithValue("osid", org.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", org.Identity.Identity);
                return Db.Exists(cmd);
            }
            return false;
        }

        //\"UserId\", \"OrgSystemId\", \"OrgId\"
        //:uid :osid :oid
        public override IEnumerable<CompoundIdentity> GetIds(IUserIdentity user)
        {
            if (user!=null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.WhereUser;
                cmd.Parameters.AddWithValue("uid", user.Uid);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                HashSet<CompoundIdentity> permissions = new HashSet<CompoundIdentity>();
                try
                {
                    CompoundIdentity o;
                    while (rdr.Read())
                    {
                        o = new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 1), DbReaderUtils.GetGuid(rdr, 2));
                        if (!o.IsEmpty)
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

        public override IEnumerable<Guid> GetIds(Organization org)
        {
            if (org != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.WhereOrg;
                cmd.Parameters.AddWithValue("osid", org.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("oid", org.Identity.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                HashSet<Guid> permissions = new HashSet<Guid>();
                try
                {
                    Guid o;
                    while (rdr.Read())
                    {
                        o = DbReaderUtils.GetGuid(rdr, 0);
                        if (!Guid.Empty.Equals(o))
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

        public override bool Add(IEnumerable<IUserIdentity> user, Organization org)
        {
            if (user!=null && org!=null)
            {
                bool res = true;
                foreach(IUserIdentity u in user)
                {
                    if (u!=null)
                        res = res & Add(u, org);
                }
                return res;
            }

            return false;
        }

        public override bool Add(IUserIdentity user, IEnumerable<Organization> org)
        {
            if (user != null && org != null)
            {
                bool res = true;
                foreach (Organization u in org)
                {
                    if (u != null)
                        res = res & Add(user, u);
                }
                return res;
            }

            return false;
        }

        public override bool Add(IUserIdentity user, Organization org)
        {
            if (org != null && user!=null && this.CanAdd(org) && this.CanAdd(user))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Insert;
                    cmd.Parameters.AddWithValue("uid", user.Uid);
                    cmd.Parameters.AddWithValue("osid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", org.Identity.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override bool Remove(IUserIdentity user, IEnumerable<Organization> org)
        {
            if (user != null && org != null)
            {
                bool res = true;
                foreach (Organization u in org)
                {
                    if (u != null)
                        res = res & Remove(user, u);
                }
                return res;
            }

            return false;
        }

        public override bool Remove(IEnumerable<IUserIdentity> user, Organization org)
        {
            if (user != null && org != null)
            {
                bool res = true;
                foreach (IUserIdentity u in user)
                {
                    if (u != null)
                        res = res & Remove(u, org);
                }
                return res;
            }

            return false;
        }

        public override bool Remove(IUserIdentity user, Organization org)
        {
            if (org != null && user != null && this.CanRemove(org) && this.CanRemove(user))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Delete;
                    cmd.Parameters.AddWithValue("uid", user.Uid);
                    cmd.Parameters.AddWithValue("osid", org.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", org.Identity.Identity);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgUserAffiliationProvider(UserSecurityContext context) : base(context)
        { }
    }
}
