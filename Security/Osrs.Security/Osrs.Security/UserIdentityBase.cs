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

namespace Osrs.Security
{
    public enum UserState
    {
        Unknown,
        Created,
        Pending,
        Active,
        Inactive,
        Dead
    }

    public abstract class UserIdentityBase : IUserIdentity
    {
        private readonly Guid uid;
        public Guid Uid
        {
            get
            {
                return this.uid;
            }
        }

        private readonly UserType userType;
        public UserType UserType
        {
            get
            {
                return this.userType;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime? ExpiresAt
        {
            get;
            set;
        }

        public UserState UserState
        {
            get;
            set;
        }

        public UserIdentityBase(Guid uid, UserType userType) : this(uid, userType, null, UserState.Unknown)
        { }

        public UserIdentityBase(Guid uid, UserType userType, UserState state) : this(uid, userType, null, state)
        { }

        public UserIdentityBase(Guid uid, UserType userType, string name) : this(uid, userType, name, UserState.Unknown)
        { }

        public UserIdentityBase(Guid uid, UserType userType, string name, UserState state)
        {
            this.uid = uid;
            this.userType = UserType;
            this.Name = name;
            this.UserState = state;
        }

        public bool Equals(IUserIdentity other)
        {
            if (null != other)
                return this.uid.Equals(other.Uid) && this.userType.Equals(other.UserType);
            return false;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IUserIdentity);
        }

        public override int GetHashCode()
        {
            return this.uid.GetHashCode();
        }
    }

    /// <summary>
    /// A basic user implementation for modules to use to represent themselves. Only system users are creatable via this class.
    /// </summary>
    public class LocalSystemUser : UserIdentityBase
    {
        public LocalSystemUser(Guid uid, string name, UserState state) : base(uid, UserType.System, name, state)
        { }
    }
}
