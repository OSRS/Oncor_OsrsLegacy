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

using Osrs.Threading;
using System;

namespace Osrs.Runtime.Services
{
    public sealed class TaskPoolOptions
    {
        private int threads = Environment.ProcessorCount;
        public int MaxActiveWorkers
        {
            get { return this.threads; }
            set
            {
                if (value > 0)
                    this.threads = value;
            }
        }

        private int maxTasks=short.MaxValue; //65535 seems ok
        public int MaxWaitingRequests
        {
            get { return this.maxTasks; }
            set
            {
                if (value > 0)
                    this.maxTasks = value;
            }
        }

        private Timeout timeout = null;//infinite
        public Timeout Timeout
        {
            get { return this.timeout; }
            set
            {
                this.timeout = value;
            }
        }

        public TaskPoolOptions()
        { }
    }
}
