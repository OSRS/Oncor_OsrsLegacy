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

namespace Osrs.Security.Authorization.Providers
{
    internal sealed class RoleMemorySet
    {
        private static RoleMemorySet instance = null;
        internal static RoleMemorySet Instance
        {
            get { return instance; }
        }

        private readonly Dictionary<Guid, Role> roles = new Dictionary<Guid, Role>();
        private readonly Dictionary<string, Guid> roleNames = new Dictionary<string, Guid>();

        internal IEnumerable<Role> GetRoles()
        {
            return roles.Values;
        }

        internal IEnumerable<Role> GetRoles(IEnumerable<Guid> roleIds)
        {
            if (roleIds != null)
            {
                List<Role> tmp = new List<Role>();
                foreach (Guid cur in roleIds)
                {
                    if (roles.ContainsKey(cur))
                        tmp.Add(roles[cur]);
                }
                return tmp;
            }
            return null;
        }

        internal Role Get(string name)
        {
            try
            {
                if (roleNames.ContainsKey(name))
                {
                    return roles[roleNames[name]];
                }
            }
            catch
            { }
            return null;
        }
        internal Role Get(Guid id)
        {
            try
            {
                if (roles.ContainsKey(id))
                {
                    return roles[id];
                }
            }
            catch
            { }
            return null;
        }

        internal bool Exists(string name)
        {
            return roleNames.ContainsKey(name);
        }
        internal bool Exists(Guid id)
        {
            return roles.ContainsKey(id);
        }

        internal bool Update(Role r)
        {
            string tmpName=null;
            foreach(KeyValuePair<string, Guid> item in roleNames)
            {
                if (item.Value == r.Id)
                {
                    if (item.Key != r.Name)
                    {
                        tmpName = r.Name;
                    }
                    break;
                }
            }

            if (tmpName!=null)
            {
                roleNames.Remove(tmpName);
                roleNames[r.Name] = r.Id;
            }
            return true;
        }

        internal bool RegisterRole(Role role)
        {
            try
            {
                roles.Add(role.Id, role);
                roleNames.Add(role.Name, role.Id);
                return true;
            }
            catch
            { }
            return false;
        }
        internal bool UnregisterRole(Role role)
        {
            try
            {
                if (roles.Remove(role.Id))
                {
                    roleNames.Remove(role.Name);
                    RoleMembershipMemorySet.Instance.RemoveRole(role.Id); //notice this doesn't get called during reset
                    return true;
                }
            }
            catch
            { }
            return false;
        }

        internal static bool Reset(IRoleProvider prov)
        {
            try
            {
                IEnumerable<Role> perms = prov.GetRoles();
                if (perms != null)
                {
                    RoleMemorySet tmp = new RoleMemorySet();
                    foreach (Role p in perms)
                    {
                        tmp.RegisterRole(p);
                    }
                    instance = tmp;
                    return true;
                }
            }
            catch
            { }

            return false;
        }
    }
}
