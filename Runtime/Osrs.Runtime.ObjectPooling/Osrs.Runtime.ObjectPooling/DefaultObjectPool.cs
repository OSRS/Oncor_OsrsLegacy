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

using System;
using System.Threading;

namespace Osrs.Runtime.ObjectPooling
{
    public class DefaultObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly T[] _items;
        private readonly IPooledObjectPolicy<T> _policy;

        public DefaultObjectPool(IPooledObjectPolicy<T> policy)
            : this(policy, Environment.ProcessorCount * 2)
        {
        }

        public DefaultObjectPool(IPooledObjectPolicy<T> policy, int maximumRetained)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policy = policy;
            _items = new T[maximumRetained];
        }

        public override T Get()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                if (item != null && Interlocked.CompareExchange(ref _items[i], null, item) == item)
                {
                    return item;
                }
            }

            return _policy.Create();
        }

        public override void Return(T obj)
        {
            if (!_policy.Return(obj))
            {
                return;
            }

            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = obj;
                    return;
                }
            }
        }
    }
}
