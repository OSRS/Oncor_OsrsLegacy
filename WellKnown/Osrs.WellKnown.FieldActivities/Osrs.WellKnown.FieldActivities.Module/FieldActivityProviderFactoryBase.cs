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
using Osrs.Security;
using Osrs.Security.Authorization;

namespace Osrs.WellKnown.FieldActivities
{
    public abstract class FieldActivityProviderFactoryBase : SubclassableSingletonBase<FieldActivityProviderFactoryBase>
    {
        protected internal UserSecurityContext LocalContext
        {
            get;
            internal set;
        }

        protected internal abstract bool Initialize();

        //----------------------------------------------
        protected internal static Permission FieldActivityCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldActivity"), FieldActivityUtils.FieldActivityCreatePermissionId);
            }
        }

        protected internal static Permission FieldActivityGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldActivity"), FieldActivityUtils.FieldActivityGetPermissionId);
            }
        }

        protected internal static Permission FieldActivityUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldActivity"), FieldActivityUtils.FieldActivityUpdatePermissionId);
            }
        }

        protected internal static Permission FieldActivityDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldActivity"), FieldActivityUtils.FieldActivityDeletePermissionId);
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        protected internal static Permission FieldTripCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTrip"), FieldActivityUtils.FieldTripCreatePermissionId);
            }
        }

        protected internal static Permission FieldTripGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTrip"), FieldActivityUtils.FieldTripGetPermissionId);
            }
        }

        protected internal static Permission FieldTripUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTrip"), FieldActivityUtils.FieldTripUpdatePermissionId);
            }
        }

        protected internal static Permission FieldTripDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTrip"), FieldActivityUtils.FieldTripDeletePermissionId);
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        protected internal static Permission SampleEventCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "SampleEvent"), FieldActivityUtils.SampleEventCreatePermissionId);
            }
        }

        protected internal static Permission SampleEventGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "SampleEvent"), FieldActivityUtils.SampleEventGetPermissionId);
            }
        }

        protected internal static Permission SampleEventUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "SampleEvent"), FieldActivityUtils.SampleEventUpdatePermissionId);
            }
        }

        protected internal static Permission SampleEventDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "SampleEvent"), FieldActivityUtils.SampleEventDeletePermissionId);
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        protected internal static Permission FieldTeamCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeam"), FieldActivityUtils.FieldTeamCreatePermissionId);
            }
        }

        protected internal static Permission FieldTeamGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeam"), FieldActivityUtils.FieldTeamGetPermissionId);
            }
        }

        protected internal static Permission FieldTeamUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeam"), FieldActivityUtils.FieldTeamUpdatePermissionId);
            }
        }

        protected internal static Permission FieldTeamDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeam"), FieldActivityUtils.FieldTeamDeletePermissionId);
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        protected internal static Permission FieldTeamMemberRoleCreatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Create, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleCreatePermissionId);
            }
        }

        protected internal static Permission FieldTeamMemberRoleGetPermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Retrive, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleGetPermissionId);
            }
        }

        protected internal static Permission FieldTeamMemberRoleUpdatePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Update, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleUpdatePermissionId);
            }
        }

        protected internal static Permission FieldTeamMemberRoleDeletePermission
        {
            get
            {
                return new Permission(PermissionUtils.PermissionName(OperationType.Delete, "FieldTeamMemberRole"), FieldActivityUtils.FieldTeamMemberRoleDeletePermissionId);
            }
        }
        //----------------------------------------------

        protected internal abstract FieldActivityProviderBase GetFieldActivityProvider(UserSecurityContext context);

        protected internal abstract FieldTripProviderBase GetFieldTripProvider(UserSecurityContext context);

        protected internal abstract FieldTeamProviderBase GetFieldTeamProvider(UserSecurityContext context);

        protected internal abstract FieldTeamMemberRoleProviderBase GetFieldTeamMemberRoleProvider(UserSecurityContext context);

        protected internal abstract SampleEventProviderBase GetSampleEventProvider(UserSecurityContext context);
    }
}
