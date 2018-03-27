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
using Osrs.Security;
using System;

namespace Osrs.WellKnown.Sites
{
    public sealed class SiteManager : ModuleBase
    {
        private static readonly LocalSystemUser user = new LocalSystemUser(new Guid("{6EEE4100-27D1-4358-9040-51F873612B53}"), NameReflectionUtils.GetName(typeof(SiteManager)), UserState.Active);
        private LogProviderBase logger;
        private SiteProviderFactoryBase factory;

        public SiteProviderBase GetSiteProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteProvider(new UserSecurityContext(null));
            return null;
        }
        public SiteProviderBase GetSiteProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteProvider(context);
            return null;
        }

        public SiteAliasSchemeProviderBase GetSiteAliasSchemeProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteAliasSchemeProvider(new UserSecurityContext(null));
            return null;
        }
        public SiteAliasSchemeProviderBase GetSiteAliasSchemeProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteAliasSchemeProvider(context);
            return null;
        }

        public SiteAliasProviderBase GetSiteAliasProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteAliasProvider(new UserSecurityContext(null));
            return null;
        }
        public SiteAliasProviderBase GetSiteAliasProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetSiteAliasProvider(context);
            return null;
        }

        protected override void InitializeImpl()
        {
            lock (instance)
            {
                if (RuntimeUtils.Initializable(this.State))
                {
                    string meth = "Initialize";
                    this.State = RunState.Initializing;
                    this.logger = LogManager.Instance.GetProvider(typeof(SiteManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(SiteManager), "provider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    this.Initialize(typeName);
                                    return;
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


                    this.State = RunState.FailedInitializing;
                }
            }
        }

        public void Initialize(TypeNameReference providerFactory)
        {
            lock (instance)
            {
                if (RuntimeUtils.Initializable(this.State) || this.State == RunState.Initializing)
                {
                    this.State = RunState.Initializing;
                    string meth = "Initialize";
                    if (this.logger == null) //in case we're directly bootstrapping
                    {
                        this.logger = LogManager.Instance.GetProvider(typeof(SiteManager));
                        Log(meth, LogLevel.Info, "Called");
                    }

                    if (providerFactory != null)
                    {
                        this.factory = NameReflectionUtils.CreateInstance<SiteProviderFactoryBase>(providerFactory);
                        if (this.factory != null)
                        {
                            if (this.factory.Initialize())
                            {
                                this.factory.LocalContext = new UserSecurityContext(new LocalSystemUser(user.Uid, user.Name, user.UserState));
                                Log(meth, LogLevel.Info, "Succeeded");
                                this.State = RunState.Initialized;
                                return;
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to initialize provider factory instance");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to create permission factory instance");
                    }
                    else
                        Log(meth, LogLevel.Error, "No typename provided");
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
                    string meth = "Start";
                    this.State = RunState.Starting;
                    Log(meth, LogLevel.Info, "Called");
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
                    string meth = "Stop";
                    this.State = RunState.Stopping;
                    Log(meth, LogLevel.Info, "Called");
                    this.State = RunState.Stopped;
                }
            }
        }

        private void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        private SiteManager()
        {
            SingletonHelper<SiteManager> help = new SingletonHelper<SiteManager>();
            help.Construct(this);
        }

        private static SiteManager instance = new SiteManager();
        public static SiteManager Instance
        {
            get { return instance; }
        }
    }
}
