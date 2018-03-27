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

using System;
using Osrs.Runtime;
using Osrs.Reflection;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Configuration;

namespace Osrs.Security.Authentication
{
    public sealed class AuthenticationManager : SystemModuleBase
    {
        private readonly LocalSystemUser localUser = new LocalSystemUser(new Guid("{1474F04C-9409-45C6-9045-557A5941C5C7}"), TypeNameReference.Create(typeof(AuthenticationManager)).ToString(), UserState.Active);
        private LogProviderBase logger;

        private AuthenticationProviderFactory authFactory;
        private CredentialStoreFactory credFactory;

        private AuthenticationManager() : base()
        { }

        public IAuthenticationProvider GetProvider()
        {
            return GetProvider(new UserSecurityContext(localUser));
        }

        public IAuthenticationProvider GetProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.authFactory.GetProvider(context);
            return null;
        }

        internal ICredentialStore GetCredentialProvider()
        {
            return GetCredentialProvider(new UserSecurityContext(localUser));
        }

        internal ICredentialStore GetCredentialProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.credFactory.GetProvider(context);
            return null;
        }

        protected override void BootstrapImpl()
        {
            lock (instance)
            {
                if (this.State == RunState.Created)
                {
                    string meth = "Bootstrap";
                    this.State = RunState.Bootstrapping;
                    this.logger = LogManager.Instance.GetProvider(typeof(AuthenticationManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(AuthenticationManager), "authenticationProvider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    param = config.Get(typeof(AuthenticationManager), "credentialProvider");
                                    if (param != null)
                                    {
                                        tName = param.Value as string;
                                        if (!string.IsNullOrEmpty(tName))
                                        {
                                            TypeNameReference credName = TypeNameReference.Parse(tName);
                                            if (credName != null)
                                            {
                                                this.Bootstrap(typeName, credName);
                                                return;
                                            }
                                            else
                                                Log(meth, LogLevel.Error, "Failed to parse credential provider param value");
                                        }
                                        else
                                            Log(meth, LogLevel.Error, "Failed to get credential provider param value");
                                    }
                                    else
                                        Log(meth, LogLevel.Error, "Failed to get credential provider param");
                                }
                                else
                                    Log(meth, LogLevel.Error, "Failed to parse authentication provider param value");
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to get authentication provider param value");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to get authentication provider param");
                    }
                    else
                        Log(meth, LogLevel.Error, "Failed to get ConfigurationProvider");

                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        public void Bootstrap(TypeNameReference authFactoryType, TypeNameReference credFactoryType)
        {
            lock (instance)
            {
                if (this.State == RunState.Created || this.State == RunState.Bootstrapping)
                {
                    this.State = RunState.Bootstrapping;
                    string meth = "Bootstrap";
                    if (this.logger == null) //in case we're directly bootstrapping
                    {
                        this.logger = LogManager.Instance.GetProvider(typeof(AuthenticationManager));
                        Log(meth, LogLevel.Info, "Called");
                    }

                    if (authFactoryType != null && credFactoryType!=null)
                    {
                        this.credFactory = NameReflectionUtils.CreateInstance<CredentialStoreFactory>(credFactoryType);
                        if (this.credFactory != null)
                        {
                            this.authFactory = NameReflectionUtils.CreateInstance<AuthenticationProviderFactory>(authFactoryType);
                            if (this.authFactory != null)
                            {
                                Log(meth, LogLevel.Info, "Succeeded");
                                this.State = RunState.Bootstrapped;
                                return;
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to create authentication factory instance");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to create credential factory instance");
                    }
                    else
                        Log(meth, LogLevel.Error, "No typename provided");
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        private void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        protected override void InitializeImpl()
        {
            lock (instance)
            {
                if (this.State == RunState.Bootstrapped)
                {
                    this.State = RunState.Initializing;
                    Log("Initialize", LogLevel.Info, "Called");
                    if (this.credFactory.Initialize())
                    {
                        if (this.authFactory.Initialize())
                        {
                            Log("Initialize", LogLevel.Info, "Succeeded");
                            this.State = RunState.Initialized;
                            return;
                        }
                        Log("Initialize", LogLevel.Error, "Failed, AuthenticationFactory");
                    }
                    Log("Initialize", LogLevel.Error, "Failed, CredentialFactory");
                    this.State = RunState.FailedInitializing;
                }
            }
        }

        protected override void StartImpl()
        {
            lock (instance)
            {
                if (RuntimeUtils.Startable(this.State))
                {
                    Log("Start", LogLevel.Info, "Called");
                    this.State = RunState.Running;
                }
            }
        }

        protected override void StopImpl()
        {
            lock (instance)
            {
                if (RuntimeUtils.Stoppable(this.State))
                {
                    Log("Stop", LogLevel.Info, "Called");
                    this.State = RunState.Stopped;
                }
            }
        }

        private static readonly AuthenticationManager instance = new AuthenticationManager();
        public static AuthenticationManager Instance
        {
            get { return instance; }
        }
    }
}
