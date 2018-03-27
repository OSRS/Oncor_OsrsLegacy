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

using System.Collections;
using System.Collections.Generic;

namespace Osrs.Security.Passwords
{
    public delegate bool PasswordMatch<T>(T passwordNew, T passwordOld);

    public sealed class UsernamePassword
    {
        public readonly string UserName;
        public readonly string Password;

        public UsernamePassword(string username, string password)
        {
            this.UserName = username;
            this.Password = password;
        }

        public static bool Matches(UsernamePassword passwordNew, UsernamePassword passwordOld)
        {
            if (passwordNew!=null && passwordOld!=null && 
                passwordNew.UserName!=null && passwordOld.UserName!=null && 
                passwordNew.Password != null && passwordOld.Password != null)
            {
                return passwordNew.UserName.Equals(passwordOld.UserName, System.StringComparison.OrdinalIgnoreCase) && 
                    passwordNew.Password.Equals(passwordOld.Password, System.StringComparison.Ordinal);
            }
            return false;
        }
    }

    public class PasswordHistory<T> : IEnumerable<T>
    {
        private ushort maxHistory = 5;
        /// <summary>
        /// number of passwords to keep in history
        /// </summary>
        public ushort MaxHistory
        {
            get { return this.maxHistory; }
            set
            {
                this.maxHistory = value;
                lock (this.passwordPayloads)
                {
                    if (this.passwordPayloads.Count > this.maxHistory)
                        this.passwordPayloads.RemoveRange(this.maxHistory, this.passwordPayloads.Count - maxHistory);
                }
            }
        }

        private readonly List<T> passwordPayloads = new List<T>();

        public T this[ushort index]
        {
            get
            {
                if (index < this.passwordPayloads.Count)
                    return this.passwordPayloads[index];
                return default(T);
            }
        }

        public ushort Count
        {
            get
            {
                lock (this.passwordPayloads)
                {
                    return (ushort)this.passwordPayloads.Count;
                }
            }
        }

        public void Add(T password)
        {
            lock(this.passwordPayloads)
            {
                this.passwordPayloads.Insert(0, password);
                if (this.passwordPayloads.Count > this.maxHistory)
                    this.passwordPayloads.RemoveAt(this.passwordPayloads.Count - 1);
            }
        }

        public bool MatchesHistory(PasswordMatch<T> matcher, T passwordNew)
        {
            lock(this.passwordPayloads)
            {
                foreach(T item in this.passwordPayloads)
                {
                    if (matcher(passwordNew, item))
                        return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.passwordPayloads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
