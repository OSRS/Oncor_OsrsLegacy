//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Osrs.Reflection;
using Osrs.Runtime;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using System;

namespace Osrs.Security.Authorization
{
    public sealed class AuthorizationManager : NonSubclassableSingletonBase<AuthorizationManager>, ISystemModule
    {
        private static readonly LocalSystemUser user = new LocalSystemUser(new Guid("{7E11CB03-E7F9-4138-AA79-FF1A2D0A518C}"), NameReflectionUtils.GetName(typeof(AuthorizationManager)), UserState.Active);

        private LogProviderBase logger;
        private PermissionProviderFactory permissionFactory;
        private RoleProviderFactory roleFactory;

        public RunState State
        {
            get;
            private set;
        }

        public IRoleProvider GetRoleProvider()
        {
            if (this.State == RunState.Running)
                return this.roleFactory.GetProvider();
            else if (this.State == RunState.Initializing && this.roleFactory != null)
                return this.roleFactory.GetProvider();
            return null;
        }

        public IRoleProvider GetRoleProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.roleFactory.GetProvider(context);
            else if (this.State == RunState.Initializing && this.roleFactory != null)
                return this.roleFactory.GetProvider(context);
            return null;
        }

        public IPermissionProvider GetPermissionProvider()
        {
            if (this.State == RunState.Running)
                return this.permissionFactory.GetProvider();
            else if (this.State == RunState.Initializing && this.permissionFactory != null)
                return this.permissionFactory.GetProvider();
            return null;
        }

        public IPermissionProvider GetPermissionProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.permissionFactory.GetProvider(context);
            else if (this.State == RunState.Initializing && this.permissionFactory!=null)
                return this.permissionFactory.GetProvider(context);
            return null;
        }

        public void Pause()
        {
            this.Stop();
        }

        public void Resume()
        {
            this.Start();
        }

        public void Start()
        {
            lock (instance)
            {
                if (RuntimeUtils.Startable(this.State))
                {
                    string meth = "Start";
                    this.State = RunState.Starting;
                    Log(meth, LogLevel.Info, "Called");
                    this.State = RunState.Running;
                }
            }
        }

        public void Stop()
        {
            lock (instance)
            {
                if (RuntimeUtils.Stoppable(this.State))
                {
                    string meth = "Stop";
                    this.State = RunState.Stopping;
                    Log(meth, LogLevel.Info, "Called");
                    this.State = RunState.Stopped;
                }
            }
        }

        public void Initialize()
        {
            lock (instance)
            {
                if (this.State == RunState.Bootstrapped)
                {
                    this.State = RunState.Initializing;
                    if (this.permissionFactory.Initialize() && this.roleFactory.Initialize())
                    {
                        this.State = RunState.Initialized;
                        return;
                    }
                    this.State = RunState.FailedInitializing;
                }
            }
        }

        public void Bootstrap()
        {
            lock (instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State))
                {
                    string meth = "Bootstrap";
                    this.State = RunState.Bootstrapping;
                    this.logger = LogManager.Instance.GetProvider(typeof(AuthorizationManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(AuthorizationManager), "roleProvider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    AuthorizationConfiguration iConfig = new AuthorizationConfiguration();
                                    iConfig.RoleFactoryTypeName = typeName;

                                    param = config.Get(typeof(AuthorizationManager), "permissionProvider");
                                    if (param != null)
                                    {
                                        tName = param.Value as string;
                                        if (!string.IsNullOrEmpty(tName))
                                        {
                                            typeName = TypeNameReference.Parse(tName);
                                            if (typeName != null)
                                            {
                                                iConfig.PermissionFactoryTypeName = typeName;
                                                Bootstrap(iConfig);
                                                return;
                                            }
                                            else
                                                Log(meth, LogLevel.Error, "Failed to parse permission provider param value");
                                        }
                                        else
                                            Log(meth, LogLevel.Error, "Failed to get permission provider param value");
                                    }
                                    else
                                        Log(meth, LogLevel.Error, "Failed to get permission provider param");
                                }
                                else
                                    Log(meth, LogLevel.Error, "Failed to parse role provider param value");
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to get role provider param value");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to get role provider param");
                    }
                    else
                        Log(meth, LogLevel.Error, "Failed to get ConfigurationProvider");


                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        public void Bootstrap(AuthorizationConfiguration config)
        {
            lock (instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State) || this.State == RunState.Bootstrapping)
                {
                    this.State = RunState.Bootstrapping;
                    string meth = "Bootstrap";
                    if (this.logger == null) //in case we're directly bootstrapping
                    {
                        this.logger = LogManager.Instance.GetProvider(typeof(AuthorizationManager));
                        Log(meth, LogLevel.Info, "Called");
                    }

                    if (config != null)
                    {
                        TypeNameReference typ = config.PermissionFactoryTypeName;
                        if (typ != null)
                        {
                            this.permissionFactory = NameReflectionUtils.CreateInstance<PermissionProviderFactory>(typ);
                            if (this.permissionFactory != null)
                            {
                                typ = config.RoleFactoryTypeName;
                                if (typ != null)
                                {
                                    this.roleFactory = NameReflectionUtils.CreateInstance<RoleProviderFactory>(typ);
                                    if (this.roleFactory != null)
                                    {
                                        this.roleFactory.LocalContext = new UserSecurityContext(new LocalSystemUser(user.Uid, user.Name, user.UserState));
                                        this.permissionFactory.LocalContext = new UserSecurityContext(new LocalSystemUser(user.Uid, user.Name, user.UserState));
                                        Log(meth, LogLevel.Info, "Succeeded");
                                        this.State = RunState.Bootstrapped;
                                        return;
                                    }
                                    else
                                        Log(meth, LogLevel.Error, "Failed to create role factory instance");
                                }
                                else
                                    Log(meth, LogLevel.Error, "Failed to extract role typename");
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to create permission factory instance");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to extract permission typename");
                    }
                    else
                        Log(meth, LogLevel.Error, "No config provided");
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        private void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        private AuthorizationManager()
        {
        }

        private static AuthorizationManager instance = new AuthorizationManager();
        public static AuthorizationManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
