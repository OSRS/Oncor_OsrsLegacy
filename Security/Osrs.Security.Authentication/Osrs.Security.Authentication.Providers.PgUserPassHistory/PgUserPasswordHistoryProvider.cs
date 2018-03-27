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
using Osrs.Data;
using Osrs.Data.Postgres;
using Osrs.Runtime.Configuration;
using Osrs.Security.Passwords;
using System;
using System.Collections.Generic;
using System.Text;

namespace Osrs.Security.Authentication.Providers
{
    public sealed class PgUserPasswordHistoryProviderFactory : UserPasswordHistoryProviderFactory
    {
        protected override UserPasswordHistoryProvider GetProvider()
        {
            return new PgUserPasswordHistoryProvider();
        }

        private bool initialized = false;
        protected override bool Initialize()
        {
            lock (this)
            {
                if (!this.initialized)
                {
                    ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                    if (config!=null)
                    {
                        ConfigurationParameter param = config.Get(typeof(PgUserPasswordHistoryProviderFactory), "connectionString");
                        if (param != null)
                        {
                            string tName = param.Value as string;
                            if (!string.IsNullOrEmpty(tName))
                            {
                                if (NpgSqlCommandUtils.TestConnection(tName))
                                {
                                    Db.ConnectionString = tName;
                                    this.initialized = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public PgUserPasswordHistoryProviderFactory()
        { }
    }

    public sealed class PgUserPasswordHistoryProvider : UserPasswordHistoryProvider
    {
        public override bool Delete(Guid userId)
        {
            if (!Guid.Empty.Equals(userId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Delete;
                    cmd.Parameters.AddWithValue("id", userId);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override IEnumerable<PasswordHistory<UsernamePassword>> Get(Guid userId)
        {
            if (!Guid.Empty.Equals(userId))
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select;
                cmd.Parameters.AddWithValue("id", userId);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<PasswordHistory<UsernamePassword>> permissions = new List<PasswordHistory<UsernamePassword>>();
                try
                {
                    PasswordHistory<UsernamePassword> o;
                    while (rdr.Read())
                    {
                        string user = DbReaderUtils.GetString(rdr, 1);
                        if (!string.IsNullOrEmpty(user))
                        {
                            string payload = DbReaderUtils.GetString(rdr, 2);
                            if (!string.IsNullOrEmpty(payload))
                            {
                                o = ToHistory(payload);
                                if (o!=null)
                                    permissions.Add(o);
                            }
                        }
                    }
                    if (cmd.Connection.State == System.Data.ConnectionState.Open)
                        cmd.Connection.Close();
                }
                catch
                { }
                finally
                {
                    cmd.Dispose();
                }

                return permissions;
            }
            return null;
        }

        public override bool Update(Guid userId, PasswordHistory<UsernamePassword> hist)
        {
            if (!Guid.Empty.Equals(userId))
            {
                if (hist!=null)
                {
                    try
                    {
                        NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                        cmd.CommandText = Db.Update;
                        cmd.Parameters.AddWithValue("id", userId);
                        cmd.Parameters.AddWithValue("uname", UserName(hist));
                        cmd.Parameters.AddWithValue("payload", ToPayload(hist));

                        Db.ExecuteNonQuery(cmd);

                        return true;
                    }
                    catch
                    { }
                }
            }
            return false;
        }

        private string UserName(PasswordHistory<UsernamePassword> hist)
        {
            if (hist.Count>0)
            {
                return hist[0].UserName;
            }
            return null;
        }

        private string ToPayload(PasswordHistory<UsernamePassword> hist)
        {
            if (hist.Count>0)
            {
                StringBuilder sb = new StringBuilder();
                foreach(UsernamePassword cur in hist)
                {
                    sb.Append(Encode(cur));
                    sb.Append(sep);
                }
                return sb.ToString();
            }
            return null;
        }

        private PasswordHistory<UsernamePassword> ToHistory(string payload)
        {
            PasswordHistory<UsernamePassword> tmp = new PasswordHistory<UsernamePassword>();
            StringBuilder sb = new StringBuilder();
            char prev = ' ';
            string user = null;
            foreach(char cur in payload)
            {
                if (cur != escape && cur != sep)
                {
                    sb.Append(cur);
                    prev = cur;
                }
                else
                {
                    if (cur == escape)
                    {
                        if (prev == escape)
                        {
                            sb.Append(cur);
                            prev = ' ';
                        }
                        else
                            prev = cur;
                    }
                    else //cur == sep
                    {
                        if (prev == escape) //literal
                            sb.Append(cur);
                        else //separation of items
                        {
                            if (user == null) //we're hitting the user separator
                                user = sb.ToString();
                            else //ok, we're at the end of a payload
                            {
                                tmp.Add(new UsernamePassword(user, sb.ToString()));
                                user = null;
                            }
                            sb.Clear();
                        }
                        prev = cur;
                    }
                }
            }
            if (sb.Length>0) //have to deal with the last element
            {
                tmp.Add(new UsernamePassword(user, sb.ToString()));
            }
            return tmp;
        }

        private const char escape = '\t';
        private const char sep = '\n';
        private string Encode(UsernamePassword item)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char cur in item.UserName)
            {
                if (cur != escape && cur != sep)
                    sb.Append(cur);
                else
                {
                    if (cur == escape)
                    {
                        sb.Append(escape);
                        sb.Append(escape);
                    }
                    else
                    {
                        sb.Append(escape);
                        sb.Append(sep);
                    }
                }
            }
            sb.Append(sep); //separates user from pass
            foreach(char cur in item.Password)
            {
                if (cur != escape && cur != sep)
                    sb.Append(cur);
                else
                {
                    if (cur == escape)
                    {
                        sb.Append(escape);
                        sb.Append(escape);
                    }
                    else
                    {
                        sb.Append(escape);
                        sb.Append(sep);
                    }
                }
            }


            return sb.ToString();
        }

        internal PgUserPasswordHistoryProvider()
        { }
    }
}
