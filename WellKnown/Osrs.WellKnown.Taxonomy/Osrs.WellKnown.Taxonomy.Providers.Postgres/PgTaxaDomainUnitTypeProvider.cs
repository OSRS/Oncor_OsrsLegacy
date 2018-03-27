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
	public sealed class PgTaxaDomainUnitTypeProvider : TaxaDomainUnitTypeProviderBase
	{
		public override bool AddChild(CompoundIdentity domainId, CompoundIdentity parentId, CompoundIdentity childId)
		{
			if (!domainId.IsNullOrEmpty() && !parentId.IsNullOrEmpty() && !childId.IsNullOrEmpty()&& this.CanCreate())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.InsertDomainUnitType;
				cmd.Parameters.AddWithValue("tdid", domainId.Identity);
				cmd.Parameters.AddWithValue("tutid", childId.Identity);
				cmd.Parameters.AddWithValue("putid", parentId.Identity);
				Db.ExecuteNonQuery(cmd);
				return true;
			}
			return false;
		}

		public override IEnumerable<TaxaUnitType> GetChildren(TaxaDomain domain, TaxaUnitType parent)
		{
			if (domain != null && parent != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnitType + " WHERE \"Id\"=(SELECT \"TaxonUnitTypeId\" FROM oncor.\"TaxaDomainUnitTypes\" WHERE \"TaxonDomainId\"=:tdid AND \"ParentUnitTypeId\"=:putid)";
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("putid", parent.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnitType> unitTypes = new List<TaxaUnitType>();
				TaxaUnitType t = null;
				if(rdr != null)
				{
					try
					{
						while(rdr.Read())
						{
							t = TaxaUnitTypeBuilder.Instance.Build(rdr);
							if (t != null)
								unitTypes.Add(t);
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
				return unitTypes;
			}
			return null;
		}

		public override IEnumerable<TaxaUnitType> GetDescendants(TaxaDomain domain, TaxaUnitType taxaUnitType)
		{
			if (domain != null && taxaUnitType != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = "SELECT oncor.\"taxadomainunittypegetdescendants(:did, :tutid)";
				cmd.Parameters.AddWithValue("did", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("tutid", taxaUnitType.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnitType> unitTypes = new List<TaxaUnitType>();
				TaxaUnitType t = null;
				if (rdr != null)
				{
					try
					{
						while (rdr.Read())
						{
							t = TaxaUnitTypeBuilder.Instance.Build(rdr);
							if (t != null)
								unitTypes.Add(t);
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
				return unitTypes;
			}
			return null;
		}

		public override TaxaUnitType GetParent(TaxaDomain domain, TaxaUnitType child)
		{
			if(domain != null && child != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnitType + " WHERE \"Id\"=(SELECT \"ParentUnitTypeId\" FROM oncor.\"TaxaDomainUnitTypes\" WHERE \"TaxonDomainId\"=:tdid AND \"TaxonUnitTypeId\"=:tutid)";
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("tutid", child.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				TaxaUnitType t = null;
				if(rdr != null)
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

		public override IEnumerable<TaxaUnitType> GetTaxaUnitTypeByDomain(TaxaDomain domain)
		{
			if(domain != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectTaxaUnitType + " WHERE \"Id\" IN (SELECT \"TaxonUnitTypeId\" FROM oncor.\"TaxaDomainUnitTypes\" WHERE \"TaxonDomainId\"=:tdid)";
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<TaxaUnitType> unitTypes = new List<TaxaUnitType>();
				TaxaUnitType t = null;
				if (rdr != null)
				{
					try
					{
						while(rdr.Read())
						{
							t = TaxaUnitTypeBuilder.Instance.Build(rdr);
							if (t != null)
								unitTypes.Add(t);
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
				return unitTypes;
			}
			return null;
		}

		public override bool IsAncestorOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child)
		{
			if (domain != null && parent != null && child != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = "SELECT oncor\"taxadomainunittypeisancestors\"(:tdid, :cutid, putid)";
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("cutid", child.Identity.Identity);
				cmd.Parameters.AddWithValue("putid", parent.Identity.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool IsChildOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child)
		{
			if (domain != null && parent != null && child != null && parent.TaxonomyId.Identity == child.TaxonomyId.Identity)
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountDomainUnitType + Db.SelectDomainUnitTypeByDomain + " AND" + Db.SelectDomainUnitTypeByTaxaUnitType + " AND" + Db.SelectDomainUnitTypeByParent;
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("tutid", child.Identity.Identity);
				cmd.Parameters.AddWithValue("putid", parent.Identity.Identity);
				return Db.Exists(cmd);
			} 
			return false;
		}

		public override bool IsDescendantOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child)
		{
			if (domain != null && parent != null && child != null)
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = "SELECT oncor\"taxadomainunittypeisdescendant\"(:tdid, :cutid, :putid)";
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("tutid", child.Identity.Identity);
				cmd.Parameters.AddWithValue("putid", parent.Identity.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool IsParentOf(TaxaDomain domain, TaxaUnitType parent, TaxaUnitType child)
		{
			if (domain != null && parent != null && child != null && this.CanGet() && parent.TaxonomyId.Identity == child.TaxonomyId.Identity)
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountDomainUnitType + Db.SelectDomainUnitTypeByDomain + " AND" + Db.SelectDomainUnitTypeByTaxaUnitType + " AND" + Db.SelectDomainUnitTypeByParent;
				cmd.Parameters.AddWithValue("tdid", domain.Identity.Identity);
				cmd.Parameters.AddWithValue("tutid", child.Identity.Identity);
				cmd.Parameters.AddWithValue("putid", parent.Identity.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override bool RemoveChild(CompoundIdentity domainId, CompoundIdentity parentId, CompoundIdentity childId)
		{
			if (!domainId.IsNullOrEmpty() && !parentId.IsNullOrEmpty() && !childId.IsNullOrEmpty() && this.CanDelete())
			{
				NpgsqlCommand delCmd = Db.GetCmd(Db.ConnectionString);
				delCmd.CommandText = Db.DeleteDomainUnitType;
				delCmd.Parameters.AddWithValue("tdid", domainId.Identity);
				delCmd.Parameters.AddWithValue("tutid", childId.Identity);
				delCmd.Parameters.AddWithValue("putid", parentId.Identity);
				Db.ExecuteNonQuery(delCmd);

				NpgsqlCommand updateCmd = Db.GetCmd(Db.ConnectionString);
				updateCmd.CommandText = "UPDATE oncor.\"TaxaDomainUnitTypes\" SET \"ParentUnitTypeId\"=:nputid" + Db.SelectDomainUnitTypeByDomain + " AND" + Db.SelectDomainUnitTypeByParent;
				updateCmd.Parameters.AddWithValue("nputid", parentId.Identity);
				updateCmd.Parameters.AddWithValue("tdid", domainId.Identity);
				updateCmd.Parameters.AddWithValue("putid", childId.Identity);
				Db.ExecuteNonQuery(updateCmd);

				return true;
			}
			return false;
		}

		internal PgTaxaDomainUnitTypeProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
