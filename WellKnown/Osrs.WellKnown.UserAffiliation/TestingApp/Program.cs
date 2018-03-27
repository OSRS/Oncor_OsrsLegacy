using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.Security.Identity;
using Osrs.WellKnown.Organizations;
using Osrs.WellKnown.UserAffiliation;
using System;
using System.Collections.Generic;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Startup())
            {
                LocalSystemUser usr = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
                UserSecurityContext context = new UserSecurityContext(usr);

                UserAffiliationManager.Instance.Initialize();
                UserAffiliationManager.Instance.Start();
                Console.WriteLine("Affil state: " + UserAffiliationManager.Instance.State);
                if (UserAffiliationManager.Instance.State == Osrs.Runtime.RunState.Running)
                {
                    //RegisterPermissions(context);
                    //Grant(context);

                    Create(context);
                }
            }

            Console.WriteLine("ALL DONE - enter to exit");
            Console.ReadLine();
        }

        static void Create(UserSecurityContext context)
        {
            IIdentityProvider idProv = IdentityManager.Instance.GetProvider(context);
            OrganizationProviderBase orgProv = OrganizationManager.Instance.GetOrganizationProvider(context);
            UserAffiliationProviderBase prov = UserAffiliationManager.Instance.GetProvider(context);

            Organization org = orgProv.Get(new Osrs.Data.CompoundIdentity(new Guid("5914629d-dd2d-4f1f-a06f-1b199fe19b37"), new Guid("f9e1d49f-0b91-41cc-a88f-a24afa1a669e")));

            IEnumerable<UserIdentityBase> ids = idProv.Get();
            foreach(UserIdentityBase cur in ids)
            {
                if (!prov.HasAffiliation(cur, org))
                {
                    Console.WriteLine("Adding user " + cur.Name + " to org " + org.Name);
                    prov.Add(cur, org);
                    Console.WriteLine("Has affiliation? " + prov.HasAffiliation(cur, org) + ": " + cur.Name + " to " + org.Name);
                }
                else
                    Console.WriteLine("Already has affiliation " + cur.Name + " to org " + org.Name);
            }
        }

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;
            if (!perms.Exists(UserAffiliationUtils.AddPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "UserAffiliation"), UserAffiliationUtils.AddPermissionId);
                Console.Write("Registering Permission: Add " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(UserAffiliationUtils.AddPermissionId));
            }
            if (!perms.Exists(UserAffiliationUtils.GetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "UserAffiliation"), UserAffiliationUtils.GetPermissionId);
                Console.Write("Registering Permission: Get " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(UserAffiliationUtils.GetPermissionId));
            }
            if (!perms.Exists(UserAffiliationUtils.RemovePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "UserAffiliation"), UserAffiliationUtils.RemovePermissionId);
                Console.Write("Registering Permission: Remove " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(UserAffiliationUtils.RemovePermissionId));
            }
        }

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "UserAffiliation"), UserAffiliationUtils.AddPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "UserAffiliation"), UserAffiliationUtils.GetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "UserAffiliation"), UserAffiliationUtils.RemovePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
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

            IdentityManager.Instance.Bootstrap();
            IdentityManager.Instance.Initialize();
            IdentityManager.Instance.Start();
            Console.WriteLine("Identity state: " + IdentityManager.Instance.State);
            if (IdentityManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            AuthorizationManager.Instance.Bootstrap();
            AuthorizationManager.Instance.Initialize();
            AuthorizationManager.Instance.Start();
            Console.WriteLine("Auth state: " + AuthorizationManager.Instance.State);
            if (AuthorizationManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            OrganizationManager.Instance.Initialize();
            OrganizationManager.Instance.Start();
            Console.WriteLine("Orgs state: " + OrganizationManager.Instance.State);
            if (OrganizationManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            return true;
        }
    }
}
