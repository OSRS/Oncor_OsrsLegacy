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
using Osrs.Reflection;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging.Providers;

namespace Osrs.Runtime.Logging
{
    public sealed class LogManager : SystemModuleBase
    {
        private object syncRoot = new object();
        private LogProviderFactory provider;

        protected override void InitializeImpl() //do this on first request for a provider
        { }

        public LogProviderBase GetProvider(Type t)
        {
            if (this.provider==null)
            {
                this.Initialize();
                if (this.State == RunState.Initialized)
                    this.Start();
                if (this.provider == null)
                    return null;
            }
            return this.provider.GetLogger(t);
        }

        private LogManager()
        { }

        private static LogManager instance = new LogManager();
        public static LogManager Instance
        {
            get { return instance; }
        }

        protected override void StartImpl()
        { }

        protected override void StopImpl()
        { }

        public override void Shutdown()
        {
            if (this.provider != null)
            {
                this.provider.Shutdown();
                return;
            }
            this.State = RunState.FailedStopping;
        }

        protected override void BootstrapImpl()
        {
            lock (this.syncRoot)
            {
                this.State = RunState.Bootstrapping;
                try
                {
                    ConfigurationProviderBase prov = ConfigurationManager.Instance.GetProvider();
                    if (prov != null)
                    {
                        ConfigurationParameter param = prov.Get(typeof(LogManager), "provider");
                        if (param != null)
                        {
                            string fName = (string)param.Value;
                            if (!string.IsNullOrEmpty(fName))
                            {
                                TypeNameReference tnr = TypeNameReference.Parse(fName, ',');
                                if (tnr != null)
                                {
                                    this.Bootstrap(tnr);
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

        public void Bootstrap(TypeNameReference logfactory)
        {
            lock (this.syncRoot)
            {
                if (RuntimeUtils.Bootstrappable(this.State) || this.State == RunState.Bootstrapping)
                {
                    this.State = RunState.Bootstrapping;
                    if (logfactory != null)
                    {
                        this.provider = new MemoryLoggerFactory();
                        this.provider.Initialize();
                        if (!logfactory.Equals(TypeNameReference.Create(typeof(MemoryLoggerFactory)))) //confirm we're not just using memory logger
                        {
                            LogProviderFactory configFinal = NameReflectionUtils.CreateInstance<LogProviderFactory>(logfactory);
                            if (configFinal != null)
                            {
                                if (configFinal.Initialize())
                                {
                                    this.provider = configFinal; //for the time being
                                    foreach (LogItem cur in MemoryLoggerFactory.Items)
                                    {
                                        LogProviderBase log = this.provider.GetLogger(NameReflectionUtils.GetType(cur.TypeName));
                                        if (log != null)
                                            log.Log(cur.Severity, cur.Message);
                                    }
                                    MemoryLoggerFactory.Items.Clear();
                                    this.State = RunState.Bootstrapped;
                                    return;
                                }
                            }
                        }
                        else //permanent logger is memory logger, so do nothing else
                        {
                            this.State = RunState.Bootstrapped;
                            return;
                        }
                    }
                    this.provider = null;
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }
    }
}
