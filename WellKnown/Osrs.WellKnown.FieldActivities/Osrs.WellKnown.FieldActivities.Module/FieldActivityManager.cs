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

namespace Osrs.WellKnown.FieldActivities
{
    public sealed class FieldActivityManager : ModuleBase
    {
        private static readonly LocalSystemUser user = new LocalSystemUser(new Guid("{99126DBA-9AF8-4602-BC20-B38CAB08283D}"), NameReflectionUtils.GetName(typeof(FieldActivityManager)), UserState.Active);
        private LogProviderBase logger;
        private FieldActivityProviderFactoryBase factory;

        public FieldActivityProviderBase GetFieldActivityProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldActivityProvider(new UserSecurityContext(null));
            return null;
        }

        public FieldActivityProviderBase GetFieldActivityProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldActivityProvider(context);
            return null;
        }

        public FieldTripProviderBase GetFieldTripProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTripProvider(new UserSecurityContext(null));
            return null;
        }

        public FieldTripProviderBase GetFieldTripProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTripProvider(context);
            return null;
        }

        public FieldTeamProviderBase GetFieldTeamProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTeamProvider(new UserSecurityContext(null));
            return null;
        }

        public FieldTeamProviderBase GetFieldTeamProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTeamProvider(context);
            return null;
        }

        public FieldTeamMemberRoleProviderBase GetFieldTeamMemberRoleProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTeamMemberRoleProvider(new UserSecurityContext(null));
            return null;
        }

        public FieldTeamMemberRoleProviderBase GetFieldTeamMemberRoleProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetFieldTeamMemberRoleProvider(context);
            return null;
        }

        public SampleEventProviderBase GetSampleEventProvider()
        {
            if (this.State == RunState.Running)
                return this.factory.GetSampleEventProvider(new UserSecurityContext(null));
            return null;
        }

        public SampleEventProviderBase GetSampleEventProvider(UserSecurityContext context)
        {
            if (this.State == RunState.Running)
                return this.factory.GetSampleEventProvider(context);
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
                    this.logger = LogManager.Instance.GetProvider(typeof(FieldActivityManager));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(FieldActivityManager), "provider");
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
                        this.logger = LogManager.Instance.GetProvider(typeof(FieldActivityManager));
                        Log(meth, LogLevel.Info, "Called");
                    }

                    if (providerFactory != null)
                    {
                        this.factory = NameReflectionUtils.CreateInstance<FieldActivityProviderFactoryBase>(providerFactory);
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

        private FieldActivityManager()
        {
            SingletonHelper<FieldActivityManager> help = new SingletonHelper<FieldActivityManager>();
            help.Construct(this);
        }

        private static FieldActivityManager instance = new FieldActivityManager();
        public static FieldActivityManager Instance
        {
            get { return instance; }
        }
    }
}
