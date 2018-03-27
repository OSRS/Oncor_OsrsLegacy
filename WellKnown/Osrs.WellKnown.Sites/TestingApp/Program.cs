using Osrs.Data;
using Osrs.Numerics.Spatial.Geometry;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.Sites;
using System;
using System.Collections.Generic;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CompoundIdentity pnnlId = new CompoundIdentity(new Guid("5914629d-dd2d-4f1f-a06f-1b199fe19b37"), new Guid("f9e1d49f-0b91-41cc-a88f-a24afa1a669e"));

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

            SiteManager.Instance.Initialize();
            Console.WriteLine("Sites state: " + SiteManager.Instance.State);
            SiteManager.Instance.Start();
            Console.WriteLine("Sites state: " + SiteManager.Instance.State);

            if (SiteManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                SiteProviderBase prov = SiteManager.Instance.GetSiteProvider(context);
                if (prov != null)
                {
                    //CreateSites(context, pnnlId);

                    foreach (Site o in prov.Get())
                    {
                        Console.WriteLine(o.Name);
                        if (o.Name.Contains("2"))
                        {
                            UpdateSite(prov, o);
                        }
                    }
                }
            }

            Console.WriteLine("ALL DONE");
            Console.ReadLine();
        }

        static void UpdateSite(SiteProviderBase prov, Site org)
        {
            Console.WriteLine("Updating "+org.Name);
            org.Name = org.Name + " newName";

            GeometryFactory2Double fact = GeometryFactory2Double.Instance;

            org.Location = fact.ConstructRing(new Point2<double>[] { fact.ConstructPoint(22.45, 33.33), fact.ConstructPoint(23.45, 33.33), fact.ConstructPoint(22.45, 35.33) });
            org.LocationMark = fact.ConstructPoint(22.45, 33.33);

            prov.Update(org);
        }

        static void CreateSites(UserSecurityContext context, CompoundIdentity oid)
        {
            SiteProviderBase prov = SiteManager.Instance.GetSiteProvider(context);
            SiteAliasSchemeProviderBase schProv = SiteManager.Instance.GetSiteAliasSchemeProvider(context);
            SiteAliasProviderBase alProv = SiteManager.Instance.GetSiteAliasProvider(context);
            string orgName;
            Site org;
            SiteAliasScheme scheme1;
            SiteAliasScheme scheme2;

            scheme1 = MakeOrg(schProv, "PNNL Aliases scheme 1", oid);
            if (scheme1==null)
            {
                Console.WriteLine("Failed to make/get scheme 1");
                return;
            }
            scheme2 = MakeOrg(schProv, "PNNL Aliases scheme 2", oid);
            if (scheme2 == null)
            {
                Console.WriteLine("Failed to make/get scheme 2");
                return;
            }

            orgName = "My site 1";
            org = MakeOrg(prov, orgName, oid);
            if (org != null)
            {
                MakeOrg(alProv, scheme1, org, "My site 1 alias on scheme 1");
                MakeOrg(alProv, scheme2, org, "My site 1 alias on scheme 2");
            }

            orgName = "My site 2";
            org = MakeOrg(prov, orgName, oid);
            if (org != null)
            {
                MakeOrg(alProv, scheme1, org, "My site 2 alias 1 on scheme 1");
                MakeOrg(alProv, scheme1, org, "My site 2 alias 2 on scheme 1");
                MakeOrg(alProv, scheme1, org, "My site 2 alias 3 on scheme 1");
                MakeOrg(alProv, scheme2, org, "My site 2 alias 1 on scheme 2");
                MakeOrg(alProv, scheme2, org, "My site 2 alias 2 on scheme 2");
                MakeOrg(alProv, scheme2, org, "My site 2 alias 3 on scheme 2");
            }

            orgName = "My site 3";
            org = MakeOrg(prov, orgName, oid);
            if (org != null)
            {
                MakeOrg(alProv, scheme1, org, "My site 3 alias 1 on scheme 1");
                MakeOrg(alProv, scheme1, org, "My site 3 alias 2 on scheme 1");
                MakeOrg(alProv, scheme1, org, "My site 3 alias 3 on scheme 1");
                MakeOrg(alProv, scheme2, org, "My site 3 alias 1 on scheme 2");
                MakeOrg(alProv, scheme2, org, "My site 3 alias 2 on scheme 2");
                MakeOrg(alProv, scheme2, org, "My site 3 alias 3 on scheme 2");
            }

            orgName = "My site 4";
            org = MakeOrg(prov, orgName, oid);
            if (org != null)
            {
                MakeOrg(alProv, scheme1, org, "My site 4 alias on scheme 1");
                MakeOrg(alProv, scheme2, org, "My site 4 alias on scheme 2");
            }
        }

        static SiteAlias MakeOrg(SiteAliasProviderBase prov, SiteAliasScheme scheme, Site org, string orgName)
        {
            SiteAlias sch = null;
            if (!prov.Exists(scheme, orgName))
            {
                sch = prov.Create(scheme, org, orgName);
                if (sch != null)
                    Console.WriteLine("Created Alias: For: " + org.Name + " Alias: " + sch.Name + " In: " + scheme.Name);
                else
                    Console.WriteLine("Failed to create org alias");
            }
            else
            {
                IEnumerable<SiteAlias> orgs = prov.Get(scheme, orgName);
                if (orgs != null)
                {
                    foreach (SiteAlias o in orgs)
                    {
                        if (o.SiteEquals(org))
                        {
                            sch = o;
                            Console.WriteLine("Fetched Alias: For: " + org.Name + " Alias: " + sch.Name + " In: " + scheme.Name);
                            break;
                        }
                    }
                }
            }
            return sch;
        }

        static SiteAliasScheme MakeOrg(SiteAliasSchemeProviderBase prov, string orgName, CompoundIdentity oid)
        {
            SiteAliasScheme sch = null;
            if (!prov.Exists(oid, orgName))
            {
                sch = prov.Create(oid, orgName);
                if (sch != null)
                    Console.WriteLine("Create Alias Scheme: For: " + oid + " Scheme Name: " + sch.Name);
                else
                    Console.WriteLine("Failed to create org scheme");
            }
            else
            {
                IEnumerable<SiteAliasScheme> orgs = prov.GetByOwner(oid, orgName);
                if (orgs != null)
                {
                    foreach (SiteAliasScheme o in orgs)
                    {
                        sch = o;
                        Console.WriteLine("Fetched Alias Scheme: For: " + oid + " Scheme Name: " + sch.Name);
                        break;
                    }
                }
            }
            return sch;
        }

        static Site MakeOrg(SiteProviderBase prov, string orgName, CompoundIdentity oid)
        {
            Site org = null;
            if (!prov.Exists(orgName))
            {
                org = prov.Create(oid, orgName);
                if (org != null)
                    Console.WriteLine("Created Org: " + org.Name);
                else
                    Console.WriteLine("Failed to create org");
            }
            else
            {
                IEnumerable<Site> orgs = prov.GetByOwner(oid);
                if (orgs != null)
                {
                    foreach (Site o in orgs)
                    {
                        if (orgName.Equals(o.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            org = o;
                            Console.WriteLine("Fetched Site: " + org.Name);
                            UpdateSite(prov, org);
                            break;
                        }
                    }
                }
            }
            return org;
        }

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;
            if (!perms.Exists(SiteUtils.SiteAliasCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAlias"), SiteUtils.SiteAliasCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasCreatePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAlias"), SiteUtils.SiteAliasDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasDeletePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAlias"), SiteUtils.SiteAliasGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasGetPermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAlias"), SiteUtils.SiteAliasUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasUpdatePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasSchemeCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasSchemeCreatePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasSchemeDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasSchemeDeletePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasSchemeGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasSchemeGetPermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteAliasSchemeUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteAliasSchemeUpdatePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Site"), SiteUtils.SiteCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteCreatePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Site"), SiteUtils.SiteDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteDeletePermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Site"), SiteUtils.SiteGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteGetPermissionId));
            }

            if (!perms.Exists(SiteUtils.SiteUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Site"), SiteUtils.SiteUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(SiteUtils.SiteUpdatePermissionId));
            }
        }

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAlias"), SiteUtils.SiteAliasCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAlias"), SiteUtils.SiteAliasDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAlias"), SiteUtils.SiteAliasGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAlias"), SiteUtils.SiteAliasUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SiteAliasScheme"), SiteUtils.SiteAliasSchemeUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Site"), SiteUtils.SiteCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Site"), SiteUtils.SiteDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Site"), SiteUtils.SiteGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Site"), SiteUtils.SiteUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
        }
    }
}
