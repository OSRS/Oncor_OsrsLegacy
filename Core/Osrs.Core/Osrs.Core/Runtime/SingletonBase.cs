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
using System.Collections.Generic;

namespace Osrs.Runtime
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>
    {
        internal SingletonBase()
        { }
    }

    public abstract class SubclassableSingletonBase<T> : SingletonBase<T> where T : SubclassableSingletonBase<T>
    {
        private static readonly HashSet<Type> instances = new HashSet<Type>();

        protected SubclassableSingletonBase()
            : base()
        {
            lock (instances)
            {
                Type t = this.GetType();
                if (instances.Contains(t))
                    throw new SingletonException(t.Name + " already exists - use static Instance property for access");
                instances.Add(t);
            }
        }
    }

    public abstract class NonSubclassableSingletonBase<T> : SingletonBase<T> where T : NonSubclassableSingletonBase<T>
    {
        private static string created = null;
        private static readonly object syncRoot = new object();

        protected NonSubclassableSingletonBase()
            : base()
        {
            lock (syncRoot)
            {
                if (created != null)
                    throw new SingletonException("Singleton instance already exists: " + created);
                created = NameReflectionUtils.GetName(this);
            }
        }
    }

    public class SingletonException : InstantiationException
    {
        public SingletonException() { }

        public SingletonException(string message) : base(message) { }

        public SingletonException(string message, Exception inner) : base(message, inner) { }
    }

    public sealed class SingletonHelper<T>
    {
        private T instance;
        public T Instance
        {
            get { return this.instance; }
        }

        public void Construct(T instanceToBe)
        {
            lock (this) //we want this to be truly exclusive
            {
                if (this.instance == null)
                    this.instance = instanceToBe;
                else
                    throw new SingletonException("Singleton instance already exists");
            }
        }

        public SingletonHelper()
        { }
    }
}
