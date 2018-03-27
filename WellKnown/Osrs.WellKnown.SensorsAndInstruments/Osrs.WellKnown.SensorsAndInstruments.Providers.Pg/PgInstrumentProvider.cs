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
	public sealed class PgInstrumentProvider : InstrumentProviderBase
	{
		public override bool CanDelete(Instrument org)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(Instrument org)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override Instrument Create(CompoundIdentity ownerId, string name, CompoundIdentity typeId, string description, string serialNumber, CompoundIdentity manufId)
		{
			if (!string.IsNullOrEmpty(name) && ownerId != null && typeId != null && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrument;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("osid", ownerId.DataStoreIdentity);
					cmd.Parameters.AddWithValue("owner", ownerId.Identity);
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
					return new Instrument(new CompoundIdentity(Db.DataStoreIdentity, id), ownerId, name, typeId, description, serialNumber, manufId);
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
					InstrumentKnownArchetypeProviderBase archprov = InstrumentManager.Instance.GetInstrumentKnownArchetypeProvider(this.Context);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteInstrument;
					cmd.Parameters.AddWithValue("id", id.Identity);
					Db.ExecuteNonQuery(cmd);
					archprov.Delete(id);
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
				cmd.CommandText = Db.CountInstrument + Db.SelectById;
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
				cmd.CommandText = Db.CountInstrument + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);

			}
			return false;
		}

		public override IEnumerable<Instrument> Get()
		{
			if (this.CanGet())
				return new Enumerable<Instrument>(new EnumerableCommand<Instrument>(InstrumentBuilder.Instance, Db.SelectInstrument, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<Instrument> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectInstrument + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> permissions = new List<Instrument>();
				try
				{
					Instrument f;
					while (rdr.Read())
					{
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override Instrument Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrument + Db.SelectById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				Instrument f = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> Get(IEnumerable<CompoundIdentity> ids)
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
				cmd.CommandText = Db.SelectInstrument + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> inst = new List<Instrument>();
				try
				{
					Instrument f;
					while (rdr.Read())
					{
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForInstrumentType(CompoundIdentity typeId)
		{
			if (typeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrument + " WHERE \"TypeId\"=:tid";
				cmd.Parameters.AddWithValue("tid", typeId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> permissions = new List<Instrument>();
				try
				{
					Instrument t;
					while (rdr.Read())
					{
						t = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForInstrumentType(IEnumerable<CompoundIdentity> typeId)
		{
			if (typeId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"TypeId\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in typeId)
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
				cmd.CommandText = Db.SelectInstrument + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> inst = new List<Instrument>();
				try
				{
					Instrument f;
					while (rdr.Read())
					{
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForManufacturer(CompoundIdentity manufId)
		{
			if (manufId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrument + " WHERE \"ManufacturerId\"=:mid";
				cmd.Parameters.AddWithValue("mid", manufId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> permissions = new List<Instrument>();
				try
				{
					Instrument t;
					while (rdr.Read())
					{
						t = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForManufacturer(IEnumerable<CompoundIdentity> manufId)
		{
			if (manufId != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"ManufacturerId\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in manufId)
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
				cmd.CommandText = Db.SelectInstrument + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> inst = new List<Instrument>();
				try
				{
					Instrument f;
					while (rdr.Read())
					{
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForOwner(CompoundIdentity ownerId)
		{
			if (ownerId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrument + " WHERE \"Owner\"=:oid";
				cmd.Parameters.AddWithValue("oid", ownerId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> permissions = new List<Instrument>();
				try
				{
					Instrument t;
					while (rdr.Read())
					{
						t = InstrumentBuilder.Instance.Build(rdr);
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

		public override IEnumerable<Instrument> GetForOwner(IEnumerable<CompoundIdentity> ownerId)
		{
			if (ownerId != null && this.CanGet())
			{ 
				StringBuilder where = new StringBuilder(" WHERE \"Owner\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in ownerId)
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
				cmd.CommandText = Db.SelectInstrument + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Instrument> inst = new List<Instrument>();
				try
				{
					Instrument f;
					while (rdr.Read())
					{
						f = InstrumentBuilder.Instance.Build(rdr);
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

		public override bool Update(Instrument org)
		{
			if (org != null && this.CanUpdate(org))
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateInstrument + Db.SelectById;
					cmd.Parameters.AddWithValue("osid", org.OwningOrganizationIdentity.DataStoreIdentity);
					cmd.Parameters.AddWithValue("owner", org.OwningOrganizationIdentity.Identity);
					cmd.Parameters.AddWithValue("name", org.Name);
					cmd.Parameters.AddWithValue("tid", org.InstrumentTypeIdentity.Identity);
					if (string.IsNullOrEmpty(org.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", org.Description);
					if (string.IsNullOrEmpty(org.SerialNumber))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("sn", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("sn", org.SerialNumber);
					if (org.ManufacturerId.IsNullOrEmpty())
					{
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("msid", NpgsqlTypes.NpgsqlDbType.Uuid));
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("mid", NpgsqlTypes.NpgsqlDbType.Uuid));
					}
					else
					{
						cmd.Parameters.AddWithValue("msid", org.ManufacturerId.DataStoreIdentity);
						cmd.Parameters.AddWithValue("mid", org.ManufacturerId.Identity);
					}
					cmd.Parameters.AddWithValue("id", org.Identity.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgInstrumentProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
