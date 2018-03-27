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

namespace Osrs.WellKnown.Taxonomy
{
	public static class TaxonomyUtils
	{
		public static readonly Guid TaxaCommonNameCreatePermissionId = new Guid("{70BF5022-E503-464E-9A44-851249F84684}");
		public static readonly Guid TaxaCommonNameGetPermissionId = new Guid("{2DC0F3D2-AB46-4488-B196-F50211353B32}");
		public static readonly Guid TaxaCommonNameUpdatePermissionId = new Guid("{8BE358DA-76D1-4449-B0B1-9E2F99DEEEA4}");
		public static readonly Guid TaxaCommonNameDeletePermissionId = new Guid("{D4827756-6498-42FA-AFC7-3AE09CA33EC9}");

		public static readonly Guid TaxaUnitCreatePermissionId = new Guid("{5B2D9ACD-12EA-4F4B-9738-74C807AD7516}");
		public static readonly Guid TaxaUnitGetPermissionId = new Guid("{20EDBCF8-7742-4861-8A0E-FBFAC78E4976}");
		public static readonly Guid TaxaUnitUpdatePermissionId = new Guid("{38908AD0-D1F5-4AE4-BB59-F184A2812700}");
		public static readonly Guid TaxaUnitDeletePermissionId = new Guid("{0DBF348C-AB46-40F3-B081-FCECA45B9FAF}");

		public static readonly Guid TaxonomyCreatePermissionId = new Guid("{88ED5DD1-3B8F-4D2B-AB6E-B84EC8A6198C}");
		public static readonly Guid TaxonomyGetPermissionId = new Guid("{08FB495D-D4EB-4445-BA22-26DF677E3D40}");
		public static readonly Guid TaxonomyUpdatePermissionId = new Guid("{18E3674E-8352-453A-8BAF-B1A43986FAA6}");
		public static readonly Guid TaxonomyDeletePermissionId = new Guid("{71B01750-BDB2-4598-B950-DC4427B970A6}");
	}
}
