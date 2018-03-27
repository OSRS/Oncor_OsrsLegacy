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
using System.Collections.Generic;

namespace Osrs.Security.Authorization.Providers
{
    public sealed class CachingPermissionProviderFactory : PermissionProviderFactory
    {
        private bool initialized = false;
        private LogProviderBase logger;
        private PermissionProviderFactory innerFact;

        protected override IPermissionProvider GetProvider(UserSecurityContext context)
        {
            return new CachingPermissionProvider(this.GetProviderOther(innerFact, context));
        }

        protected override bool Initialize()
        {
            lock (instance)
            {
                if (!this.initialized)
                {
                    string meth = "Initialize";
                    this.logger = LogManager.Instance.GetProvider(typeof(CachingPermissionProviderFactory));
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(typeof(CachingPermissionProviderFactory), "provider");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                TypeNameReference typeName = TypeNameReference.Parse(tName);
                                if (typeName != null)
                                {
                                    innerFact = NameReflectionUtils.CreateInstance<PermissionProviderFactory>(typeName);
                                    if (innerFact != null)
                                    {
                                        if (InitializeOther(innerFact))
                                        {
                                            //ok preload the cache
                                            LocalSystemUser u = new LocalSystemUser(SecurityUtils.AdminIdentity, "Admin", UserState.Active);
                                            IPermissionProvider prov = this.GetProviderOther(this.innerFact, new UserSecurityContext(u));
                                            if (prov != null)
                                            {
                                                IEnumerable<Permission> perms = prov.GetPermissions();
                                                if (perms!=null)
                                                {
                                                    foreach(Permission p in perms)
                                                    {
                                                        PermissionMemorySet.RegisterPermission(p);
                                                    }

                                                    this.initialized = true;
                                                    return true;
                                                }
                                                else
                                                    Log(meth, LogLevel.Error, "Failed to get existing permissions");
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
                                Log(meth, LogLevel.Error, "Failed to get provider param value");
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

        internal void Log(string method, LogLevel level, string message)
        {
            if (this.logger != null)
                this.logger.Log(method, LogLevel.Info, message);
        }

        public CachingPermissionProviderFactory()
        {
            instance = this;
        }

        private static CachingPermissionProviderFactory instance;
        public static CachingPermissionProviderFactory Instance
        {
            get { return instance; }
        }
    }
}
