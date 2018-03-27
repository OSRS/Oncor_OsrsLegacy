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

namespace Osrs.Security.Authorization
{
    public sealed class Role : IEquatable<Role>
    {
        private readonly Guid id;
        public Guid Id
        {
            get { return this.id; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.name = value;
                }
            }
        }

        public Role() : this(null, Guid.Empty)
        {
        }
        public Role(string name, Guid id)
        {
            if (string.IsNullOrEmpty(name) || Guid.Empty.Equals(id))
            {
                this.name = null;
                this.id = Guid.Empty;
            }
            else
            {
                this.name = name;
                this.id = id;
            }
        }

        public bool Equals(Role other)
        {
            if (other != null)
            {
                if (Guid.Empty.Equals(this.id))
                {
                    return this.id.Equals(other.Id);
                }
                return this.id.Equals(other.Id) &&
                    this.name.Equals(other.name);
            }
            return false;
        }
    }
}
