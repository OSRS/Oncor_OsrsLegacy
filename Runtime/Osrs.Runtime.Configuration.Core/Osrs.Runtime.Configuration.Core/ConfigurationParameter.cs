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

namespace Osrs.Runtime.Configuration
{
    public sealed class ConfigurationParameter //yes, it's an ugly mess
    {
        public string OwningType { get; private set; }

        public string Name { get; private set; }

        public object Value { get; private set; }

        internal ConfigurationParameter(string owningType, string name)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = null;
        }

        internal ConfigurationParameter(string owningType, string name, string value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, bool value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, int value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, long value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, float value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, double value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, string[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, byte[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, int[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, long[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, float[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, double[] value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, Dictionary<string, string> value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, Dictionary<string, int> value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, Dictionary<string, long> value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, Dictionary<string, float> value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal ConfigurationParameter(string owningType, string name, Dictionary<string, double> value)
        {
            this.OwningType = owningType;
            this.Name = name;
            this.Value = value;
        }

        internal static ConfigurationParameter Create(string owningType, string name, object o)
        {
            if (o==null)
                return new ConfigurationParameter(owningType, name);

            if (o is Array)
            {
                if (o is byte[])
                    return new ConfigurationParameter(owningType, name, (byte[])o);
                if (o is string[])
                    return new ConfigurationParameter(owningType, name, (string[])o);
                if (o is int[])
                    return new ConfigurationParameter(owningType, name, (int[])o);
                if (o is long[])
                    return new ConfigurationParameter(owningType, name, (long[])o);
                if (o is float[])
                    return new ConfigurationParameter(owningType, name, (float[])o);
                if (o is double[])
                    return new ConfigurationParameter(owningType, name, (double[])o);
            }

            if (o is string)
                return new ConfigurationParameter(owningType, name, (string)o);
            if (o is bool)
                return new ConfigurationParameter(owningType, name, (bool)o);
            if (o is int)
                return new ConfigurationParameter(owningType, name, (int)o);
            if (o is long)
                return new ConfigurationParameter(owningType, name, (long)o);
            if (o is float)
                return new ConfigurationParameter(owningType, name, (float)o);
            if (o is double)
                return new ConfigurationParameter(owningType, name, (double)o);

            if (o is Dictionary<string, string>)
                return new ConfigurationParameter(owningType, name, (Dictionary<string, string>)o);
            if (o is Dictionary<string, int>)
                return new ConfigurationParameter(owningType, name, (Dictionary<string, int>)o);
            if (o is Dictionary<string, long>)
                return new ConfigurationParameter(owningType, name, (Dictionary<string, long>)o);
            if (o is Dictionary<string, float>)
                return new ConfigurationParameter(owningType, name, (Dictionary<string, float>)o);
            if (o is Dictionary<string, double>)
                return new ConfigurationParameter(owningType, name, (Dictionary<string, double>)o);

            return null; //unknown type
        }
    }
}
