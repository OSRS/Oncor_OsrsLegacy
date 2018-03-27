using Osrs.Data;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.SensorsAndInstruments;
using System;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Startup())
            {
                Exit();
                return; //quick exit
            }

            //setup our default user to use for all testing
            LocalSystemUser usr = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
            UserSecurityContext context = new UserSecurityContext(usr);

            //Install(context); //this only needs to be run once, then commented out


            if (!StartLocal())
            {
                Exit();
                return; //quick exit
            }

			TestInstrumentTypes(context);
			TestInstrumentFamilies(context);
			TestInstruments(context);
			TestSensorTypes(context);
			TestSensors(context);
        }

		static void TestInstrumentTypes(UserSecurityContext context)
		{
			InstrumentTypeProviderBase prov = InstrumentManager.Instance.GetInstrumentTypeProvider(context);
			CompoundIdentity fid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());

			Console.WriteLine("Testing Instrument Type");

			var instType = prov.Create("Shovel", fid);
			if (instType != null)
				Console.WriteLine("Create Instrument Type: " + instType.Identity.Identity.ToString());
			else
				Console.WriteLine("Failed to Create Instrument Type");

			instType.Description = "Shovels Poo";
			bool updated = prov.Update(instType);
			if (updated)
				Console.WriteLine("Updated Instrument Type");
			else
				Console.WriteLine("Failed to Update Instrument Type");

			var retrievedType = prov.Get(instType.Identity);
			if (retrievedType != null)
				Console.WriteLine("Get Instrument Type: " + retrievedType.Name + "\t" + retrievedType.Description);
			else
				Console.WriteLine("Failed to Retrieve Instrument Type");

			bool deleted = prov.Delete(instType.Identity);
			if (deleted)
				Console.WriteLine("Deleted Instrument Type");
			else
				Console.WriteLine("Failed to Delete Instrument Type");

			Console.WriteLine("Instrument Type Test Complete!");

			return;
		}

		static void TestInstrumentFamilies(UserSecurityContext context)
		{
			InstrumentFamilyProviderBase prov = InstrumentManager.Instance.GetInstrumentFamilyProvider(context);

			Console.WriteLine("Testing Instrument Family");

			var instFam = prov.Create("Shovels");
			if (instFam != null)
				Console.WriteLine("Create Instrument Family: " + instFam.Identity.Identity.ToString());
			else
				Console.WriteLine("Failed to Create Instrument Family");

			instFam.Description = "Shovels Poo";
			bool updated = prov.Update(instFam);
			if (updated)
				Console.WriteLine("Updated Instrument Family");
			else
				Console.WriteLine("Failed to Update Instrument Family");

			var retrievedFam = prov.Get(instFam.Identity);
			if (retrievedFam != null)
				Console.WriteLine("Get Instrument Family: " + retrievedFam.Name + "\t" + retrievedFam.Description);
			else
				Console.WriteLine("Failed to Retrieve Instrument Family");

			bool deleted = prov.Delete(instFam.Identity);
			if (deleted)
				Console.WriteLine("Deleted Instrument Family");
			else
				Console.WriteLine("Failed to Delete Instrument Family");

			Console.WriteLine("Instrument Family Test Complete!");

			return;
		}

		static void TestInstruments(UserSecurityContext context)
		{
			InstrumentProviderBase prov = InstrumentManager.Instance.GetInstrumentProvider(context);
			CompoundIdentity oid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());
			CompoundIdentity tid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());

			Console.WriteLine("Testing Instruments");

			var inst = prov.Create(oid, "Shovel", tid);
			if (inst != null)
				Console.WriteLine("Create Instrument: " + inst.Identity.Identity.ToString());
			else
				Console.WriteLine("Failed to Create Instrument");

			inst.Description = "Shovels Poo";
			bool updated = prov.Update(inst);
			if (updated)
				Console.WriteLine("Updated Instrument");
			else
				Console.WriteLine("Failed to Update Instrument");

			var retrievedInst = prov.Get(inst.Identity);
			if (retrievedInst != null)
				Console.WriteLine("Get Instrument: " + retrievedInst.Name + "\t" + retrievedInst.Description);
			else
				Console.WriteLine("Failed to Retrieve Instrument");

			bool deleted = prov.Delete(inst.Identity);
			if (deleted)
				Console.WriteLine("Deleted Instrument");
			else
				Console.WriteLine("Failed to Delete Instrument");

			Console.WriteLine("Instrument Test Complete!");

			return;
		}

		static void TestSensorTypes(UserSecurityContext context)
		{
			SensorTypeProviderBase prov = InstrumentManager.Instance.GetSensorTypeProvider(context);

			Console.WriteLine("Testing Sensor Types");

			var stype = prov.Create("Thermometer");
			if (stype != null)
				Console.WriteLine("Create Sensor Type: " + stype.Identity.Identity.ToString());
			else
				Console.WriteLine("Failed to Sensor Type");

			stype.Description = "Checks temperatures of things";
			bool updated = prov.Update(stype);
			if (updated)
				Console.WriteLine("Updated SensorType");
			else
				Console.WriteLine("Failed to Update Sensor Type");

			var retrievedType = prov.Get(stype.Identity);
			if (retrievedType != null)
				Console.WriteLine("Get Sensor Type: " + retrievedType.Name + "\t" + retrievedType.Description);
			else
				Console.WriteLine("Failed to Retrieve Sensor Type");

			bool deleted = prov.Delete(stype.Identity);
			if (deleted)
				Console.WriteLine("Deleted Sensor Type");
			else
				Console.WriteLine("Failed to Delete Sensor Type");

			Console.WriteLine("Sensor Type Test Complete!");

			return;
		}

		static void TestSensors(UserSecurityContext context)
		{
			SensorProviderBase prov = InstrumentManager.Instance.GetSensorProvider(context);
			CompoundIdentity id = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());
			CompoundIdentity iid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());
			CompoundIdentity oid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());
			CompoundIdentity tid = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());

			Console.WriteLine("Testing Sensor");

			var sens = prov.Create(id, iid, oid, "Thermometer", tid);
			if (sens != null)
				Console.WriteLine("Create Sensor: " + sens.Identity.Identity.ToString());
			else
				Console.WriteLine("Failed to Sensor");

			sens.Description = "Checks temperatures of things";
			bool updated = prov.Update(sens);
			if (updated)
				Console.WriteLine("Updated Sensor");
			else
				Console.WriteLine("Failed to Update Sensor");

			var retrievedSens = prov.Get(sens.Identity);
			if (retrievedSens != null)
				Console.WriteLine("Get Sensor: " + retrievedSens.Name + "\t" + retrievedSens.Description);
			else
				Console.WriteLine("Failed to Retrieve Sensor");

			bool deleted = prov.Delete(sens.Identity);
			if (deleted)
				Console.WriteLine("Deleted Sensor");
			else
				Console.WriteLine("Failed to Delete Sensor");

			Console.WriteLine("Sensor Test Complete!");

			return;
		}

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;

			if (!perms.Exists(InstrumentUtils.InstrumentTypeCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentType"), InstrumentUtils.InstrumentTypeCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentTypeCreatePermissionId));
			}
            if (!perms.Exists(InstrumentUtils.InstrumentTypeGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentType"), InstrumentUtils.InstrumentTypeGetPermissionId);
                Console.Write("Registering Permission: Get " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentTypeGetPermissionId));
            }
			if (!perms.Exists(InstrumentUtils.InstrumentTypeUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentType"), InstrumentUtils.InstrumentTypeUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentTypeUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentTypeDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentType"), InstrumentUtils.InstrumentTypeDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentTypeDeletePermissionId));
			}

			if (!perms.Exists(InstrumentUtils.InstrumentFamilyCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentFamilyCreatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentFamilyGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentFamilyGetPermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentFamilyUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentFamilyUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentFamilyDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentFamilyDeletePermissionId));
			}

			if (!perms.Exists(InstrumentUtils.InstrumentCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Instrument"), InstrumentUtils.InstrumentCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentCreatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Instrument"), InstrumentUtils.InstrumentGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentGetPermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Instrument"), InstrumentUtils.InstrumentUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Instrument"), InstrumentUtils.InstrumentDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentDeletePermissionId));
			}

			if (!perms.Exists(InstrumentUtils.SensorTypeCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SensorType"), InstrumentUtils.SensorTypeCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorTypeCreatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorTypeGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SensorType"), InstrumentUtils.SensorTypeGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorTypeGetPermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorTypeUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SensorType"), InstrumentUtils.SensorTypeUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorTypeUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorTypeDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SensorType"), InstrumentUtils.SensorTypeDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorTypeDeletePermissionId));
			}

			if (!perms.Exists(InstrumentUtils.SensorCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Sensor"), InstrumentUtils.SensorCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorCreatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Sensor"), InstrumentUtils.SensorGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorGetPermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Sensor"), InstrumentUtils.SensorUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.SensorDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Sensor"), InstrumentUtils.SensorDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.SensorDeletePermissionId));
			}

			if(!perms.Exists(InstrumentUtils.InstrumentKnownArchetypeCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentKnownArchetypeCreatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentKnownArchetypeGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentKnownArchetypeGetPermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentKnownArchetypeUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentKnownArchetypeUpdatePermissionId));
			}
			if (!perms.Exists(InstrumentUtils.InstrumentKnownArchetypeDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(InstrumentUtils.InstrumentKnownArchetypeDeletePermissionId));
			}
		}

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentType"), InstrumentUtils.InstrumentTypeCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentType"), InstrumentUtils.InstrumentTypeGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentType"), InstrumentUtils.InstrumentTypeUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentType"), InstrumentUtils.InstrumentTypeDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentFamily"), InstrumentUtils.InstrumentFamilyDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Instrument"), InstrumentUtils.InstrumentCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Instrument"), InstrumentUtils.InstrumentGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Instrument"), InstrumentUtils.InstrumentUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Instrument"), InstrumentUtils.InstrumentDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SensorType"), InstrumentUtils.SensorTypeCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SensorType"), InstrumentUtils.SensorTypeGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SensorType"), InstrumentUtils.SensorTypeUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SensorType"), InstrumentUtils.SensorTypeDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Sensor"), InstrumentUtils.SensorCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Sensor"), InstrumentUtils.SensorGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Sensor"), InstrumentUtils.SensorUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Sensor"), InstrumentUtils.SensorDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "InstrumentKnownArchetype"), InstrumentUtils.InstrumentKnownArchetypeDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);
		}

        static bool StartLocal()
        {
            InstrumentManager.Instance.Initialize();
            Console.WriteLine("IM state: " + InstrumentManager.Instance.State);
            InstrumentManager.Instance.Start();
            Console.WriteLine("IM state: " + InstrumentManager.Instance.State);
            if (InstrumentManager.Instance.State == Osrs.Runtime.RunState.Running)
                return true;
            return false;
        }

        static void Install(UserSecurityContext context)
        {
            if (AuthorizationManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                RegisterPermissions(context);
                Grant(context);
            }
        }

        static bool Startup()
        {
            ConfigurationManager.Instance.Bootstrap();
            ConfigurationManager.Instance.Initialize();
            ConfigurationManager.Instance.Start();
            Console.WriteLine("Config state: " + ConfigurationManager.Instance.State);
            if (ConfigurationManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            LogManager.Instance.Bootstrap();
            LogManager.Instance.Initialize();
            LogManager.Instance.Start();
            Console.WriteLine("Log state: " + LogManager.Instance.State);
            if (LogManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            AuthorizationManager.Instance.Bootstrap();
            AuthorizationManager.Instance.Initialize();
            AuthorizationManager.Instance.Start();
            Console.WriteLine("Auth state: " + AuthorizationManager.Instance.State);
            if (AuthorizationManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            return true;
        }

        static void Exit()
        {
            Console.WriteLine("ALL DONE");
            Console.ReadLine(); //prevents console from closing
        }
    }
}
