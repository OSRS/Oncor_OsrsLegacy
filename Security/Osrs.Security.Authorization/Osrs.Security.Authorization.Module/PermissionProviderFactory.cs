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

using Osrs.Runtime;

namespace Osrs.Security.Authorization
{
    public abstract class PermissionProviderFactory : SubclassableSingletonBase<PermissionProviderFactory>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal abstract bool Initialize();

        protected bool InitializeOther(PermissionProviderFactory otherFactory)
        {
            if (otherFactory != null)
                return otherFactory.Initialize();
            return false;
        }

        protected internal IPermissionProvider GetProvider()
        {
            return this.GetProvider(new UserSecurityContext(null));
        }

        protected IPermissionProvider GetProviderOther(PermissionProviderFactory otherFactory)
        {
            if (otherFactory != null)
                return otherFactory.GetProvider();
            return null;
        }

        protected internal abstract IPermissionProvider GetProvider(UserSecurityContext context);

        protected IPermissionProvider GetProviderOther(PermissionProviderFactory otherFactory, UserSecurityContext context)
        {
            if (otherFactory != null)
                return otherFactory.GetProvider(context);
            return null;
        }

        protected PermissionProviderFactory()
        { }
    }
}
