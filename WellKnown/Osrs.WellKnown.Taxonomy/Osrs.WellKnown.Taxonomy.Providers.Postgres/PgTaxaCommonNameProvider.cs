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
	public sealed class PgTaxaCommonNameProvider : TaxaCommonNameProviderBase
	{
		public override bool CanDelete(TaxaCommonName commonName)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(TaxaCommonName commonName)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override TaxaCommonName Create(string name, string description)
		{
			if(!string.IsNullOrEmpty(name) && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertCommonName;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);

					Db.ExecuteNonQuery(cmd);

					return new TaxaCommonName(new CompoundIdentity(Db.SysId, id), name, description);
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
					cmd.CommandText = Db.DeleteCommonName;
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
				cmd.CommandText = Db.CountCommonName + Db.SelectCommonNameById;
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
				cmd.CommandText = Db.CountCommonName + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<TaxaCommonName> Get(string name, StringComparison comparisonOption)
		{
			if(!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectCommonName + where;
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaCommonName> permissions = new List<TaxaCommonName>();
				try
				{
					TaxaCommonName t;
					while (rdr.Read())
					{
						t = TaxaCommonNameBuilder.Instance.Build(rdr);
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

		public override TaxaCommonName Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectCommonName + Db.SelectCommonNameById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				TaxaCommonName t = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						t = TaxaCommonNameBuilder.Instance.Build(rdr);
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

		public override bool Update(TaxaCommonName commonName)
		{
			if(commonName != null && this.CanUpdate(commonName))
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateCommonName + Db.SelectById;
					cmd.Parameters.AddWithValue("name", commonName.Name);
					if (string.IsNullOrEmpty(commonName.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", commonName.Description);
					cmd.Parameters.AddWithValue("id", commonName.Identity.Identity);
					Db.ExecuteNonQuery(cmd);

					return true;
				}
				catch
				{ }
			}
			return false;
		}

		//Methods for TaxaCommonNameTaxa
		public override IEnumerable<TaxaCommonName> GetCommonNamesByTaxa(TaxaUnit taxa)
		{
			if (taxa != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectCommonName + " WHERE \"Id\" IN (SELECT \"TaxaCommonNameId\" FROM oncor.\"TaxaCommonNamesTaxa\" WHERE \"TaxaUnitId\"=:tuid)";
				cmd.Parameters.AddWithValue("tuid", taxa.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaCommonName> commonNames = new List<TaxaCommonName>();
				try
				{
					TaxaCommonName t;
					while (rdr.Read())
					{
						t = TaxaCommonNameBuilder.Instance.Build(rdr);
						if (t != null)
							commonNames.Add(t);
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
				return commonNames;
			}
			return null;
		}
	
		public override IEnumerable<TaxaUnit> GetTaxaUnitsByCommonName(TaxaCommonName commonName)
		{
			if (commonName != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + " WHERE \"Id\" IN (SELECT \"TaxaUnitId\" FROM oncor.\"TaxaCommonNamesTaxa\" WHERE \"TaxaCommonNameId\"=:tcnid)";
				cmd.Parameters.AddWithValue("tcnid", commonName.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> units = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while (rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
						if (t != null)
							units.Add(t);
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
			return null;
		}

		public override bool Add(CompoundIdentity taxaUnitId, CompoundIdentity commonNameId)
		{
			if(!taxaUnitId.IsNullOrEmpty() && !commonNameId.IsNullOrEmpty() && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertTaxaCommonNameTaxa;
					cmd.Parameters.AddWithValue("tcnid", commonNameId.Identity);
					cmd.Parameters.AddWithValue("tuid", taxaUnitId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override bool Remove(CompoundIdentity taxaUnitId, CompoundIdentity commonNameId)
		{
			if(!taxaUnitId.IsNullOrEmpty() && commonNameId.IsNullOrEmpty() && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteTaxaCommonNameTaxa;
					cmd.Parameters.AddWithValue("tcnid", commonNameId.Identity);
					cmd.Parameters.AddWithValue("tuid", taxaUnitId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgTaxaCommonNameProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
