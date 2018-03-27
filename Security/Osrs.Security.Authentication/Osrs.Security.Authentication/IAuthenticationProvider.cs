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

using Osrs.Runtime;

namespace Osrs.Security.Authentication
{
    public interface IAuthenticationProvider
    {
        bool CanAuthenticate(ICredential credential);

        bool Authenticates(ICredential credential);
        IUserIdentity Authenticate(ICredential credential);

        bool AddCredential(IUserIdentity user, ICredential credential);
        bool ReplaceCredential(IUserIdentity user, ICredential existing, ICredential proposed);
        bool DeleteCredential(IUserIdentity user, ICredential credential);
    }    
}
