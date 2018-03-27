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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Osrs.Runtime.ObjectPooling
{
    public class LeakTrackingObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly ConditionalWeakTable<T, Tracker> _trackers = new ConditionalWeakTable<T, Tracker>();
        private readonly ObjectPool<T> _inner;

        public LeakTrackingObjectPool(ObjectPool<T> inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            _inner = inner;
        }

        public override T Get()
        {
            var value = _inner.Get();
            _trackers.Add(value, new Tracker());
            return value;
        }

        public override void Return(T obj)
        {
            Tracker tracker;
            if (_trackers.TryGetValue(obj, out tracker))
            {
                _trackers.Remove(obj);
                tracker.Dispose();
            }

            _inner.Return(obj);
        }

        private class Tracker : IDisposable
        {
            private readonly string _stack;
            private bool _disposed;

            public Tracker()
            {
                _stack = Environment.StackTrace;
            }

            public void Dispose()
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }

            ~Tracker()
            {
                if (!_disposed && !Environment.HasShutdownStarted)
                {
                    Debug.Fail($"{typeof(T).Name} was leaked. Created at: {Environment.NewLine}{_stack}");
                }
            }
        }
    }
}
