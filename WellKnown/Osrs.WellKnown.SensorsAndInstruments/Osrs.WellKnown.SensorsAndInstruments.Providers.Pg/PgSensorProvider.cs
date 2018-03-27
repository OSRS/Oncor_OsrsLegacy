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
	public sealed class PgSensorProvider : SensorProviderBase
	{
		public override bool CanDelete(Sensor item)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(Sensor item)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override Sensor Create(CompoundIdentity id, CompoundIdentity instrumentId, CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId)
		{
			if (!string.IsNullOrEmpty(name) && instrumentId != null && ownerId != null && typeId != null && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertSensor;
					cmd.Parameters.AddWithValue("id", id.Identity);
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					cmd.Parameters.AddWithValue("osid", ownerId.DataStoreIdentity);
					cmd.Parameters.AddWithValue("oid", ownerId.Identity);
					cmd.Parameters.AddWithValue("name", name);
					cmd.Parameters.AddWithValue("tid", typeId.Identity);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);
					if (string.IsNullOrEmpty(serialNumber))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("sn", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("sn", serialNumber);
					if (manufId.IsNullOrEmpty())
					{
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("msid", NpgsqlTypes.NpgsqlDbType.Uuid));
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("mid", NpgsqlTypes.NpgsqlDbType.Uuid));
					}
					else
					{
						cmd.Parameters.AddWithValue("msid", manufId.DataStoreIdentity);
						cmd.Parameters.AddWithValue("mid", manufId.Identity);
					}
					Db.ExecuteNonQuery(cmd);
					return new Sensor(id, instrumentId, ownerId, name, typeId, description, serialNumber, manufId);
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
					cmd.CommandText = Db.DeleteSensor;
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
				cmd.CommandText = Db.CountSensor + Db.SelectById;
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
				cmd.CommandText = Db.CountSensor + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<Sensor> Get()
		{
			if (this.CanGet())
				return new Enumerable<Sensor>(new EnumerableCommand<Sensor>(SensorBuilder.Instance, Db.SelectSensor, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<Sensor> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectSensor + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
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

		public override Sensor Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				Sensor f = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						f = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> Get(IEnumerable<CompoundIdentity> ids)
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
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override IEnumerable<Sensor> GetFor(string serialNumber)
		{
			if (!string.IsNullOrEmpty(serialNumber) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + " WHERE \"SerialNumber\"=:sid";
				cmd.Parameters.AddWithValue("sid", serialNumber);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor t;
					while (rdr.Read())
					{
						t = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> GetFor(IEnumerable<string> serialNumber)
		{
			if (serialNumber != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"SerialNumber\" IN (");
				int count = 0;
				foreach (string sn in serialNumber)
				{
					where.Append('\'');
					where.Append(sn);
					where.Append("',");
					count++;
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override IEnumerable<Sensor> GetForInstrument(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + " WHERE \"InstrumentId\"=:iid";
				cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor t;
					while (rdr.Read())
					{
						t = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> GetForInstrument(IEnumerable<CompoundIdentity> instrumentId)
		{
			if (instrumentId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"InstrumentId\" IN (");
				int count = 0;
				foreach (CompoundIdentity iid in instrumentId)
				{
					if (!iid.IsNullOrEmpty())
					{
						where.Append('\'');
						where.Append(iid.Identity);
						where.Append("',");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override IEnumerable<Sensor> GetForManufacturer(CompoundIdentity manufId)
		{
			if (manufId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + " WHERE \"ManufacturerId\"=:mid";
				cmd.Parameters.AddWithValue("mid", manufId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor t;
					while (rdr.Read())
					{
						t = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> GetForManufacturer(IEnumerable<CompoundIdentity> manufId)
		{
			if (manufId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"ManufacturerId\" IN (");
				int count = 0;
				foreach (CompoundIdentity mid in manufId)
				{
					if (!mid.IsNullOrEmpty())
					{
						where.Append('\'');
						where.Append(mid.Identity);
						where.Append("',");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override IEnumerable<Sensor> GetForOwner(CompoundIdentity ownerId)
		{
			if (ownerId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + " WHERE \"OwnerId\"=:oid";
				cmd.Parameters.AddWithValue("oid", ownerId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor t;
					while (rdr.Read())
					{
						t = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> GetForOwner(IEnumerable<CompoundIdentity> ownerId)
		{
			if (ownerId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"OwnerId\" IN (");
				int count = 0;
				foreach (CompoundIdentity oid in ownerId)
				{
					if (!oid.IsNullOrEmpty())
					{
						where.Append('\'');
						where.Append(oid.Identity);
						where.Append("',");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override IEnumerable<Sensor> GetForSensorType(CompoundIdentity typeId)
		{
			if (typeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + " WHERE \"TypeId\"=:tid";
				cmd.Parameters.AddWithValue("tid", typeId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> permissions = new List<Sensor>();
				try
				{
					Sensor t;
					while (rdr.Read())
					{
						t = SensorBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Sensor> GetForSensorType(IEnumerable<CompoundIdentity> typeId)
		{
			if (typeId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"TypeId\" IN (");
				int count = 0;
				foreach (CompoundIdentity tid in typeId)
				{
					if (!tid.IsNullOrEmpty())
					{
						where.Append('\'');
						where.Append(tid.Identity);
						where.Append("',");
						count++;
					}
				}
				if (count > 0)
					where[where.Length - 1] = ')';
				else
					where.Append(')');

				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectSensor + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Sensor> inst = new List<Sensor>();
				try
				{
					Sensor f;
					while (rdr.Read())
					{
						f = SensorBuilder.Instance.Build(rdr);
						if (f != null)
							inst.Add(f);
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
				return inst;
			}
			return null;
		}

		public override bool Update(Sensor item)
		{
			if (item != null && this.CanUpdate(item))
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateSensor + Db.SelectById;
					cmd.Parameters.AddWithValue("iid", item.InstrumentIdentity.Identity);
					cmd.Parameters.AddWithValue("osid", item.OwningOrganizationIdentity.DataStoreIdentity);
					cmd.Parameters.AddWithValue("oid", item.OwningOrganizationIdentity.Identity);
					cmd.Parameters.AddWithValue("name", item.Name);
					cmd.Parameters.AddWithValue("tid", item.SensorTypeIdentity.Identity);
					if (string.IsNullOrEmpty(item.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", item.Description);
					if (string.IsNullOrEmpty(item.SerialNumber))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("sn", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("sn", item.SerialNumber);
					if (item.ManufacturerId.IsNullOrEmpty())
					{
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("msid", NpgsqlTypes.NpgsqlDbType.Uuid));
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("mid", NpgsqlTypes.NpgsqlDbType.Uuid));
					}
					else
					{
						cmd.Parameters.AddWithValue("msid", item.ManufacturerId.DataStoreIdentity);
						cmd.Parameters.AddWithValue("mid", item.ManufacturerId.Identity);
					}
					cmd.Parameters.AddWithValue("id", item.Identity.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgSensorProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
