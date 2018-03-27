using Osrs.Runtime;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security.Sessions;
using System;

namespace TestingApp
{
    class Program
    {
        static void DoWork(string[] args)
        {
            Console.WriteLine("PreBootstrap State: " + SessionManager.Instance.State.ToString());
            SessionManager.Instance.Bootstrap(); // new Osrs.Reflection.TypeNameReference("Osrs.Security.Sessions.Module", "Osrs.Security.Sessions.Providers.MemorySessionProviderFactory"), 0);
            Console.WriteLine("PostBootstrap State: " + SessionManager.Instance.State.ToString());
            SessionManager.Instance.Initialize();
            Console.WriteLine("PostInitialize State: " + SessionManager.Instance.State.ToString());
            SessionManager.Instance.Start();
            Console.WriteLine("PostStart State: " + SessionManager.Instance.State.ToString());

            SessionProviderBase prov = SessionManager.Instance.GetProvider();
            Console.WriteLine("Provider null: " + (prov == null));
            if (prov != null)
            {
                ModuleRuntimeSession sess = prov.Create();
                Console.WriteLine("Session null: " + (sess == null));

                Console.WriteLine("exists: " + prov.Exists(sess.SessionId));
                Console.WriteLine("extend: " + prov.Extend(sess.SessionId));
                Console.WriteLine("expire: " + prov.Expire(sess.SessionId));
                Console.WriteLine("exists: " + prov.Exists(sess.SessionId));
            }
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
