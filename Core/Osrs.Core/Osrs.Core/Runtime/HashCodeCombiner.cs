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

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Osrs.Runtime
{
    public struct HashCodeCombiner
    {
        private long combinedHash64;

        public int CombinedHash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return combinedHash64.GetHashCode(); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashCodeCombiner(long seed)
        {
            combinedHash64 = seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IEnumerable e)
        {
            if (e == null)
            {
                Add(0);
            }
            else
            {
                var count = 0;
                foreach (object o in e)
                {
                    Add(o);
                    count++;
                }
                Add(count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(HashCodeCombiner self)
        {
            return self.CombinedHash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int i)
        {
            combinedHash64 = ((combinedHash64 << 5) + combinedHash64) ^ i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string s)
        {
            var hashCode = (s != null) ? s.GetHashCode() : 0;
            Add(hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(object o)
        {
            var hashCode = (o != null) ? o.GetHashCode() : 0;
            Add(hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<TValue>(TValue value, IEqualityComparer<TValue> comparer)
        {
            var hashCode = value != null ? comparer.GetHashCode(value) : 0;
            Add(hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashCodeCombiner Start()
        {
            return new HashCodeCombiner(0x1505L);
        }
    }
}
