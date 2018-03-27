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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Osrs.Data;
using Osrs.Security;
using Osrs.Security.Authorization;
using Npgsql;
using Osrs.Data.Postgres;

namespace Osrs.WellKnown.Projects.Providers
{
    public sealed class PgStatusTypeProvider : ProjectStatusTypeProviderBase
    {
        public override bool CanDelete(ProjectStatusType statusType)
        {
            return this.CanDelete();
        }

        public override bool CanUpdate(ProjectStatusType statusType)
        {
            return this.CanUpdate();
        }

        public override ProjectStatusType Create(string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && this.CanCreate())
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.InsertStatTyp;
                    Guid id = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("sid", Db.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", name);
                    if (string.IsNullOrEmpty(description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", description);

                    Db.ExecuteNonQuery(cmd);

                    return new ProjectStatusType(new CompoundIdentity(Db.DataStoreIdentity, id), name, description);
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
                    cmd.CommandText = Db.DeleteStatTyp;
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

        public override bool Exists(CompoundIdentity id)
        {
            if (id != null && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountStatTyp + Db.SelectById;
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
                cmd.CommandText = Db.CountStatTyp + where;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public override IEnumerable<ProjectStatusType> Get()
        {
            if (this.CanGet())
                return new Enumerable<ProjectStatusType>(new EnumerableCommand<ProjectStatusType>(ProjecStatusTypeBuilder.Instance, Db.SelectStatTyp, Db.ConnectionString));
            return null;
        }

        public override ProjectStatusType Get(CompoundIdentity id)
        {
            if (!id.IsNullOrEmpty() && this.CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectStatTyp + Db.SelectById;
                cmd.Parameters.AddWithValue("sid", id.DataStoreIdentity);
                cmd.Parameters.AddWithValue("id", id.Identity);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                ProjectStatusType o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                            o = ProjecStatusTypeBuilder.Instance.Build(rdr);
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

        public override IEnumerable<ProjectStatusType> Get(string name, StringComparison comparisonOption)
        {
            if (!string.IsNullOrEmpty(name) && this.CanGet())
            {
                string where = " WHERE ";
                if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
                    where += "lower(\"Name\")=lower(:name)";
                else
                    where += "\"Name\"=:name";
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.SelectStatTyp + where;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<ProjectStatusType> schemes = new List<ProjectStatusType>();
                ProjectStatusType o = null;
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            o = ProjecStatusTypeBuilder.Instance.Build(rdr);
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

        public override bool Update(ProjectStatusType statusType)
        {
            if (statusType != null && !statusType.Identity.IsNullOrEmpty() && CanUpdate(statusType))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.UpdateStatTyp;
                    cmd.Parameters.AddWithValue("sid", statusType.Identity.DataStoreIdentity);
                    cmd.Parameters.AddWithValue("id", statusType.Identity.Identity);
                    cmd.Parameters.AddWithValue("name", statusType.Name);
                    if (string.IsNullOrEmpty(statusType.Description))
                        cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
                    else
                        cmd.Parameters.AddWithValue("desc", statusType.Description);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgStatusTypeProvider(UserSecurityContext context) : base(context)
        { }
    }
}
