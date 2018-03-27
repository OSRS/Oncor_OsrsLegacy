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
    public sealed class PermissionAssignment : IEquatable<PermissionAssignment>
    {
        public Permission Permission
        {
            get;
        }

        public GrantType GrantType
        {
            get;
        }

        public PermissionAssignment(Permission perm, GrantType grant)
        {
            this.Permission = perm;
            this.GrantType = grant;
        }

        public bool Equals(PermissionAssignment other)
        {
            if (other != null)
                return this.Permission.Equals(other.Permission) && this.GrantType.Equals(other.GrantType);
            return false;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PermissionAssignment);
        }

        public override int GetHashCode()
        {
            return this.Permission.GetHashCode() ^ this.GrantType.GetHashCode();
        }
    }

    public sealed class Permission : IEquatable<Permission>, IEmpty<Permission>
    {
        private readonly Guid id;
        public Guid Id
        {
            get { return this.id; }
        }

        private readonly string name;
        public string Name
        {
            get { return this.name; }
        }

        public bool IsEmpty
        {
            get
            {
                return this.id.Equals(Guid.Empty);
            }
        }

        public Permission() : this(null, Guid.Empty)
        {
        }

        public Permission(string name, Guid id)
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

        public bool Equals(Permission other)
        {
            if (other != null)
            {
                if (Guid.Empty.Equals(this.id))
                {
                    return this.id.Equals(other.Id);
                }
                return this.id.Equals(other.Id) &&
                    this.name.Equals(other.Name);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Permission);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
