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

namespace Osrs.WellKnown.Taxonomy.Providers
{
	public sealed class PgTaxaUnitTypeProvider : TaxaUnitTypeProviderBase
	{
		public override bool CanDelete(TaxaUnitType unitType)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(TaxaUnitType unitType)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override TaxaUnitType Create(string name, CompoundIdentity taxonomyId, string description)
		{
			if(!string.IsNullOrEmpty(name) && taxonomyId != null && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertTaxaUnitType;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);
					cmd.Parameters.AddWithValue("tid", taxonomyId.Identity);

					Db.ExecuteNonQuery(cmd);

					return new TaxaUnitType(new CompoundIdentity(Db.SysId, id), name, new CompoundIdentity(Db.SysId, taxonomyId.Identity), description);
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
					cmd.CommandText = Db.DeleteTaxaUnitType;
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
				cmd.CommandText = Db.CountTaxaUnitType + Db.SelectTaxaUnitTypeById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool Exists(string name, StringComparison comparisonOption)
		{
			if(!string.IsNullOrEmpty(name) && this.CanGet())
			{
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountTaxaUnitType + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<TaxaUnitType> Get()
		{
			if (this.CanGet())
				return new Enumerable<TaxaUnitType>(new EnumerableCommand<TaxaUnitType>(TaxaUnitTypeBuilder.Instance, Db.SelectTaxaUnitType, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<TaxaUnitType> Get(string name, StringComparison comparisonOption)
		{
			if(!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectTaxaUnitType + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnitType> permissions = new List<TaxaUnitType>();
				try
				{
					TaxaUnitType t;
					while (rdr.Read())
					{
						t = TaxaUnitTypeBuilder.Instance.Build(rdr);
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

		public override IEnumerable<TaxaUnitType> GetTaxaUnitTypeByTaxonomy(CompoundIdentity taxonomyId)
		{
			if (taxonomyId.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnitType + " WHERE \"TaxonomyId\"=:tid";
				cmd.Parameters.AddWithValue("tid", taxonomyId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnitType> permissions = new List<TaxaUnitType>();
				try
				{
					TaxaUnitType t;
					while (rdr.Read())
					{
						t = TaxaUnitTypeBuilder.Instance.Build(rdr);
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

		public override TaxaUnitType Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnitType + Db.SelectTaxaUnitTypeById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				TaxaUnitType t = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						t = TaxaUnitTypeBuilder.Instance.Build(rdr);
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

		public override bool Update(TaxaUnitType unitType)
		{
			if (unitType != null && this.CanUpdate(unitType))
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateTaxaUnitType + Db.SelectById;
					cmd.Parameters.AddWithValue("name", unitType.Name);
					if (string.IsNullOrEmpty(unitType.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", unitType.Description);
					cmd.Parameters.AddWithValue("tid", unitType.TaxonomyId.Identity);
					cmd.Parameters.AddWithValue("id", unitType.Identity.Identity);
					Db.ExecuteNonQuery(cmd);

					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgTaxaUnitTypeProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
