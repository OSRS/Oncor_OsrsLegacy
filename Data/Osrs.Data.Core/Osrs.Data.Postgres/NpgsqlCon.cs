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
using System.Threading;

namespace Osrs.Data.Postgres
{
    public class NpgsqlCon
    {
        public ConnectionState State
        {
            get { return this.conn.State; }
        }

        private NpgsqlConnection conn;
        public NpgsqlConnection Connection
        {
            get { return this.conn; }
        }

        private string connStr;
        public string ConnectionString
        {
            get { return this.connStr; }
        }

        private string schema;
        public string Schema
        {
            get { return this.schema; }
        }

        private NpgsqlCon()
        { }

        private NpgsqlCon(string connectionString, string schema)
        {
            this.connStr = connectionString;
            this.schema = schema;
            this.conn = new NpgsqlConnection(connectionString);
        }

        internal NpgsqlConnection SafeOpen()
        {
            try
            {
                if (this.conn.State == ConnectionState.Closed)
                {
                    this.conn.Open();
                    return this.conn;
                }
                else
                {   //Wait for operation to be complete, up to 5 seconds
                    int i = 0;
                    while (this.conn.State != ConnectionState.Open)
                    {
                        if (i > 50 || this.conn.State == ConnectionState.Broken)
                        {
                            break;
                        }
                        i++;
                        Thread.Sleep(100);
                    }
                    if (this.conn.State == ConnectionState.Open)
                    {
                        return this.conn;
                    }
                    if (this.SafeClose())
                    {
                        this.conn.Open();
                        return this.conn;
                    }
                }
            }
            catch
            {
                if (this.Reset())
                {
                    return this.conn;
                }
            }
            return null;
        }

        private bool Reset()
        {
            this.conn.Close();
            this.conn = new NpgsqlConnection(this.connStr);
            try
            {
                this.conn.Open();
                return true;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
            }
#pragma warning restore 0168
            return false;
        }

        internal bool SafeClose()
        {
            try
            {
                this.conn.Close();
            }
#pragma warning disable 0168
            catch (Exception e)
            {
            }
#pragma warning restore 0168
            if (this.conn.State != ConnectionState.Closed)
            {
                this.conn.Dispose();
                this.conn = null;
                this.conn = new NpgsqlConnection(this.connStr);
            }
            return true;
        }

        public void Open()
        {
            this.SafeOpen();
        }

        public void Close()
        {
            this.SafeClose();
        }

        public NpgsqlCmd GetCommand(string procName)
        {
            if (string.IsNullOrEmpty(procName))
            {
                return null;
            }
            return new NpgsqlCmd(this, procName);
        }

        public static NpgsqlCon Create(string connectionString, string schema)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(schema))
                {
                    return null;
                }
                if (!NpgSqlCommandUtils.TestConnection(connectionString))
                {
                    return null;
                }
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return null;
            }
#pragma warning restore 0168
            return new NpgsqlCon(connectionString, schema);
        }
    }
}
