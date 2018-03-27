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
using System.Reflection;

namespace Osrs.Runtime.Configuration
{
    public abstract class ConfigurationFactoryBase : SubclassableSingletonBase<ConfigurationFactoryBase>
    {
        protected internal abstract bool Initialize();

        protected internal abstract ConfigurationProviderBase GetProvider();
    }

    public abstract class ConfigurationProviderBase
    {
        public ConfigurationParameter Get(Type t, string name)
        {
            if (ConfigurationManager.Instance.State == RunState.Running && t != null)
                return this.GetImpl(string.Format("{0}.{1}, {2}", t.Namespace, t.Name, t.GetTypeInfo().Assembly.GetName().Name), name);
            return null;
        }

        protected internal abstract ConfigurationParameter GetImpl(string typeName, string name);
    }
}
