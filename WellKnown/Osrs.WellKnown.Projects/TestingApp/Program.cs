using Osrs.Data;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Security;
using Osrs.Security.Authorization;
using Osrs.WellKnown.Projects;
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

            ProjectManager.Instance.Initialize();
            Console.WriteLine("Projects state: " + ProjectManager.Instance.State);
            ProjectManager.Instance.Start();
            Console.WriteLine("Projects state: " + ProjectManager.Instance.State);

            if (ProjectManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                IProjectStatusTypeProvider statProv = ProjectManager.Instance.GetStatusTypeProvider(context);
                if (statProv != null)
                {
                    IEnumerable<ProjectStatusType> typs = statProv.Get("Active");
                    ProjectStatusType activeStatus = null;
                    if (typs!=null)
                    {
                        foreach (ProjectStatusType cur in typs)
                            activeStatus = cur;
                    }
                    if (activeStatus==null)
                    {
                        activeStatus = statProv.Create("Active", "An active project");
                    }


                    IProjectProvider prov = ProjectManager.Instance.GetProvider(context);
                    if (prov != null)
                    {
                        Console.WriteLine("Creating projects");
                        CreateProjs(prov, activeStatus);

                        IEnumerable<Project> projs = prov.Get();
                        foreach (Project p in projs)
                        {
                            Console.WriteLine(p.Name);
                        }
                    }
                }
            }

            Console.WriteLine("ALL COMPLETE - Enter to exit");
            Console.ReadLine();
        }

        static void CreateProjs(IProjectProvider prov, ProjectStatusType activeStatus)
        {
            Guid sysId = new Guid("{5914629d-dd2d-4f1f-a06f-1b199fe19b37}");
            Guid pnlId = new Guid("{f9e1d49f-0b91-41cc-a88f-a24afa1a669e}");
            Guid aceId = new Guid("{6ca626ef-aab6-4746-8f18-7d97097055df}");

            CompoundIdentity pnnlId = new CompoundIdentity(sysId, pnlId);
            CompoundIdentity usaceId = new CompoundIdentity(sysId, aceId);

            string orgName;

            orgName = "Columbia River System Project";
            if (!prov.Exists(orgName))
            {
                Project org = prov.Create(orgName, usaceId);
                if (org != null)
                {
                    orgName = "Columbia River System Monitoring";
                    org = prov.Create(orgName, usaceId, org);

                    if (org != null)
                    {
                        orgName = "PNNL USACE Columbia River Monitoring Support";
                        prov.Create(orgName, pnnlId, org);
                    }

                    if (!prov.Exists(orgName))
                        Console.WriteLine("Exists returns false after creating");
                }
            }

            if (activeStatus!=null)
            {
                orgName = "Columbia River System Project";
                IEnumerable<Project> projs = prov.Get(orgName);
                Project org = null;
                foreach (Project cur in projs)
                {
                    org = cur;
                    break;
                }
                org.Affiliates.Add(pnnlId);
                org.Affiliates.Add(usaceId);
                prov.Update(org);

                prov.AddInfo(new ProjectInformation(org.Identity, Guid.NewGuid(), "Test info", null, null));
                prov.AddStatus(new ProjectStatus(org.Identity, Guid.NewGuid(), activeStatus.Identity, "Test status", DateTime.UtcNow));
            }
        }

        static void RegisterPermissions(UserSecurityContext context)
        {
            IPermissionProvider perms = AuthorizationManager.Instance.GetPermissionProvider(context);
            Permission p;

            if (!perms.Exists(ProjectUtils.ProjectCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Project"), ProjectUtils.ProjectCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectCreatePermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Project"), ProjectUtils.ProjectDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectDeletePermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Project"), ProjectUtils.ProjectGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectGetPermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Project"), ProjectUtils.ProjectUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectUpdatePermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectStatusTypeCreatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeCreatePermissionId);
                Console.Write("Registering Permission: Create " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectStatusTypeCreatePermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectStatusTypeDeletePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeDeletePermissionId);
                Console.Write("Registering Permission: Delete " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectStatusTypeDeletePermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectStatusTypeGetPermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeGetPermissionId);
                Console.Write("Registering Permission: Retrive " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectStatusTypeGetPermissionId));
            }

            if (!perms.Exists(ProjectUtils.ProjectStatusTypeUpdatePermissionId))
            {
                p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeUpdatePermissionId);
                Console.Write("Registering Permission: Update " + p.Name + " ");
                perms.RegisterPermission(p);
                Console.WriteLine(perms.Exists(ProjectUtils.ProjectStatusTypeUpdatePermissionId));
            }
        }

        static void Grant(UserSecurityContext context)
        {
            IRoleProvider perms = AuthorizationManager.Instance.GetRoleProvider(context);
            Permission p;
            Role r = perms.Get(SecurityUtils.AdminRole);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "Project"), ProjectUtils.ProjectCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "Project"), ProjectUtils.ProjectDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "Project"), ProjectUtils.ProjectGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "Project"), ProjectUtils.ProjectUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Create, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeCreatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Delete, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeDeletePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeGetPermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);

            p = new Permission(PermissionUtils.PermissionName(OperationType.Update, "ProjectStatusType"), ProjectUtils.ProjectStatusTypeUpdatePermissionId);
            Console.WriteLine("Granting Permission: " + p.Name);
            perms.AddToRole(r, p);
        }
    }
}
