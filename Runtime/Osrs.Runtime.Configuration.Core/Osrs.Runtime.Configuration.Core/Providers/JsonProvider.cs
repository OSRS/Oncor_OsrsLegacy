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
using System.IO;

namespace Osrs.Runtime.Configuration.Providers
{
    public sealed class JsonProviderFactory : ConfigurationFactoryBase
    {
        private readonly Dictionary<string, Dictionary<string, object>> configs;

        public JsonProviderFactory()
        {
            this.configs = new Dictionary<string, Dictionary<string, object>>();
        }

        protected internal override ConfigurationProviderBase GetProvider()
        {
            return new JsonProvider(this.configs);
        }

        protected internal override bool Initialize()
        {
            ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
            ConfigurationParameter param = config.Get(typeof(JsonProviderFactory), "fileName");
            if (param == null)
                return false;
            try
            {
                string fileName = (string)param.Value;
                JsonProvider p = new JsonProvider(this.configs); //init will fill this dictionary for reuse
                bool x = p.Init(Path.GetFullPath(fileName));
                if (x)
                {
                    return true; //so we can finish init successfully
                }
            }
            catch
            { }
            return false;
        }
    }

    /// <summary>
    /// File format for config params in json
    /// {
    ///     "|typeName|":
    ///     {
    ///         "|paramName|":|data|
    ///     },
    ///     ...
    /// }
    /// where |typeName| is: namespace.class, assemblyname
    ///     example: System.String, System.Core
    /// and |paramName| is any legal parameter name in json
    /// and |data| is any:
    ///     string, int, long, float, double, array of {string|int|float|double}, dictionary of string,{string|int|float|double}
    ///     the dictionaries are stored/formatted as an object in json
    /// </summary>
    public sealed class JsonProvider : ConfigurationProviderBase
    {
        private readonly Dictionary<string, Dictionary<string, object>> configs;

        internal JsonProvider()
        {
            this.configs = new Dictionary<string, Dictionary<string, object>>();
        }

        internal JsonProvider(Dictionary<string, Dictionary<string, object>> configs)
        {
            this.configs = configs;
        }

        protected internal override ConfigurationParameter GetImpl(string typeName, string name)
        {
            if (this.configs.ContainsKey(typeName))
            {
                if (this.configs[typeName].ContainsKey(name))
                    return ConfigurationParameter.Create(typeName, name, this.configs[typeName][name]);
                else
                {
                    TypeNameReference ty = TypeNameReference.Parse(typeName);
                    if(ty!=null) //try for a new file
                    {
                        string corePath = Path.GetDirectoryName(AppContext.BaseDirectory);
                        if (File.Exists(Path.Combine(corePath, ty.AssemblyFileName + ".jconfig")))
                        {
                            try
                            {
                                this.Init(Path.Combine(corePath, ty.AssemblyFileName + ".jconfig"));
                            }
                            catch
                            { }

                            if (this.configs[typeName].ContainsKey(name))
                                return ConfigurationParameter.Create(typeName, name, this.configs[typeName][name]);
                        }
                    }
                }
            }
            return null;
        }
                
        internal bool Init(string filename)
        {
            //format of file is specific for config params
            JsonReader r = new JsonReader();
            JObject o = r.Read(filename);
            if (o != null)
            {
                foreach(string s in o.Keys) //types
                {
                    JEntity e = o[s];
                    if (e!=null && e is JObject)
                    {
                        Dictionary<string, object> parms = new Dictionary<string,object>();
                        if (!this.configs.ContainsKey(s))
                            this.configs.Add(s, parms);
                        else
                            parms = this.configs[s];

                        JObject cur = (JObject)e;
                        foreach(string k in cur.Keys) //param names
                        {
                            e = cur[k]; //now it's a param
                            object paramData = null;
                            if (e!=null)
                            {
                                if (e is JObject) //treat as dictionary
                                {
                                    JObject param = (JObject)e;
                                    paramData = BuildDictionary(param);
                                    if (paramData != null)
                                        parms[k]= paramData;
                                    else
                                        return false;
                                }
                                else if (e is JArray) //as array
                                {
                                    JArray param = (JArray)e;
                                    paramData = BuildArray(param);
                                    if (paramData != null)
                                        parms[k] = paramData;
                                    else
                                        return false;
                                }
                                else //JPrimitive
                                {
                                    JPrimitive param = (JPrimitive)e;
                                    if (param != null)
                                    {
                                        if (param.Type == JType.JString)
                                            parms[k] = param.ToString();
                                        else if (param.Type == JType.JNumber)
                                        {
                                            try
                                            {
                                                int i = param.ToInt();
                                                parms[k] = i;
                                            }
                                            catch 
                                            {
                                                try
                                                {
                                                    long l = param.ToLong();
                                                    parms[k] = l;
                                                }
                                                catch
                                                {
                                                    try
                                                    {
                                                        float f = param.ToFloat();
                                                        parms[k] = f;
                                                    }
                                                    catch
                                                    {
                                                        try
                                                        {
                                                            double d = param.ToDouble();
                                                            parms[k] = d;
                                                        }
                                                        catch
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (param.Type == JType.JBool)
                                            parms[k] = param.ToBool();
                                    }
                                    else
                                        return false;
                                }
                            }
                        }
                    }
                    else
                        return false; //we read it all, or none
                }
                return true;
            }

            return false;
        }

        private object BuildDictionary(JObject o)
        {
            JType lookType = JType.JString;

            foreach(string s in o.Keys)
            {
                JEntity e = o[s];
                if (e!=null)
                {
                    if (e is JPrimitive) //only thing currently supported
                    {
                        JPrimitive p = (JPrimitive)e;
                        lookType = p.Type;
                    }
                    else
                        return null; //fail fast

                    break; //end the loop anyway -- only needed to see the type of the first element
                }
                return null; //got a null thing - which shouldn't be in the config
            }

            if (lookType == JType.JString)
            {
                Dictionary<string, string> sVals = new Dictionary<string, string>();
                foreach(string key in o.Keys)
                {
                    JEntity e = o[key];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type== JType.JString)
                            sVals.Add(key, p.ToString());
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals;
            }
            else if (lookType == JType.JBool)
            {
                Dictionary<string, bool> sVals = new Dictionary<string, bool>();
                foreach (string key in o.Keys)
                {
                    JEntity e = o[key];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JBool)
                            sVals.Add(key, p.ToBool());
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals;
            }
            else //number
            {
                object res = BuildDictionary(o, false);
                if (res == null)
                    res = BuildDictionary(o, true);
                if (res == null)
                    return null;
            }

            return null;
        }

        private object BuildDictionary(JObject o, bool hasDecimal)
        {
            if (hasDecimal) //try float, then double, then give up
            {
                Dictionary<string, float> sVals = new Dictionary<string, float>();
                foreach (string key in o.Keys)
                {
                    JEntity e = o[key];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JNumber)
                        {
                            try
                            {
                                sVals.Add(key, p.ToFloat());
                            }
                            catch
                            {
                                sVals.Clear();
                                sVals = null; //just in case a GC run comes along
                                Dictionary<string, double> kVals = new Dictionary<string, double>();
                                foreach (string key2 in o.Keys)
                                {
                                    e = o[key2];
                                    if (e != null && e is JPrimitive)
                                    {
                                        p = (JPrimitive)e;
                                        if (p.Type == JType.JNumber)
                                        {
                                            try
                                            {
                                                kVals.Add(key, p.ToDouble());
                                            }
                                            catch
                                            {
                                                return null; //not an integer type we can handle
                                            }
                                        }
                                        else
                                            return null; //short circuit early
                                    }
                                    else
                                        return null; //short circuit early
                                }
                                return kVals;
                            }
                        }
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals;
            }
            else //try int, then long, then give up
            {
                //try ints first, then widen if needed
                Dictionary<string, int> sVals = new Dictionary<string, int>();
                foreach (string key in o.Keys)
                {
                    JEntity e = o[key];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JNumber)
                        {
                            try
                            {
                                sVals.Add(key, p.ToInt());
                            }
                            catch
                            {
                                if (p.ToString().IndexOf('.') >= 0)
                                    return null; //not an integer type
                                else //widen and try longs
                                {
                                    sVals.Clear();
                                    sVals = null; //just in case a GC run comes along
                                    Dictionary<string, long> kVals = new Dictionary<string, long>();
                                    foreach (string key2 in o.Keys)
                                    {
                                        e = o[key2];
                                        if (e != null && e is JPrimitive)
                                        {
                                            p = (JPrimitive)e;
                                            if (p.Type == JType.JNumber)
                                            {
                                                try
                                                {
                                                    kVals.Add(key, p.ToLong());
                                                }
                                                catch
                                                {
                                                    return null; //not an integer type we can handle
                                                }
                                            }
                                            else
                                                return null; //short circuit early
                                        }
                                        else
                                            return null; //short circuit early
                                    }
                                    return kVals;
                                }
                            }
                        }
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals;
            }
        }

        private object BuildArray(JArray o)
        {
            JType lookType = JType.JString;
            if (o.Count>0)
            {
                if (o[0] is JPrimitive)
                {
                    JPrimitive p = (JPrimitive)o[0];
                    lookType = p.Type;
                }
            }

            if (lookType == JType.JString)
            {
                List<string> sVals = new List<string>();
                for (int i = 0; i < o.Count;i++ )
                {
                    JEntity e = o[i];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JString)
                            sVals.Add(p.ToString());
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals.ToArray();
            }
            else if (lookType == JType.JBool)
            {
                List<bool> sVals = new List<bool>();
                for (int i = 0; i < o.Count; i++)
                {
                    JEntity e = o[i];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JString)
                            sVals.Add(p.ToBool());
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals.ToArray();
            }
            else //number
            {
                object res = BuildArray(o, false);
                if (res == null)
                    res = BuildArray(o, true);
                if (res == null)
                    return null;
            }

            return null;
        }

        private object BuildArray(JArray o, bool hasDecimal)
        {
            if (hasDecimal) //try float, then double, then give up
            {
                List<float> sVals = new List<float>();
                for (int i = 0; i < o.Count; i++)
                {
                    JEntity e = o[i];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JNumber)
                        {
                            try
                            {
                                sVals.Add(p.ToFloat());
                            }
                            catch
                            {
                                sVals.Clear();
                                sVals = null; //just in case a GC run comes along
                                List<double> kVals = new List<double>();
                                for (int j = 0; j < o.Count; j++)
                                {
                                    e = o[j];
                                    if (e != null && e is JPrimitive)
                                    {
                                        p = (JPrimitive)e;
                                        if (p.Type == JType.JNumber)
                                        {
                                            try
                                            {
                                                kVals.Add(p.ToDouble());
                                            }
                                            catch
                                            {
                                                return null; //not an integer type we can handle
                                            }
                                        }
                                        else
                                            return null; //short circuit early
                                    }
                                    else
                                        return null; //short circuit early
                                }
                                return kVals.ToArray();
                            }
                        }
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals.ToArray();
            }
            else //try int, then long, then give up
            {
                //try ints first, then widen if needed
                List<int> sVals = new List<int>();
                for (int i = 0; i < o.Count; i++)
                {
                    JEntity e = o[i];
                    if (e != null && e is JPrimitive)
                    {
                        JPrimitive p = (JPrimitive)e;
                        if (p.Type == JType.JNumber)
                        {
                            try
                            {
                                sVals.Add(p.ToInt());
                            }
                            catch
                            {
                                if (p.ToString().IndexOf('.') >= 0)
                                    return null; //not an integer type
                                else //widen and try longs
                                {
                                    sVals.Clear();
                                    sVals = null; //just in case a GC run comes along
                                    List<long> kVals = new List<long>();
                                    for (int j = 0; j < o.Count; j++)
                                    {
                                        e = o[j];
                                        if (e != null && e is JPrimitive)
                                        {
                                            p = (JPrimitive)e;
                                            if (p.Type == JType.JNumber)
                                            {
                                                try
                                                {
                                                    kVals.Add(p.ToLong());
                                                }
                                                catch
                                                {
                                                    return null; //not an integer type we can handle
                                                }
                                            }
                                            else
                                                return null; //short circuit early
                                        }
                                        else
                                            return null; //short circuit early
                                    }
                                    return kVals.ToArray();
                                }
                            }
                        }
                        else
                            return null; //short circuit early
                    }
                    else
                        return null; //short circuit early
                }
                return sVals.ToArray();
            }
        }
    }
}
