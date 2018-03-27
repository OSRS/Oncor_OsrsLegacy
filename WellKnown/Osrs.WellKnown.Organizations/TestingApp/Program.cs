using Osrs.Data;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.OrganizationHierarchies;
using Osrs.WellKnown.Organizations;
using System;
using System.Collections.Generic;

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

            OrganizationManager.Instance.Initialize();
            Console.WriteLine("Orgs state: " + OrganizationManager.Instance.State);
            OrganizationManager.Instance.Start();
            Console.WriteLine("Orgs state: " + OrganizationManager.Instance.State);

            if (OrganizationManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                OrganizationProviderBase prov = OrganizationManager.Instance.GetOrganizationProvider(context);
                if (prov!=null)
                {
                    //CreateOrgs(context);


                    //Console.WriteLine("Getting all orgs");
                    //foreach (Organization o in prov.Get())
                    //{
                    //    Console.WriteLine(o.Name);
                    //}
                    //Console.WriteLine("Getting subset orgs");
                    //CompoundIdentity[] sample = new CompoundIdentity[] { new CompoundIdentity(new Guid("5914629d-dd2d-4f1f-a06f-1b199fe19b37"), new Guid("469cc38d-5de0-470b-920d-1387c1d73b1a")), new CompoundIdentity(new Guid("5914629d-dd2d-4f1f-a06f-1b199fe19b37"), new Guid("6ca626ef-aab6-4746-8f18-7d97097055df")) };
                    //foreach (Organization o in prov.Get(sample))
                    //{
                    //    Console.WriteLine(o.Name);
                    //}
                }

                BuildTestCTE(context);
            }

            Console.WriteLine("ALL DONE");
            Console.ReadLine();
        }

        static void BuildTestCTE(UserSecurityContext context)
        {
            OrganizationHierarchyManager.Instance.Initialize();
            Console.WriteLine("OHM state: " + OrganizationHierarchyManager.Instance.State);
            OrganizationHierarchyManager.Instance.Start();
            Console.WriteLine("OHM state: " + OrganizationHierarchyManager.Instance.State);

            if (OrganizationHierarchyManager.Instance.State!= Osrs.Runtime.RunState.Running)
            {
                Console.WriteLine("Cannot test OHM - not running");
                return;
            }
            CompoundIdentity systemId = new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid());
            IEnumerable<Organization> orgs = OrganizationManager.Instance.GetOrganizationProvider(context).Get("Pacific Northwest National Lab");
            Organization owner=null;
            foreach (Organization cur in orgs)
            {
                owner = cur;
                break;
            }

            OrganizationHierarchyProviderBase prov = OrganizationHierarchyManager.Instance.GetProvider(context);
            OrganizationHierarchy hier=null;
            if (prov.Exists("temp"))
            {
                hier = prov.Get("temp");
                if (hier != null)
                    Console.WriteLine("Fetched hierarchy");
            }

            if (hier == null)
            {
                hier = prov.Create("temp", owner);
                if (hier != null)
                    Console.WriteLine("Created hierarchy");
            }

            if (hier != null)
            {
                List<CompoundIdentity> idList = new List<CompoundIdentity>();

                for (int i = 0; i <= 100; i++)
                {
                    idList.Add(new CompoundIdentity(Guid.NewGuid(), Guid.NewGuid()));
                }

                for (int i = 0; i < 10; i++)
                {
                    hier.Add(idList[i], idList[i + 10]);
                    hier.Add(idList[i + 10], idList[i + 20]);
                    hier.Add(idList[i + 20], idList[i + 30]);
                    hier.Add(idList[i + 30], idList[i + 40]);
                    hier.Add(idList[i + 40], idList[i + 50]);
                    hier.Add(idList[i + 50], idList[i + 60]);
                    hier.Add(idList[i + 60], idList[i + 70]);
                    hier.Add(idList[i + 70], idList[i + 80]);
                    hier.Add(idList[i + 80], idList[i + 90]);
                }

                //Console.WriteLine("Targeting org: 0403e3d2-b8a2-42cf-80fc-848fcf86aaad,b32cad24-9338-45f4-8d99-bd1aeb43b804");
                Console.WriteLine("Targeting org: " + idList[30].DataStoreIdentity.ToString() + "," + idList[30].Identity.ToString());
                Console.WriteLine("Getting children - no limit specified");
                IEnumerable<CompoundIdentity> orgIds = hier.GetChildrenIds(idList[30]);
                //IEnumerable<CompoundIdentity> orgIds = hier.GetChildrenIds(new CompoundIdentity(new Guid("0403e3d2-b8a2-42cf-80fc-848fcf86aaad"), new Guid("b32cad24-9338-45f4-8d99-bd1aeb43b804")));
                int ct = 0;
                foreach(CompoundIdentity curId in orgIds)
                {
                    //Console.Write(curId.DataStoreIdentity.ToString() + "," + curId.Identity.ToString()+" ");
                    ct++;
                }
                Console.WriteLine();
                Console.WriteLine("Got " + ct + " items");

                Console.WriteLine("Getting children - max limit specified");
                orgIds = hier.GetChildrenIds(idList[30], true);
                //orgIds = hier.GetChildrenIds(new CompoundIdentity(new Guid("0403e3d2-b8a2-42cf-80fc-848fcf86aaad"), new Guid("b32cad24-9338-45f4-8d99-bd1aeb43b804")), true);
                ct = 0;
                foreach (CompoundIdentity curId in orgIds)
                {
                    //Console.Write(curId.DataStoreIdentity.ToString() + "," + curId.Identity.ToString()+" ");
                    ct++;
                }
                Console.WriteLine();
                Console.WriteLine("Got " + ct + " items");


                if (prov.Delete(hier))
                    Console.WriteLine("Deleted hierarchy");
                else
                    Console.WriteLine("Failed deleting hierarchy");
            }
            else
                Console.WriteLine("Failed to create hierarchy");
        }

        static void CreateOrgs(UserSecurityContext context)
        {
            Console.WriteLine("Creating orgs");
            OrganizationProviderBase prov = OrganizationManager.Instance.GetOrganizationProvider(context);
            OrganizationAliasSchemeProviderBase schProv = OrganizationManager.Instance.GetOrganizationAliasSchemeProvider(context);
            OrganizationAliasProviderBase alProv = OrganizationManager.Instance.GetOrganizationAliasProvider(context);
            string orgName;
            Organization org;
            OrganizationAliasScheme scheme;

            orgName = "Pacific Northwest National Lab";
            org = MakeOrg(prov, orgName);
            if (org!=null)
            {
                scheme = MakeOrg(schProv, org, "PNNL Aliases");
                if (scheme!=null)
                {
                    MakeOrg(alProv, scheme, org, "PNNL");
                }
            }

            orgName = "US Department of Energy";
            org = MakeOrg(prov, orgName);
            if (org != null)
            {
                scheme = MakeOrg(schProv, org, "US DOE Aliases");
                if (scheme != null)
                {
                    MakeOrg(alProv, scheme, org, "US DOE");
                    MakeOrg(alProv, scheme, org, "DOE");
                    MakeOrg(alProv, scheme, org, "Department of Energy");
                }
            }

            orgName = "US Army Corps of Engineers";
            org = MakeOrg(prov, orgName);
            if (org != null)
            {
                scheme = MakeOrg(schProv, org, "USACE Aliases");
                if (scheme != null)
                {
                    MakeOrg(alProv, scheme, org, "USACE");
                    MakeOrg(alProv, scheme, org, "ACOE");
                    MakeOrg(alProv, scheme, org, "Army Corps of Engineers");
                }
            }

            orgName = "Bonneville Power Authority";
            org = MakeOrg(prov, orgName);
            if (org != null)
            {
                scheme = MakeOrg(schProv, org, "BPA Aliases");
                if (scheme != null)
                {
                    MakeOrg(alProv, scheme, org, "BPA");
                }
            }
        }

        static OrganizationAlias MakeOrg(OrganizationAliasProviderBase prov, OrganizationAliasScheme scheme, Organization org, string orgName)
        {
            OrganizationAlias sch = null;
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
                IEnumerable<OrganizationAlias> orgs = prov.Get(scheme, orgName);
                if (orgs != null)
                {
                    foreach (OrganizationAlias o in orgs)
                    {
                        if (o.OrganizationEquals(org))
                        {
                            sch = o;
                            break;
                        }
                    }
                }
            }
            return sch;
        }

        static OrganizationAliasScheme MakeOrg(OrganizationAliasSchemeProviderBase prov, Organization org, string orgName)
        {
            OrganizationAliasScheme sch = null;
            if (!prov.Exists(org, orgName))
            {
                sch = prov.Create(org, orgName);
                if (sch != null)
                    Console.WriteLine("Create Alias Scheme: For: " + org.Name + " Scheme Name: " + sch.Name);
                else
                    Console.WriteLine("Failed to create org scheme");
            }
            else
            {
                IEnumerable<OrganizationAliasScheme> orgs = prov.Get(org, orgName);
                if (orgs != null)
                {
                    foreach (OrganizationAliasScheme o in orgs)
                    {
                        sch = o;
                        break;
                    }
                }
            }
            return sch;
        }

        static Organization MakeOrg(OrganizationProviderBase prov, string orgName)
        {
            Organization org=null;
            if (!prov.Exists(orgName))
            {
                org = prov.Create(orgName);
                if (org!=null)
                    Console.WriteLine("Created Org: " + org.Name);
                else
                    Console.WriteLine("Failed to create org");
            }
            else
            {
                Console.Write("Getting org");
                IEnumerable<Organization> orgs = prov.Get(orgName);
                if (orgs != null)
                {
                    foreach (Organization o in orgs)
                    {
                        org = o;
                        Console.WriteLine(" success");
                        break;
                    }
                    Console.WriteLine(" failed - empty");
                }
                else
                    Console.WriteLine(" failed - null");
            }
            return org;
        }

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyCreatePermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyDeletePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyDeletePermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyGetPermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyGetPermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipAddPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipAddPermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipAddPermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipGetPermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipGetPermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipMovePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipMovePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipMovePermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipRemovePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipRemovePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyMembershipRemovePermissionId));
            }
            if (!perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyUpdatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationHierarchyUtils.OrganizationHierarchyUpdatePermissionId));
            }


            if (!perms.Exists(OrganizationUtils.OrganizationAliasCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAlias"), OrganizationUtils.OrganizationAliasCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasCreatePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAlias"), OrganizationUtils.OrganizationAliasDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasDeletePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAlias"), OrganizationUtils.OrganizationAliasGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasGetPermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAlias"), OrganizationUtils.OrganizationAliasUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasUpdatePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasSchemeCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasSchemeCreatePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasSchemeDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasSchemeDeletePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasSchemeGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasSchemeGetPermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationAliasSchemeUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationAliasSchemeUpdatePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Organization"), OrganizationUtils.OrganizationCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationCreatePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Organization"), OrganizationUtils.OrganizationDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationDeletePermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Organization"), OrganizationUtils.OrganizationGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationGetPermissionId));
            }

            if (!perms.Exists(OrganizationUtils.OrganizationUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Organization"), OrganizationUtils.OrganizationUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(OrganizationUtils.OrganizationUpdatePermissionId));
            }
        }

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchy"), OrganizationHierarchyUtils.OrganizationHierarchyUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipAddPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipMovePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationHierarchyMembership"), OrganizationHierarchyUtils.OrganizationHierarchyMembershipRemovePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAlias"), OrganizationUtils.OrganizationAliasCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAlias"), OrganizationUtils.OrganizationAliasDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAlias"), OrganizationUtils.OrganizationAliasGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAlias"), OrganizationUtils.OrganizationAliasUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "OrganizationAliasScheme"), OrganizationUtils.OrganizationAliasSchemeUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Organization"), OrganizationUtils.OrganizationCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Organization"), OrganizationUtils.OrganizationDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Organization"), OrganizationUtils.OrganizationGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Organization"), OrganizationUtils.OrganizationUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
        }
    }
}
