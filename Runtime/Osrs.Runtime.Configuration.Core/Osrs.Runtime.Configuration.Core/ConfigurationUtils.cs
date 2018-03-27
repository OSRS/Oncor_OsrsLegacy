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
using System.Text;

namespace Osrs.Runtime.Configuration
{
    public static class ConfigurationUtils
    {
        public const string FileName = "fileName";
        public const string ProviderName = "provider";
        public const string ConnectionStringName = "connectionString";

        public static string PrefixName(string prefix, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(prefix))
                    return (prefix + char.ToUpperInvariant(name[0]) + name.Substring(1));
                return name;
            }

            return prefix;
        }

        public static object ConfigBytes(byte[] data, Type dataType)
        {
            if (data!=null)
                return ConfigBytes(data, dataType, 0, data.Length);

            return ConfigBytes(data, dataType, 0, 0);
        }

        public static object ConfigBytes(byte[] data, Type dataType, int start, int length)
        {
            if (dataType!=null)
            {
                //TODO -- actually finish these -- only byte[] is done
                if (TypeList.IsArray(dataType))
                {
                    if (TypeList.StringArray.Equals(dataType))
                    {
                        if (data != null)
                            return Encoding.UTF8.GetString(data, start, length);
                        return (string[])null;
                    }
                    else if (TypeList.ByteArray.Equals(dataType))
                    {
                        if (data != null)
                        {
                            byte[] tmp = new byte[length];
                            Array.Copy(data, start, tmp, 0, length);
                        }
                        return (byte[])null;
                    }
                    else if (TypeList.DoubleArray.Equals(dataType))
                    {
                        if (data != null)
                            return Encoding.UTF8.GetString(data, start, length);
                        return (double[])null;
                    }
                    else if (TypeList.FloatArray.Equals(dataType))
                    {
                        if (data != null)
                            return Encoding.UTF8.GetString(data, start, length);
                        return (float[])null;
                    }
                    else if (TypeList.IntArray.Equals(dataType))
                    {
                        if (data != null)
                            return Encoding.UTF8.GetString(data, start, length);
                        return (int[])null;
                    }
                    else if (TypeList.LongArray.Equals(dataType))
                    {
                        if (data != null)
                            return Encoding.UTF8.GetString(data, start, length);
                        return (long[])null;
                    }
                }

                if (TypeList.IsDict(dataType))
                {
                    if (TypeList.StringDict.Equals(dataType))
                    {

                    }
                    else if (TypeList.DoubleDict.Equals(dataType))
                    {

                    }
                    else if (TypeList.FloatDict.Equals(dataType))
                    {

                    }
                    else if (TypeList.IntDict.Equals(dataType))
                    {

                    }
                    else if (TypeList.LongDict.Equals(dataType))
                    {

                    }
                }

                //primitive
                if (TypeList.String.Equals(dataType))
                {
                    if (data!=null)
                        return Encoding.UTF8.GetString(data, start, length);
                    return (string)null;
                }
                else if (TypeList.Bool.Equals(dataType))
                {
                    if (data != null)
                        return BitConverter.ToBoolean(data, start);
                    throw new ArgumentException(nameof(data));
                }
                else if (TypeList.Double.Equals(dataType))
                {
                    if (data != null)
                        return BitConverter.ToDouble(data, start);
                    throw new ArgumentException(nameof(data));
                }
                else if (TypeList.Float.Equals(dataType))
                {
                    if (data != null)
                        return BitConverter.ToSingle(data, start);
                    throw new ArgumentException(nameof(data));
                }
                else if (TypeList.Int.Equals(dataType))
                {
                    if (data != null)
                        return BitConverter.ToInt32(data, start);
                    throw new ArgumentException(nameof(data));
                }
                else if (TypeList.Long.Equals(dataType))
                {
                    if (data != null)
                        return BitConverter.ToInt64(data, start);
                    throw new ArgumentException(nameof(data));
                }
            }

            if (data == null)
                return null;

            throw new ArgumentException(nameof(dataType));
        }

        public static byte[] DataBytes(ConfigurationParameter param)
        {
            if (param.Value !=null)
            {
                object o = param.Value;

                if (o is Array)
                {
                    if (o is byte[])
                        return (byte[])o; //already done

                    List<byte[]> raw = new List<byte[]>();
                    if (o is string[])
                    {
                        string[] s = (string[])o;
                        foreach (string cur in s)
                        {
                            if (!string.IsNullOrEmpty(cur))
                                raw.Add(Encoding.UTF8.GetBytes(cur));
                            else if (cur == null)
                                raw.Add(null);
                            else
                                raw.Add(new byte[0]);
                        }
                    }
                    if (o is int[])
                    {
                        int[] s = (int[])o;
                        foreach (int cur in s)
                        {
                            raw.Add(BitConverter.GetBytes(cur));
                        }
                    }
                    if (o is long[])
                    {
                        long[] s = (long[])o;
                        foreach (long cur in s)
                        {
                            raw.Add(BitConverter.GetBytes(cur));
                        }
                    }
                    if (o is float[])
                    {
                        float[] s = (float[])o;
                        foreach (float cur in s)
                        {
                            raw.Add(BitConverter.GetBytes(cur));
                        }
                    }
                    if (o is double[])
                    {
                        double[] s = (double[])o;
                        foreach (double cur in s)
                        {
                            raw.Add(BitConverter.GetBytes(cur));
                        }
                    }

                    return Merge(raw);
                }

                if (o is string)
                {
                    string s = (string)o;
                    if (!string.Empty.Equals(s))
                        return Encoding.UTF8.GetBytes(s);
                    return new byte[0];
                }
                if (o is bool)
                {
                    return BitConverter.GetBytes((bool)o);
                }
                if (o is int)
                {
                    return BitConverter.GetBytes((int)o);
                }
                if (o is long)
                {
                    return BitConverter.GetBytes((long)o);
                }
                if (o is float)
                {
                    return BitConverter.GetBytes((float)o);
                }
                if (o is double)
                {
                    return BitConverter.GetBytes((double)o);
                }

                if (o is Dictionary<string, string>)
                {
                    Dictionary<string, string> ss = (Dictionary<string, string>)o;
                    List<byte[]> raw = new List<byte[]>();

                    byte[] tmpKey;
                    byte[] tmpVal;
                    byte[] tmpRes;
                    int keyLen;
                    int valLen;
                    foreach (KeyValuePair<string, string> si in ss)
                    {
                        tmpKey = Encoding.UTF8.GetBytes(si.Key);
                        keyLen = tmpKey.Length;
                        tmpVal = Encoding.UTF8.GetBytes(si.Value);
                        valLen = tmpVal.Length;
                        tmpRes = new byte[keyLen + valLen + 8];
                        Array.Copy(BitConverter.GetBytes(keyLen), 0, tmpRes, 0, 4);
                        Array.Copy(BitConverter.GetBytes(valLen), 0, tmpRes, 4, 4);
                        Array.Copy(tmpKey, 0, tmpRes, 8, keyLen);
                        Array.Copy(tmpVal, 0, tmpRes, 8 + keyLen, valLen);
                        raw.Add(tmpRes);
                    }
                    tmpKey = null;
                    tmpVal = null;
                    tmpRes = null;
                    return Merge(raw);
                }
                if (o is Dictionary<string, int>)
                {
                    Dictionary<string, int> ss = (Dictionary<string, int>)o;
                    List<byte[]> raw = new List<byte[]>();

                    byte[] tmpKey;
                    byte[] tmpRes;
                    int keyLen;
                    foreach (KeyValuePair<string, int> si in ss)
                    {
                        tmpKey = Encoding.UTF8.GetBytes(si.Key);
                        keyLen = tmpKey.Length;
                        tmpRes = new byte[keyLen + 8];
                        Array.Copy(BitConverter.GetBytes(keyLen), 0, tmpRes, 0, 4);
                        Array.Copy(tmpKey, 0, tmpRes, 4, keyLen);
                        Array.Copy(BitConverter.GetBytes(si.Value), 0, tmpRes, 4 + keyLen, 4);
                        raw.Add(tmpRes);
                    }
                    tmpKey = null;
                    tmpRes = null;
                    return Merge(raw);
                }
                if (o is Dictionary<string, long>)
                {
                    Dictionary<string, long> ss = (Dictionary<string, long>)o;
                    List<byte[]> raw = new List<byte[]>();

                    byte[] tmpKey;
                    byte[] tmpRes;
                    int keyLen;
                    foreach (KeyValuePair<string, long> si in ss)
                    {
                        tmpKey = Encoding.UTF8.GetBytes(si.Key);
                        keyLen = tmpKey.Length;
                        tmpRes = new byte[keyLen + 12];
                        Array.Copy(BitConverter.GetBytes(keyLen), 0, tmpRes, 0, 4);
                        Array.Copy(tmpKey, 0, tmpRes, 4, keyLen);
                        Array.Copy(BitConverter.GetBytes(si.Value), 0, tmpRes, 4 + keyLen, 8);
                        raw.Add(tmpRes);
                    }
                    tmpKey = null;
                    tmpRes = null;
                    return Merge(raw);
                }
                if (o is Dictionary<string, float>)
                {
                    Dictionary<string, float> ss = (Dictionary<string, float>)o;
                    List<byte[]> raw = new List<byte[]>();

                    byte[] tmpKey;
                    byte[] tmpRes;
                    int keyLen;
                    foreach (KeyValuePair<string, float> si in ss)
                    {
                        tmpKey = Encoding.UTF8.GetBytes(si.Key);
                        keyLen = tmpKey.Length;
                        tmpRes = new byte[keyLen + 8];
                        Array.Copy(BitConverter.GetBytes(keyLen), 0, tmpRes, 0, 4);
                        Array.Copy(tmpKey, 0, tmpRes, 4, keyLen);
                        Array.Copy(BitConverter.GetBytes(si.Value), 0, tmpRes, 4 + keyLen, 4);
                        raw.Add(tmpRes);
                    }
                    tmpKey = null;
                    tmpRes = null;
                    return Merge(raw);
                }
                if (o is Dictionary<string, double>)
                {
                    Dictionary<string, double> ss = (Dictionary<string, double>)o;
                    List<byte[]> raw = new List<byte[]>();

                    byte[] tmpKey;
                    byte[] tmpRes;
                    int keyLen;
                    foreach (KeyValuePair<string, double> si in ss)
                    {
                        tmpKey = Encoding.UTF8.GetBytes(si.Key);
                        keyLen = tmpKey.Length;
                        tmpRes = new byte[keyLen + 12];
                        Array.Copy(BitConverter.GetBytes(keyLen), 0, tmpRes, 0, 4);
                        Array.Copy(tmpKey, 0, tmpRes, 4, keyLen);
                        Array.Copy(BitConverter.GetBytes(si.Value), 0, tmpRes, 4 + keyLen, 8);
                        raw.Add(tmpRes);
                    }
                    tmpKey = null;
                    tmpRes = null;
                    return Merge(raw);
                }
            }
            return null;
        }


        private static byte[] Merge(List<byte[]> raw)
        {
            //get a total count of bytes by computing length of every element
            int len = 0;
            for (int i = 0; i < raw.Count; i++)
            {
                if (raw[i] != null)
                    len += raw[i].Length;
            }

            len = len + 4 + (4 * raw.Count); //4 bytes per element for len + initial count
            byte[] polished = new byte[len];
            byte[] tmp = BitConverter.GetBytes(raw.Count);
            Array.Copy(tmp, 0, polished, 0, 4); //copy into head

            len = 4;
            for (int i = 0; i < raw.Count; i++)
            {
                if (raw[i] != null)
                {
                    if (raw[i].Length > 0)
                    {
                        tmp = BitConverter.GetBytes(raw[i].Length);
                        Array.Copy(tmp, 0, polished, len, 4);
                        len += 4;
                        Array.Copy(raw[i], 0, polished, len, raw[i].Length);
                        len += raw[i].Length;
                    }
                    else
                    {
                        tmp = BitConverter.GetBytes((int)0); //empty array
                        Array.Copy(tmp, 0, polished, len, 4);
                        len += 4;
                    }
                }
                else
                {
                    tmp = BitConverter.GetBytes((int)-1); //null array
                    Array.Copy(tmp, 0, polished, len, 4);
                    len += 4;
                }
            }
            return polished;
        }
    }
}
