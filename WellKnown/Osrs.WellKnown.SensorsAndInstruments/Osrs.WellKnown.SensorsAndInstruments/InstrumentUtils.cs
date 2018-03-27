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

namespace Osrs.WellKnown.SensorsAndInstruments
{
	public static class InstrumentUtils
	{
		public static readonly Guid InstrumentTypeCreatePermissionId = new Guid("{1B9A9426-D766-400B-BF2F-0B8AD9FAC314}");
		public static readonly Guid InstrumentTypeGetPermissionId = new Guid("{AF9BC917-4829-48AB-BFA2-E5367DD81DED}");
		public static readonly Guid InstrumentTypeUpdatePermissionId = new Guid("{C4F2801B-8888-431E-A12C-6A43CD490969}");
		public static readonly Guid InstrumentTypeDeletePermissionId = new Guid("{91E0C642-AD20-4133-9F1C-05C06033FCEF}");

		public static readonly Guid InstrumentFamilyCreatePermissionId = new Guid("{E7DB5B1A-241B-40B9-AB81-07CA2350F1DA}");
		public static readonly Guid InstrumentFamilyGetPermissionId = new Guid("{A87A80AA-BC8D-42F9-AF85-5F203E3C634A}");
		public static readonly Guid InstrumentFamilyUpdatePermissionId = new Guid("{475B82E3-4C80-4D30-B845-E9C9ECA9B211}");
		public static readonly Guid InstrumentFamilyDeletePermissionId = new Guid("{D97BB160-CFF2-40B6-8723-DFA513095A0B}");

		public static readonly Guid InstrumentCreatePermissionId = new Guid("{8EA40EBD-09C2-4DD9-89E7-891BFEED09B6}");
		public static readonly Guid InstrumentGetPermissionId = new Guid("{3CF5F21C-3A2A-44AE-99E5-89F3B637C6DB}");
		public static readonly Guid InstrumentUpdatePermissionId = new Guid("{BDB37A69-F8B7-4A8D-BC0F-E99F217B2ED6}");
		public static readonly Guid InstrumentDeletePermissionId = new Guid("{1695C040-F2FB-4ED1-A119-91754E84CC9F}");

		public static readonly Guid SensorTypeCreatePermissionId = new Guid("{D3E55304-476E-4AB3-9662-65A890B14B11}");
		public static readonly Guid SensorTypeGetPermissionId = new Guid("{2ED71714-BB45-4108-83BB-F0B75339F4F1}");
		public static readonly Guid SensorTypeUpdatePermissionId = new Guid("{E3F262EA-2EAC-4637-826B-30A15ABB7F4C}");
		public static readonly Guid SensorTypeDeletePermissionId = new Guid("{29E9B3F4-C4A3-47BB-A107-356853C09F01}");

		public static readonly Guid SensorCreatePermissionId = new Guid("{15B55432-1548-4D1B-99F6-BBF262EF0725}");
		public static readonly Guid SensorGetPermissionId = new Guid("{925E5580-4D6F-41E6-9C0F-5C5B506AA634}");
		public static readonly Guid SensorUpdatePermissionId = new Guid("{AA97D5D6-7EEF-49FC-A0AE-30DAB29D791E}");
		public static readonly Guid SensorDeletePermissionId = new Guid("{DC3926AB-948A-4346-84AE-314F2DA8DE1F}");

		public static readonly Guid InstrumentKnownArchetypeCreatePermissionId = new Guid("{A8623EF5-84BA-4380-99A5-D1CDEC00EA26}");
		public static readonly Guid InstrumentKnownArchetypeGetPermissionId = new Guid("{77F848A4-C7C3-4788-97B6-9BB1FC04663F}");
		public static readonly Guid InstrumentKnownArchetypeUpdatePermissionId = new Guid("{0CD3D4FE-AC72-4CBB-B9D4-6608DC75DF04}");
		public static readonly Guid InstrumentKnownArchetypeDeletePermissionId = new Guid("{819DB744-8D73-4A67-86F3-D2897798B109}");
	}
}
