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
using System.Reflection;

namespace Osrs.Reflection
{
    public static class NameReflectionUtils
    {
        public static object CreateInstance(TypeNameReference typName)
        {
            Type ty = GetType(typName);
            if (ty != null)
            {
                try
                {
                    return Activator.CreateInstance(ty);
                }
                catch
                { }
            }

            return null;
        }

        public static T CreateInstance<T>(TypeNameReference typName)
        {
            Type ty = GetType(typName);
            if (ty!=null)
            {
                try
                {
                    return (T)(Activator.CreateInstance(ty));
                }
                catch
                { }
            }

            return default(T);
        }

        public static Type GetType(TypeNameReference typName)
        {
            if (typName == null)
                throw new ArgumentNullException();
            return GetType(typName.ClassName, typName.AssemblyFileName);
        }

        public static TypeNameReference GetTypeName(Type t)
        {
            return TypeNameReference.Create(t);
        }

        public static TypeNameReference GetTypeName(object o)
        {
            return TypeNameReference.Create(o);
        }

        public static string GetBaseName(this Type t)
        {
            if (t == null)
                return null;
            return t.Namespace + "." + t.Name;
        }

        public static string GetBaseName(this object o)
        {
            if (o == null)
                return null;
            return o.GetType().GetBaseName();
        }

        public static string GetName(this Type t)
        {
            if (t == null)
                return null;
            return t.GetBaseName() + ", " + t.GetTypeInfo().Assembly.FullName;
        }

        public static string GetName(this object o)
        {
            if (o == null)
                return null;
            return o.GetType().GetName();
        }

        public static bool HasAttribute(this object o, Type attributeType)
        {
            if (o == null)
                return false;
            return o.GetType().HasAttribute(attributeType);
        }

        public static bool HasAttribute(this Type objectType, Type attributeType)
        {
            if (objectType == null || attributeType == null)
                return false;
            if (!attributeType.Is(typeof(Attribute)))
                return false; //it's not an attribute

            IEnumerable<Attribute> attribs = objectType.GetTypeInfo().GetCustomAttributes(true);
            foreach (Attribute ob in attribs)
            {
                if (ob.Is(attributeType))
                    return true;
            }
            return false;
        }

        public static T GetAttribute<T>(this object o) where T : Attribute
        {
            if (o == null)
                return null;
            return o.GetType().GetAttribute<T>();
        }

        public static T GetAttribute<T>(this Type objectType) where T : Attribute
        {
            Type attributeType = typeof(T);
            if (objectType == null || attributeType == null)
                return null;

            IEnumerable<Attribute> attribs = objectType.GetTypeInfo().GetCustomAttributes(true);
            foreach (Attribute ob in attribs)
            {
                if (ob.Is(attributeType))
                    return (T)ob;
            }
            return null;
        }

        public static Assembly Load(string assemblyName)
        {
            return Load(new AssemblyName(assemblyName));
        }

        public static Assembly Load(AssemblyName name)
        {
            if (name != null)
            {
                try
                {
                    Assembly asy = Assembly.Load(name);
                    return asy;
                }
                catch
                { }
            }
            return null;
        }

        public static string GetSimpleAssemblyName(object o)
        {
            if (o != null)
            {
                return o.GetType().GetTypeInfo().Assembly.GetName().Name;
            }
            return null;
        }
        public static string GetFullAssemblyName(object o)
        {
            if (o != null)
            {
                return o.GetType().GetTypeInfo().Assembly.GetName().FullName;
            }
            return null;
        }
        public static AssemblyName GetAssemblyName(object o)
        {
            if (o != null)
            {
                return o.GetType().GetTypeInfo().Assembly.GetName();
            }
            return null;
        }

        public static string GetName(string baseName, string assemblyName)
        {
            if (string.IsNullOrEmpty(baseName) || string.IsNullOrEmpty(assemblyName))
                return null;
            return baseName + ", " + assemblyName;
        }

        public static string GetName(string baseName, Assembly assy)
        {
            if (string.IsNullOrEmpty(baseName) || assy == null)
                return null;
            return baseName + ", " + assy.FullName;
        }

        public static Type GetType(string baseName, string assemblyName)
        {
            return Type.GetType(GetName(baseName, assemblyName));
        }
    }
}
