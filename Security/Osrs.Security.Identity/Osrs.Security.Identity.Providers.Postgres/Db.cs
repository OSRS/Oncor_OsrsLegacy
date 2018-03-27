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
using System;
using System.Data;

namespace Osrs.Security.Identity.Providers
{
    internal static class Db
    {
        internal static string ConnectionString;

        internal const string CountSelect = "SELECT COUNT(*) FROM osrs.\"SecurityIdentity\"";
        internal const string Select = "SELECT \"Id\", \"UserType\", \"UserState\", \"ExpiresAt\", \"Name\" FROM osrs.\"SecurityIdentity\"";
        internal const string Insert = "INSERT INTO osrs.\"SecurityIdentity\"(\"Id\", \"UserType\", \"UserState\", \"ExpiresAt\", \"Name\") VALUES (:id, :ut, :us, :ea, :name)";
        internal const string Update = "UPDATE osrs.\"SecurityIdentity\" SET \"UserType\"=:ut, \"UserState\"=:us, \"ExpiresAt\"=:ea, \"Name\"=:name WHERE \"Id\"=:id";
        internal const string Delete = "DELETE FROM osrs.\"SecurityIdentity\" WHERE \"Id\"=:id";

        internal const string SelectById = " WHERE \"Id\"=:id";
        internal const string SelectByIdType = " WHERE \"Id\"=:id AND \"UserType\"=:ut";
        internal const string SelectByType = " WHERE \"UserType\"=:ut";
        internal const string SelectByName = " WHERE lower(\"Name\")=lower(:name)";
        internal const string SelectByNameType = " WHERE lower(\"Name\")=lower(:name) AND \"UserType\"=:ut";

        internal static NpgsqlParameter GetEAParam(DateTime? expiresAt, System.Data.ParameterDirection direction)
        {
            NpgsqlParameter p = new NpgsqlParameter("ea", NpgsqlTypes.NpgsqlDbType.TimestampTZ);
            p.Direction = direction;
            p.IsNullable = true;
            if (expiresAt.HasValue)
                p.Value = expiresAt.Value;
            else
                p.Value = DBNull.Value;
            return p;
        }

        internal static NpgsqlConnection GetCon(string conString)
        {
            try
            {
                NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder(conString);
                if (sb.Timeout == 15) //default
                    sb.Timeout = 60;
                if (sb.CommandTimeout == 30) //default
                    sb.CommandTimeout = 60;
                sb.Pooling = false;
                NpgsqlConnection conn = new NpgsqlConnection(sb.ToString());
                return conn;
            }
            catch
            { }
            return null;
        }

        internal static NpgsqlCommand GetCmd(NpgsqlConnection con)
        {
            if (con == null)
                return null;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                if (cmd != null)
                {
                    cmd.Connection = con;
                    return cmd;
                }
            }
            catch
            { }
            return null;
        }

        internal static NpgsqlCommand GetCmd(string conString)
        {
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = GetCon(conString);
                return cmd;
            }
            catch
            { }
            return null;
        }

        internal static int ExecuteNonQuery(NpgsqlCommand cmd)
        {
            int res = int.MinValue;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                res = cmd.ExecuteNonQuery();
            }
            catch
            { }

            try
            {
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
            }
            catch
            { }

            return res;
        }

        internal static NpgsqlDataReader ExecuteReader(NpgsqlCommand cmd)
        {
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                return cmd.ExecuteReader();
            }
            catch
            { }
            return null;
        }

        internal static void Close(NpgsqlConnection con)
        {
            try
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
            catch
            { }
        }

        internal static void Close(NpgsqlCommand cmd)
        {
            if (cmd != null && cmd.Connection != null)
                Close(cmd.Connection);
        }

        internal static bool Exists(NpgsqlCommand cmd)
        {
            NpgsqlDataReader rdr = null;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                rdr = ExecuteReader(cmd);
                rdr.Read();

                try
                {
                    long ct = (long)(rdr[0]);
                    if (cmd.Connection.State == System.Data.ConnectionState.Open)
                        cmd.Connection.Close();

                    return ct > 0L;
                }
                catch
                { }
            }
            catch
            {
                Close(cmd);
            }
            finally
            {
                cmd.Dispose();
            }
            return false;
        }
    }
}
