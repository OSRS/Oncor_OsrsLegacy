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
using Osrs.Security;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Configuration;
using Osrs.Data.Postgres;

namespace Osrs.WellKnown.Organizations.Providers
{
    public sealed class PostgresOrganizationProviderFactory : OrganizationProviderFactoryBase
    {
        private readonly Type myType = typeof(PostgresOrganizationProviderFactory);
        private bool initialized = false;
        private LogProviderBase logger;

        protected override OrganizationAliasProviderBase GetOrganizationAliasProvider(UserSecurityContext context)
        {
            return new PostgresOrganizationAliasProvider(context);
        }

        protected override OrganizationAliasSchemeProviderBase GetOrganizationAliasSchemeProvider(UserSecurityContext context)
        {
            return new PostgresOrganizationAliasSchemeProvider(context);
        }

        protected override OrganizationProviderBase GetOrganizationProvider(UserSecurityContext context)
        {
            return new PostgresOrganizationProvider(context);
        }

        protected override bool Initialize()
        {
            lock (instance)
            {
                if (!this.initialized)
                {
                    string meth = "Initialize";
                    this.logger = LogManager.Instance.GetProvider(myType);
                    Log(meth, LogLevel.Info, "Called");

                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        ConfigurationParameter param = config.Get(myType, "connectionString");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                if (NpgSqlCommandUtils.TestConnection(tName))
                                {
                                    Db.ConnectionString = tName;
                                    this.initialized = true;
                                    return true;
                                }
                            }
                            else
                                Log(meth, LogLevel.Error, "Failed to get connectionString param value");
                        }
                        else
                            Log(meth, LogLevel.Error, "Failed to get connectionString param");
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

        public PostgresOrganizationProviderFactory()
        {
            instance = this;
        }

        private static PostgresOrganizationProviderFactory instance;
        public static PostgresOrganizationProviderFactory Instance
        {
            get { return instance; }
        }
    }
}
