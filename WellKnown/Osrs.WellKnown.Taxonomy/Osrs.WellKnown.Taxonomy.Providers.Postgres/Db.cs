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

using Npgsql;
using Osrs.Data;
using System;
using System.Data;
using System.Data.Common;

namespace Osrs.WellKnown.Taxonomy.Providers
{
	internal static class Db
	{
		internal static string ConnectionString;
		internal static readonly Guid SysId = new Guid("{E578CA70-6CEC-4961-BB43-14FD45F455BD}");

		internal const string CountCommonName = "SELECT COUNT(*) FROM oncor.\"TaxaCommonNames\"";
		internal const string CountDomain = "SELECT COUNT(*) FROM oncor.\"TaxaDomains\"";
		internal const string CountDomainUnitType = "SELECT COUNT(*) FROM oncor.\"TaxaDomainUnitTypes\"";
		internal const string CountTaxaUnit = "SELECT COUNT(*) FROM oncor.\"TaxaUnits\"";
		internal const string CountTaxaUnitType = "SELECT COUNT(*) FROM oncor.\"TaxaUnitTypes\"";
		internal const string CountTaxonomy = "SELECT COUNT (*) FROM oncor.\"Taxonomies\"";

		internal const string SelectCommonName = "SELECT \"Id\", \"Name\", \"Description\" FROM oncor.\"TaxaCommonNames\"";
		internal const string SelectDomain = "SELECT \"Id\", \"Name\", \"Description\", \"TaxonomyId\" FROM oncor.\"TaxaDomains\"";
		internal const string SelectDomainUnitType = "SELECT \"TaxonDomainId\", \"TaxonUnitTypeId\", \"ParentUnitTypeId\" FROM oncor.\"TaxaDomainUnitTypes\"";
		internal const string SelectTaxaUnit = "SELECT \"Id\", \"Name\", \"Description\", \"ParentId\", \"DomainId\", \"TaxonomyId\", \"UnitTypeId\" FROM oncor.\"TaxaUnits\"";
		internal const string SelectTaxaUnitType = "SELECT \"Id\", \"Name\", \"Description\", \"TaxonomyId\" FROM oncor.\"TaxaUnitTypes\"";
		internal const string SelectTaxonomy = "SELECT \"Id\", \"Name\", \"Description\" FROM oncor.\"Taxonomies\"";

		internal const string SelectCommonNameById = " WHERE \"Id\"=:id";
		internal const string SelectDomainById =  "WHERE \"Id\"=:id";
		internal const string SelectDomainUnitTypeByDomain = " WHERE \"TaxonDomainId\"=:tdid";
		internal const string SelectDomainUnitTypeByParent = " WHERE \"ParentUnitTypeId\"=:putid";
		internal const string SelectDomainUnitTypeByTaxaUnitType = " WHERE \"TaxonUnitTypeId\"=:tutid";
		internal const string SelectTaxaUnitById = " WHERE \"Id\"=:id";
		internal const string SelectTaxaUnitByUnitType = " WHERE \"UnitTypeId\"=:utid";
		internal const string SelectTaxaUnitByDomain = " WHERE \"DomainId\"=:did";
		internal const string SelectTaxaUnitByTaxonomy = " WHERE \"TaxonomyId\"=:tid";
		internal const string SelectTaxaUnitByParent = " WHERE \"ParentId\"=:pid";
		internal const string SelectTaxaUnitTypeById = " WHERE \"Id\"=:id";
		internal const string SelectTaxonomyById = " WHERE \"Id\"=:id";
		internal const string SelectById = "WHERE \"Id\"=:id";

		internal const string InsertCommonName = "INSERT INTO oncor.\"TaxaCommonNames\"(\"Id\", \"Name\", \"Description\") VALUES (:id, :name, :desc)";
		internal const string InsertTaxaCommonNameTaxa = "INSERT INTO oncor.\"TaxaCommonNamesTaxa\"(\"TaxaCommonNameId\", \"TaxaUnitId\") VALUES (:tcnid, :tuid)";
		internal const string InsertDomain = "INSERT INTO oncor.\"TaxaDomains\"(\"Id\", \"Name\", \"Description\", \"TaxonomyId\") VALUES (:id, :name, :desc, :tid)";
		internal const string InsertDomainUnitType = "INSERT INTO oncor.\"TaxaDomainUnitTypes\"(\"TaxonDomainId\", \"TaxonUnitTypeId\", \"ParentUnitTypeId\") VALUES (:tdid, :tutid, :putid)";
		internal const string InsertTaxaUnit = "INSERT INTO oncor.\"TaxaUnits\"(\"Id\", \"Name\", \"Description\", \"ParentId\", \"DomainId\", \"TaxonomyId\", \"UnitTypeId\") VALUES (:id, :name, :desc, :pid, :did, :tid, :utid)";
		internal const string InsertTaxaUnitType = "INSERT INTO oncor.\"TaxaUnitTypes\"(\"Id\", \"Name\", \"Description\", \"TaxonomyId\") VALUES (:id, :name, :desc, :tid)";
		internal const string InsertTaxonomy = "INSERT INTO oncor.\"Taxonomies\"(\"Id\", \"Name\", \"Description\") VALUES (:id, :name, :desc)";

		internal const string UpdateCommonName = "UPDATE oncor.\"TaxaCommonNames\" SET \"Name\"=:name, \"Description\"=:desc";
		internal const string UpdateDomain = "UPDATE oncor.\"TaxaDomains\" SET \"Name\"=:name, \"Description\"=:desc, \"TaxonomyId\"=:tid";
		internal const string UpdateDomainUnitType = "UPDATE oncor.\"TaxaDomainUnitTypes\" SET \"TaxonDomainId\"=:tdid, \"TaxonUnitTypeId\"=:tutid, \"ParentUnitTypeId\"=:putid";
		internal const string UpdateTaxaUnit = "UPDATE oncor.\"TaxaUnits\" SET \"Name\"=:name, \"Description\"=:desc, \"ParentId\"=:pid, \"DomainId\"=:did, \"TaxonomyId\"=:tid, \"UnitTypeId\"=:utid";
		internal const string UpdateTaxaUnitType = "UPDATE oncor.\"TaxaUnitTypes\" SET \"Name\"=:name, \"Description\"=:desc, \"TaxonomyId\"=:tid";
		internal const string UpdateTaxonomy = "UPDATE oncor.\"Taxonomies\" SET \"Name\"=:name, \"Description\"=:desc";

		internal const string DeleteCommonName = "DELETE FROM oncor.\"TaxaCommonNames\" WHERE \"Id\"=:id";
		internal const string DeleteTaxaCommonNameTaxa = "DELETE FROM oncor.\"TaxaCommonNamesTaxa\" WHERE \"TaxaCommonNameId\"=:tcnid AND \"TaxaUnitId\"=:tuid";
		internal const string DeleteDomain = "DELETE FROM oncor.\"TaxaDomains\" WHERE \"Id\"=:id";
		internal const string DeleteDomainUnitType = "DELETE FROM oncor.\"TaxaDomainUnitTypes\" WHERE \"TaxonDomainId\"=:tdid AND \"TaxonUnitTypeId\"=:tutid AND \"ParentUnitTypeId\"=:putid";
		internal const string DeleteTaxaUnit = "DELETE FROM oncor.\"TaxaUnits\" WHERE \"Id\"=:id";
		internal const string DeleteTaxaUnitType = "DELETE FROM oncor.\"TaxaUnitTypes\" WHERE \"Id\"=:id";
		internal const string DeleteTaxonomy = "DELETE FROM oncor.\"Taxonomies\" WHERE \"Id\"=:id";

		internal static NpgsqlConnection GetCon(string conString)
		{
			try
			{
				NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder(conString);
				if (sb.Timeout == 15) //default
					sb.Timeout = 60;
				if (sb.CommandTimeout == 30) //default
					sb.CommandTimeout = 60;
				sb.Pooling = false;
				NpgsqlConnection conn = new NpgsqlConnection(sb.ToString());
				return conn;
			}
			catch
			{ }
			return null;
		}

		internal static NpgsqlCommand GetCmd(NpgsqlConnection con)
		{
			if (con == null)
				return null;
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand();
				if (cmd != null)
				{
					cmd.Connection = con;
					return cmd;
				}
			}
			catch
			{ }
			return null;
		}

		internal static NpgsqlCommand GetCmd(string conString)
		{
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand();
				cmd.Connection = GetCon(conString);
				return cmd;
			}
			catch
			{ }
			return null;
		}

		internal static int ExecuteNonQuery(NpgsqlCommand cmd)
		{
			int res = int.MinValue;
			try
			{
				if (cmd.Connection.State != ConnectionState.Open)
					cmd.Connection.Open();
				res = cmd.ExecuteNonQuery();
			}
			catch
			{ }

			try
			{
				if (cmd.Connection.State == ConnectionState.Open)
					cmd.Connection.Close();
			}
			catch
			{ }

			return res;
		}

		internal static NpgsqlDataReader ExecuteReader(NpgsqlCommand cmd)
		{
			try
			{
				if (cmd.Connection.State != ConnectionState.Open)
					cmd.Connection.Open();
				return cmd.ExecuteReader();
			}
			catch
			{ }
			return null;
		}

		internal static void Close(NpgsqlConnection con)
		{
			try
			{
				if (con != null && con.State == ConnectionState.Open)
					con.Close();
			}
			catch
			{ }
		}

		internal static void Close(NpgsqlCommand cmd)
		{
			if (cmd != null && cmd.Connection != null)
				Close(cmd.Connection);
		}

		internal static bool Exists(NpgsqlCommand cmd)
		{
			NpgsqlDataReader rdr = null;
			try
			{
				if (cmd.Connection.State != ConnectionState.Open)
					cmd.Connection.Open();
				rdr = ExecuteReader(cmd);
				rdr.Read();

				try
				{
					long ct = (long)(rdr[0]);
					if (cmd.Connection.State == System.Data.ConnectionState.Open)
						cmd.Connection.Close();

					return ct > 0L;
				}
				catch
				{ }
			}
			catch
			{
				Close(cmd);
			}
			finally
			{
				cmd.Dispose();
			}
			return false;
		}
	}

	//Field order: Id, Name, Description
	internal sealed class TaxaCommonNameBuilder : IBuilder<TaxaCommonName>
	{
		internal static readonly TaxaCommonNameBuilder Instance = new TaxaCommonNameBuilder();

		public TaxaCommonName Build(DbDataReader reader)
		{
			return new TaxaCommonName(new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), DbReaderUtils.GetString(reader, 2));
		}
	}
		 
	//Field order: Id, Name, Description, TaxonomyId
	internal sealed class TaxaDomainBuilder : IBuilder<TaxaDomain>
	{
		internal static readonly TaxaDomainBuilder Instance = new TaxaDomainBuilder();

		public TaxaDomain Build(DbDataReader reader)
		{
			return new TaxaDomain(new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 0)), new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 3)),
				DbReaderUtils.GetString(reader, 1), DbReaderUtils.GetString(reader, 2));
		}
	}

	//Field order: Id, Name, Description, ParentId, DomainId, TaxonomyId, UnitTypeId
	internal sealed class TaxaUnitBuilder : IBuilder<TaxaUnit>
	{
		internal static readonly TaxaUnitBuilder Instance = new TaxaUnitBuilder();

		public TaxaUnit Build(DbDataReader reader)
		{
			return new TaxaUnit(new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 4)),
				new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 5)), new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 6)),
					new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 3)), DbReaderUtils.GetString(reader, 2));
		}
	}

	//Field order: Id, Name, Description, TaxonomyId
	internal sealed class TaxaUnitTypeBuilder : IBuilder<TaxaUnitType>
	{
		internal static readonly TaxaUnitTypeBuilder Instance = new TaxaUnitTypeBuilder();
		
		public TaxaUnitType Build(DbDataReader reader)
		{
			return new TaxaUnitType(new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 3)),
				DbReaderUtils.GetString(reader, 2));
		}
	}

	//Field order: Id, Name, Description
	internal sealed class TaxonomyBuilder : IBuilder<Taxonomy>
	{
		internal static readonly TaxonomyBuilder Instance = new TaxonomyBuilder();

		public Taxonomy Build(DbDataReader reader)
		{
			return new Taxonomy(new CompoundIdentity(Db.SysId, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), DbReaderUtils.GetString(reader, 2));
		}
	}
}
