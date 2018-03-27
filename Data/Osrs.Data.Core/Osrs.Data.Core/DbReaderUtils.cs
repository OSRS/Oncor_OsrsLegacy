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
using System.Data.Common;

namespace Osrs.Data
{
    public static class DbReaderUtils
    {
        public static DateTime GetDate(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return DateTime.MinValue;
            return (DateTime)rdr[index];
        }

        public static bool GetBoolean(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return false;
            return (bool)rdr[index];
        }

        public static TriStateResult GetTriStateBoolean(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return TriStateResult.Unknown;
            if ((bool)rdr[index])
                return TriStateResult.True;
            return TriStateResult.False;
        }

        public static float GetSingle(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return float.NaN;
            return (float)rdr[index];
        }

        public static double GetDouble(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return double.NaN;
            return (double)rdr[index];
        }

        public static sbyte GetInt8(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return sbyte.MinValue;
            return (sbyte)rdr[index];
        }

        public static short GetInt16(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return short.MinValue;
            return (short)rdr[index];
        }

        public static int GetInt32(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return int.MinValue;
            return (int)rdr[index];
        }

        public static long GetInt64(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return long.MinValue;
            return (long)rdr[index];
        }

        public static byte GetUInt8(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return byte.MinValue;
            return (byte)rdr[index];
        }

        public static ushort GetUInt16(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return ushort.MinValue;
            return (ushort)rdr[index];
        }

        public static uint GetUInt32(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return uint.MinValue;
            return (uint)rdr[index];
        }

        public static ulong GetUInt64(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return ulong.MinValue;
            return (ulong)rdr[index];
        }

        public static Guid GetGuid(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return Guid.Empty;
            return (Guid)rdr[index];
        }

        public static DateTime? GetNullableDateTime(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (DateTime)rdr[index];
        }

        public static bool? GetNullableBoolean(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (bool)rdr[index];
        }

        public static float? GetNullableSingle(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (float)rdr[index];
        }

        public static double? GetNullableDouble(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (double)rdr[index];
        }

        public static sbyte? GetNullableInt8(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (sbyte)rdr[index];
        }

        public static short? GetNullableInt16(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (short)rdr[index];
        }

        public static int? GetNullableInt32(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (int)rdr[index];
        }

        public static long? GetNullableInt64(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (long)rdr[index];
        }

        public static byte? GetNullableUInt8(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (byte)rdr[index];
        }

        public static ushort? GetNullableUInt16(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (ushort)rdr[index];
        }

        public static uint? GetNullableUInt32(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (uint)rdr[index];
        }

        public static ulong? GetNullableUInt64(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (ulong)rdr[index];
        }

        public static Guid? GetNullableGuid(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (Guid)rdr[index];
        }

        public static string GetString(DbDataReader rdr, int index, bool clean)
        {
            if (clean)
            {
                string tmp = GetString(rdr, index);
                if (string.IsNullOrEmpty(tmp))
                    return tmp;
                return tmp.Trim();
            }
            return GetString(rdr, index);
        }
        public static string GetString(DbDataReader rdr, int index)
        {
            if (DBNull.Value.Equals(rdr[index]))
                return null;
            return (string)rdr[index];
        }
    }
}
