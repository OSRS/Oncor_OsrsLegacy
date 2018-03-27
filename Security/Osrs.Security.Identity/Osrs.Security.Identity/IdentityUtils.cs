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

namespace Osrs.Security.Identity
{
    public static class IdentityUtils
    {
        private static readonly Guid create = new Guid("{4A38823A-F06B-4BA3-9171-2CBA33E6769C}");
        public static Guid CreatePermisionId
        {
            get { return create; }
        }

        private static readonly Guid get = new Guid("{D5DA3BF4-7B86-4191-A810-662DEDFA2888}");
        public static Guid GetPermissionId
        {
            get { return get; }
        }

        private static readonly Guid update = new Guid("{1E3400E4-694F-4A68-8ECA-BDD1E0AFC9BE}");
        public static Guid UpdatePermissionId
        {
            get { return update; }
        }

        private static readonly Guid delete = new Guid("{9266DDA2-A282-4DAF-8169-6F368D864E4A}");
        public static Guid DeletePermissionId
        {
            get { return delete; }
        }

        public static bool IsActive(this IIdentityProvider provider, Guid uid)
        {
            if (provider != null)
            {
                UserIdentityBase uib = provider.Get(uid);
                if (uib != null)
                    return uib.UserState == UserState.Active;
            }

            return false;
        }

        public static bool IsActive(this IIdentityProvider provider, IUserIdentity user)
        {
            if (provider != null && user!=null)
            {
                UserIdentityBase uib = provider.Get(user.Uid);
                if (uib!=null)
                    return uib.UserState == UserState.Active;
            }

            return false;
        }
    }
}
