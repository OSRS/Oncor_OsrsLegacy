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
using System.Text;
using Osrs.Data;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;

namespace Osrs.WellKnown.SensorsAndInstruments.Providers
{
	public sealed class PgInstrumentFamilyProvider : InstrumentFamilyProviderBase  //change this to implement InstrumentFamilyProviderBase
	{
		public override bool CanDelete(InstrumentFamily org)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(InstrumentFamily org)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override InstrumentFamily Create(string name, string description, CompoundIdentity parentId)
		{
			if(!string.IsNullOrEmpty(name) && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentFamily;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					if (!string.IsNullOrEmpty(description))
						cmd.Parameters.AddWithValue("desc", description);
					else
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					if (parentId != null)
						cmd.Parameters.AddWithValue("pid", parentId.Identity);
					else
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));

					Db.ExecuteNonQuery(cmd);

					return new InstrumentFamily(new CompoundIdentity(Db.DataStoreIdentity, id), name, description, parentId);
				}
				catch
				{ }
			}
			return null;
		}

		public override bool Delete(CompoundIdentity id)
		{
			if(!id.IsNullOrEmpty() && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteInstrumentFamily;
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
			if(id != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountInstrumentFamily + Db.SelectById;
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
				cmd.CommandText = Db.CountInstrumentFamily + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<InstrumentFamily> Get()
		{
			if (this.CanGet())
				return new Enumerable<InstrumentFamily>(new EnumerableCommand<InstrumentFamily>(InstrumentFamilyBuilder.Instance, Db.SelectInstrumentFamily, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<InstrumentFamily> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectInstrumentFamily + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentFamily> permissions = new List<InstrumentFamily>();
				try
				{
					InstrumentFamily f;
					while(rdr.Read())
					{
						f = InstrumentFamilyBuilder.Instance.Build(rdr);
						if (f != null)
							permissions.Add(f);
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

		public override InstrumentFamily Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentFamily + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				InstrumentFamily f = null;
				if(rdr != null)
				{
					try
					{
						rdr.Read();
						f = InstrumentFamilyBuilder.Instance.Build(rdr);
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
				return f;
			}
			return null;
		}

		public override IEnumerable<InstrumentFamily> Get(IEnumerable<CompoundIdentity> ids)
		{
			if (ids != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"Id\" IN (");
				int count = 0;
				foreach(CompoundIdentity id in ids)
				{
					if (!id.IsNullOrEmpty())
					{
						where.Append('\'');
						where.Append(id.Identity);
						where.Append("',");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentFamily + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentFamily> fams = new List<InstrumentFamily>();
				try
				{
					InstrumentFamily f;
					while(rdr.Read())
					{
						f = InstrumentFamilyBuilder.Instance.Build(rdr);
						if (f != null)
							fams.Add(f);
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
				return fams;
			}
			return null;
		}

		public override IEnumerable<InstrumentFamily> GetChildren(CompoundIdentity id)
		{
			if (id != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentFamily +  " WHERE \"ParentId\"=:pid";
				cmd.Parameters.AddWithValue("pid", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentFamily> permissions = new List<InstrumentFamily>();
				try
				{
					InstrumentFamily f;
					while (rdr.Read())
					{
						f = InstrumentFamilyBuilder.Instance.Build(rdr);
						if (f != null)
							permissions.Add(f);
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

		public override bool Update(InstrumentFamily org)
		{
			if(org != null && this.CanUpdate(org))
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.UpdateInstrumentFamily + Db.SelectById;
				cmd.Parameters.AddWithValue("name", org.Name);
				if (!string.IsNullOrEmpty(org.Description))
					cmd.Parameters.AddWithValue("desc", org.Description);
				else
					cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
				if (org.ParentId != null)
					cmd.Parameters.AddWithValue("pid", org.ParentId.Identity);
				else
					cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
				cmd.Parameters.AddWithValue("id", org.Identity.Identity);
				Db.ExecuteNonQuery(cmd);
				return true;
			}
			return false;
		}

		internal PgInstrumentFamilyProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
