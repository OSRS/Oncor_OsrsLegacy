using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
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

            Console.WriteLine("Bootstrapping");
            AuthorizationManager.Instance.Bootstrap();
            Console.WriteLine("State: " + AuthorizationManager.Instance.State.ToString());
            Console.WriteLine("Initializing");
            AuthorizationManager.Instance.Initialize();
            Console.WriteLine("State: " + AuthorizationManager.Instance.State.ToString());
            Console.WriteLine("Starting");
            AuthorizationManager.Instance.Start();
            Console.WriteLine("State: " + AuthorizationManager.Instance.State.ToString());

            if (AuthorizationManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
                UserSecurityContext ctx = new UserSecurityContext(u);

                IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(ctx);
                string permName = PermissionUtils.PermissionName(OperationType.Manage, "Permission");
                Console.WriteLine("ManagePermissions Exists: " + perms.Exists(PermissionUtils.ManagePermissionsPermissionId));
                Console.WriteLine("CanCreate: " + perms.CanManagePermissions());

                IRoleProvider roles = AuthorizationManager.Instance.GetRoleProvider(ctx);
                Permission p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Site"), new Guid("{285890CC-C05F-43D0-B4B2-C9A8EE716BE1}"));
                Console.WriteLine("GetSitesPermission: " + roles.HasPermission(ctx.User, p));
            }

            Console.WriteLine("Done, enter to exit");
            Console.ReadLine();
        }
    }
}
