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
using Osrs.Security.Passwords;
using System;

namespace Osrs.Security.Authentication.Providers
{
    public sealed class UserPasswordProviderFactory : AuthenticationProviderFactory
    {
        private bool initialized = false;
        private readonly AuthenticationManager mgr = AuthenticationManager.Instance;
        internal readonly MultiRulePasswordComplexityRule ComplexityChecker = new MultiRulePasswordComplexityRule();
        internal SaltShaker Shaker;
        internal UserPasswordHistoryProviderFactory HistoryProvider;
        internal ushort MaxHistory;

        protected override IAuthenticationProvider GetProvider(UserSecurityContext context)
        {
            return new UserPasswordAuthenticationProvider(context, this.GetCredentialProvider(context), this.HistoryProvider.GetProvider());
        }

        protected override bool Initialize()
        {
            lock (this)
            {
                if (!initialized)
                {
                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config != null)
                    {
                        Type myType = typeof(UserPasswordProviderFactory);
                        ConfigurationParameter parm = config.Get(myType, "historyProvider");
                        if (parm != null)
                        {
                            TypeNameReference fact = TypeNameReference.Parse((string)parm.Value);
                            if (fact != null)
                            {
                                UserPasswordConfig myConfig = new UserPasswordConfig();
                                myConfig.HistoryProvider = fact;
                                parm = config.Get(myType, "maxHistory");
                                if (parm != null)
                                {
                                    try
                                    {
                                        myConfig.MaxHistory = (ushort)(int)parm.Value;
                                        parm = config.Get(myType, "hashLength");
                                        if (parm != null)
                                        {
                                            try
                                            {
                                                myConfig.HashLength = (int)parm.Value;
                                                if (myConfig.HashLength > 0)
                                                {
                                                    parm = config.Get(myType, "hashMinChar");
                                                    if (parm != null)
                                                    {
                                                        try
                                                        {
                                                            myConfig.MinChar = (char)(int)parm.Value;
                                                            if (myConfig.MinChar > 0)
                                                            {
                                                                parm = config.Get(myType, "hashMaxChar");
                                                                if (parm != null)
                                                                {
                                                                    try
                                                                    {
                                                                        myConfig.MaxChar = (char)(int)parm.Value;
                                                                        if (myConfig.MaxChar > 0)
                                                                        {
                                                                            //TODO -- add complexity rule config params
                                                                            return this.Initialize(myConfig);
                                                                        }
                                                                    }
                                                                    catch
                                                                    { }
                                                                }
                                                            }
                                                        }
                                                        catch
                                                        { }
                                                    }
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool Initialize(UserPasswordConfig config)
        {
            lock (this)
            {
                if (config != null && !this.initialized)
                {
                    if (config.HistoryProvider != null)
                    {
                        this.HistoryProvider = this.HistoryProvider = NameReflectionUtils.CreateInstance<UserPasswordHistoryProviderFactory>(config.HistoryProvider);
                        if (this.HistoryProvider != null)
                        {
                            if (this.HistoryProvider.Initialize())
                            {
                                this.MaxHistory = config.MaxHistory;
                                if (config.MinChar < config.MaxChar)
                                {
                                    try
                                    {
                                        this.Shaker = new SaltShaker((char)config.MinChar, (char)config.MaxChar, config.HashLength, SaltCreationModel.Repeatable, SaltEmbeddingModel.Randomized, -1);
                                        foreach (PasswordComplexityRule curRule in config.Rules)
                                        {
                                            if (curRule != null)
                                                this.ComplexityChecker.Rules.Add(curRule);
                                        }
                                        this.initialized = true;
                                        return true;
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public UserPasswordProviderFactory()
        {
            instance = this;
        }

        private static UserPasswordProviderFactory instance;
        internal static UserPasswordProviderFactory Instance
        {
            get { return instance; }
        }
    }
}
