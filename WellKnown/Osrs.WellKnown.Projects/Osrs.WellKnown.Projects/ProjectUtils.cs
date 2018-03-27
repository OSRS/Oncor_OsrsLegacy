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

namespace Osrs.WellKnown.Projects
{
    public static class ProjectUtils
    {
        public static readonly Guid ProjectCreatePermissionId = new Guid("{A4A1582A-A756-4DC3-BBD4-B9B3FEF3C033}");
        public static readonly Guid ProjectGetPermissionId = new Guid("{6CFAD508-882F-4C55-94D3-6C18BAF99146}");
        public static readonly Guid ProjectUpdatePermissionId = new Guid("{62A875D1-5E5F-4F24-806F-FE7C68EB7AB4}");
        public static readonly Guid ProjectDeletePermissionId = new Guid("{67F522BA-F930-493C-A2DE-C40189165D3B}");

        public static readonly Guid ProjectStatusTypeCreatePermissionId = new Guid("{E5710D9C-DD20-435C-9E73-331F108B7C75}");
        public static readonly Guid ProjectStatusTypeGetPermissionId = new Guid("{88586ABB-3B99-4D74-A1FD-6EB10B84F836}");
        public static readonly Guid ProjectStatusTypeUpdatePermissionId = new Guid("{4272D37D-F6EE-4899-B1C1-E4696FBE4799}");
        public static readonly Guid ProjectStatusTypeDeletePermissionId = new Guid("{CB04F2E6-F017-412B-BB6F-D49694A218DC}");
    }
}
