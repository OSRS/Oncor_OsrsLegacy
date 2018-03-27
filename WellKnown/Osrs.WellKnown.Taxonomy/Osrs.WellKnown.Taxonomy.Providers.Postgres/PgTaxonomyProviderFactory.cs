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
using Osrs.Data.Postgres;
using Osrs.Runtime.Configuration;

namespace Osrs.WellKnown.Taxonomy.Providers
{
	public sealed class PgTaxonomyProviderFactory : TaxonomyProviderFactoryBase
	{
		private readonly Type myType = typeof(PgTaxonomyProviderFactory);
		private bool initialized = false;
		private LogProviderBase logger;

		protected override TaxaCommonNameProviderBase GetTaxaCommonNameProvider(UserSecurityContext context)
		{
			return new PgTaxaCommonNameProvider(context);
		}

		protected override TaxaDomainProviderBase GetTaxaDomainProvider(UserSecurityContext context)
		{
			return new PgTaxaDomainProvider(context);
		}

		protected override TaxaDomainUnitTypeProviderBase GetTaxaDomainUnitTypeProvider(UserSecurityContext context)
		{
			return new PgTaxaDomainUnitTypeProvider(context);
		}

		protected override TaxaUnitProviderBase GetTaxaUnitProvider(UserSecurityContext context)
		{
			return new PgTaxaUnitProvider(context);
		}

		protected override TaxaUnitTypeProviderBase GetTaxaUnitTypeProvider(UserSecurityContext context)
		{
			return new PgTaxaUnitTypeProvider(context);
		}

		protected override TaxonomyProviderBase GetTaxonomyProvider(UserSecurityContext context)
		{
			return new PgTaxonomyProvider(context);
		}

		protected override bool Initialize()
		{
			lock (instance)
			{
				if(!this.initialized)
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

		public PgTaxonomyProviderFactory()
		{
			instance = this;
		}

		private static PgTaxonomyProviderFactory instance;
		public static PgTaxonomyProviderFactory Instance
		{
			get { return instance; }
		}
	}
}
