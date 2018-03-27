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
	public sealed class PgTaxonomyProvider : TaxonomyProviderBase
	{
		public override bool CanDelete(Taxonomy taxonomy)
		{
			return this.CanDelete(); //TODO -- add fine grained security
		}

		public override bool CanUpdate(Taxonomy taxonomy)
		{
			return this.CanUpdate(); //TODO -- add fine grained security
		}

		public override Taxonomy Create(string name, string description)
		{
			if (!string.IsNullOrEmpty(name) && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertTaxonomy;
					Guid id = Guid.NewGuid();
					cmd.Parameters.AddWithValue("id", id);
					cmd.Parameters.AddWithValue("name", name);
					if (string.IsNullOrEmpty(description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", description);

					Db.ExecuteNonQuery(cmd);

					return new Taxonomy(new CompoundIdentity(Db.SysId, id), name, description);
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
					cmd.CommandText = Db.DeleteTaxonomy;
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
				cmd.CommandText = Db.CountTaxonomy + Db.SelectTaxonomyById;
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
				cmd.CommandText = Db.CountTaxonomy + where;
				cmd.Parameters.AddWithValue("name", name);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IEnumerable<Taxonomy> Get()
		{
			if (this.CanGet())
				return new Enumerable<Taxonomy>(new EnumerableCommand<Taxonomy>(TaxonomyBuilder.Instance, Db.SelectTaxonomy, Db.ConnectionString));
			return null;
		}

		public override IEnumerable<Taxonomy> Get(string name, StringComparison comparisonOption)
		{
			if (!string.IsNullOrEmpty(name) && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				string where;
				if (comparisonOption == StringComparison.CurrentCultureIgnoreCase || comparisonOption == StringComparison.OrdinalIgnoreCase)
					where = " WHERE lower(\"Name\")=lower(:name)";
				else
					where = " WHERE \"Name\"=:name";
				cmd.CommandText = Db.SelectTaxonomy + where;
				cmd.Parameters.AddWithValue("name", name);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Taxonomy> permissions = new List<Taxonomy>();
				try
				{
					Taxonomy t;
					while (rdr.Read())
					{
						t = TaxonomyBuilder.Instance.Build(rdr);
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

		public override Taxonomy Get(CompoundIdentity id)
		{
			if (!id.IsNullOrEmpty() && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxonomy + Db.SelectTaxonomyById;
				cmd.Parameters.AddWithValue("id", id.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				Taxonomy t = null;
				if(rdr != null)
				{
					try
					{
						rdr.Read();
						t = TaxonomyBuilder.Instance.Build(rdr);
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

		public override bool Update(Taxonomy taxonomy)
		{
			if (taxonomy != null && this.CanUpdate(taxonomy))
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.UpdateTaxonomy + Db.SelectById;
					cmd.Parameters.AddWithValue("name", taxonomy.Name);
					if (string.IsNullOrEmpty(taxonomy.Description))
						cmd.Parameters.Add(NpgSqlCommandUtils.GetNullInParam("desc", NpgsqlTypes.NpgsqlDbType.Varchar));
					else
						cmd.Parameters.AddWithValue("desc", taxonomy.Description);
					cmd.Parameters.AddWithValue("id", taxonomy.Identity.Identity);
					Db.ExecuteNonQuery(cmd);

					return true;
				}
				catch
				{ }
			}
			return false;
		}

		internal PgTaxonomyProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
