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
using Osrs.Security.Authorization;
using System;
using System.Collections.Generic;

namespace Osrs.Security.Identity.Providers
{
    public sealed class PostgresIdentityProvider : IIdentityProvider
    {
        private IRoleProvider rp;
        private IRoleProvider Role
        {
            get
            {
                if (rp == null)
                    rp=AuthorizationManager.Instance.GetRoleProvider(this.userContext);
                return rp;
            }
        }

        public bool CanCreate()
        {
            IPermissionProvider perm = PostgresIdentityProviderFactory.Instance.Permissions;
            if (perm!=null)
            {
                IRoleProvider rol = Role;
                if (rol != null)
                    return rol.HasPermission(perm.Get(IdentityUtils.CreatePermisionId));
            }
            return false;
        }

        //TODO -- add fine grained permissions
        public bool CanCreate(UserType type)
        {
            return this.CanCreate();
        }

        public bool CanDelete()
        {
            IPermissionProvider perm = PostgresIdentityProviderFactory.Instance.Permissions;
            if (perm != null)
            {
                IRoleProvider rol = Role;
                if (rol != null)
                    return rol.HasPermission(perm.Get(IdentityUtils.DeletePermissionId));
            }
            return false;
        }

        public bool CanDelete(Guid userId)
        {
            if (SecurityUtils.AdminIdentity.Equals(userId)) //cannot allow deletion of admin user account - though it can be locked down
                return false;
            return CanDelete();
        }

        public bool CanGet()
        {
            IPermissionProvider perm = PostgresIdentityProviderFactory.Instance.Permissions;
            if (perm != null)
            {
                IRoleProvider rol = Role;
                if (rol != null)
                    return rol.HasPermission(perm.Get(IdentityUtils.GetPermissionId));
            }
            return false;
        }

        public bool CanUpdate()
        {
            IPermissionProvider perm = PostgresIdentityProviderFactory.Instance.Permissions;
            if (perm != null)
            {
                IRoleProvider rol = Role;
                if (rol != null)
                    return rol.HasPermission(perm.Get(IdentityUtils.UpdatePermissionId));
            }
            return false;
        }

        public bool CanUpdate(UserIdentityBase identity)
        {
            return this.CanUpdate();
        }

        //:id, :ut, :us, :ea, :name
        public UserIdentityBase Create(UserType type)
        {
            return CreateUser(null, type);
        }

        public UserIdentityBase CreateSystem()
        {
            return CreateSystem(null);
        }

        public UserIdentityBase CreateSystem(string name)
        {
            return CreateUser(name, UserType.System);
        }

        public UserIdentityBase CreateToken()
        {
            return CreateToken(null);
        }

        public UserIdentityBase CreateToken(string name)
        {
            return CreateUser(name, UserType.Token);
        }

        public UserIdentityBase CreateUser()
        {
            return CreateUser(null);
        }

        public UserIdentityBase CreateUser(string name)
        {
            return CreateUser(name, UserType.Person);
        }

        private UserIdentityBase CreateUser(string name, UserType type)
        {
            if (CanCreate())
            {
                try
                {
                    Guid id = Guid.NewGuid();
                    if (string.IsNullOrEmpty(name))
                        name = id.ToString();
                    NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                    cmd.CommandText = Db.Insert;
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("ut", (int)type);
                    cmd.Parameters.AddWithValue("us", (int)UserState.Created);
                    cmd.Parameters.Add(Db.GetEAParam(null, System.Data.ParameterDirection.Input));
                    cmd.Parameters.AddWithValue("name", name);
                    Db.ExecuteNonQuery(cmd);

                    return new PostgresUser(id, type, name, UserState.Created);
                }
                catch
                { }
            }
            return null;
        }

        public bool Delete(Guid userId)
        {
            if (!Guid.Empty.Equals(userId) && CanDelete(userId))
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

        public bool Exists(Guid userId)
        {
            if (!Guid.Empty.Equals(userId) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountSelect + Db.SelectById;
                cmd.Parameters.AddWithValue("id", userId);
                return Db.Exists(cmd);
            }
            return false;
        }

        public bool Exists(Guid userId, UserType type)
        {
            if (!Guid.Empty.Equals(userId) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountSelect + Db.SelectByIdType;
                cmd.Parameters.AddWithValue("id", userId);
                cmd.Parameters.AddWithValue("ut", (int)type);
                return Db.Exists(cmd);
            }
            return false;
        }

        public bool Exists(string name)
        {
            if (!string.IsNullOrEmpty(name) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountSelect + Db.SelectByName;
                cmd.Parameters.AddWithValue("name", name);
                return Db.Exists(cmd);
            }
            return false;
        }

        public bool Exists(string name, UserType type)
        {
            if (!string.IsNullOrEmpty(name) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.CountSelect + Db.SelectByNameType;
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("ut", (int)type);
                return Db.Exists(cmd);
            }
            return false;
        }

        public UserIdentityBase Get(Guid userId)
        {
            if (!Guid.Empty.Equals(userId) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.SelectById;
                cmd.Parameters.AddWithValue("id", userId);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                UserIdentityBase o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                            o = UserBuilder.Instance.Build(rdr);
                        if (cmd.Connection.State == System.Data.ConnectionState.Open)
                            cmd.Connection.Close();
                    }
                    catch
                    { }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
                return o;
            }
            return null;
        }

        public UserIdentityBase Get(Guid userId, UserType type)
        {
            if (!Guid.Empty.Equals(userId) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.SelectByIdType;
                cmd.Parameters.AddWithValue("id", userId);
                cmd.Parameters.AddWithValue("ut", (int)type);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                UserIdentityBase o = null;
                if (rdr != null)
                {
                    try
                    {
                        if (rdr.Read())
                            o = UserBuilder.Instance.Build(rdr);
                        if (cmd.Connection.State == System.Data.ConnectionState.Open)
                            cmd.Connection.Close();
                    }
                    catch
                    { }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
                return o;
            }
            return null;
        }

        public IEnumerable<UserIdentityBase> Get()
        {
            if (CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select;
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<UserIdentityBase> o = new List<UserIdentityBase>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            UserIdentityBase u = UserBuilder.Instance.Build(rdr);
                            if (u != null)
                                o.Add(u);
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
                }
                return o;
            }
            return null;
        }

        public IEnumerable<UserIdentityBase> Get(UserType type)
        {
            if (CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.SelectByType;
                cmd.Parameters.AddWithValue("ut", (int)type);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<UserIdentityBase> o = new List<UserIdentityBase>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            UserIdentityBase u = UserBuilder.Instance.Build(rdr);
                            if (u != null)
                                o.Add(u);
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
                }
                return o;
            }
            return null;
        }

        public IEnumerable<UserIdentityBase> Get(string name)
        {
            if (!string.IsNullOrEmpty(name) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.SelectByName;
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<UserIdentityBase> o = new List<UserIdentityBase>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            UserIdentityBase u = UserBuilder.Instance.Build(rdr);
                            if (u != null)
                                o.Add(u);
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
                }
                return o;
            }
            return null;
        }

        public IEnumerable<UserIdentityBase> Get(string name, UserType type)
        {
            if (!string.IsNullOrEmpty(name) && CanGet())
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Select + Db.SelectByNameType;
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("ut", (int)type);
                NpgsqlDataReader rdr = Db.ExecuteReader(cmd);
                List<UserIdentityBase> o = new List<UserIdentityBase>();
                if (rdr != null)
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            UserIdentityBase u = UserBuilder.Instance.Build(rdr);
                            if (u!=null)
                                o.Add(u);
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
                }
                return o;
            }
            return null;
        }

        public bool Update(UserIdentityBase identity)
        {
            if (identity!=null && CanUpdate(identity))
            {
                NpgsqlCommand cmd = Db.GetCmd(Db.ConnectionString);
                cmd.CommandText = Db.Update;

                cmd.Parameters.AddWithValue("id", identity.Uid);
                cmd.Parameters.AddWithValue("ut", (int)identity.UserType);
                cmd.Parameters.AddWithValue("us", (int)identity.UserState);
                cmd.Parameters.Add(Db.GetEAParam(identity.ExpiresAt, System.Data.ParameterDirection.Input));
                cmd.Parameters.AddWithValue("name", identity.Name);

                Db.ExecuteNonQuery(cmd);
                return true;
            }
            return false;
        }

        private readonly UserSecurityContext userContext;
        internal PostgresIdentityProvider(UserSecurityContext context)
        {
            this.userContext = context;
            this.rp = AuthorizationManager.Instance.GetRoleProvider(context);
        }
    }
}
