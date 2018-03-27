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
using System.Data;

namespace Osrs.Data.Postgres
{
    public class NpgsqlCmd
    {
        private NpgsqlCon con;

        private NpgsqlCommand cmd;
        public NpgsqlCommand Command
        {
            get { return this.cmd; }
        }

        private NpgsqlCmd()
        {
        }

        internal NpgsqlCmd(NpgsqlCon con, string procName)
        {
            this.con = con;
            this.cmd = new NpgsqlCommand("\"" + con.Schema + "\".\"" + procName + "\"");
            this.cmd.CommandType = CommandType.StoredProcedure;
        }

        public void AddNullInParam(string name, NpgsqlDbType dbType)
        {
            NpgsqlParameter p = NpgSqlCommandUtils.GetParam(name, true, dbType, ParameterDirection.Input);
            p.Value = DBNull.Value;
            this.cmd.Parameters.Add(p);
        }

        public NpgsqlParameter AddInParam(string name, bool isNullable, NpgsqlDbType dbType)
        {
            NpgsqlParameter p = NpgSqlCommandUtils.GetParam(name, isNullable, dbType, ParameterDirection.Input);
            this.cmd.Parameters.Add(p);
            return p;
        }

        public void AddInParamWithValue(string name, object value)
        {
            this.cmd.Parameters.AddWithValue(name, value);
        }

        public void AddOutParam(string name, bool isNullable, NpgsqlDbType dbType)
        {
            NpgsqlParameter p = NpgSqlCommandUtils.GetParam(name, isNullable, dbType, ParameterDirection.Output);
            this.cmd.Parameters.Add(p);
        }

        public NpgsqlDataReader ExecuteReader()
        {
            NpgsqlConnection conn = this.con.SafeOpen();
            if (conn == null || conn.State != ConnectionState.Open)
            {
                return null;
            }
            this.cmd.Connection = conn;
            NpgsqlDataReader rdr = null;
            try
            {
                rdr = this.cmd.ExecuteReader();
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                this.Cancel();
                return null;
            }
#pragma warning restore 0168
            return rdr;
        }

        public DbOpResultStatus ExecuteScalar()
        {
            NpgsqlConnection conn = this.con.SafeOpen();
            if (conn == null || conn.State != ConnectionState.Open)
            {
                return new DbOpResultStatus(DbOpStatus.FailedOpen);
            }

            object o = null;
            try
            {
                this.cmd.Connection = conn;
                o = this.cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                this.con.SafeClose();
                return new DbOpResultStatus(DbOpStatus.FailedExecute, o, e);
            }

            cmd.Cancel();
            cmd.Connection = null;
            if (!this.con.SafeClose())
            {
                return new DbOpResultStatus(DbOpStatus.FailedClose, o);
            }
            return new DbOpResultStatus(o);
        }

        public DbOpResultStatus ExecuteNonQuery()
        {
            NpgsqlConnection conn = this.con.SafeOpen();
            if (conn == null || conn.State != ConnectionState.Open)
            {
                return new DbOpResultStatus(DbOpStatus.FailedOpen);
            }

            int i = 0;
            try
            {
                this.cmd.Connection = conn;
                i = this.cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                this.con.SafeClose();
                return new DbOpResultStatus(DbOpStatus.FailedExecute, i, e);
            }

            cmd.Cancel();
            cmd.Connection = null;
            if (!this.con.SafeClose())
            {
                return new DbOpResultStatus(DbOpStatus.FailedClose, i);
            }
            return new DbOpResultStatus(i);
        }

        public void ProcessResult(DbOpResultStatus op, string methodName)
        {
            string msg = methodName;
            if (string.IsNullOrEmpty(methodName))
            {
                msg = "DbExecute";
            }
            switch (op.Status)
            {
                case DbOpStatus.FailedOpen:
                    throw new DataException(msg);
                case DbOpStatus.FailedExecute:
                    throw new DataException(msg);
            }
        }

        public bool Cancel()
        {
            this.cmd.Cancel();
            string procName = this.cmd.CommandText;
            this.cmd = new NpgsqlCommand("\"" + con.Schema + "\".\"" + procName + "\"");
            this.cmd.CommandType = CommandType.StoredProcedure;
            return this.con.SafeClose();
        }
    }
}
