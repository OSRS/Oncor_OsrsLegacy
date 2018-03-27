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
using Osrs.Runtime;
using Osrs.Runtime.Services;

namespace Osrs.Net.Servers
{
    public abstract class ServerBase<T> : IService
    {
        private readonly object syncRootStartStop = new object();
        private readonly ServerTaskPool<T> pool;

        protected ServerBase(ServerTaskPool<T> pool)
        {
            MethodContract.NotNull(pool, nameof(pool));
            this.pool = pool;
        }

        public RunState State
        {
            get
            {
                return this.pool.State;
            }
        }

        public virtual void Pause()
        {
            this.Stop();
        }

        public virtual void Resume()
        {
            this.Start();
        }

        public virtual void Start()
        {
            lock (this.syncRootStartStop)
            {
                this.pool.Start();
            }
        }

        public virtual void Stop()
        {
            this.pool.Stop();
        }
    }
}
