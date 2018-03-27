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
using System.Collections.Generic;
using System.Threading;

namespace Osrs.Data.Postgres
{
    public sealed class NpgSqlCommandGenerator
    {
        /// <summary>
        /// Creates a new SqlCommand for the connection string
        /// </summary>
        /// <param name="cmdText">SQL Statment text</param>
        /// <param name="connectionString">Connestion string</param>
        /// <returns>SqlCommand</returns>
        public static NpgsqlCommand NewCommand(string cmdText, string connectionString)
        {
            return NewCommand(cmdText, new NpgsqlConnection(connectionString));
        }

        /// <summary>
        /// Creates a new SqlCommand for the connection string
        /// </summary>
        /// <param name="cmdText">SQL Statment text</param>
        /// <param name="con">Sql Connection object</param>
        /// <returns>SqlCommand</returns>
        public static NpgsqlCommand NewCommand(string cmdText, NpgsqlConnection con)
        {
            if (con == null)
                return null;
            NpgsqlCommand cmd = new NpgsqlCommand(cmdText, con);
            int ct = 0;
            while (con.State != System.Data.ConnectionState.Open)
            {
                if (ct > 10)
                    return null;
                try
                {
                    if (con.State == System.Data.ConnectionState.Closed)
                        con.Open();
                    else if (con.State == System.Data.ConnectionState.Open)
                        break;
                    else
                    {
                        Thread.Sleep(10);
                        con.Open();
                    }
                }
                catch
                {
                }
                ct++;
            }
            return cmd;
        }
    }

    public sealed class EnumerableCommand<T>
    {
        /// <summary>
        /// Gets the IBuilder of type T
        /// </summary>
        public IBuilder<T> Builder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the SqlCommand command text
        /// </summary>
        public string CommandText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the SqlConnection connection string
        /// </summary>
        public string ConnectionString
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of SqlParameters for Sql queries
        /// </summary>
        public List<NpgsqlParameter> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Instantiates EnumerableCommand using specified parameters and instantiates a new list of SqlParameters
        /// </summary>
        /// <param name="builder">Builder of type T</param>
        /// <param name="commandText">Sql Command text</param>
        /// <param name="connectionString">Sql Command connection string</param>
        public EnumerableCommand(IBuilder<T> builder, string commandText, string connectionString)
        {
            Builder = builder;
            CommandText = commandText;
            ConnectionString = connectionString;

            this.Parameters = new List<NpgsqlParameter>();
        }

        /// <summary>
        /// Creates a new SqlCommand object using EnumerableCommand initialized properties
        /// </summary>
        /// <returns>SqlCommand</returns>
        internal NpgsqlCommand NewCommand()
        {
            string c = this.CommandText;
            if (string.IsNullOrEmpty(c))
                return null;
            NpgsqlCommand cmd = NpgSqlCommandGenerator.NewCommand(c, ConnectionString);
            List<NpgsqlParameter> parms = this.Parameters;
            if (parms.Count > 0)
            {
                foreach (NpgsqlParameter p in parms)
                    cmd.Parameters.Add(new NpgsqlParameter(p.ParameterName, p.Value));
            }
            return cmd;
        }
    }

    public sealed class Enumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Enumerable Command of type T
        /// </summary>
        private EnumerableCommand<T> cmd;

        /// <summary>
        /// Instantiates the Enumerable class
        /// </summary>
        /// <param name="cmd">Enumberable Command of type T</param>
        public Enumerable(EnumerableCommand<T> cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            this.cmd = cmd;
        }

        /// <summary>
        /// Provides access to the enumerable collection of objects (of type T)
        /// </summary>
        /// <returns>Enumerable collection of table DataRows</returns>
        public IEnumerator<T> GetEnumerator()
        {
            NpgsqlCommand com = this.cmd.NewCommand();
            return new Enumerator<T>(this.cmd.Builder, com);
        }

        /// <summary>
        /// Provides access to the enumerable collection of objects (of type T)
        /// </summary>
        /// <returns>Enumberable collection of objects</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public sealed class Enumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// IBuilder of type T
        /// </summary>
        private IBuilder<T> builder;

        /// <summary>
        /// SqlDataReader
        /// </summary>
        private NpgsqlDataReader reader;

        /// <summary>
        /// SqlCommand
        /// </summary>
        private NpgsqlCommand com;

        /// <summary>
        /// Maintains the current object (of type T)
        /// </summary>
        private T current;
        /// <summary>
        /// Gets the current object (of type T)
        /// </summary>
        public T Current
        {
            get { return this.current; }
        }

        /// <summary>
        /// Instantiates Enumeration with specified SqlCommand object and Sql Builder
        /// </summary>
        /// <param name="builder">IBuilder</param>
        /// <param name="com">SqlCommand</param>
        public Enumerator(IBuilder<T> builder, NpgsqlCommand com)
        {
            this.builder = builder;
            this.com = com;

            if (this.com.Connection.State == System.Data.ConnectionState.Closed)
                this.com.Connection.Open();

            this.reader = com.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Runs clean up and disposes of the IBuilder
        /// </summary>
        public void Dispose()
        {
            this.builder = null;
            this.Cleanup();
        }

        /// <summary>
        /// Disposes of the SqlDataReader and SqlConnection objects
        /// </summary>
        private void Cleanup()
        {
            if (this.com != null)
            {
                NpgsqlConnection con = this.com.Connection;
                if (con != null)
                {
                    try
                    {
                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                        con.Dispose();
                    }
                    catch
                    { }
                }
                try
                {
                    this.com.Dispose();
                }
                catch
                { }
                this.com = null;
            }
            if (this.reader != null)
            {
                try
                {
                    this.reader.Dispose();
                }
                catch
                { }
                this.reader = null;
            }
        }

        /// <summary>
        /// Gets the current Enumerator object 
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.current; }
        }

        /// <summary>
        /// Moves to the next object in the Enumerator
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (this.reader != null && this.reader.Read())
            {
                this.current = this.builder.Build(this.reader);
                while (this.current == null && this.reader.Read())
                    this.current = this.builder.Build(this.reader);
                if (this.current != null)
                    return true;
            }
            if (this.reader != null)
                this.Cleanup();
            return false;
        }

        /// <summary>
        /// Runs clean up
        /// </summary>
        public void Reset()
        {
            this.Cleanup();
        }
    }
}
