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
using Osrs.Reflection;

namespace Osrs.Runtime.Logging
{
    public enum LogLevel
    {
        Info=2048,
        Warn=16384,
        Error=32768,
        Fatal=64512
    }

    public abstract class LogProviderFactory : SubclassableSingletonBase<LogProviderFactory>
    {
        protected internal LogProviderBase GetProvider(Type t)
        {
            if (LogManager.Instance.State == RunState.Running && t!=null)
                return this.GetLogger(t);
            return null;
        }

        protected internal abstract LogProviderBase GetLogger(Type t);

        protected internal abstract bool Initialize();

        protected internal abstract void Shutdown();

        protected LogProviderFactory()
        {}
    }

    public abstract class LogProviderBase
    {
        public readonly Type Type;
        private readonly TypeNameReference typeName;

        public void Log(ushort severity, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, severity, null, (string)null, message);
        }

        public void Log(string message, LogLevel severity)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, (ushort)severity, null, (string)null, message);
        }

        public void Log(ushort severity, Exception exception)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, severity, null, exception, null);
        }

        public void Log(LogLevel severity, Exception exception)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, (ushort)severity, null, exception, null);
        }

        public void Log(ushort severity, Exception exception, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, severity, null, exception, message);
        }

        public void Log(LogLevel severity, Exception exception, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, null, (ushort)severity, null, exception, message);
        }

        public void Log(string eventId, ushort severity, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, severity, null, (string)null, message);
        }

        public void Log(string eventId, LogLevel severity, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, (ushort)severity, null, (string)null, message);
        }

        public void Log(string eventId, ushort severity, string title, Exception exception)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, severity, title, exception, null);
        }

        public void Log(string eventId, LogLevel severity, string title, Exception exception)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, (ushort)severity, title, exception, null);
        }

        public void Log(string eventId, ushort severity, string title, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, severity, title, (string)null, message);
        }

        public void Log(string eventId, LogLevel severity, string title, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, (ushort)severity, title, (string)null, message);
        }

        public void Log(string eventId, ushort severity, string title, Exception exception, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, severity, title, exception, message);
        }

        public void Log(string eventId, LogLevel severity, string title, Exception exception, string message)
        {
            if (LogManager.Instance.State == RunState.Running)
                this.Log(this.typeName, DateTime.UtcNow, eventId, (ushort)severity, title, exception, message);
        }

        internal void Log(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, Exception exception, string message)
        {
            if (exception != null)
                this.Log(typeName, when, eventId, severity, title, exception.ToString(), message);
            else
                this.Log(typeName, when, eventId, severity, title, (string)null, message);
        }

        protected internal abstract void Log(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, string exception, string message);

        protected LogProviderBase(Type logType)
        {
            this.Type = logType;
            this.typeName = TypeNameReference.Create(this.Type);
        }
    }
}
