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
using System.Reflection;

namespace Osrs.Reflection
{
    public sealed class TypeNameReference : IEquatable<TypeNameReference>
    {
        public readonly string AssemblyFileName;
        public readonly string ClassName;

        public TypeNameReference(string assyName, string className)
        {
            if (string.IsNullOrEmpty(assyName) || string.IsNullOrEmpty(className))
                throw new ArgumentException();

            this.AssemblyFileName = assyName;
            this.ClassName = className;
        }

        public static TypeNameReference Create(Type t)
        {
            if (t == null)
                return null;
            return new TypeNameReference(t.GetTypeInfo().Assembly.GetName().Name, string.Format("{0}.{1}", t.Namespace, t.Name));
        }

        public static TypeNameReference Create(object o)
        {
            if (o == null)
                return null;
            return Create(o.GetType());
        }

        public static TypeNameReference Parse(string content)
        {
            return Parse(content, ',');
        }

        public static TypeNameReference Parse(string content, char sep)
        {
            try
            {
                string[] parts = content.Split(new char[] { sep });
                if (parts != null && parts.Length == 2)
                {
                    return new TypeNameReference(parts[1].Trim(), parts[0].Trim());
                }
            }
            catch
            { }
            return null;
        }

        public bool Equals(TypeNameReference other)
        {
            return this.AssemblyFileName.Equals(other.AssemblyFileName) && this.ClassName.Equals(other.ClassName);
        }

        public string ToString(char separator)
        {
            return this.ClassName + separator + this.AssemblyFileName;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeNameReference)
                return this.Equals(obj as TypeNameReference);
            return false;
        }

        public override int GetHashCode()
        {
            return this.AssemblyFileName.GetHashCode() ^ this.ClassName.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.ClassName, this.AssemblyFileName);
        }
    }
}
