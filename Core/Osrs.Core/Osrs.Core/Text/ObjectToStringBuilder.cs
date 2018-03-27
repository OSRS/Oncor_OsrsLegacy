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
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Osrs.Text
{
    public class ObjectToStringBuilder
    {
        protected internal const char StartObject = '[';
        protected internal const char EndObject = ']';
        protected internal const char StartArray = '{';
        protected internal const char EndArray = '}';
        protected internal const char StartString = '"';
        protected internal const char EndString = '"';
        protected internal const char StartChar = '\'';
        protected internal const char EndChar = '\'';
        protected internal const char NameValueDelimiter = ':';
        protected internal const char ElementDelimiter = ',';
        protected internal const char EscapeChar = '\\';

        protected internal const string NullLiteral = "NULL";

        protected internal const char NamespaceDelimiter = '.';
        protected internal const char Underscore = '_';

        private readonly StringBuilder sb = new StringBuilder();

        [DebuggerStepThrough]
        public ObjectToStringBuilder(object thing)
        {
            if (thing == null)
            {
                this.sb.Append(ObjectToStringBuilder.NullLiteral);
                this.sb.Append(ObjectToStringBuilder.StartObject);
            }
            else
            {
                this.sb.Append(NameReflectionUtils.GetBaseName(thing));
                this.sb.Append(ObjectToStringBuilder.StartObject);
            }
        }

        [DebuggerStepThrough]
        public ObjectToStringBuilder(string rootName)
        {
            if (rootName == null)
                this.sb.Append(ObjectToStringBuilder.NullLiteral);
            else if (rootName.Length > 0)
                this.sb.Append(rootName);
            this.sb.Append(ObjectToStringBuilder.StartObject);
        }

        [DebuggerStepThrough]
        public void Append(string name, IEnumerable items)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(ObjectToStringBuilder.StartArray);
            foreach (object o in items)
            {
                this.sb.Append(o.ToString());
                this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
            }
            if (this.sb[this.sb.Length - 1] != ObjectToStringBuilder.StartArray) //make sure there are elements added
                this.sb.Length--; //remove the last comma
            this.sb.Append(ObjectToStringBuilder.EndArray);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, string value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(ObjectToStringBuilder.ToString(value));
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, DateTime value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value.ToString("o"));
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, Guid value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value.ToString());
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, bool value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value.ToString());
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, int value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, long value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, double value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, float value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, byte value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, sbyte value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, uint value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, ulong value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, short value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string name, ushort value)
        {
            this.sb.Append(name);
            this.sb.Append(ObjectToStringBuilder.NameValueDelimiter);
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(string value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(DateTime value)
        {
            this.sb.Append(value.ToString("o"));
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(Guid value)
        {
            this.sb.Append(value.ToString());
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(bool value)
        {
            this.sb.Append(value.ToString());
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(int value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(long value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(float value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(double value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(byte value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(sbyte value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(uint value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(ulong value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(short value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public void Append(ushort value)
        {
            this.sb.Append(value);
            this.sb.Append(ObjectToStringBuilder.ElementDelimiter);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            int le = this.sb.Length - 1;
            if (this.sb[le] == ObjectToStringBuilder.ElementDelimiter)
                this.sb.Remove(le, 1);
            this.sb.Append(ObjectToStringBuilder.EndObject);
            return this.sb.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(string value)
        {
            if (value == null)
                return ObjectToStringBuilder.NullLiteral;
            if (value.Length < 1)
                return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.Append(ObjectToStringBuilder.StartString);
            foreach (char token in value)
            {
                //handle escaped char
                if (token.Equals('\t'))
                {
                    builder.Append('\\');
                    builder.Append('t');
                }
                else if (token.Equals('\n'))
                {
                    builder.Append('\\');
                    builder.Append('n');
                }
                else if (token.Equals('\r'))
                {
                    builder.Append('\\');
                    builder.Append('r');
                }
                else if (token.Equals('\f'))
                {
                    builder.Append('\\');
                    builder.Append('f');
                }
                else if (token.Equals('\a'))
                {
                    builder.Append('\\');
                    builder.Append('a');
                }
                else if (token.Equals('\b'))
                {
                    builder.Append('\\');
                    builder.Append('b');
                }
                else if (token.Equals('\v'))
                {
                    builder.Append('\\');
                    builder.Append('v');
                }
                //else if (token.Equals('\0'))
                //{
                //    builder.Append('\\');
                //    builder.Append('0');
                //}
                //else if (token.Equals('\U'))
                //{
                //    builder.Append('\\');
                //    builder.Append('U');
                //}
                //else if (token.Equals('\u'))
                //{
                //    builder.Append('\\');
                //    builder.Append('u');
                //}
                //else if (token.Equals("\x"))
                //{
                //    builder.Append('\\');
                //    builder.Append('x');
                //}
                else
                    builder.Append(token);
            }
            builder.Append(ObjectToStringBuilder.EndString);
            return builder.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(DateTime value)
        {
            return value.ToString("o");//roundtrip ISO8601 format
        }

        [DebuggerStepThrough]
        public static string ToString(Guid value)
        {
            return value.ToString(); //bare formatted
        }

        [DebuggerStepThrough]
        public static string ToString(bool value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(int value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(long value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(float value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(double value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(byte value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(sbyte value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(uint value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(ulong value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(short value)
        {
            return value.ToString();
        }

        [DebuggerStepThrough]
        public static string ToString(ushort value)
        {
            return value.ToString();
        }
    }
}
