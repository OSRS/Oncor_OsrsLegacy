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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Osrs.WellKnown.SensorsAndInstruments.Providers
{
	internal static class Db
	{
		internal static string ConnectionString;
		internal static readonly Guid DataStoreIdentity = new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}");

		internal const string CountInstrument = "SELECT COUNT(*) FROM oncor.\"Instruments\"";
		internal const string CountInstrumentFamily = "SELECT COUNT(*) FROM oncor.\"InstrumentFamilies\"";
		internal const string CountInstrumentType = "SELECT COUNT(*) FROM oncor.\"InstrumentTypes\"";
		internal const string CountSensor = "SELECT COUNT(*) FROM oncor.\"Sensors\"";
		internal const string CountSensorType = "SELECT COUNT(*) FROM oncor.\"SensorTypes\"";

		internal const string SelectInstrument = "SELECT \"Id\", \"OwnerSystemId\", \"Owner\", \"Name\", \"TypeId\", \"Description\", \"SerialNumber\", \"ManufacturerSystemId\", \"ManufacturerId\" FROM oncor.\"Instruments\"";
		internal const string SelectInstrumentFamily = "SELECT \"Id\", \"Name\", \"Description\", \"ParentId\" FROM oncor.\"InstrumentFamilies\"";
		internal const string SelectInstrumentType = "SELECT \"Id\", \"Name\", \"FamilyId\", \"Description\", \"ParentId\" FROM oncor.\"InstrumentTypes\"";
		internal const string SelectSensor = "SELECT \"Id\", \"InstrumentId\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"TypeId\", \"Description\", \"SerialNumber\", \"ManufacturerSystemId\", \"ManufacturerId\" FROM oncor.\"Sensors\"";
		internal const string SelectSensorType = "SELECT \"Id\", \"Name\", \"Description\", \"ParentId\", \"SensorTypeInstrumentFamilies\" FROM oncor.\"SensorTypes\"";

		internal const string SelectById = " WHERE \"Id\"=:id";

		internal const string InsertInstrument = "INSERT INTO oncor.\"Instruments\"(\"Id\", \"OwnerSystemId\", \"Owner\", \"Name\", \"TypeId\", \"Description\", \"SerialNumber\", \"ManufacturerSystemId\", \"ManufacturerId\") VALUES (:id, :osid, :owner, :name, :tid, :desc, :sn, :msid, :mid)";
		internal const string InsertInstrumentFamily = "INSERT INTO oncor.\"InstrumentFamilies\"(\"Id\", \"Name\", \"Description\", \"ParentId\") VALUES (:id, :name, :desc, :pid)";
		internal const string InsertInstrumentType = "INSERT INTO oncor.\"InstrumentTypes\"(\"Id\", \"Name\", \"FamilyId\", \"Description\", \"ParentId\") VALUES (:id, :name, :fid, :desc, :pid)";
		internal const string InsertSensor = "INSERT INTO oncor.\"Sensors\"(\"Id\", \"InstrumentId\", \"OwnerSystemId\", \"OwnerId\", \"Name\", \"TypeId\", \"Description\", \"SerialNumber\", \"ManufacturerSystemId\", \"ManufacturerId\") VALUES (:id, :iid, :osid, :oid, :name, :tid, :desc, :sn, :msid, :mid)";
		internal const string InsertSensorType = "INSERT INTO oncor.\"SensorTypes\"(\"Id\", \"Name\", \"Description\", \"ParentId\", \"SensorTypeInstrumentFamilies\") VALUES (:id, :name, :desc, :pid, :stif)";

		internal const string UpdateInstrument = "UPDATE oncor.\"Instruments\" SET \"OwnerSystemId\"=:osid, \"Owner\"=:owner, \"Name\"=:name, \"TypeId\"=:tid, \"Description\"=:desc, \"SerialNumber\"=:sn, \"ManufacturerSystemId\"=:msid, \"ManufacturerId\"=:mid";
		internal const string UpdateInstrumentFamily = "UPDATE oncor.\"InstrumentFamilies\" SET \"Name\"=:name, \"Description\"=:desc, \"ParentId\"=:pid";
		internal const string UpdateInstrumentType = "UPDATE oncor.\"InstrumentTypes\" SET \"Name\"=:name, \"FamilyId\"=:fid, \"Description\"=:desc, \"ParentId\"=:pid";
		internal const string UpdateSensor = "UPDATE oncor.\"Sensors\" SET \"InstrumentId\"=:iid, \"OwnerSystemId\"=:osid, \"OwnerId\"=:oid, \"Name\"=:name, \"TypeId\"=:tid, \"Description\"=:desc, \"SerialNumber\"=:sn, \"ManufacturerSystemId\"=:msid, \"ManufacturerId\"=:mid";
		internal const string UpdateSensorType = "UPDATE oncor.\"SensorTypes\" SET \"Name\"=:name, \"Description\"=:desc, \"ParentId\"=:pid, \"SensorTypeInstrumentFamilies\"=:stif";

		internal const string DeleteInstrument = "DELETE FROM oncor.\"Instruments\" WHERE \"Id\"=:id";
		internal const string DeleteInstrumentFamily = "DELETE FROM oncor.\"InstrumentFamilies\" WHERE \"Id\"=:id";
		internal const string DeleteInstrumentType = "DELETE FROM oncor.\"InstrumentTypes\" WHERE \"Id\"=:id";
		internal const string DeleteSensor = "DELETE FROM oncor.\"Sensors\" WHERE \"Id\"=:id";
		internal const string DeleteSensorType = "DELETE FROM oncor.\"SensorTypes\" WHERE \"Id\"=:id";

		//InstrumentTypeKnownArchetype
		internal const string InsertInstrumentTypeKnownArchetype = "INSERT INTO oncor.\"InstrumentTypeKnownArchetypes\"(\"InstrumentTypeId\", \"ArchetypeId\") VALUES (:itid, :aid)";
		internal const string DeleteInstrumentTypeKnownArchetype = "DELETE FROM oncor.\"InstrumentTypeKnownArchetypes\" WHERE \"InstrumentTypeId\"=:itid AND \"ArchetypeId\"=:aid";
		internal const string SelectInstrumentTypeKnownArchetype = "SELECT \"InstrumentTypeId\", \"ArchetypeId\" FROM oncor.\"InstrumentTypeKnownArchetypes\"";

		//InstrumentArchetypeInstance
		internal const string CountInstrumentArchetypeInstance = "SELECT COUNT(*) FROM oncor.\"InstrumentArchetypeInstances\"";
		internal const string SelectInstrumentArchetypeInstance = "SELECT \"InstrumentId\", \"ArchetypeId\", \"Data\" FROM oncor.\"InstrumentArchetypeInstances\"";
		internal const string InsertInstrumentArchetypeInstance = "INSERT INTO oncor.\"InstrumentArchetypeInstances\"(\"InstrumentId\", \"ArchetypeId\", \"Data\") VALUES (:iid, :aid, :data)";
		internal const string UpdateInstrumentArchetypeInstance = "UPDATE oncor.\"InstrumentArchetypeInstances\" SET \"Data\"=:data";
		internal const string DeleteInstrumentArchetypeInstance = "DELETE FROM oncor.\"InstrumentArchetypeInstances\" WHERE \"InstrumentId\"=:iid";

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
			catch (Exception e)
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

		internal static JArray ToJson(IEnumerable<CompoundIdentity> ids)
		{
			if (ids != null)
			{
				JArray o = new JArray();
				foreach (CompoundIdentity cur in ids)
				{
					if (cur != null)
						o.Add(ToJson(cur));
				}
				return o;
			}
			return null;
		}

		internal static JObject ToJson(CompoundIdentity id)
		{
			if (id != null)
			{
				JObject o = new JObject();
				o.Add("dsid", id.DataStoreIdentity);
				o.Add("id", id.Identity);
				return o;
			}
			return null;
		}

		internal static HashSet<CompoundIdentity> ToIds(JArray dataPayload)
		{
			if (dataPayload != null)
			{
				try
				{
					HashSet<CompoundIdentity> ids = new HashSet<CompoundIdentity>();
					CompoundIdentity item;
					foreach (JToken cur in dataPayload)
					{
						item = ToId(cur as JObject);
						if (item != null)
							ids.Add(item);
					}
					return ids;
				}
				catch
				{ }
			}
			return null;
		}

		internal static CompoundIdentity ToId(JObject ob)
		{
			if (ob != null)
			{
				if (ob["dsid"] != null && ob["id"] != null)
				{
					JToken d = ob["dsid"];
					JToken i = ob["id"];

					Guid ds;
					Guid id;

					if (Guid.TryParse(d.ToString(), out ds) && Guid.TryParse(i.ToString(), out id))
						return new CompoundIdentity(ds, id);
				}
			}
			return null;
		}

		public static CompoundIdentity ToId(JToken ob)
		{
			if (ob != null)
				return ToId(ob as JObject);
			return null;
		}

		internal static JObject ToJson(Archetypes.SimpleTrapDredge std)
		{
			if (std != null)
			{
				JObject o = new JObject();
				o.Add("InstrumentId", ToJson(std.InstrumentId));
				o.Add("OpenArea", std.OpenArea);
				return o;
			}
			return null;
		}

		internal static Archetypes.SimpleTrapDredge ToSimpleTrapDredge(JObject ob)
		{
			if (ob != null)
			{
				if (ob["InstrumentId"] != null && ob["OpenArea"] != null)
				{
					CompoundIdentity c = ToId(ob["InstrumentId"]);
					double oa = double.Parse(ob["OpenArea"].ToString());
					return new Archetypes.SimpleTrapDredge(c, oa);
				}
			}
			return null;
		}

		internal static JObject ToJson(Archetypes.StandardMeshNet smn)
		{
			if (smn != null)
			{
				JObject o = new JObject();
				o.Add("InstrumentId", ToJson(smn.InstrumentId));
				o.Add("Length", smn.Length);
				o.Add("Depth", smn.Depth);
				o.Add("MeshSize", smn.MeshSize);
				return o;
			}
			return null;
		}

		internal static Archetypes.StandardMeshNet ToStandardMeshNet(JObject ob)
		{
			if (ob != null)
			{
				if (ob["InstrumentId"] != null && ob["Length"] != null && ob["Depth"] != null && ob["MeshSize"] != null)
				{
					CompoundIdentity c = ToId(ob["InstrumentId"]);
					double l = double.Parse(ob["Length"].ToString());
					double d = double.Parse(ob["Depth"].ToString());
					double m = double.Parse(ob["MeshSize"].ToString());
					return new Archetypes.StandardMeshNet(c, l, d, m);
				}
			}
			return null;
		}

		internal static JObject ToJson(Archetypes.StandardPlanktonNet spn)
		{
			if (spn != null)
			{
				JObject o = new JObject();
				o.Add("InstrumentId", ToJson(spn.InstrumentId));
				o.Add("OpenArea", spn.OpenArea);
				o.Add("MeshSize", spn.MeshSize);
				o.Add("CodSize", spn.CodSize);
				return o;
			}
			return null;
		}

		internal static Archetypes.StandardPlanktonNet ToStandardPlanktonNet(JObject ob)
		{
			if (ob != null)
			{
				if (ob["InstrumentId"] != null && ob["OpenArea"] != null && ob["MeshSize"] != null && ob["CodSize"] != null)
				{
					CompoundIdentity c = ToId(ob["InstrumentId"]);
					double o = double.Parse(ob["OpenArea"].ToString());
					double m = double.Parse(ob["MeshSize"].ToString());
					double cs = double.Parse(ob["CodSize"].ToString());
					return new Archetypes.StandardPlanktonNet(c, o, m, cs);
				}
			}
			return null;
		}

		internal static JObject ToJson(Archetypes.WingedBagNet wbn)
		{
			if (wbn != null)
			{
				JObject o = new JObject();
				o.Add("InstrumentId", ToJson(wbn.InstrumentId));
				o.Add("Length", wbn.Length);
				o.Add("Depth", wbn.Depth);
				o.Add("MeshSizeWings", wbn.MeshSizeWings);
				o.Add("MeshSizeBag", wbn.MeshSizeBag);
				return o;
			}
			return null;
		}

		internal static Archetypes.WingedBagNet ToWingedBagNet(JObject ob)
		{
			if (ob != null)
			{
				if (ob["InstrumentId"] != null && ob["Length"] != null && ob["Depth"] != null && ob["MeshSizeWings"] != null && ob["MeshSizeBag"] != null)
				{
					CompoundIdentity c = ToId(ob["InstrumentId"]);
					double l = double.Parse(ob["Length"].ToString());
					double d = double.Parse(ob["Depth"].ToString());
					double w = double.Parse(ob["MeshSizeWings"].ToString());
					double b = double.Parse(ob["MeshSizeBag"].ToString());
					return new Archetypes.WingedBagNet(c, l, d, w, b);
				}
			}
			return null;
		}
	}

	//Field Order: Id, OwnerSystemId, Owner, Name, TypeId, Description, SerialNumber, ManufacturerSystemId, ManufacturerId
	internal sealed class InstrumentBuilder : IBuilder<Instrument>
	{
		internal static readonly InstrumentBuilder Instance = new InstrumentBuilder();
		public Instrument Build(DbDataReader reader)
		{
			CompoundIdentity manufId = null;
			Guid manufSysId = DbReaderUtils.GetGuid(reader, 7);
			if (!Guid.Empty.Equals(manufSysId))
				manufId = new CompoundIdentity(manufSysId, DbReaderUtils.GetGuid(reader, 8));

			return new Instrument(new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 0)), new CompoundIdentity(DbReaderUtils.GetGuid(reader, 1), DbReaderUtils.GetGuid(reader, 2)),
				DbReaderUtils.GetString(reader, 3), new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 4)), DbReaderUtils.GetString(reader, 5),
				DbReaderUtils.GetString(reader, 6), manufId);
		}
	}

	//Field Order: Id, Name, Description, ParentId
	internal sealed class InstrumentFamilyBuilder : IBuilder<InstrumentFamily>
	{
		internal static readonly InstrumentFamilyBuilder Instance = new InstrumentFamilyBuilder();
		public InstrumentFamily Build(DbDataReader reader)
		{
			CompoundIdentity parentId = null;
			Guid parentGuid = DbReaderUtils.GetGuid(reader, 3);
			if (!Guid.Empty.Equals(parentGuid))
				parentId = new CompoundIdentity(Db.DataStoreIdentity, parentGuid);

			return new InstrumentFamily(new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), DbReaderUtils.GetString(reader, 2), parentId);
		}
	}

	//Field Order: Id, Name, FamilyId, Description, ParentId
	internal sealed class InstrumentTypeBuilder : IBuilder<InstrumentType>
	{
		internal static readonly InstrumentTypeBuilder Instance = new InstrumentTypeBuilder();
		public InstrumentType Build(DbDataReader reader)
		{
			CompoundIdentity parentId = null;
			Guid parentGuid = DbReaderUtils.GetGuid(reader, 4);
			if (!Guid.Empty.Equals(parentGuid))
				parentId = new CompoundIdentity(Db.DataStoreIdentity, parentGuid);

			return new InstrumentType(new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), 
				new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 2)), DbReaderUtils.GetString(reader, 3), parentId);
		}
	}
	
	//Field Order: Id, InstrumentId, OwnerSystemId, OwnerId, Name, TypeId, Description, SerialNumber, ManufacturerSystemId, ManufacturerId
	internal sealed class SensorBuilder : IBuilder<Sensor>
	{
		internal static readonly SensorBuilder Instance = new SensorBuilder();
		public Sensor Build(DbDataReader reader)
		{
			CompoundIdentity manufId = null;
			Guid manufSysId = DbReaderUtils.GetGuid(reader, 8);
			if (!Guid.Empty.Equals(manufSysId))
				manufId = new CompoundIdentity(manufSysId, DbReaderUtils.GetGuid(reader, 9));

			return new Sensor(new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 0)), new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 1)),
				new CompoundIdentity(DbReaderUtils.GetGuid(reader, 2), DbReaderUtils.GetGuid(reader, 3)), DbReaderUtils.GetString(reader, 4), new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 5)),
				DbReaderUtils.GetString(reader, 6), DbReaderUtils.GetString(reader, 7), manufId);
		}
	}

	//Field Order: Id, Name, Description, ParentId, SensorTypeInstrumentFamilies
	internal sealed class SensorTypeBuilder : IBuilder<SensorType>
	{
		internal static readonly SensorTypeBuilder Instance = new SensorTypeBuilder();
		public SensorType Build(DbDataReader reader)
		{
			CompoundIdentity parentId = null;
			Guid parentGuid = DbReaderUtils.GetGuid(reader, 3);
			if (!Guid.Empty.Equals(parentGuid))
				parentId = new CompoundIdentity(Db.DataStoreIdentity, parentGuid);

			SensorType sensorType = new SensorType(new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(reader, 0)), DbReaderUtils.GetString(reader, 1), DbReaderUtils.GetString(reader, 2), parentId);

			string s = DbReaderUtils.GetString(reader, 4);
			JToken t = JRaw.Parse(s);
			if(t is JArray)
			{
				JArray a = (JArray)t;
				HashSet<CompoundIdentity> ids = Db.ToIds(a);
				foreach(CompoundIdentity id in ids)
				{
					sensorType.InstrumentFamilies.Add(id);
				}
			}

			return sensorType;
		}
	}
}
