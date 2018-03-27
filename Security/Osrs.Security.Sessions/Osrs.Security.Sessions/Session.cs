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
    public class Session
    {
        public Guid SessionId
        {
            get;
            protected set;
        }

        public DateTime ExpiresAt
        {
            get;
            protected set;
        }

        public Session(Guid sessionId, DateTime expiresAt)
        {
            this.SessionId = sessionId;
            this.ExpiresAt = expiresAt;
        }
    }

    public class RuntimeSession : Session
    {
        public Guid UserId
        {
            get;
            protected set;
        }

        public string Binding
        {
            get;
            protected set;
        }

        public RuntimeSession(Guid sessionId, DateTime expiresAt) : base(sessionId, expiresAt)
        { }

        public RuntimeSession(Guid sessionId, DateTime expiresAt, Guid userId, string binding) : base(sessionId, expiresAt)
        {
            this.UserId = userId;
            this.Binding = binding;
        }
    }
}
