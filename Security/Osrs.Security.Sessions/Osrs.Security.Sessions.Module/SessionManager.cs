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

namespace Osrs.Security.Sessions
{
    public class SessionEventArgs : EventArgs
    {
        public Guid SessionId
        {
            get;
        }

        public SessionEventArgs(Guid id)
        {
            this.SessionId = id;
        }
    }

    public sealed class SessionManager : SystemModuleBase
    {
        private LogProviderBase logger;
        private SessionProviderFactory factory;

        public SessionProviderBase GetProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetProvider();
            return null;
        }

        public SessionProviderBase GetProvider(SecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetProvider(context);
            return null;
        }

        public event EventHandler<SessionEventArgs> Create;
        public event EventHandler<SessionEventArgs> Extend;
        public event EventHandler<SessionEventArgs> Expire;

        internal void OnCreate(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = Create;
            if (handler!=null)
            {
                handler(null, e);
            }
        }

        internal void OnExtend(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = Extend;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        internal void OnExpire(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = Expire;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        internal void OnUpdate(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = Expire;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        /// <summary>
        /// duration of a session in seconds, before expiration
        /// </summary>
        public uint SessionDuration
        {
            get;
            private set;
        }

        protected override void BootstrapImpl()
        {
            lock(instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State))
                {
                    string meth = "Bootstrap";
                    this.State = RunState.Bootstrapping;
                    this.logger = LogManager.Instance.GetProvider(typeof(SessionManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(SessionManager), "provider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    uint sessionDuration;
                                    param = config.Get(typeof(SessionManager), "duration");
                                    if (param != null)
                                    {
                                        try
                                        {
                                            sessionDuration = (uint)(int)param.Value;
                                            if (sessionDuration == 0)
                                                sessionDuration = 900; //default if not provided 15 minutes
                                            this.Bootstrap(typeName, sessionDuration);
                                            return;
                                        }
                                        catch
                                        { }

                                        Log(meth, LogLevel.Error, "Failed to parse duration param value");
                                    }
                                    else
                                        Log(meth, LogLevel.Error, "Failed to get duration param");
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

        public void Bootstrap(TypeNameReference factoryType, uint sessionDuration)
        {
            lock (instance)
            {
                if (RuntimeUtils.Bootstrappable(this.State) || this.State == RunState.Bootstrapping)
                {
                    if (factoryType != null)
                    {
                        SessionProviderFactory fact = NameReflectionUtils.CreateInstance<SessionProviderFactory>(factoryType);
                        if (fact != null)
                        {
                            this.factory = fact;
                            this.SessionDuration = sessionDuration;
                            this.State = RunState.Bootstrapped;
                            return;
                        }
                    }
                    this.State = RunState.FailedBootstrapping;
                }
            }
        }

        protected override void InitializeImpl()
        {
            lock (instance)
            {
                if (this.State == RunState.Bootstrapped)
                {
                    try
                    {
                        if (this.factory.Initialize())
                        {
                            this.State = RunState.Initialized;
                            return;
                        }
                    }
                    catch
                    { }
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
                    this.State = RunState.Running;
                }
            }
        }

        private void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        private SessionManager()
        { }

        private static SessionManager instance = new SessionManager();
        public static SessionManager Instance
        {
            get { return instance; }
        }
    }
}
