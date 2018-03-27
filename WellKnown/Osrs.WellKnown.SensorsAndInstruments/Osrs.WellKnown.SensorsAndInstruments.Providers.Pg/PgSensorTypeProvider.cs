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
using Newtonsoft.Json.Linq;

namespace Osrs.WellKnown.SensorsAndInstruments.Providers
{
	public sealed class PgSensorTypeProvider : SensorTypeProviderBase
	{
		public override bool CanDelete(SensorType org)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(SensorType org)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override SensorType Create(string name, string description, CompoundIdentity parentId)
		{
			if (!string.IsNullOrEmpty(name) && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertSensorType;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);
					if (parentId == null)
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
					else
						cmd.Parameters.AddWithValue("pid", parentId.Identity);
					cmd.Parameters.AddWithValue("stif", Db.ToJson(new List<CompoundIdentity>()).ToString());
					Db.ExecuteNonQuery(cmd);
					return new SensorType(new CompoundIdentity(Db.DataStoreIdentity, id), name, description, parentId);
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
					cmd.CommandText = Db.DeleteSensorType;
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
				cmd.CommandText = Db.CountSensorType + Db.SelectById;
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
				cmd.CommandText = Db.CountSensorType + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<SensorType> Get()
		{
			if (this.CanGet())
				return new Enumerable<SensorType>(new EnumerableCommand<SensorType>(SensorTypeBuilder.Instance, Db.SelectSensorType, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<SensorType> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name))
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectSensorType + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<SensorType> stypes = new List<SensorType>();
				try
				{
					SensorType s;
					while (rdr.Read())
					{
						s = SensorTypeBuilder.Instance.Build(rdr);
						if (s != null)
							stypes.Add(s);
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
				return stypes;
			}
			return null;
		}

		public override SensorType Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensorType + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				SensorType s = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						s = SensorTypeBuilder.Instance.Build(rdr);
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
				return s;
			}
			return null;
		}

		public override IEnumerable<SensorType> Get(IEnumerable<CompoundIdentity> ids)
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
				cmd.CommandText = Db.SelectSensorType + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<SensorType> sens = new List<SensorType>();
				try
				{
					SensorType s;
					while (rdr.Read())
					{
						s = SensorTypeBuilder.Instance.Build(rdr);
						if (s != null)
							sens.Add(s);
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
				return sens;
			}
			return null;
		}

		public override IEnumerable<SensorType> GetChildren(CompoundIdentity id)
		{
			if (id != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensorType + " WHERE \"ParentId\"=:pid";
				cmd.Parameters.AddWithValue("pid", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<SensorType> permissions = new List<SensorType>();
				try
				{
					SensorType t;
					while (rdr.Read())
					{
						t = SensorTypeBuilder.Instance.Build(rdr);
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

		public override IEnumerable<SensorType> GetFor(CompoundIdentity instrumentFamilyId)
		{
			if (instrumentFamilyId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensorType + " WHERE \"SensorTypeInstrumentFamilies\" LIKE '%:stif%'";
				cmd.Parameters.AddWithValue("stif", instrumentFamilyId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<SensorType> permissions = new List<SensorType>();
				try
				{
					SensorType t;
					while (rdr.Read())
					{
						t = SensorTypeBuilder.Instance.Build(rdr);
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

		public override IEnumerable<SensorType> GetFor(IEnumerable<CompoundIdentity> instrumentFamilyId)
		{
			if (instrumentFamilyId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"SensorTypeInstrumentFamilies\" SIMILAR TO '");
				int count = 0;
				foreach (CompoundIdentity id in instrumentFamilyId)
				{
					if (!id.IsNullOrEmpty())
					{
						where.Append('%');
						where.Append(id.Identity);
						where.Append("%|");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = '\'';
				else
					where.Append('\'');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensorType + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<SensorType> sens = new List<SensorType>();
				try
				{
					SensorType s;
					while (rdr.Read())
					{
						s = SensorTypeBuilder.Instance.Build(rdr);
						if (s != null)
							sens.Add(s);
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
				return sens;
			}
			return null;
		}

		public override bool Update(SensorType org)
		{
			if (org != null && this.CanUpdate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateSensorType + Db.SelectById;
					cmd.Parameters.AddWithValue("name", org.Name);
					if (string.IsNullOrEmpty(org.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", org.Description);
					if (org.ParentId == null)
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
					else
						cmd.Parameters.AddWithValue("pid", org.ParentId.Identity);
					cmd.Parameters.AddWithValue("stif", Db.ToJson(org.InstrumentFamilies).ToString());
					cmd.Parameters.AddWithValue("id", org.Identity.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgSensorTypeProvider(UserSecurityContext context) : base(context)
		{

		}
	}
}
