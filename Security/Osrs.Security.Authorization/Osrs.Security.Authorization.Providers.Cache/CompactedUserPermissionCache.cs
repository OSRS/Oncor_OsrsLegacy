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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Security.Authorization.Providers
{
    internal static class CompactedUserPermissionCache
    {
        private static readonly Dictionary<Guid, HashSet<Guid>> userPermissions = new Dictionary<Guid, HashSet<Guid>>();

        internal static int MaxCacheSize
        {
            get;
            set;
        }

        internal static bool UserExists(Guid userId)
        {
            return userPermissions.ContainsKey(userId);
        }

        internal static bool HasPermission(Guid userId, Guid permissionId)
        {
            if (userPermissions.ContainsKey(userId))
            { 
                return userPermissions[userId].Contains(permissionId);
            }
            return false;
        }

        internal static void Set(Guid userId, HashSet<Guid> permissions)
        {
            userPermissions[userId] = permissions;
        }

        private static void PushUp(Guid userId) //puts this userId at top of history
        {
        }

        private sealed class UserList
        {
            internal void Add(Guid id)
            {
            }

            internal Guid RemoveLast() //we can trust size is bigger than 1
            {
                UserNode tmp = this.Last;

                tmp.Previous.Next = null;
                this.Last = tmp.Previous;
                return tmp.Value;
            }

            private UserNode First
            {
                get;
                set;
            }

            private UserNode Last
            {
                get;
                set;
            }

            internal sealed class UserNode
            {
                internal UserNode Next
                {
                    get;
                    set;
                }

                internal UserNode Previous
                {
                    get;
                    set;
                }

                internal Guid Value
                {
                    get;
                    set;
                }
            }
        }
    }
}
