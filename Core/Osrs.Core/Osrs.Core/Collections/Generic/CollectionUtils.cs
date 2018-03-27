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

namespace Osrs.Collections.Generic
{
    public static class CollectionUtils
    {
        public static T[][] Init<T>(int x, int y)
        {
            T[][] tmp = new T[x][];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new T[y];
            }
            return tmp;
        }

        public static T[][][] Init<T>(int x, int y, int z)
        {
            T[][][] tmp = new T[x][][];
            T[][] tmp2;
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp2 = new T[y][];
                for (int j = 0; j < tmp2.Length; j++)
                {
                    tmp2[j] = new T[z];
                }
                tmp[i] = tmp2;
            }
            return tmp;
        }

        public static T[][][][] Init<T>(int x, int y, int z, int d)
        {
            T[][][][] tmp = new T[x][][][];
            T[][][] tmp2;
            T[][] tmp3;
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp2 = new T[y][][];
                for (int j = 0; j < tmp2.Length; j++)
                {
                    tmp3 = new T[z][];
                    for (int k = 0; k < tmp3.Length; k++)
                    {
                        tmp3[k] = new T[d];
                    }
                    tmp2[j] = tmp3;
                }
                tmp[i] = tmp2;
            }
            return tmp;
        }

        public static List<T> ToList<T>(this IEnumerable<T> items)
        {
            if (items == null)
                return null;
            List<T> tmp = new List<T>();
            foreach (T item in items)
                tmp.Add(item);
            return tmp;
        }

        public static void Sort<T>(this T[] data) where T : IComparable<T>
        {
            Array.Sort<T>(data, 0, data.Length, null);
        }

        public static void Sort<T>(this T[] data, IComparer<T> comparer)
        {
            Array.Sort<T>(data, 0, data.Length, comparer);
        }
    }
}
