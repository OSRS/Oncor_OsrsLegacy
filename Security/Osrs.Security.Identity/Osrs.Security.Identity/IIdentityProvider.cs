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
using System.Collections.Generic;

namespace Osrs.Security.Identity
{
    public interface IIdentityProvider
    {
        bool CanGet();
        UserIdentityBase Get(Guid userId);
        UserIdentityBase Get(Guid userId, UserType type);
        IEnumerable<UserIdentityBase> Get();
        IEnumerable<UserIdentityBase> Get(UserType type);
        IEnumerable<UserIdentityBase> Get(string name);
        IEnumerable<UserIdentityBase> Get(string name, UserType type);

        bool CanCreate();
        bool CanCreate(UserType type);
        UserIdentityBase Create(UserType type);

        UserIdentityBase CreateUser();
        UserIdentityBase CreateUser(string name);

        UserIdentityBase CreateSystem();
        UserIdentityBase CreateSystem(string name);

        UserIdentityBase CreateToken();
        UserIdentityBase CreateToken(string name);

        bool Exists(Guid userId);
        bool Exists(string name);
        bool Exists(Guid userId, UserType type);
        bool Exists(string name, UserType type);

        bool CanDelete();
        bool CanDelete(Guid userId);
        bool Delete(Guid userId);

        bool CanUpdate();
        bool CanUpdate(UserIdentityBase identity);
        bool Update(UserIdentityBase identity);
    }
}
