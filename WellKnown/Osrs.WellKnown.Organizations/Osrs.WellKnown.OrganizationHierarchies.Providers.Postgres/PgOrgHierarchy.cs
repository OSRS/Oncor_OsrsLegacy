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

using System.Collections.Generic;
using Osrs.Data;
using Osrs.WellKnown.Organizations;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;
using System;

namespace Osrs.WellKnown.OrganizationHierarchies.Providers
{
    public sealed class PgOrgHierarchy : OrganizationHierarchyBase
    {
        public override bool CanAdd(CompoundIdentity parentOrgId, CompoundIdentity childOrgId)
        {
            return this.CanAdd();
        }

        public override bool CanMove(CompoundIdentity orgId)
        {
            return this.CanMove();
        }

        public override bool CanRemove(CompoundIdentity parentOrgId, CompoundIdentity childOrgId)
        {
            return this.CanRemove();
        }

        public override bool UpdateInfo()
        {
            if (this.CanUpdateInfo())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Update;
                    cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", this.Identity.Identity);
                    cmd.Parameters.AddWithValue("osid", this.OwningOrgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("oid", this.OwningOrgId.Identity);
                    cmd.Parameters.AddWithValue("name", this.Name);
                    if (string.IsNullOrEmpty(this.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", this.Description);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override IEnumerable<Organization> GetChildren(Organization org, uint maxLevels)
        {
            if (org != null)
            {
                IEnumerable<CompoundIdentity> ids = this.GetChildrenIds(org.Identity, maxLevels); //does the permission check here
                if (ids != null)
                    return OrganizationManager.Instance.GetOrganizationProvider(this.Context).Get(ids);
            }
            return null;
        }

        public override IEnumerable<CompoundIdentity> GetChildrenIds(CompoundIdentity orgId, uint maxLevels)
        {
            if (!orgId.IsNullOrEmpty() && CanGet())
            {
                HashSet<CompoundIdentity> res = new HashSet<CompoundIdentity>();
                HashSet<CompoundIdentity> localA = new HashSet<CompoundIdentity>();
                HashSet<CompoundIdentity> localB = new HashSet<CompoundIdentity>();

                bool useA = true;
                uint curDepth = 0;

                GetChilds(localA, orgId);
                if (maxLevels == 0)
                    useA = false;
                else
                {
                    while (curDepth < maxLevels)
                    {
                        if (useA)
                        {
                            localB.Clear();
                            foreach (CompoundIdentity cur in localA)
                            {
                                res.Add(cur);
                                GetChilds(localB, cur);
                            }
                            useA = false;
                            curDepth++;
                            if (localB.Count == 0)//no new items added -so exit early
                                curDepth = maxLevels;
                        }
                        else
                        {
                            localA.Clear();
                            foreach (CompoundIdentity cur in localB)
                            {
                                res.Add(cur);
                                GetChilds(localA, cur);
                            }
                            useA = true;
                            curDepth++;
                            if (localA.Count == 0)//no new items added -so exit early
                                curDepth = maxLevels;
                        }
                    }
                }

                if (useA) //add the last items if any
                {
                    if (localB.Count>0)
                    {
                        foreach (CompoundIdentity cur in localB)
                        {
                            res.Add(cur);
                        }
                    }
                }
                else
                {
                    if (localA.Count > 0)
                    {
                        foreach (CompoundIdentity cur in localA)
                        {
                            res.Add(cur);
                        }
                    }
                }
                return res;
            }
            return null;
        }

        private void GetChilds(HashSet<CompoundIdentity> res, CompoundIdentity orgId)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectMemCh;
                cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", this.Identity.Identity);
                cmd.Parameters.AddWithValue("psid", orgId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("pid", orgId.Identity);

                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            res.Add(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)));
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
            }
            catch
            { }
        }

        //:sid,:id,:psid,:pid,:csid,:cid
        private bool Up(CompoundIdentity parentId, CompoundIdentity childId)
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.UpdateMem;
                cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", this.Identity.Identity);
                cmd.Parameters.AddWithValue("psid", parentId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("pid", parentId.Identity);
                cmd.Parameters.AddWithValue("csid", childId.DataStoreIdentity);
                cmd.Parameters.AddWithValue("cid", childId.Identity);

                Db.ExecuteNonQuery(cmd);
                return true;
            }
            catch
            { }
            return false;
        }

        public override bool Add(Organization parent, IEnumerable<Organization> child)
        {
            if (parent!=null && child!=null && this.CanAdd()) //short circuit quit
            {
                bool res = true;
                foreach(Organization cur in child)
                {
                    if (cur != null)
                        res = res & Add(parent.Identity, cur.Identity);
                }
                return res;
            }
            return false;
        }

        public override bool Add(CompoundIdentity parentId, IEnumerable<CompoundIdentity> childId)
        {
            if (!parentId.IsNullOrEmpty() && childId != null && this.CanAdd()) //short circuit quit
            {
                bool res = true;
                foreach (CompoundIdentity cur in childId)
                {
                    if (!cur.IsNullOrEmpty())
                        res = res & Add(parentId, cur);
                }
                return res;
            }
            return false;
        }

        public override bool Add(CompoundIdentity parentId, CompoundIdentity childId)
        {
            if (!parentId.IsNullOrEmpty() && !childId.IsNullOrEmpty() && this.CanAdd(parentId, childId))
            {
                return Up(parentId, childId);
            }
            return false;
        }

        public override bool Move(CompoundIdentity oldParentId, CompoundIdentity newParentId, IEnumerable<CompoundIdentity> childId)
        {
            if (oldParentId != null && newParentId != null && childId != null && CanMove())
            {
                bool res = true;
                foreach (CompoundIdentity o in childId)
                {
                    if (!o.IsNullOrEmpty())
                        res = res & Move(oldParentId, newParentId, o);
                }
                return res;
            }
            return false;
        }

        public override bool Move(Organization oldParent, Organization newParent, IEnumerable<Organization> child)
        {
            if (oldParent!=null && newParent!=null && child!=null && CanMove())
            {
                bool res = true;
                foreach (Organization o in child)
                {
                    if (o!=null)
                        res = res & Move(oldParent.Identity, newParent.Identity, o.Identity);
                }
                return res;
            }
            return false;
        }

        public override bool Move(CompoundIdentity oldParentId, CompoundIdentity newParentId, CompoundIdentity childId)
        {
            if (!oldParentId.IsNullOrEmpty() && !newParentId.IsNullOrEmpty() && !childId.IsNullOrEmpty() && CanMove(childId))
            {
                IEnumerable<CompoundIdentity> ids = GetChildrenIds(oldParentId, false);
                if (ids!=null)
                {
                    foreach(CompoundIdentity cur in ids)
                    {
                        if (cur.Equals(childId)) //we have a match
                        {
                            return Up(newParentId, childId);
                        }
                    }
                }
            }
            return false;
        }

        public override bool Remove(Organization parent, IEnumerable<Organization> child)
        {
            if (parent != null && child != null && this.CanRemove()) //short circuit quit
            {
                bool res = true;
                foreach (Organization cur in child)
                {
                    if (cur != null)
                        res = res & Remove(parent.Identity, cur.Identity);
                }
                return res;
            }
            return false;
        }

        public override bool Remove(CompoundIdentity parentId, IEnumerable<CompoundIdentity> childId)
        {
            if (!parentId.IsNullOrEmpty() && childId != null && this.CanRemove()) //short circuit quit
            {
                bool res = true;
                foreach (CompoundIdentity cur in childId)
                {
                    if (!cur.IsNullOrEmpty())
                        res = res & Remove(parentId, cur);
                }
                return res;
            }
            return false;
        }

        public override bool Remove(CompoundIdentity parentId, CompoundIdentity childId)
        {
            if (!parentId.IsNullOrEmpty() && !childId.IsNullOrEmpty() && CanRemove(parentId, childId))
            {
                IEnumerable<CompoundIdentity> ids = GetChildrenIds(parentId, false);
                if (ids != null)
                {
                    foreach (CompoundIdentity cur in ids)
                    {
                        if (cur.Equals(childId)) //we have a match
                        {
                            try
                            {
                                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                                cmd.CommandText = Db.DeleteMem + Db.WhereElemParent + Db.WhereElem;
                                cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                                cmd.Parameters.AddWithValue("id", this.Identity.Identity);
                                cmd.Parameters.AddWithValue("psid", parentId.DataStoreIdentity);
                                cmd.Parameters.AddWithValue("pid", parentId.Identity);
                                cmd.Parameters.AddWithValue("csid", childId.DataStoreIdentity);
                                cmd.Parameters.AddWithValue("cid", childId.Identity);

                                Db.ExecuteNonQuery(cmd);
                                return true;
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            return false;
        }

        public override IEnumerable<CompoundIdentity> GetParentIds(CompoundIdentity orgId)
        {
            if (!orgId.IsNullOrEmpty())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.SelectMem + Db.WhereElem;
                    cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", this.Identity.Identity);
                    cmd.Parameters.AddWithValue("csid", orgId.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("cid", orgId.Identity);

                    NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                    if (rdr != null)
                    {
                        HashSet<CompoundIdentity> res = new HashSet<CompoundIdentity>();
                        try
                        {
                            while (rdr.Read())
                            {
                                res.Add(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)));
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
                        return res;
                    }
                }
                catch
                { }
            }
            return null;
        }

        public override IEnumerable<KeyValuePair<CompoundIdentity, CompoundIdentity>> GetAllPairs()
        {
            try
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectMem;
                cmd.Parameters.AddWithValue("sid", this.Identity.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", this.Identity.Identity);

                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                if (rdr != null)
                {
                    List<KeyValuePair<CompoundIdentity, CompoundIdentity>> res = new List<KeyValuePair<CompoundIdentity, CompoundIdentity>>();
                    try
                    {
                        while (rdr.Read())
                        {
                            res.Add(new KeyValuePair<CompoundIdentity, CompoundIdentity>(new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 0), DbReaderUtils.GetGuid(rdr, 1)), 
                                new CompoundIdentity(DbReaderUtils.GetGuid(rdr, 2), DbReaderUtils.GetGuid(rdr, 3))));
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
                    return res;
                }
            }
            catch
            { }
            return null;
        }

        internal PgOrgHierarchy(UserSecurityContext context, CompoundIdentity identity, CompoundIdentity owningOrgId, string name, string description) : base(context, identity, owningOrgId, name, description)
        {
        }
    }
}
