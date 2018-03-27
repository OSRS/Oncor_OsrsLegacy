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

using Osrs.Data;
using System;
using System.Data.Common;

namespace Osrs.Security.Identity.Providers
{
    internal sealed class UserBuilder : IBuilder<PostgresUser>
    {
        internal static UserBuilder Instance = new UserBuilder();

        private UserBuilder()
        { }

        //Guid uid, UserType type, UserState state, DateTime? ExpiresAt, string name
        public PostgresUser Build(DbDataReader reader)
        {
            Guid id = DbReaderUtils.GetGuid(reader, 0);
            UserType type = (UserType)DbReaderUtils.GetInt32(reader, 1);
            UserState state = (UserState)DbReaderUtils.GetInt32(reader, 2);
            DateTime? exp = DbReaderUtils.GetNullableDateTime(reader, 3);
            string name = DbReaderUtils.GetString(reader, 4);
            PostgresUser user = new PostgresUser(id, type, name, state);
            user.ExpiresAt = exp;
            return user;
        }
    }

    public sealed class PostgresUser : UserIdentityBase
    {
        internal PostgresUser(Guid uid, UserType type, string name, UserState state) : base(uid, type, name, state)
        { }
    }
}
