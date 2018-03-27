using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authentication;
using Osrs.Security.Authentication.Providers;
using Osrs.Security.Authorization;
using Osrs.Security.Identity;
using System;
using System.Collections.Generic;

namespace TestingApp
{
    class Program
    {
        static void DoWork(string[] args)
        {
            AuthenticationManager.Instance.Bootstrap();
            Console.WriteLine("Authent state: " + AuthenticationManager.Instance.State);
            if (AuthenticationManager.Instance.State != Osrs.Runtime.RunState.Bootstrapped)
                return;

            AuthenticationManager.Instance.Initialize();
            Console.WriteLine("Authent state: " + AuthenticationManager.Instance.State);
            if (AuthenticationManager.Instance.State != Osrs.Runtime.RunState.Initialized)
                return;

            AuthenticationManager.Instance.Start();
            Console.WriteLine("Authent state: " + AuthenticationManager.Instance.State);
            if (AuthenticationManager.Instance.State != Osrs.Runtime.RunState.Running)
                return;

            LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
            UserSecurityContext ctx = new UserSecurityContext(u);

            string myUname = "Mike_Corsello@Hotmail.com";
            IIdentityProvider accts = IdentityManager.Instance.GetProvider(ctx);
            UserIdentityBase user=null;
            if (!accts.Exists(myUname))
            {
                Console.WriteLine("Creating user account");
                user = accts.CreateUser(myUname);
            }
            else
            {
                Console.WriteLine("Fetching user account");
                IEnumerable<UserIdentityBase> users = accts.Get(myUname, UserType.Person);
                if (users!=null)
                {
                    foreach(UserIdentityBase cur in users)
                    {
                        user = cur;
                        break;
                    }
                }
            }

            if (user == null)
            {
                Console.WriteLine("Failed to get/create user");
                return;
            }


            IAuthenticationProvider provider = AuthenticationManager.Instance.GetProvider(ctx);
            UserPasswordCredential cred = new UserPasswordCredential(myUname, "Hello World");
            IUserIdentity u2 = provider.Authenticate(cred);
            if (u2 == null)
            {
                Console.WriteLine("Didn't authenticate -- adding credential");
                if (!provider.AddCredential(user, cred))
                {
                    Console.WriteLine("Failed to add credential");
                    return;
                }

                u2 = provider.Authenticate(cred);
                if (u2 == null)
                {
                    Console.WriteLine("Didn't authenticate -- giving up");
                    return;
                }
                else
                    Console.WriteLine("Authenticated second try");
            }
            else
                Console.WriteLine("Authenticated first try");

            Console.WriteLine("Replacing credential with same (should fail)");
            if (provider.ReplaceCredential(u2, cred, cred))
            {
                Console.WriteLine("Replace credential succeeded -- a failing result");
                return;
            }
            else
                Console.WriteLine("Replace credential failed -- a successful result");

            UserPasswordCredential cred2 = new UserPasswordCredential(myUname, "Alabaster Barkers 123");
            Console.WriteLine("Replacing credential with different (should succeed)");
            if (provider.ReplaceCredential(u2, cred, cred2))
                Console.WriteLine("Replace credential succeeded -- a successful result");
            else
            {
                Console.WriteLine("Replace credential failed -- a failing result");
                return;
            }

            u2 = provider.Authenticate(cred);
            if (u2 == null)
            {
                Console.WriteLine("Didn't authenticate with old credential -- successful");
                u2 = provider.Authenticate(cred2);
                if (u2 != null)
                {
                    Console.WriteLine("Authenticated with new credential -- successful");
                    return;
                }
            }
            Console.WriteLine("Password change didn't work out");
        }

        static void Main(string[] args)
        {
            if (BootUp())
            {
                Console.WriteLine("Starting work phase");
                DoWork(args);
            }
            else
                Console.WriteLine("Boot up failed, quitting");

            Console.WriteLine("ALL COMPLETE - (enter to exit)");
            Console.ReadLine();
        }

        static bool BootUp()
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

            IdentityManager.Instance.Bootstrap();
            IdentityManager.Instance.Initialize();
            IdentityManager.Instance.Start();
            Console.WriteLine("Id state: " + IdentityManager.Instance.State);

            if (IdentityManager.Instance.State != Osrs.Runtime.RunState.Running)
                return false;

            return true;
        }

        static void Hline()
        {
            Console.WriteLine("-----------------------------------------------------------");
        }

        static void HDoubleLine()
        {
            Console.WriteLine("===========================================================");
        }
    }
}
