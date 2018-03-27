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

using System.IO;
using Osrs.Reflection;
using Osrs.Runtime.Configuration.Providers;
using System;
using System.Reflection;

namespace Osrs.Runtime.Configuration
{
    public sealed class ConfigurationManager : SystemModuleBase
    {
        private object syncRoot = new object();
        private ConfigurationFactoryBase provider;
        private ConfigurationProviderBase initialProv;

        protected override void InitializeImpl() //do this on first request for a provider
        { }

        public ConfigurationProviderBase GetProvider()
        {
            if (this.provider==null)
            {
                if (this.State == RunState.Bootstrapping) //allows cheat
                {
                    this.State = RunState.Running; //just to let the config be read
                    return this.initialProv;
                }
                this.Bootstrap();
                this.Initialize();
                this.Start();
            }
            if (this.provider!=null)
                return this.provider.GetProvider();
            return null;
        }

        private ConfigurationManager()
        { }

        private static ConfigurationManager instance = new ConfigurationManager();
        public static ConfigurationManager Instance
        {
            get { return instance; }
        }

        protected override void StartImpl()
        {
            if (RuntimeUtils.Startable(this.State))
            {
                if (this.provider == null)
                    this.State = RunState.FailedStarting;
            }
        }

        protected override void StopImpl()
        { }

        protected override void BootstrapImpl()
        {
            lock (this.syncRoot)
            {
                try
                {
                    this.State = RunState.Bootstrapping;
                    string corePath = Path.GetDirectoryName(AppContext.BaseDirectory);
                    JsonProvider tmpProvider = new JsonProvider();
                    if (tmpProvider.Init(Path.Combine(corePath, "Osrs.Runtime.Configuration.jconfig")))
                    {
                        Type t = typeof(ConfigurationManager);
                        string name = string.Format("{0}.{1}, {2}", t.Namespace, t.Name, t.GetTypeInfo().Assembly.GetName().Name);
                        ConfigurationParameter param = tmpProvider.GetImpl(name, "provider");
                        if (param != null)
                        {
                            string fName = (string)param.Value;
                            if (!string.IsNullOrEmpty(fName))
                            {
                                TypeNameReference tnr = TypeNameReference.Parse(fName, ',');
                                if (tnr != null)
                                {
                                    Bootstrap(tnr, tmpProvider);
                                    return;
                                }
                            }
                        }
                    }
                }
                catch
                { }
                this.State = RunState.FailedBootstrapping;
            }
        }

        public void Bootstrap(TypeNameReference configSource, ConfigurationProviderBase initialConfig)
        {
            lock(this.syncRoot)
            {
                if (RuntimeUtils.Bootstrappable(this.State) || this.State == RunState.Bootstrapping)
                {
                    this.State = RunState.Bootstrapping;
                    if (configSource != null)
                    {
                        this.initialProv = initialConfig;
                        ConfigurationFactoryBase configFinal = NameReflectionUtils.CreateInstance<ConfigurationFactoryBase>(configSource);
                        if (configFinal != null)
                        {
                            if (configFinal.Initialize())
                            {
                                this.provider = configFinal;
                                this.State = RunState.Bootstrapped;
                                return;
                            }
                            else
                                this.provider = null;
                        }
                    }
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }
    }
}
