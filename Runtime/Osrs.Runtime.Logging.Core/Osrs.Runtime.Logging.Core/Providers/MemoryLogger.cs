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
using Osrs.Reflection;

namespace Osrs.Runtime.Logging.Providers
{
    public sealed class NullLoggerFactory : LogProviderFactory
    {
        protected internal override bool Initialize()
        {
            return true;
        }

        protected internal override void Shutdown()
        { }

        protected internal override LogProviderBase GetLogger(Type t)
        {
            return new NullLogger(t);
        }
    }

    public sealed class NullLogger : LogProviderBase
    {
        protected internal override void Log(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, string exception, string message)
        {
        }

        public NullLogger(Type t):base(t)
        { }
    }

    public sealed class MemoryLoggerFactory : LogProviderFactory
    {
        internal static readonly List<LogItem> Items = new List<LogItem>();

        protected internal override bool Initialize()
        { return true; }

        protected internal override void Shutdown()
        {
            Items.Clear();
        }

        protected internal override LogProviderBase GetLogger(Type t)
        {
            return new MemoryLogger(t);
        }
    }

    public sealed class MemoryLogger : LogProviderBase
    {
        protected internal override void Log(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, string exception, string message)
        {
            MemoryLoggerFactory.Items.Add(new LogItem(typeName, when, eventId, severity, title, exception, message));
        }

        internal MemoryLogger(Type t) : base(t)
        { }
    }
}
