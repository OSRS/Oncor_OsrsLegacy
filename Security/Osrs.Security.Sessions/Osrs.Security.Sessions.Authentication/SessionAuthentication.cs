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
using Osrs.Security.Authentication;
using System;

namespace Osrs.Security.Sessions.Authentication
{
    public class SessionAuthentication : SubclassableSingletonBase<SessionAuthentication>
    {
        private ISessionProvider sess;
        internal ISessionProvider SessionProvider
        {
            get
            {
                if (sess==null)
                    this.sess = SessionManager.Instance.GetProvider();
                return sess;
            }
        }

        private IAuthenticationProvider auth;
        internal IAuthenticationProvider AuthenticationProvider
        {
            get
            {
                if (sess == null)
                    this.auth = AuthenticationManager.Instance.GetProvider();
                return auth;
            }
        }

        public bool LogIn(Guid sessionId, ICredential cred)
        {
            if (cred != null)
            {
                ModuleRuntimeSession session = SessionProvider.Get(sessionId) as ModuleRuntimeSession;
                if (session != null)
                {
                    IUserIdentity user = AuthenticationProvider.Authenticate(cred);
                    if (user != null)
                    {
                        session.SetUserBinding(Guid.Empty, session.Binding);
                        return SessionProvider.Update(session);
                    }
                }
            }
            return false;
        }

        public void LogOut(Guid sessionId)
        {
            ModuleRuntimeSession session = SessionProvider.Get(sessionId) as ModuleRuntimeSession;
            if (session!=null)
            {
                session.SetUserBinding(Guid.Empty, session.Binding);
                SessionProvider.Update(session);
            }
        }

        private SessionAuthentication()
        { }

        private static readonly SessionAuthentication instance=new SessionAuthentication();

        public static SessionAuthentication Instance
        {
            get { return instance; }
        }
    }
}
