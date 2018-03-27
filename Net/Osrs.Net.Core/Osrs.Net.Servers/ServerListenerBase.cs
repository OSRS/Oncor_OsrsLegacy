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
using Osrs.Threading;
using Osrs.Runtime.Services;

namespace Osrs.Net.Servers
{
    public abstract class ServerListenerBase<T> : IServerListener<T>
    {
        public T GetContext()
        {
            return this.GetContext(default(CancellationToken));
        }

        public abstract T GetContext(CancellationToken cancel);

        public void Handle(T context)
        {
            Handle(context, default(CancellationToken));
        }

        public abstract void Handle(T context, CancellationToken cancel);
    }
}
