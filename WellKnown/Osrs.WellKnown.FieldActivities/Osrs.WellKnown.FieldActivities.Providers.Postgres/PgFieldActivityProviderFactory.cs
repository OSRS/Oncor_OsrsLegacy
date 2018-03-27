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

using Osrs.Runtime.Logging;
using System;
using Osrs.Security;
using Osrs.Data.Postgres;
using Osrs.Runtime.Configuration;

namespace Osrs.WellKnown.FieldActivities.Providers
{
    public sealed class PgFieldActivityProviderFactory : FieldActivityProviderFactoryBase
    {
        private readonly Type myType = typeof(PgFieldActivityProviderFactory);
        private bool initialized = false;
        private LogProviderBase logger;

        protected override FieldActivityProviderBase GetFieldActivityProvider(UserSecurityContext context)
        {
            return new PgFieldActivityProvider(context);
        }

        protected override FieldTripProviderBase GetFieldTripProvider(UserSecurityContext context)
        {
            return new PgFieldTripProvider(context);
        }

        protected override FieldTeamProviderBase GetFieldTeamProvider(UserSecurityContext context)
        {
            return new PgFieldTeamProvider(context);
        }

        protected override FieldTeamMemberRoleProviderBase GetFieldTeamMemberRoleProvider(UserSecurityContext context)
        {
            return new PgFieldTeamMemberRoleProvider(context);
        }

        protected override SampleEventProviderBase GetSampleEventProvider(UserSecurityContext context)
        {
            return new PgSampleEventProvider(context);
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

        public PgFieldActivityProviderFactory()
        {
            instance = this;
        }

        private static PgFieldActivityProviderFactory instance;
        public static PgFieldActivityProviderFactory Instance
        {
            get { return instance; }
        }
    }
}
