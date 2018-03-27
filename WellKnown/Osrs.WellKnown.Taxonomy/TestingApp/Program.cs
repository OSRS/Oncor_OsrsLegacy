using System;
using System.Collections.Generic;
using Osrs.Data;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.Taxonomy;

namespace TestingApp
{
	class Program
	{
		static void Main(string[] args)
		{
			ConfigurationManager.Instance.Bootstrap();
			ConfigurationManager.Instance.Initialize();
			ConfigurationManager.Instance.Start();
			Console.WriteLine("Config state: " + ConfigurationManager.Instance.State);

			LogManager.Instance.Bootstrap();
			LogManager.Instance.Initialize();
			LogManager.Instance.Start();
			Console.WriteLine("Log state: " + LogManager.Instance.State);

			AuthorizationManager.Instance.Bootstrap();
			AuthorizationManager.Instance.Initialize();
			AuthorizationManager.Instance.Start();
			Console.WriteLine("Auth state: " + AuthorizationManager.Instance.State);

			LocalSystemUser usr = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
			UserSecurityContext context = new UserSecurityContext(usr);

			if (AuthorizationManager.Instance.State == Osrs.Runtime.RunState.Running)
			{
				//RegisterPermissions(context);
				//Grant(context);
			}

			TaxonomyManager.Instance.Initialize();
			Console.WriteLine("Taxonomy state: " + TaxonomyManager.Instance.State);
			TaxonomyManager.Instance.Start();
			Console.WriteLine("Taxonomy state: " + TaxonomyManager.Instance.State);

			if (TaxonomyManager.Instance.State == Osrs.Runtime.RunState.Running)
			{
				TaxonomyProviderBase prov = TaxonomyManager.Instance.GetTaxonomyProvider(context);
				if (prov != null)
				{
					RetrieveTaxonomy(context, "Integrated Taxonomic Information System - ITIS");
					RetrieveTaxaUnit(context, "Leptorchestes sikorskii");
					RetrieveTaxaUnitType(context, "Kingdom");
					RetrieveTaxaDomain(context, "Animalia");
					RetrieveTaxaDomainUnitType(context);
					RetrieveTaxaCommonName(context, "FILL WITH REAL VALUE");
				}
			}

			Console.WriteLine("ALL DONE");
			Console.ReadLine();
		}

		static void RetrieveTaxonomy(UserSecurityContext context, string name)
		{
			TaxonomyProviderBase prov = TaxonomyManager.Instance.GetTaxonomyProvider(context);
			var taxonomy = prov.Get(name);
			foreach(var taxa in taxonomy)
			{
				Console.WriteLine("Taxonomy: " + taxa.Name + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve Taxonomy Successful!");
		}

		static void RetrieveTaxaUnit(UserSecurityContext context, string name)
		{
			TaxaUnitProviderBase prov = TaxonomyManager.Instance.GetTaxaUnitProvider(context);
			var taxaUnit = prov.Get(name);
			foreach (var taxa in taxaUnit)
			{
				Console.WriteLine("TaxaUnit: " + taxa.Name + ", " + taxa.Description + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve TaxaUnit Successful!");
		}

		static void RetrieveTaxaUnitType(UserSecurityContext context, string name)
		{
			TaxaUnitTypeProviderBase prov = TaxonomyManager.Instance.GetTaxaUnitTypeProvider(context);
			var taxaUnitType = prov.Get(name);
			foreach (var taxa in taxaUnitType)
			{
				Console.WriteLine("TaxaUnitType: " + taxa.Name + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve TaxaUnitType Successful!");
		}

		static void RetrieveTaxaDomain(UserSecurityContext context, string name)
		{
			TaxaDomainProviderBase prov = TaxonomyManager.Instance.GetTaxaDomainProvider(context);
			var taxaDomain = prov.Get(name);
			foreach (var taxa in taxaDomain)
			{
				Console.WriteLine("TaxaDomain: " + taxa.Name + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve TaxaDomain Successful!");
		}

		static void RetrieveTaxaDomainUnitType(UserSecurityContext context)
		{
			TaxaDomainUnitTypeProviderBase prov = TaxonomyManager.Instance.GetTaxaDomainUnitTypeProvider(context);
			var taxaDomainUnitType = prov.GetTaxaUnitTypeByDomain(
				new TaxaDomain(
					new CompoundIdentity(new Guid("E578CA70-6CEC-4961-BB43-14FD45F455BD") , new Guid("56852d24-6d70-4028-83eb-a874db291cbe")),
					new CompoundIdentity(new Guid("E578CA70-6CEC-4961-BB43-14FD45F455BD"), new Guid("7497d1c2-a83b-4975-b341-855d1317afc2")),
					"Bacteria"));
			foreach (var taxa in taxaDomainUnitType)
			{
				Console.WriteLine("TaxaUnitType: " + taxa.Name + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve TaxaDomainUnitType Successful!");
		}

		static void RetrieveTaxaCommonName(UserSecurityContext context, string name)
		{
			TaxaCommonNameProviderBase prov = TaxonomyManager.Instance.GetTaxaCommonNameProvider(context);
			var taxaCommonName = prov.Get(name);
			foreach (var taxa in taxaCommonName)
			{
				Console.WriteLine("TaxaCommonName: " + taxa.Name + ", " + taxa.Identity.Identity);
			}
			Console.WriteLine("Retrieve TaxaCommonName Successful!");
		}

		static void RegisterPermissions(UserSecurityContext context)
		{
			IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
			Permission p;

			if(!perms.Exists(TaxonomyUtils.TaxaCommonNameCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaCommonNameCreatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaCommonNameGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaCommonNameGetPermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaCommonNameUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaCommonNameUpdatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaCommonNameDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaCommonNameDeletePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaUnitCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaUnit"), TaxonomyUtils.TaxaUnitCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaUnitCreatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaUnitGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaUnit"), TaxonomyUtils.TaxaUnitGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaUnitGetPermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaUnitUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaUnit"), TaxonomyUtils.TaxaUnitUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaUnitUpdatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxaUnitDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaUnit"), TaxonomyUtils.TaxaUnitDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxaUnitDeletePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxonomyCreatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Taxonomy"), TaxonomyUtils.TaxonomyCreatePermissionId);
				Console.Write("Registering Permission: Create " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxonomyCreatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxonomyGetPermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Taxonomy"), TaxonomyUtils.TaxonomyGetPermissionId);
				Console.Write("Registering Permission: Get " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxonomyGetPermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxonomyUpdatePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Taxonomy"), TaxonomyUtils.TaxonomyUpdatePermissionId);
				Console.Write("Registering Permission: Update " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxonomyUpdatePermissionId));
			}

			if (!perms.Exists(TaxonomyUtils.TaxonomyDeletePermissionId))
			{
				p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Taxonomy"), TaxonomyUtils.TaxonomyDeletePermissionId);
				Console.Write("Registering Permission: Delete " + p.Name + " ");
				perms.RegisterPermission(p);
				Console.WriteLine(perms.Exists(TaxonomyUtils.TaxonomyDeletePermissionId));
			}
		}

		static void Grant(UserSecurityContext context)
		{
			IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
			Permission p;
			Role r = perms.Get(SecurityUtils.AdminRole);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaCommonName"), TaxonomyUtils.TaxaCommonNameDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "TaxaUnit"), TaxonomyUtils.TaxaUnitCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "TaxaUnit"), TaxonomyUtils.TaxaUnitGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "TaxaUnit"), TaxonomyUtils.TaxaUnitUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "TaxaUnit"), TaxonomyUtils.TaxaUnitDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Taxonomy"), TaxonomyUtils.TaxonomyCreatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Taxonomy"), TaxonomyUtils.TaxonomyGetPermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Taxonomy"), TaxonomyUtils.TaxonomyUpdatePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);

			p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Taxonomy"), TaxonomyUtils.TaxonomyDeletePermissionId);
			Console.WriteLine("Granting Permission: " + p.Name);
			perms.AddToRole(r, p);
		}
		
	}
}
