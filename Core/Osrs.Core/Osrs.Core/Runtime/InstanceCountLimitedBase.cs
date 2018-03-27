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

namespace Osrs.Runtime
{
    public abstract class InstanceCountLimitedBase<T> : IDisposable where T : InstanceCountLimitedBase<T>
    {
        //<Type, <maxCt, instCt>>
        private static readonly Dictionary<Type, KeyValuePair<uint, uint>> instances = new Dictionary<Type, KeyValuePair<uint, uint>>();

        protected uint GetInstanceCount()
        {
            Type t = this.GetType();
            if (instances.ContainsKey(t))
                return instances[t].Value;
            else
                return 0;
        }

        protected uint GetMaxInstanceCount()
        {
            Type t = this.GetType();
            if (instances.ContainsKey(t))
                return instances[t].Key;
            else
                return 0;
        }

        protected InstanceCountLimitedBase(uint maxInstances)
        {
            if (maxInstances > 0)
            {
                lock (instances)
                {
                    Type t = this.GetType();
                    if (instances.ContainsKey(t))
                    {
                        KeyValuePair<uint, uint> cts = instances[t];

                        if (maxInstances > cts.Key)
                            cts = new KeyValuePair<uint, uint>(maxInstances, cts.Value);

                        if (cts.Key >= cts.Value)
                            throw new SingletonException(t.Name + " already exists - use static Instance property for access");
                        instances[t] = new KeyValuePair<uint, uint>(cts.Key, cts.Value + 1);
                    }
                    else
                        instances[t] = new KeyValuePair<uint, uint>(maxInstances, 1);
                }
            }
            else
                throw new ArgumentOutOfRangeException("maxInstances cannot be zero");
        }

        protected InstanceCountLimitedBase()
        {
            lock (instances)
            {
                Type t = this.GetType();
                if (instances.ContainsKey(t))
                {
                    KeyValuePair<uint, uint> cts = instances[t];
                    if (cts.Key >= cts.Value)
                        throw new SingletonException(t.Name + " already exists - use static Instance property for access");
                    instances[t] = new KeyValuePair<uint, uint>(cts.Key, cts.Value + 1);
                }
                else //gee, we made it a singleton
                    instances[t] = new KeyValuePair<uint, uint>(1, 1);
            }
        }

        public virtual void Dispose()
        {
            try
            {
                lock (instances)
                {
                    Type t = this.GetType();
                    KeyValuePair<uint, uint> cts = instances[t];
                    instances[t] = new KeyValuePair<uint, uint>(cts.Key, cts.Value - 1);
                }
            }
            catch { }
        }

        ~InstanceCountLimitedBase()
        {
            try
            {
                if (instances != null)
                    instances.Clear();
            }
            catch { }
        }
    }

    public sealed class InstanceCountLimitedHelper<T>
    {
        private readonly uint maxInstances;
        public uint MaxInstances
        {
            get { return this.maxInstances; }
        }

        private uint instances;
        public uint Instances
        {
            get { return this.instances; }
        }

        public void Construct()
        {
            lock (this) //we want this to be truly exclusive
            {
                if (this.instances >= this.maxInstances)
                    throw new InstanceCountLimitException("Max instance count exceeded.");
                else
                    this.instances++;
            }
        }

        public void Destruct()
        {
            lock (this) //we want this to be truly exclusive
            {
                if (this.instances > 0)
                    this.instances--;
            }
        }

        public InstanceCountLimitedHelper(uint maxInstances)
        {
            if (maxInstances == 0)
                throw new ArgumentOutOfRangeException("maxInstances cannot be zero");
            this.maxInstances = maxInstances;
            this.instances = 0;
        }
    }

    public class InstanceCountLimitException : InstantiationException
    {
        public InstanceCountLimitException() { }

        public InstanceCountLimitException(string message) : base(message) { }

        public InstanceCountLimitException(string message, Exception inner) : base(message, inner) { }
    }
}
