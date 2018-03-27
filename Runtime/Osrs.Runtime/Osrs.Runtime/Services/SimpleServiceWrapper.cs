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

namespace Osrs.Runtime.Services
{
    /// <summary>
    /// A wrapper class to treat an IBaseService as an IService.
    /// In this case, it simply routes calls to Pause to Start, and calls to Resume to Stop.
    /// All references to State will pass through to the IBaseService, so it will never get into the Paused or Resumed states.
    /// </summary>
    public class SimpleServiceWrapper : IService
    {
        private readonly ISimpleService inner;

        public RunState State
        {
            get
            {
                return this.inner.State;
            }
        }

        public SimpleServiceWrapper(IService inner)
        {
            MethodContract.NotNull(inner, nameof(inner));
            this.inner = inner;
        }

        public void Pause()
        {
            this.Stop();
        }

        public void Resume()
        {
            this.Start();
        }

        public void Start()
        {
            this.inner.Start();
        }

        public void Stop()
        {
            this.inner.Stop();
        }
    }
}
