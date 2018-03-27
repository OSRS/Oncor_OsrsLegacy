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
using Osrs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Security.Authentication
{
    public sealed class PgCredentialStore : ICredentialStore
    {
        public bool AddCredential(Guid userId, IAuthenticationProvider provider, byte[] token, byte[] payload)
        {
            return AddCredential(userId, provider, Convert.ToBase64String(token), Convert.ToBase64String(payload));
        }

        public bool AddCredential(Guid userId, IAuthenticationProvider provider, byte[] token, string payload)
        {
            return AddCredential(userId, provider, Convert.ToBase64String(token), payload);
        }

        public bool AddCredential(Guid userId, IAuthenticationProvider provider, string token, byte[] payload)
        {
            return AddCredential(userId, provider, token, Convert.ToBase64String(payload));
        }

        //:id, :oType, :payload, :token, :locked, :dateFrom, :dateTo
        public bool AddCredential(Guid userId, IAuthenticationProvider provider, string token, string payload)
        {
            if (!Guid.Empty.Equals(userId) && provider!=null && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(payload))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Insert;
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                    cmd.Parameters.AddWithValue("payload", payload);
                    cmd.Parameters.AddWithValue("token", token);
                    cmd.Parameters.AddWithValue("locked", false);
                    cmd.Parameters.AddWithValue("dateFrom", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("dateTo", new DateTime(3000, 1, 1)); //should move this to config

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool DeleteCredential(Guid userId, IAuthenticationProvider provider, byte[] token)
        {
            return DeleteCredential(userId, provider, Convert.ToBase64String(token));
        }

        public bool DeleteCredential(Guid userId, IAuthenticationProvider provider, string token)
        {
            if (!Guid.Empty.Equals(userId) && provider != null && !string.IsNullOrEmpty(token))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Delete;
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                    cmd.Parameters.AddWithValue("token", token);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool Expire(Guid userId)
        {
            if (!Guid.Empty.Equals(userId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Expire;
                    cmd.Parameters.AddWithValue("id", userId);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool Expire(IAuthenticationProvider provider, PersistedCredential credential)
        {
            if (provider != null && credential!=null)
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Expire + "AND \"OwnerType\"=:oType AND \"Token\"=:token";
                    cmd.Parameters.AddWithValue("id", credential.UserId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                    cmd.Parameters.AddWithValue("token", credential.TextToken);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool Expire(IAuthenticationProvider provider, Guid userId)
        {
            if (provider != null && !Guid.Empty.Equals(userId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Expire + "AND \"OwnerType\"=:oType";
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider)
        {
            if (provider!=null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + "\"OwnerType\"=:oType";
                cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<PersistedCredential> permissions = new List<PersistedCredential>();
                try
                {
                    PersistedCredential o;
                    while (rdr.Read())
                    {
                        o = PgCredentialStoreFactory.Instance.Create(rdr);
                        if (o != null)
                            permissions.Add(o);
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

        public IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, byte[] token)
        {
            return Get(provider, Convert.ToBase64String(token));
        }

        public IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, string token)
        {
            if (provider != null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + "\"OwnerType\"=:oType AND \"Token\"=:token";
                cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                cmd.Parameters.AddWithValue("token", token);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<PersistedCredential> permissions = new List<PersistedCredential>();
                try
                {
                    PersistedCredential o;
                    while (rdr.Read())
                    {
                        o = PgCredentialStoreFactory.Instance.Create(rdr);
                        if (o != null)
                            permissions.Add(o);
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

        public IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, Guid userId, byte[] token)
        {
            return Get(provider, userId, Convert.ToBase64String(token));
        }

        public IEnumerable<PersistedCredential> Get(IAuthenticationProvider provider, Guid userId, string token)
        {
            if (provider != null)
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + "\"Id\"=:id AND \"OwnerType\"=:oType AND \"Token\"=:token";
                cmd.Parameters.AddWithValue("id", userId);
                cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                cmd.Parameters.AddWithValue("token", token);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<PersistedCredential> permissions = new List<PersistedCredential>();
                try
                {
                    PersistedCredential o;
                    while (rdr.Read())
                    {
                        o = PgCredentialStoreFactory.Instance.Create(rdr);
                        if (o != null)
                            permissions.Add(o);
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

        public bool Lock(Guid userId)
        {
            if (!Guid.Empty.Equals(userId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Lock;
                    cmd.Parameters.AddWithValue("id", userId);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool Lock(IAuthenticationProvider provider, PersistedCredential credential)
        {
            if (provider != null && credential != null)
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Lock + "AND \"OwnerType\"=:oType AND \"Token\"=:token";
                    cmd.Parameters.AddWithValue("id", credential.UserId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                    cmd.Parameters.AddWithValue("token", credential.TextToken);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool Lock(IAuthenticationProvider provider, Guid userId)
        {
            if (provider != null && !Guid.Empty.Equals(userId))
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Lock + "AND \"OwnerType\"=:oType";
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, bool isLocked)
        {
            return UpdateCredential(provider, credential, credential.ValidFrom, credential.ValidTo, isLocked);
        }

        public bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, DateTime validFrom, DateTime validTo)
        {
            return UpdateCredential(provider, credential, validFrom, validTo, credential.IsLocked);
        }

        public bool UpdateCredential(IAuthenticationProvider provider, PersistedCredential credential, DateTime validFrom, DateTime validTo, bool isLocked)
        {
            if (provider != null && credential != null)
            {
                try
                {
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Update;
                    cmd.Parameters.AddWithValue("id", credential.UserId);
                    cmd.Parameters.AddWithValue("oType", TypeNameReference.Create(provider).ToString());
                    cmd.Parameters.AddWithValue("token", credential.TextToken);
                    cmd.Parameters.AddWithValue("dateFrom", validFrom);
                    cmd.Parameters.AddWithValue("dateTo", validTo);
                    cmd.Parameters.AddWithValue("locked", isLocked);

                    Db.ExecuteNonQuery(cmd);

                    return true;
                }
                catch
                { }
            }
            return false;
        }

        internal PgCredentialStore(UserSecurityContext context)
        { }
    }
}
