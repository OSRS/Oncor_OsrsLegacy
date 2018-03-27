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

using Osrs.Runtime;
using Osrs.Reflection;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Configuration;

namespace Osrs.Security.Identity
{
    public sealed class IdentityManager : NonSubclassableSingletonBase<IdentityManager>, ISystemModule
    {
        private LogProviderBase logger;
        private IdentityProviderFactory factory;

        public RunState State
        {
            get;
            private set;
        }

        public IIdentityProvider GetProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetProvider();
            return null;
        }

        public IIdentityProvider GetProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetProvider(context);
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
                    if (this.factory.Initialize())
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
            lock(instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State))
                {
                    string meth = "Bootstrap";
                    this.State = RunState.Bootstrapping;
                    this.logger = LogManager.Instance.GetProvider(typeof(IdentityManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(IdentityManager), "provider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName!=null)
                                {
                                    IdentityConfiguration iConfig = new IdentityConfiguration();
                                    iConfig.FactoryTypeName = typeName;
                                    Bootstrap(iConfig);
                                    return;
                                }
                                else
                                    Log(meth, LogLevel.Error, "Failed to parse provider param value");
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to get provider param value");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to get provider param");
                    }
                    else
                        Log(meth, LogLevel.Error, "Failed to get ConfigurationProvider");


                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        public void Bootstrap(IdentityConfiguration config)
        {
            lock (instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State) || this.State == RunState.Bootstrapping)
                {
                    this.State = RunState.Bootstrapping;
                    string meth = "Bootstrap";
                    if (this.logger == null) //in case we're directly bootstrapping
                    {
                        this.logger = LogManager.Instance.GetProvider(typeof(IdentityManager));
                        Log(meth, LogLevel.Info, "Called");
                    }

                    if (config!=null)
                    {
                        TypeNameReference typ = config.FactoryTypeName;
                        if (typ != null)
                        {
                            this.factory = NameReflectionUtils.CreateInstance<IdentityProviderFactory>(typ);
                            if (this.factory != null)
                            {
                                Log(meth, LogLevel.Info, "Succeeded");
                                this.State = RunState.Bootstrapped;
                                return;
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to create factory instance");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to extract typename");
                    }
                    else
                        Log(meth, LogLevel.Error, "No config provided");
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        private void Log(string method, LogLevel level, string message)
        {
            if (this.logger!=null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        private IdentityManager()
        {
        }

        private static IdentityManager instance=new IdentityManager();
        public static IdentityManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
