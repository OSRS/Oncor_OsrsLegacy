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
using System.Text;
using Osrs.Data;
using Osrs.Security;
using Npgsql;
using Osrs.Data.Postgres;
using Osrs.WellKnown.SensorsAndInstruments.Archetypes;
using Newtonsoft.Json.Linq;

namespace Osrs.WellKnown.SensorsAndInstruments.Providers
{
	public sealed class PgInstrumentKnownArchetypeProvider : InstrumentKnownArchetypeProviderBase
	{
		//Methods for InstrumentTypeKnownArchetypes
		public override bool AddInstrumentTypeArchetype(CompoundIdentity archetypeId, CompoundIdentity instrumentTypeId)
		{
			if (archetypeId != null && instrumentTypeId != null && this.CanCreate())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentTypeKnownArchetype;
					cmd.Parameters.AddWithValue("itid", instrumentTypeId.Identity);
					cmd.Parameters.AddWithValue("aid", archetypeId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override bool RemoveInstrumentTypeArchetype(CompoundIdentity archetypeId, CompoundIdentity instrumentTypeId)
		{
			if (archetypeId != null && instrumentTypeId != null && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteInstrumentTypeKnownArchetype;
					cmd.Parameters.AddWithValue("itid", instrumentTypeId.Identity);
					cmd.Parameters.AddWithValue("aid", archetypeId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override bool RemoveArchetype(CompoundIdentity archetypeId)
		{
			if (archetypeId != null && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = "DELETE FROM oncor.\"InstrumentTypeKnownArchetypes\" WHERE \"ArchetypeId\"=:aid";
					cmd.Parameters.AddWithValue("aid", archetypeId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override bool RemoveInstrumentType(CompoundIdentity instrumentTypeId)
		{
			if (instrumentTypeId != null && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = "DELETE FROM oncor.\"InstrumentTypeKnownArchetypes\" WHERE \"InstrumentTypeId\"=:itid";
					cmd.Parameters.AddWithValue("itid", instrumentTypeId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override IEnumerable<CompoundIdentity> ArchetypesForInstrumentType(CompoundIdentity instrumentTypeId)
		{
			if (instrumentTypeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = "SELECT \"ArchetypeId\" FROM oncor.\"InstrumentTypeKnownArchetypes\" WHERE \"InstrumentTypeId\"=:itid";
				cmd.Parameters.AddWithValue("itid", instrumentTypeId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<CompoundIdentity> permissions = new List<CompoundIdentity>();
				try
				{
					CompoundIdentity t;
					while (rdr.Read())
					{
						t = new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(rdr, 0));
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

		public override IEnumerable<CompoundIdentity> InstrumentTypesForArchetype(CompoundIdentity archetypeId)
		{
			if (archetypeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = "SELECT \"InstrumentTypeId\" FROM oncor.\"InstrumentTypeKnownArchetypes\" WHERE \"ArchetypeId\"=:aid";
				cmd.Parameters.AddWithValue("aid", archetypeId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<CompoundIdentity> permissions = new List<CompoundIdentity>();
				try
				{
					CompoundIdentity t;
					while (rdr.Read())
					{
						t = new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(rdr, 0));
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

		/// <summary>
		/// Tuple Order: InstrumentTypeId, ArchetypeId
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<Tuple<CompoundIdentity, CompoundIdentity>> GetInstrumentTypeKnownArchetypes()
		{
			//Dsid for all known archetypes
			Guid archetypeDsid = new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}");
			if (this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentTypeKnownArchetype;
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				List<Tuple<CompoundIdentity, CompoundIdentity>> perms = new List<Tuple<CompoundIdentity, CompoundIdentity>>();
				try
				{
					CompoundIdentity inst;
					CompoundIdentity arch;
					while (rdr.Read())
					{
						inst = new CompoundIdentity(Db.DataStoreIdentity, DbReaderUtils.GetGuid(rdr, 0));
						arch = new CompoundIdentity(archetypeDsid, DbReaderUtils.GetGuid(rdr, 1));
						if (inst != null && arch != null)
							perms.Add(new Tuple<CompoundIdentity, CompoundIdentity>(inst, arch));
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
				return perms;
			}
			return null;
		}

		//Methods for InstrumentArchetypeInstances
		//TODO -- Ask what default value to use for archetypes
		public override SimpleTrapDredge AddSimpleTrapDredge(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanCreate())
			{
				try
				{
					SimpleTrapDredge n = new SimpleTrapDredge(instrumentId, 0);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentArchetypeInstance;
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					cmd.Parameters.AddWithValue("aid", n.Identity.Identity);
					cmd.Parameters.AddWithValue("data", Db.ToJson(n).ToString());
					Db.ExecuteNonQuery(cmd);
					return n;
				}
				catch
				{ }
			}
			return null;
		}

		public override StandardMeshNet AddStandardMeshNet(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanCreate())
			{
				try
				{
					StandardMeshNet n = new StandardMeshNet(instrumentId, 0, 0, 0);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentArchetypeInstance;
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					cmd.Parameters.AddWithValue("aid", n.Identity.Identity);
					cmd.Parameters.AddWithValue("data", Db.ToJson(n).ToString());
					Db.ExecuteNonQuery(cmd);
					return n;
				}
				catch
				{ }
			}
			return null;
		}

		public override StandardPlanktonNet AddStandardPlanktonNet(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanCreate())
			{
				try
				{
					StandardPlanktonNet n = new StandardPlanktonNet(instrumentId, 0, 0, 0);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentArchetypeInstance;
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					cmd.Parameters.AddWithValue("aid", n.Identity.Identity);
					cmd.Parameters.AddWithValue("data", Db.ToJson(n).ToString());
					Db.ExecuteNonQuery(cmd);
					return n;
				}
				catch
				{ }
			}
			return null;
		}

		public override WingedBagNet AddWingedBagNet(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanCreate())
			{
				try
				{
					WingedBagNet n = new WingedBagNet(instrumentId, 0, 0, 0, 0);
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.InsertInstrumentArchetypeInstance;
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					cmd.Parameters.AddWithValue("aid", n.Identity.Identity);
					cmd.Parameters.AddWithValue("data", Db.ToJson(n).ToString());
					Db.ExecuteNonQuery(cmd);
					return n;
				}
				catch
				{ }
			}
			return null;
		}

		public override bool Delete(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanDelete())
			{
				try
				{
					NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
					cmd.CommandText = Db.DeleteInstrumentArchetypeInstance;
					cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
					Db.ExecuteNonQuery(cmd);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		public override bool Exists(CompoundIdentity instrumentId, CompoundIdentity archetypeId)
		{
			if (instrumentId != null && archetypeId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.CountInstrumentArchetypeInstance + " WHERE \"InstrumentId\"=:iid AND \"ArchetypeId\"=:aid";
				cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
				cmd.Parameters.AddWithValue("aid", archetypeId.Identity);
				return Db.Exists(cmd);
			}
			return false;
		}

		public override IArchetype Get(CompoundIdentity instrumentId)
		{
			if (instrumentId != null && this.CanGet())
			{
				NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
				cmd.CommandText = Db.SelectInstrumentArchetypeInstance + " WHERE \"InstrumentId\"=:iid";
				cmd.Parameters.AddWithValue("iid", instrumentId.Identity);
				NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
				if (rdr != null)
				{
					while (rdr.Read())
					{
						string archetype = this.GetArchetypeType(new CompoundIdentity(new Guid("{5F297502-B620-42BF-80BC-A4AF5A597267}"), DbReaderUtils.GetGuid(rdr, 1)));
						string s = DbReaderUtils.GetString(rdr, 2);
						JToken t = JRaw.Parse(s);
						if (t is JObject)
						{
							JObject a = (JObject)t;
							switch (archetype)
							{
								case "SimpleTrapDredge":
									return Db.ToSimpleTrapDredge(a);
								case "StandardMeshNet":
									return Db.ToStandardMeshNet(a);
								case "StandardPlanktonNet":
									return Db.ToStandardPlanktonNet(a);
								case "WingedBagNet":
									return Db.ToWingedBagNet(a);
							}
						}
					}
				}
			}
			return null;
		}

		public override bool Update(IArchetype a)
		{
			if(a != null && this.CanUpdate())
			{
				try
				{
					string archetype = this.GetArchetypeType(a.Identity);
					if (!string.IsNullOrEmpty(archetype))
					{
						NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
						cmd.CommandText = Db.UpdateInstrumentArchetypeInstance + " WHERE \"InstrumentId\"=:iid AND \"ArchetypeId\"=:aid";
						switch (archetype)
						{
							case "SimpleTrapDredge":
								SimpleTrapDredge std = (SimpleTrapDredge)a;
								cmd.Parameters.AddWithValue("data", Db.ToJson(std).ToString());
								cmd.Parameters.AddWithValue("iid", std.InstrumentId.Identity);
								cmd.Parameters.AddWithValue("aid", std.Identity.Identity);
								break;
							case "StandardMeshNet":
								StandardMeshNet smn = (StandardMeshNet)a;
								cmd.Parameters.AddWithValue("data", Db.ToJson(smn).ToString());
								cmd.Parameters.AddWithValue("iid", smn.InstrumentId.Identity);
								cmd.Parameters.AddWithValue("aid", smn.Identity.Identity);
								break;
							case "StandardPlanktonNet":
								StandardPlanktonNet spn = (StandardPlanktonNet)a;
								cmd.Parameters.AddWithValue("data", Db.ToJson(spn).ToString());
								cmd.Parameters.AddWithValue("iid", spn.InstrumentId.Identity);
								cmd.Parameters.AddWithValue("aid", spn.Identity.Identity);
								break;
							case "WingedBagNet":
								WingedBagNet wbn = (WingedBagNet)a;
								cmd.Parameters.AddWithValue("data", Db.ToJson(wbn).ToString());
								cmd.Parameters.AddWithValue("iid", wbn.InstrumentId.Identity);
								cmd.Parameters.AddWithValue("aid", wbn.Identity.Identity);
								break;
						}
						Db.ExecuteNonQuery(cmd);
						return true;
					}
				}
				catch { }
			}
			return false;
		}

		internal PgInstrumentKnownArchetypeProvider(UserSecurityContext context) : base(context)
		{ }
	}
}
