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
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using System;
using System.Threading;

namespace Osrs.Security.Authorization.Providers
{
    public sealed class CachingRoleProviderFactory : RoleProviderFactory
    {
        private readonly Type myType = typeof(CachingRoleProvider);
        private bool initialized = false;
        private LogProviderBase logger;
        private RoleProviderFactory innerFact;
        private Timer scavengeTimer;

        protected override IRoleProvider GetProvider(UserSecurityContext context)
        {
            return new CachingRoleProvider(this.GetProviderOther(innerFact, context), context);
        }

        protected override bool Initialize()
        {
            lock (instance)
            {
                if (!this.initialized)
                {
                    string meth = "Initialize";
                    this.logger = LogManager.Instance.GetProvider(typeof(CachingRoleProviderFactory));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(CachingRoleProviderFactory), "provider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    innerFact = NameReflectionUtils.CreateInstance<RoleProviderFactory>(typeName);
                                    if (innerFact!=null)
                                    {
                                        if (InitializeOther(innerFact))
                                        {
                                            //ok preload the cache
                                            LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
                                            IRoleProvider prov = this.GetProviderOther(this.innerFact, new UserSecurityContext(u));
                                            if (prov != null)
                                            {
                                                
                                                if (RoleMemorySet.Reset(prov))
                                                {
                                                    if (RoleMembershipMemorySet.Reset(prov))
                                                    {
                                                        this.initialized = true;
                                                        scavengeTimer = new Timer(this.Scavenge, null, 0, 300000); //5 minutes
                                                        return true;
                                                    }
                                                    else
                                                        Log(meth, LogLevel.Error, "Failed to initialize caching");
                                                }
                                                else
                                                    Log(meth, LogLevel.Error, "Failed to initialize caching");
                                            }
                                            else
                                                Log(meth, LogLevel.Error, "Failed to get inner provider for preload");
                                        }
                                        else
                                            Log(meth, LogLevel.Error, "Failed to initialize inner provider");
                                    }
                                    else
                                        Log(meth, LogLevel.Error, "Failed to create inner provider factory");
                                }
                                else
                                    Log(meth, LogLevel.Error, "Failed to parse permission provider param value");
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to get permission provider param value");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to get provider param");
                    }
                    else
                        Log(meth, LogLevel.Error, "Failed to get ConfigurationProvider");
                }
            }
            return false;
        }

        private void Scavenge(object notUsed)
        {
            if (AuthorizationManager.Instance.State == Runtime.RunState.Running)
            {
                LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
                IRoleProvider prov = this.GetProviderOther(this.innerFact, new UserSecurityContext(u));
                if (prov != null)
                {

                    if (RoleMemorySet.Reset(prov))
                    {
                        RoleMembershipMemorySet.Reset(prov);
                    }
                }
            }
        }

        internal void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        public CachingRoleProviderFactory()
        {
            instance = this;
        }

        private static CachingRoleProviderFactory instance;
        public static CachingRoleProviderFactory Instance
        {
            get { return instance; }
        }
    }
}
