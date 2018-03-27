using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.Security.Identity;
using System;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManager.Instance.Bootstrap();
            ConfigurationManager.Instance.Initialize();
            ConfigurationManager.Instance.Start();
            Console.WriteLine("Config: " + ConfigurationManager.Instance.State.ToString());

            LogManager.Instance.Bootstrap();
            LogManager.Instance.Initialize();
            LogManager.Instance.Start();

            Console.WriteLine("Log: " + LogManager.Instance.State.ToString());

            AuthorizationManager.Instance.Bootstrap();
            AuthorizationManager.Instance.Initialize();
            AuthorizationManager.Instance.Start();
            Console.WriteLine("Auth: " + AuthorizationManager.Instance.State.ToString());

            LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
            UserSecurityContext ctx = new UserSecurityContext(u);
            RegisterPerms(ctx);

            Console.WriteLine("Bootstrapping");
            IdentityManager.Instance.Bootstrap();
            Console.WriteLine("State: " + IdentityManager.Instance.State.ToString());
            Console.WriteLine("Initializing");
            IdentityManager.Instance.Initialize();
            Console.WriteLine("State: " + IdentityManager.Instance.State.ToString());
            Console.WriteLine("Starting");
            IdentityManager.Instance.Start();
            Console.WriteLine("State: " + IdentityManager.Instance.State.ToString());


            if (IdentityManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                IIdentityProvider perms = IdentityManager.Instance.GetProvider(ctx);
                Console.WriteLine("AdminUser Exists: " + perms.Exists(SecurityUtils.AdminIdentity));
                Console.WriteLine("CanCreate: " + perms.CanCreate());
            }

            Console.WriteLine("Done, enter to exit");
            Console.ReadLine();
        }

        static void RegisterPerms(UserSecurityContext ctx)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(ctx);
            Permission p;
            if (!perms.Exists(IdentityUtils.CreatePermisionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Permission"), IdentityUtils.CreatePermisionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(IdentityUtils.CreatePermisionId));
            }
            if (!perms.Exists(IdentityUtils.DeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Permission"), IdentityUtils.DeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(IdentityUtils.CreatePermisionId));
            }
            if (!perms.Exists(IdentityUtils.GetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Permission"), IdentityUtils.GetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(IdentityUtils.CreatePermisionId));
            }
            if (!perms.Exists(IdentityUtils.UpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Permission"), IdentityUtils.UpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(IdentityUtils.CreatePermisionId));
            }
        }
    }
}
