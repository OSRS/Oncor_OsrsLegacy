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
    public static class TypeList
    {
        internal static readonly Type ByteArray = typeof(byte[]);
        internal static readonly Type StringArray = typeof(string[]);
        internal static readonly Type IntArray = typeof(int[]);
        internal static readonly Type LongArray = typeof(long[]);
        internal static readonly Type FloatArray = typeof(float[]);
        internal static readonly Type DoubleArray = typeof(double[]);

        internal static readonly Type Bool = typeof(bool);
        internal static readonly Type String = typeof(string);
        internal static readonly Type Int = typeof(int);
        internal static readonly Type Long = typeof(long);
        internal static readonly Type Float = typeof(float);
        internal static readonly Type Double = typeof(double);

        internal static readonly Type StringDict = typeof(Dictionary<string,string>);
        internal static readonly Type IntDict = typeof(Dictionary<string, int>);
        internal static readonly Type LongDict = typeof(Dictionary<string, long>);
        internal static readonly Type FloatDict = typeof(Dictionary<string, float>);
        internal static readonly Type DoubleDict = typeof(Dictionary<string, double>);

        internal static bool IsArray(Type t)
        {
            return (ByteArray.Equals(t) || StringArray.Equals(t) || IntArray.Equals(t) || LongArray.Equals(t) || FloatArray.Equals(t) || DoubleArray.Equals(t));
        }

        internal static bool IsDict(Type t)
        {
            return (StringDict.Equals(t) || IntDict.Equals(t) || LongDict.Equals(t) || FloatDict.Equals(t) || DoubleDict.Equals(t));
        }
    }
}
