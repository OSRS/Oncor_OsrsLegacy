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
using System;

namespace Osrs.Security.Authentication
{
    public abstract class CredentialStoreFactory : SubclassableSingletonBase<CredentialStoreFactory>
    {
        public RunState State { get; protected set; } = RunState.Created;

        protected CredentialStoreFactory()
        { }

        protected internal abstract bool Initialize();

        protected internal abstract ICredentialStore GetProvider(UserSecurityContext context);

        protected bool Lock(PersistedCredential cred)
        {
            if (cred != null)
            {
                cred.IsLocked = true;
                return true;
            }
            return false;
        }

        protected bool Expire(PersistedCredential cred)
        {
            if (cred != null)
            {
                cred.ValidTo = DateTime.Now;
                return true;
            }
            return false;
        }

        protected bool Update(PersistedCredential cred, bool isLocked)
        {
            if (cred != null)
            {
                cred.IsLocked = isLocked;
                return true;
            }
            return false;
        }

        protected bool Update(PersistedCredential cred, DateTime validFrom, DateTime validTo)
        {
            if (cred != null)
            {
                return Update(cred, validFrom, validTo, cred.IsLocked);
            }
            return false;
        }

        protected bool Update(PersistedCredential cred, DateTime validFrom, DateTime validTo, bool isLocked)
        {
            if (cred != null)
            {
                cred.IsLocked = isLocked;
                cred.ValidFrom = validFrom;
                if (validTo >= validFrom)
                    cred.ValidTo = validTo;
                else
                    cred.ValidTo = validFrom;
                return true;
            }
            return false;
        }

        protected PersistedCredential Create(Guid userId, string payload)
        {
            return Create(userId, payload, (string)null, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, string payload, DateTime validFrom, DateTime validTo)
        {
            return Create(userId, payload, (string)null, validFrom, validTo);
        }

        protected PersistedCredential Create(Guid userId, byte[] payload)
        {
            return Create(userId, payload, (string)null, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, byte[] payload, DateTime validFrom, DateTime validTo)
        {
            return Create(userId, payload, (string)null, validFrom, validTo);
        }

        protected PersistedCredential Create(Guid userId, string payload, string token)
        {
            return Create(userId, payload, token, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, string payload, string token, DateTime validFrom, DateTime validTo)
        {
            PersistedCredential cred = new PersistedCredential(userId, payload, token);
            cred.ValidFrom = validFrom;
            if (validTo >= validFrom)
                cred.ValidTo = validTo;
            else
                cred.ValidTo = validFrom;
            return cred;
        }

        protected PersistedCredential Create(Guid userId, string payload, byte[] token)
        {
            return Create(userId, payload, token, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, string payload, byte[] token, DateTime validFrom, DateTime validTo)
        {
            PersistedCredential cred = new PersistedCredential(userId, payload, token);
            cred.ValidFrom = validFrom;
            if (validTo >= validFrom)
                cred.ValidTo = validTo;
            else
                cred.ValidTo = validFrom;
            return cred;
        }

        protected PersistedCredential Create(Guid userId, byte[] payload, string token)
        {
            return Create(userId, payload, token, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, byte[] payload, string token, DateTime validFrom, DateTime validTo)
        {
            PersistedCredential cred = new PersistedCredential(userId, payload, token);
            cred.ValidFrom = validFrom;
            if (validTo >= validFrom)
                cred.ValidTo = validTo;
            else
                cred.ValidTo = validFrom;
            return cred;
        }

        protected PersistedCredential Create(Guid userId, byte[] payload, byte[] token)
        {
            return Create(userId, payload, token, DateTime.Now, DateTime.MaxValue);
        }

        protected PersistedCredential Create(Guid userId, byte[] payload, byte[] token, DateTime validFrom, DateTime validTo)
        {
            PersistedCredential cred = new PersistedCredential(userId, payload, token);
            cred.ValidFrom = validFrom;
            if (validTo >= validFrom)
                cred.ValidTo = validTo;
            else
                cred.ValidTo = validFrom;
            return cred;
        }
    }
}
