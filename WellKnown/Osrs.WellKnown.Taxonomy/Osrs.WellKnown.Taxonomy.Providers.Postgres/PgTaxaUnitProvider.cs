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

namespace Osrs.WellKnown.Taxonomy.Providers
{
	public sealed class PgTaxaUnitProvider : TaxaUnitProviderBase
	{
		public override bool CanDelete(TaxaUnit taxaUnit)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(TaxaUnit taxaUnit)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override TaxaUnit Create(string name, CompoundIdentity domainId, CompoundIdentity taxonomyId, CompoundIdentity unitTypeId, CompoundIdentity parentId, string description)
		{
			if (!string.IsNullOrEmpty(name) && domainId != null && taxonomyId != null && unitTypeId != null && this.CanUpdate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertTaxaUnit;
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
					cmd.Parameters.AddWithValue("did", domainId);
					cmd.Parameters.AddWithValue("tid", taxonomyId);
					cmd.Parameters.AddWithValue("utid", unitTypeId);

					Db.ExecuteNonQuery(cmd);

					return new TaxaUnit(new CompoundIdentity(Db.SysId, id), name, domainId, taxonomyId, unitTypeId, parentId, description);
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
					cmd.CommandText = Db.DeleteTaxaUnit;
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
				cmd.CommandText = Db.CountTaxaUnit + Db.SelectTaxaUnitById;
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
				cmd.CommandText = Db.CountTaxaUnit + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);

			}
			return false;
		}

		public override IEnumerable<TaxaUnit> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectTaxaUnit + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> permissions = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while(rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
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

		public override TaxaUnit Get(CompoundIdentity id)
		{
			if(!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + Db.SelectTaxaUnitById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				TaxaUnit t = null;
				if(rdr != null)
				{
					try
					{
						rdr.Read();
						t = TaxaUnitBuilder.Instance.Build(rdr);
						if (cmd.Connection.State == System.Data.ConnectionState.Open)
							cmd.Connection.Close();
					}
					catch
					{ }
					finally
					{
						cmd.Dispose();
					}
					return t;
				}
			}
			return null;
		}

		public override IEnumerable<TaxaUnit> Get(IEnumerable<CompoundIdentity> taxaUnitIds)
		{
			if(taxaUnitIds != null && this.CanGet())
			{
				StringBuilder where = new StringBuilder(" WHERE \"Id\" IN (");
				int count = 0;
				foreach (CompoundIdentity id in taxaUnitIds)
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
				cmd.CommandText = Db.SelectTaxaUnit + where.ToString();
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> units = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while(rdr.Read())
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
				return units;
			}
			return null;
		}

		public override IEnumerable<TaxaUnit> GetByTaxaDomain(CompoundIdentity domainId)
		{
			if (domainId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + Db.SelectTaxaUnitByDomain;
				cmd.Parameters.AddWithValue("did", domainId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> permissions = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while (rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
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

		public override IEnumerable<TaxaUnit> GetByTaxaUnitTypeId(CompoundIdentity taxaUnitTypeId)
		{
			if (taxaUnitTypeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + Db.SelectTaxaUnitByUnitType;
				cmd.Parameters.AddWithValue("utid", taxaUnitTypeId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> permissions = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while (rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
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

		public override IEnumerable<TaxaUnit> GetByTaxonomy(CompoundIdentity taxonomyId)
		{
			if (taxonomyId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + Db.SelectTaxaUnitByTaxonomy;
				cmd.Parameters.AddWithValue("tid", taxonomyId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> permissions = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while (rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
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

		public override IEnumerable<TaxaUnit> GetChildren(CompoundIdentity parentTaxaUnitId)
		{
			if (parentTaxaUnitId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + Db.SelectTaxaUnitByParent;
				cmd.Parameters.AddWithValue("pid", parentTaxaUnitId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnit> permissions = new List<TaxaUnit>();
				try
				{
					TaxaUnit t;
					while (rdr.Read())
					{
						t = TaxaUnitBuilder.Instance.Build(rdr);
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

		public override TaxaUnit GetParent(CompoundIdentity taxaUnitId)
		{
			if (!taxaUnitId.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnit + " WHERE \"Id\"=(SELECT \"ParentId\" FROM oncor.\"TaxaUnits\" WHERE \"Id\"=:id)";
				cmd.Parameters.AddWithValue("id", taxaUnitId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				TaxaUnit t = null;
				if (rdr != null)
				{
					try
					{
						rdr.Read();
						t = TaxaUnitBuilder.Instance.Build(rdr);
						if (cmd.Connection.State == System.Data.ConnectionState.Open)
							cmd.Connection.Close();
					}
					catch
					{ }
					finally
					{
						cmd.Dispose();
					}
					return t;
				}
			}
			return null;
		}

		public override bool HasChildren(TaxaUnit taxaUnit)
		{
			if(taxaUnit != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountTaxaUnit + Db.SelectTaxaUnitByParent;
				cmd.Parameters.AddWithValue("pid", taxaUnit.Identity.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool HasParent(TaxaUnit taxaUnit)
		{
			if(taxaUnit != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountTaxaUnit + Db.SelectTaxaUnitById;
				cmd.Parameters.AddWithValue("id", taxaUnit.ParentId.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool Update(TaxaUnit taxaUnit)
		{
			if (taxaUnit != null && this.CanUpdate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateTaxaUnit + Db.SelectById;
					cmd.Parameters.AddWithValue("name", taxaUnit.Name);
					if (string.IsNullOrEmpty(taxaUnit.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", taxaUnit.Description);
					if (taxaUnit.ParentId == null)
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("pid", NpgsqlTypes.NpgsqlDbType.Uuid));
					else
						cmd.Parameters.AddWithValue("pid", taxaUnit.ParentId.Identity);
					cmd.Parameters.AddWithValue("did", taxaUnit.TaxaDomainId);
					cmd.Parameters.AddWithValue("tid", taxaUnit.TaxonomyId);
					cmd.Parameters.AddWithValue("utid", taxaUnit.TaxaUnitTypeId);
					cmd.Parameters.AddWithValue("id", taxaUnit.Identity.Identity);
					Db.ExecuteNonQuery(cmd);

					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgTaxaUnitProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
