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
using System;
using System.Collections.Generic;

namespace Osrs.Security.Authorization
{
    public enum GrantType
    {
        Grant,
        Deny
    }

    public enum OperationType
    {
        Create,
        Retrive,
        Update,
        Delete,
        Purge,
        Execute,
        Manage
    }

    public enum AccessType
    {
        /// <summary>
        /// Class, Table or Entity level access - the most general
        /// </summary>
        Entity,
        /// <summary>
        /// Instance, Row or Object level access - grant or deny to a specific instance, must be updated on deletes
        /// </summary>
        Instance,
        /// <summary>
        /// Field, Column, Property or Attribute level access - grant or deny to a specific field within an entity
        /// </summary>
        Property,
        /// <summary>
        /// Value or cell level access - grant or deny to a specific property on a specific instance, must be updated on deletes
        /// </summary>
        Value,
        /// <summary>
        /// Specialized location-based access control, requires use of location data on both user requesting access and entity being requested.
        /// </summary>
        Spatial
    }

    public static class PermissionUtils
    {
        public static IEnumerable<Permission> CompactToEffective(IEnumerable<PermissionAssignment> assigns)
        {
            HashSet<Permission> grants = new HashSet<Permission>();
            HashSet<Permission> denies = new HashSet<Permission>();
            foreach (PermissionAssignment cur in assigns)
            {
                if (cur.GrantType == GrantType.Grant)
                    grants.Add(cur.Permission);
                else
                    denies.Add(cur.Permission);
            }

            foreach (Permission cur in denies)
            {
                if (grants.Contains(cur))
                    grants.Remove(cur);
            }
            return grants;
        }

        private static readonly Guid manage = new Guid("{9E19DE9F-7557-46F7-B9D7-FB2039220264}");
        public static Guid ManagePermissionsPermissionId
        {
            get { return manage; }
        }

        private static readonly Guid create = new Guid("{02BFEB8C-720C-4F0F-AF5C-E32303C60597}");
        public static Guid CreateRolePermissionId
        {
            get { return create; }
        }

        private static readonly Guid delete = new Guid("{1D798E7A-B41C-4892-9751-3D601500F9BA}");
        public static Guid DeleteRolePermissionId
        {
            get { return delete; }
        }

        private static readonly Guid modRole = new Guid("{7EE1D8DE-FB0C-44B5-B411-A6C48069D6BC}");
        public static Guid ManageRolePermissionId
        {
            get { return modRole; }
        }

        public static string PermissionName(ContextScopeElement scope, AccessType access, string name)
        {
            return PermissionUtils.PermissionName(scope) +
                PermissionUtils.PermissionName(access) + name;
        }
        public static string PermissionName(OperationType opType, AccessType access, string name)
        {
            return PermissionUtils.PermissionName(opType) +
                PermissionUtils.PermissionName(access) + name;
        }
        public static string PermissionName(OperationType opType, ContextScopeElement scope, string name)
        {
            return PermissionUtils.PermissionName(opType) +
                PermissionUtils.PermissionName(scope) + name;
        }
        public static string PermissionName(OperationType opType, ContextScopeElement scope, AccessType access, string name)
        {
            return PermissionUtils.PermissionName(opType) +
                PermissionUtils.PermissionName(scope) +
                PermissionUtils.PermissionName(access) + name;
        }
        public static string PermissionName(OperationType opType, string name)
        {
            return PermissionUtils.PermissionName(opType) + name;
        }
        public static string PermissionName(OperationType opType)
        {
            switch (opType)
            {
                case OperationType.Create:
                    return "New";
                case OperationType.Delete:
                    return "Del";
                case OperationType.Execute:
                    return "Run";
                case OperationType.Purge:
                    return "Kill";
                case OperationType.Retrive:
                    return "Get";
                case OperationType.Update:
                    return "Edit";
                case OperationType.Manage:
                    return "Man";
                default:
                    return "";
            }
        }
        public static string PermissionName(ContextScopeElement scope)
        {
            switch (scope)
            {
                case ContextScopeElement.Application:
                    return "App";
                case ContextScopeElement.Global:
                    return "Gbl";
                case ContextScopeElement.Module:
                    return "Mod";
                case ContextScopeElement.System:
                    return "Sys";
                case ContextScopeElement.Container:
                    return "Con";
                case ContextScopeElement.Enterprise:
                    return "Ent";
                case ContextScopeElement.HostingSite:
                    return "Fac";
                case ContextScopeElement.Machine:
                    return "Srv";
                case ContextScopeElement.MachineGroup:
                    return "Clu";
                case ContextScopeElement.Process:
                    return "Pro";
                case ContextScopeElement.Request:
                    return "Req";
                case ContextScopeElement.Service:
                    return "Svc";
                case ContextScopeElement.User:
                    return "Usr";
                default:
                    return "Unk";
            }
        }
        public static string PermissionName(AccessType access)
        {
            switch (access)
            {
                case AccessType.Entity:
                    return "Tbl";
                case AccessType.Instance:
                    return "Rec";
                case AccessType.Property:
                    return "Col";
                case AccessType.Value:
                    return "Fld";
                case AccessType.Spatial:
                    return "Loc";
                default:
                    return "Unk";
            }
        }
    }
}
