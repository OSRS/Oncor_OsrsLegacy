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

namespace Osrs.Security.Authentication.Providers
{
    public class UserPasswordCredential : ICredential, IEquatable<UserPasswordCredential>
    {
        private readonly string userName;
        public string UserName
        {
            get { return this.userName; }
        }

        internal readonly string password;

        public bool IsEmpty
        {
            get
            {
                return string.Empty.Equals(this.userName) && string.Empty.Equals(this.password);
            }
        }

        public UserPasswordCredential(string userName, string password)
        {
            MethodContract.NotNull(userName, nameof(userName));
            MethodContract.NotNull(password, nameof(password));
            this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// Expects the persisted credential payload is a salted password hash and that the token is already lower cased
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal bool Matches(PersistedCredential other)
        {
            if (this.userName.Length>0 && this.password.Length>0 && other!=null)
            {
                if (other.TextToken.Equals(this.userName, StringComparison.OrdinalIgnoreCase))
                {
                    SaltPair salted = UserPasswordProviderFactory.Instance.Shaker.Salt(this.password);
                    return salted.SaltedPayload.Equals(other.Text);
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a username/password pair that is: lower case username, and salted password hash ready for comparing
        /// </summary>
        /// <returns></returns>
        internal UsernamePassword ToHistoryPair()
        {
            SaltPair salted = UserPasswordProviderFactory.Instance.Shaker.Salt(this.password);
            return new UsernamePassword(this.userName.ToLowerInvariant(), salted.SaltedPayload);
        }

        public bool Equals(UserPasswordCredential other)
        {
            if (other == null)
                return false;
            return this.userName.Equals(other.userName, StringComparison.OrdinalIgnoreCase) && this.password.Equals(other.password);
        }

        public bool Equals(ICredential other)
        {
            return this.Equals(other as UserPasswordCredential);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserPasswordCredential);
        }

        public override int GetHashCode()
        {
            return this.userName.GetHashCode() ^ this.password.GetHashCode();
        }
    }
}
