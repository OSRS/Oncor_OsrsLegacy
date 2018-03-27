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

namespace Osrs.Security.Sessions
{
    public sealed class ModuleRuntimeSession : RuntimeSession
    {
        public void SetUserBinding(Guid userId, string binding)
        {
            this.UserId = userId;
            this.Binding = binding;
        }

        public ModuleRuntimeSession(Guid sessionId, DateTime expiresAt) : base(sessionId, expiresAt)
        { }

        public ModuleRuntimeSession(Guid sessionId, DateTime expiresAt, Guid userId, string binding) : base(sessionId, expiresAt)
        {
            this.UserId = userId;
            this.Binding = binding;
        }
    }

    public abstract class SessionProviderFactory
    {
        public SessionProviderBase GetProvider()
        {
            return GetProvider(new SecurityContext());
        }

        public abstract SessionProviderBase GetProvider(SecurityContext context);
        protected internal abstract bool Initialize();
    }

    public abstract class SessionProviderBase : ISessionProvider
    {
        public uint DefaultSessionLength
        {
            get { return SessionManager.Instance.SessionDuration; }
        }

        public ModuleRuntimeSession Create()
        {
            ModuleRuntimeSession tmp = this.CreateImpl();

            if (tmp!=null)
                SessionManager.Instance.OnCreate(new SessionEventArgs(tmp.SessionId));

            return tmp;
        }

        Session ISessionProvider.Create()
        {
            return this.Create();
        }

        protected abstract ModuleRuntimeSession CreateImpl();

        public abstract ModuleRuntimeSession Get(Guid sessionId);

        Session ISessionProvider.Get(Guid sessionId)
        {
            return this.Get(sessionId);
        }

        public abstract bool Exists(Guid sessionId);

        public bool Expire(Session session)
        {
            if(session!=null)
                return this.Expire(session.SessionId);

            return false;
        }

        public bool Expire(Guid sessionId)
        {
            SessionManager.Instance.OnExpire(new SessionEventArgs(sessionId));
            return this.ExpireImpl(sessionId);
        }

        protected abstract bool ExpireImpl(Guid sessionId);

        public bool Extend(Session session)
        {
            if (session != null)
                return this.Extend(session.SessionId);

            return false;
        }

        public bool Extend(Guid sessionId)
        {
            if (this.ExtendImpl(sessionId))
            {
                SessionManager.Instance.OnExtend(new SessionEventArgs(sessionId));
                return true;
            }

            return false;
        }

        protected abstract bool ExtendImpl(Guid sessionId);

        public bool Update(Session session)
        {
            if (session!=null)
            {
                ModuleRuntimeSession tmp = session as ModuleRuntimeSession;
                if (tmp != null && UpdateImpl(tmp))
                {
                    SessionManager.Instance.OnUpdate(new SessionEventArgs(session.SessionId));
                    return true;
                }
            }

            return false;
        }

        protected abstract bool UpdateImpl(ModuleRuntimeSession session);
    }
}
