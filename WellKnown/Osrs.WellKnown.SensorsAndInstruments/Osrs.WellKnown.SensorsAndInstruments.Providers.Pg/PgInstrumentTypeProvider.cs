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
using System.Text;

namespace Osrs.WellKnown.SensorsAndInstruments.Providers
{
	public sealed class PgInstrumentTypeProvider : InstrumentTypeProviderBase
	{
		public override bool CanDelete(InstrumentType org)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(InstrumentType org)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override InstrumentType Create(string name, CompoundIdentity familyId, string description, CompoundIdentity parentId)
		{
			if(!string.IsNullOrEmpty(name) && familyId != null && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentType;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					cmd.Parameters.AddWithValue("fid", familyId.Identity);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);
					if (parentId == null)
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
					else
						cmd.Parameters.AddWithValue("pid", parentId.Identity);
					Db.ExecuteNonQuery(cmd);
					return new InstrumentType(new CompoundIdentity(Db.DataStoreIdentity, id), name, familyId, description, parentId);
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
					InstrumentKnownArchetypeProviderBase archprov = InstrumentManager.Instance.GetInstrumentKnownArchetypeProvider(this.Context);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteInstrumentType;
					cmd.Parameters.AddWithValue("id", id.Identity);
					Db.ExecuteNonQuery(cmd);
					archprov.RemoveInstrumentType(id);
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
				cmd.CommandText = Db.CountInstrumentType + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool Exists(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name))
			{
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountInstrumentType + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<InstrumentType> Get()
		{
			if (this.CanGet())
				return new Enumerable<InstrumentType>(new EnumerableCommand<InstrumentType>(InstrumentTypeBuilder.Instance, Db.SelectInstrumentType, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<InstrumentType> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectInstrumentType + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentType> permissions = new List<InstrumentType>();
				try
				{
					InstrumentType t;
					while (rdr.Read())
					{
						t = InstrumentTypeBuilder.Instance.Build(rdr);
						if (t != null)
							permissions.Add(t);
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

		public override InstrumentType Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentType + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				InstrumentType t = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						t = InstrumentTypeBuilder.Instance.Build(rdr);
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
				return t;
			}
			return null;
		}

		public override IEnumerable<InstrumentType> Get(IEnumerable<CompoundIdentity> ids)
		{
			if (ids != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"Id\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in ids)
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
				cmd.CommandText = Db.SelectInstrumentType + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentType> types = new List<InstrumentType>();
				try
				{
					InstrumentType t;
					while (rdr.Read())
					{
						t = InstrumentTypeBuilder.Instance.Build(rdr);
						if (t != null)
							types.Add(t);
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
				return types;
			}
			return null;
		}

		public override IEnumerable<InstrumentType> GetChildren(CompoundIdentity id)
		{
			if (id != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentType + " WHERE \"ParentId\"=:pid";
				cmd.Parameters.AddWithValue("pid", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentType> permissions = new List<InstrumentType>();
				try
				{
					InstrumentType t;
					while (rdr.Read())
					{
						t = InstrumentTypeBuilder.Instance.Build(rdr);
						if (t != null)
							permissions.Add(t);
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

		public override IEnumerable<InstrumentType> GetFor(CompoundIdentity instrumentFamilyId)
		{
			if (instrumentFamilyId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentType + " WHERE \"FamilyId\"=:fid";
				cmd.Parameters.AddWithValue("fid", instrumentFamilyId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentType> permissions = new List<InstrumentType>();
				try
				{
					InstrumentType t;
					while (rdr.Read())
					{
						t = InstrumentTypeBuilder.Instance.Build(rdr);
						if (t != null)
							permissions.Add(t);
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

		public override IEnumerable<InstrumentType> GetFor(IEnumerable<CompoundIdentity> instrumentFamilyIds)
		{
			if (instrumentFamilyIds != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"FamilyId\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in instrumentFamilyIds)
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
				cmd.CommandText = Db.SelectInstrumentType + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<InstrumentType> types = new List<InstrumentType>();
				try
				{
					InstrumentType t;
					while (rdr.Read())
					{
						t = InstrumentTypeBuilder.Instance.Build(rdr);
						if (t != null)
							types.Add(t);
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
				return types;
			}
			return null;
		}

		public override bool Update(InstrumentType org)
		{
			if(org != null && this.CanUpdate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateInstrumentType + Db.SelectById;
					cmd.Parameters.AddWithValue("name", org.Name);
					cmd.Parameters.AddWithValue("fid", org.FamilyId.Identity);
					if (string.IsNullOrEmpty(org.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", org.Description);
					if (org.ParentId == null)
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
					else
						cmd.Parameters.AddWithValue("pid", org.ParentId.Identity);
					cmd.Parameters.AddWithValue("id", org.Identity.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgInstrumentTypeProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
