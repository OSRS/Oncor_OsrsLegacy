using Osrs.Numerics;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.FieldActivities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            FieldActivityManager.Instance.Initialize();
            Console.WriteLine("FA state: " + FieldActivityManager.Instance.State);
            FieldActivityManager.Instance.Start();
            Console.WriteLine("FA state: " + FieldActivityManager.Instance.State);

            if (FieldActivityManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                FieldActivity fa = null;
                FieldTrip ft = null;

                FieldActivityProviderBase prov = FieldActivityManager.Instance.GetFieldActivityProvider(context);
                if (prov != null)
                {
                    Console.WriteLine("GOT PROVIDER: FAP");

                    fa = CreateFA(prov);

                    if (fa != null)
                        fa = prov.Get(fa.Identity);
                    if (fa != null)
                        Console.WriteLine("Fetched back");
                }

                FieldTripProviderBase tprov = FieldActivityManager.Instance.GetFieldTripProvider(context);
                if (tprov!=null)
                {
                    Console.WriteLine("GOT PROVIDER: FTP");
                    if (fa != null)
                    {
                        ft = CreateFT(tprov, fa);
                    }
                    else
                        Console.WriteLine("no activity, not trying trip");
                }

            }

            Console.WriteLine("ALL DONE");
            Console.ReadLine();
        }

        static FieldTrip CreateFT(FieldTripProviderBase prov, FieldActivity act)
        {
            string name = "hello field trip";
            if (prov.Exists(name))
            {
                IEnumerable<FieldTrip> acts = prov.Get(name);
                if (acts != null)
                {
                    foreach (FieldTrip cur in acts)
                    {
                        if (cur.Identity.Equals(act.Identity))
                        {
                            if (prov.Delete(cur))
                            {
                                Console.WriteLine("Exists, got and deleted");
                                break;
                            }
                            else
                                Console.WriteLine("Exists, got, but didn't delete");
                        }
                    }
                }
                else
                    Console.WriteLine("Exists and didn't get");
            }

            FieldTrip trip = prov.Create(name, act, new Osrs.Data.CompoundIdentity(Guid.NewGuid(), Guid.NewGuid()));
            if (trip != null)
            {
                Console.WriteLine("Created");
                trip.Description = "woof";
                if (prov.Update(trip))
                    Console.WriteLine("updated");
                else
                    Console.WriteLine("didn't update");
            }
            else
                Console.WriteLine("didn't create");
            return trip;
        }

        static FieldActivity CreateFA(FieldActivityProviderBase prov)
        {
            string name = "hello field activity";
            if (prov.Exists(name))
            {
                IEnumerable<FieldActivity> acts = prov.Get(name);
                if (acts != null)
                {
                    foreach (FieldActivity cur in acts)
                    {
                        if (prov.Delete(cur))
                        {
                            Console.WriteLine("Exists, got and deleted");
                            break;
                        }
                        else
                            Console.WriteLine("Exists, got, but didn't delete");
                    }
                }
                else
                    Console.WriteLine("Exists and didn't get");
            }

            FieldActivity act = prov.Create(name, new Osrs.Data.CompoundIdentity(Guid.NewGuid(), Guid.NewGuid()), new Osrs.Data.CompoundIdentity(Guid.NewGuid(), Guid.NewGuid()), new ValueRange<DateTime>(DateTime.MinValue, DateTime.UtcNow), null);
            if (act != null)
            {
                Console.WriteLine("Created");
                act.Description = "woof";
                if (prov.Update(act))
                    Console.WriteLine("updated");
                else
                    Console.WriteLine("didn't update");
            }
            else
                Console.WriteLine("didn't create");
            return act;
        }

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;

            //--field activity
            if (!perms.Exists(FieldActivityUtils.FieldActivityCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldActivity"), FieldActivityUtils.FieldActivityCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldActivityCreatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldActivityDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldActivity"), FieldActivityUtils.FieldActivityDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldActivityDeletePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldActivityGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldActivity"), FieldActivityUtils.FieldActivityGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldActivityGetPermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldActivityUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldActivity"), FieldActivityUtils.FieldActivityUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldActivityUpdatePermissionId));
            }


            //--field trip
            if (!perms.Exists(FieldActivityUtils.FieldTripCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTrip"), FieldActivityUtils.FieldTripCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTripCreatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTripDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTrip"), FieldActivityUtils.FieldTripDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTripDeletePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTripGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTrip"), FieldActivityUtils.FieldTripGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTripGetPermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTripUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTrip"), FieldActivityUtils.FieldTripUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTripUpdatePermissionId));
            }


            //--sample event
            if (!perms.Exists(FieldActivityUtils.SampleEventCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SampleEvent"), FieldActivityUtils.SampleEventCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.SampleEventCreatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.SampleEventDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SampleEvent"), FieldActivityUtils.SampleEventDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.SampleEventDeletePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.SampleEventGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SampleEvent"), FieldActivityUtils.SampleEventGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.SampleEventGetPermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.SampleEventUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SampleEvent"), FieldActivityUtils.SampleEventUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.SampleEventUpdatePermissionId));
            }


            //--field team
            if (!perms.Exists(FieldActivityUtils.FieldTeamCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeam"), FieldActivityUtils.FieldTeamCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamCreatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeam"), FieldActivityUtils.FieldTeamGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamGetPermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeam"), FieldActivityUtils.FieldTeamUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamUpdatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeam"), FieldActivityUtils.FieldTeamDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamUpdatePermissionId));
            }

            //--FT member role
            if (!perms.Exists(FieldActivityUtils.FieldTeamMemberRoleCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamMemberRoleCreatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamMemberRoleGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamMemberRoleGetPermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamMemberRoleUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamMemberRoleUpdatePermissionId));
            }

            if (!perms.Exists(FieldActivityUtils.FieldTeamMemberRoleDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(FieldActivityUtils.FieldTeamMemberRoleDeletePermissionId));
            }
    }

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

            //--field activity
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldActivity"), FieldActivityUtils.FieldActivityCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldActivity"), FieldActivityUtils.FieldActivityDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldActivity"), FieldActivityUtils.FieldActivityGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldActivity"), FieldActivityUtils.FieldActivityUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);


            //--field trip
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTrip"), FieldActivityUtils.FieldTripCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTrip"), FieldActivityUtils.FieldTripDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTrip"), FieldActivityUtils.FieldTripGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTrip"), FieldActivityUtils.FieldTripUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            //--sample event
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "SampleEvent"), FieldActivityUtils.SampleEventCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SampleEvent"), FieldActivityUtils.SampleEventDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SampleEvent"), FieldActivityUtils.SampleEventGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "SampleEvent"), FieldActivityUtils.SampleEventUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            //--field team
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeam"), FieldActivityUtils.FieldTeamCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeam"), FieldActivityUtils.FieldTeamGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeam"), FieldActivityUtils.FieldTeamUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeam"), FieldActivityUtils.FieldTeamDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);


            //--FT member role
            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
        }
    }
}
