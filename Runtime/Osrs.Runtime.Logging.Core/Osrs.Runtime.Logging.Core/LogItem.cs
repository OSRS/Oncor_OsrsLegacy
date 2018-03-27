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

using Osrs.Reflection;
using System;
using System.Text;

namespace Osrs.Runtime.Logging
{
    public class LogItem
    {
        public string EventId
        {
            get;
            set;
        }

        public TypeNameReference TypeName
        {
            get;
            set;
        }

        public DateTime When
        {
            get;
            set;
        }

        public ushort Severity
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Exception
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public LogItem()
        { }

        public LogItem(TypeNameReference typeName, DateTime when, ushort severity, string message)
            : this(typeName, when, null, severity, null, (string)null, message)
        { }

        public LogItem(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string message) 
            : this(typeName, when, eventId, severity, null, (string)null, message)
        { }

        public LogItem(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, Exception exception)
            : this(typeName, when, eventId, severity, title, exception, null)
        { }

        public LogItem(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, string message)
            : this(typeName, when, eventId, severity, title, (string)null, message)
        { }

        public LogItem(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, Exception exception, string message)
        {
            this.TypeName = typeName;
            this.When = when;
            this.EventId = eventId;
            this.Severity = severity;
            this.Title = title;
            if (exception!=null)
                this.Exception = exception.ToString();
            this.Message = message;
        }

        public LogItem(TypeNameReference typeName, DateTime when, string eventId, ushort severity, string title, string exception, string message)
        {
            this.TypeName = typeName;
            this.When = when;
            this.EventId = eventId;
            this.Severity = severity;
            this.Title = title;
            this.Exception = exception;
            this.Message = message;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[[");
            if (this.TypeName != null)
                sb.Append(this.TypeName.ToString());
            sb.Append("][");
            sb.Append(this.When.ToString("o"));
            sb.Append("][");
            sb.Append(this.Severity);
            sb.Append("](");
            if (this.EventId != null)
                sb.Append(this.EventId);
            sb.Append(":");
            if (this.Title != null)
                sb.Append(this.Title);
            sb.Append(")(");
            if (this.Message != null)
                sb.Append(this.Message);
            sb.Append(")");
            if (this.Exception != null)
                sb.Append(this.Exception);
            sb.Append(']');
            return sb.ToString();
        }
    }
}
