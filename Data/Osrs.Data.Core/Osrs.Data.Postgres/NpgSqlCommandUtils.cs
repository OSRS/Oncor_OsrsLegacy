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
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Osrs.Data.Postgres
{
    public static class NpgSqlCommandUtils
    {
        public static NpgsqlParameter GetParam(string name, bool isNullable, NpgsqlDbType dbType, ParameterDirection dir)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            NpgsqlParameter p = new NpgsqlParameter();
            p.ParameterName = name;
            p.NpgsqlDbType = dbType;
            p.Direction = dir;
            p.IsNullable = isNullable;
            if (dir == ParameterDirection.Output)
            {
                if (dbType == NpgsqlDbType.Varchar || dbType == NpgsqlDbType.Text)
                {
                    p.Size = -1;
                }
            }
            return p;
        }

        public static NpgsqlParameter GetInParam(string name, bool isNullable, NpgsqlDbType dbType)
        {
            return NpgSqlCommandUtils.GetParam(name, isNullable, dbType, ParameterDirection.Input);
        }

        public static NpgsqlParameter GetOutParam(string name, bool isNullable, NpgsqlDbType dbType)
        {
            return NpgSqlCommandUtils.GetParam(name, isNullable, dbType, ParameterDirection.Output);
        }

        public static NpgsqlParameter GetNullInParam(string name, NpgsqlDbType dbType)
        {
            NpgsqlParameter p = NpgSqlCommandUtils.GetParam(name, true, dbType, ParameterDirection.Input);
            p.Value = DBNull.Value;
            return p;
        }

        public static bool TestConnection(string connString)
        {
            if (connString == null || string.Empty.Equals(connString))
            {
                return false;
            }
            NpgsqlConnection con = new NpgsqlConnection(connString);
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = con;
            try
            {
                con.Open();
            }
            catch
            {
                cmd.Dispose();
                cmd = null;
                con.Dispose();
                con = null;
                return false;
            }
            cmd.Dispose();
            cmd = null;
            try
            {
                con.Close();
            }
#pragma warning disable 0168
            catch (NpgsqlException e)
            {
            }
#pragma warning restore 0168
            con.Dispose();
            con = null;
            return true;
        }

        public static void PrepareCommand(NpgsqlCommand cmd, NpgsqlParameterCollection cmdParms)
        {
            if (cmdParms != null)
            {
                bool hasRet = false;
                if (cmd.CommandType != CommandType.StoredProcedure)
                    hasRet = true;
                NpgsqlParameter p2;
                foreach (NpgsqlParameter parm in cmdParms)
                {
                    if (parm != null)
                    {
                        if (parm.Direction == ParameterDirection.ReturnValue)
                            hasRet = true;
                        p2 = parm.Clone();
                        cmd.Parameters.Add(p2);
                    }
                }
                if (!hasRet)
                    cmd.Parameters.Add(NpgSqlCommandUtils.GetParam("retVal", true, NpgsqlDbType.Integer, ParameterDirection.ReturnValue));
            }
        }

        public static NpgsqlParameter Clone(this NpgsqlParameter parm)
        {
            if (parm == null)
                return null;
            NpgsqlParameter p = new NpgsqlParameter();
            p.Direction = parm.Direction;
            p.IsNullable = parm.IsNullable;
            p.DbType = parm.DbType;
            p.NpgsqlDbType = parm.NpgsqlDbType;
            p.ParameterName = parm.ParameterName;
            p.Precision = parm.Precision;
            p.Scale = parm.Scale;
            p.Size = parm.Size;
            p.Value = parm.Value;

            return p;
        }

        public static string FormatGroupBy(string name)
        {
            return "GROUP BY \"" + name + "\"";
        }
        public static string FormatGroupBy(IEnumerable<string> names)
        {
            if (names == null)
                return null;

            StringBuilder sb = new StringBuilder();
            sb.Append("GROUP BY ");
            foreach (string s in names)
            {
                sb.Append('\"');
                sb.Append(s);
                sb.Append("\",");
            }
            sb.Length = sb.Length - 1;
            return sb.ToString();
        }

        public static string FormatName(string item)
        {
            return "\"" + item + "\"";
        }
        public static string FormatName(string schema, string item)
        {
            if (string.IsNullOrEmpty(schema))
                return "\"" + item + "\"";
            else
                return "\"" + schema + "\".\"" + item + "\"";
        }
        public static string FormatName(string database, string schema, string item)
        {
            string ret = "\"";
            if (!string.IsNullOrEmpty(database))
            {
                ret += database + "\".\"";
            }
            if (!string.IsNullOrEmpty(schema))
            {
                ret += schema + "\".\"";
            }
            return ret + item + "\"";
        }
    }
}
