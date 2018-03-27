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
using System.Collections.Generic;

namespace Osrs.Security.Authentication
{
    public interface ICredentialStore
    {
        IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider);
        IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, string token);
        IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, byte[] token);
        IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, Guid userId, string token);
        IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, Guid userId, byte[] token);

        bool AddCredential(Guid userId, IAuthenticationProvider provider, string token, string payload);
        bool AddCredential(Guid userId, IAuthenticationProvider provider, string token, byte[] payload);
        bool AddCredential(Guid userId, IAuthenticationProvider provider, byte[] token, string payload);
        bool AddCredential(Guid userId, IAuthenticationProvider provider, byte[] token, byte[] payload);

        bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, bool isLocked);
        bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, DateTime validFrom, DateTime validTo);
        bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, DateTime validFrom, DateTime validTo, bool isLocked);
        bool Lock(Guid userId);
        bool Lock(IAuthenticationProvider provider, Guid userId);
        bool Lock(IAuthenticationProvider provider, PersistedCredential credential);
        bool Expire(Guid userId);
        bool Expire(IAuthenticationProvider provider, Guid userId);
        bool Expire(IAuthenticationProvider provider, PersistedCredential credential);

        bool DeleteCredential(Guid userId, IAuthenticationProvider provider, string token);
        bool DeleteCredential(Guid userId, IAuthenticationProvider provider, byte[] token);
    }
}
