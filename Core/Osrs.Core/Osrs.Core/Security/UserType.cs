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
    public interface IUserIdentity : IEquatable<IUserIdentity>
    {
        Guid Uid
        {
            get;
        }

        UserType UserType
        {
            get;
        }
    }

    public enum UserType
    {
        /// <summary>
        /// Person is a human user, the "typical" notion of user
        /// </summary>
        Person,
        /// <summary>
        /// System is a programmatic user, often an automaton or simply code executing on behalf of other code rather than a user.
        /// This can be used to "bypass" some security schemes to allow trusted code to access data that a human user cannot to synthesize a result for a user that would otherwise not be able to do so.
        /// </summary>
        System,
        /// <summary>
        /// Token is not a user per se, but is instead a fixed known entity that can be granted permissions like a user.
        /// Tokens are created on behalf of some other user (person or system or token) - for example a bearer token for access to a resource.
        /// </summary>
        Token
    }
}
