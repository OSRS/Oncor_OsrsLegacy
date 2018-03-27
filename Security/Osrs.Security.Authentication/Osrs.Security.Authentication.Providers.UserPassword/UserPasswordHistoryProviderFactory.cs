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
using Osrs.Security.Passwords;
using System;
using System.Collections.Generic;

namespace Osrs.Security.Authentication.Providers
{
    public abstract class UserPasswordHistoryProviderFactory : NonSubclassableSingletonBase<UserPasswordHistoryProviderFactory>
    {
        protected internal abstract bool Initialize();

        protected internal abstract UserPasswordHistoryProvider GetProvider();
    }

    public abstract class UserPasswordHistoryProvider : IPasswordHistoryProvider<UsernamePassword>
    {
        public abstract bool Delete(Guid userId);
        public abstract IEnumerable<PasswordHistory<UsernamePassword>> Get(Guid userId);
        public abstract bool Update(Guid userId, PasswordHistory<UsernamePassword> hist);

        protected UserPasswordHistoryProvider()
        { }
    }
}
