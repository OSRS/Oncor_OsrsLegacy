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

namespace Osrs.Security.Sessions.Providers
{
    public sealed class MemorySessionProviderFactory : SessionProviderFactory
    {
        public override SessionProviderBase GetProvider(SecurityContext context)
        {
            if (SessionManager.Instance.State == Runtime.RunState.Running)
            {
                return new MemorySessionProvider(context);
            }
            return null;
        }

        protected internal override bool Initialize()
        {
            return true;
        }

        private static readonly MemorySessionProviderFactory instance = new MemorySessionProviderFactory();
        public static MemorySessionProviderFactory Instance
        {
            get { return instance; }
        }
    }

    public sealed class MemorySessionProvider : SessionProviderBase
    {
        private static readonly Dictionary<Guid, ModuleRuntimeSession> sessions = new Dictionary<Guid, ModuleRuntimeSession>();

        private readonly SecurityContext context;

        internal MemorySessionProvider(SecurityContext context)
        {
            this.context = context;
        }

        protected override ModuleRuntimeSession CreateImpl()
        {
            Scavenge(); //TODO -- get rid of this for a thread task
            ModuleRuntimeSession s = new ModuleRuntimeSession(Guid.NewGuid(), DateTime.UtcNow.AddSeconds(SessionManager.Instance.SessionDuration));
            sessions[s.SessionId] = s;
            return s;
        }

        public override bool Exists(Guid sessionId)
        {
            return sessions.ContainsKey(sessionId);
        }

        protected override bool ExpireImpl(Guid sessionId)
        {
            return sessions.Remove(sessionId);
        }

        protected override bool ExtendImpl(Guid sessionId)
        {
            if (sessions.ContainsKey(sessionId))
            {
                try
                {
                    ModuleRuntimeSession s = sessions[sessionId];
                    s = new ModuleRuntimeSession(s.SessionId, s.ExpiresAt.AddSeconds(SessionManager.Instance.SessionDuration), s.UserId, s.Binding);
                    sessions[sessionId] = s;
                    return true;
                }
                catch
                { }
            }
            return false;
        }

        public override ModuleRuntimeSession Get(Guid sessionId)
        {
            Scavenge(); //TODO -- get rid of this for a thread task
            if (sessions.ContainsKey(sessionId))
            {
                ModuleRuntimeSession tmp = sessions[sessionId];
                return new ModuleRuntimeSession(tmp.SessionId, tmp.ExpiresAt, tmp.UserId, tmp.Binding);
            }
            return null;
        }

        private void Scavenge()
        {
            DateTime rel = DateTime.UtcNow;
            HashSet<Guid> old = new HashSet<Guid>();
            foreach (Session cur in sessions.Values)
            {
                if (cur.ExpiresAt < rel)
                    old.Add(cur.SessionId);
            }
            foreach (Guid id in old)
            {
                this.Expire(id);
            }
        }

        protected override bool UpdateImpl(ModuleRuntimeSession session)
        {
            if (sessions.ContainsKey(session.SessionId))
            {
                ModuleRuntimeSession tmp = sessions[session.SessionId];
                if (tmp.ExpiresAt < DateTime.UtcNow)
                {
                    this.ExpireImpl(session.SessionId);
                    return false;
                }

                tmp.SetUserBinding(session.UserId, session.Binding);
                return true;
            }
            return false;
        }
    }
}
