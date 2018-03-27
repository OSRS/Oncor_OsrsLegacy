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
using System.Collections.Generic;

namespace Osrs.Security.Authentication.Providers
{
    public sealed class UserPasswordConfig
    {
        public TypeNameReference HistoryProvider
        {
            get;
            set;
        }

        public ushort MaxHistory
        {
            get;
            set;
        }

        public int HashLength
        {
            get;
            set;
        }

        public char MinChar
        {
            get;
            set;
        }

        public char MaxChar
        {
            get;
            set;
        }

        private readonly List<Passwords.PasswordComplexityRule> rules = new List<Passwords.PasswordComplexityRule>();
        public IList<Passwords.PasswordComplexityRule> Rules
        {
            get { return this.rules; }
        }
    }
}
