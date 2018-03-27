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
    /// Class to allow a service to be treated as a module with convention based initialiation rather than configuration.
    /// The implementing class is responsible for handling the initialization of the service.
    /// The concept is to simply perform the configuration by convention for the "inner" service via configuration in the implementing wrapper class via the InitializeService method.
    /// </summary>
    public abstract class ModuleConventionWrapper : IModule
    {
        private readonly object syncRoot = new object();
        private IService inner;

        private RunState state;
        public RunState State
        {
            get
            {
                if (this.inner!=null)
                    return this.inner.State;
                return this.state;
            }
            protected set
            {
                this.state = value; //if inner is already loaded, this will have no effect
            }
        }

        protected ModuleConventionWrapper()
        {
            this.state = RunState.Created;
        }

        protected internal abstract IService InitializeService();

        public void Initialize()
        {
            lock (this.syncRoot) //overhead cost is ok on this method
            {
                if (this.inner == null)
                {
                    if (RuntimeUtils.Initializable(this.state)) //we only initialize if we're created and not initialized
                    {
                        this.state = RunState.Initializing;
                        this.inner = this.InitializeService();
                        if (this.state == RunState.Initializing && this.inner != null)
                            this.state = RunState.Initialized;
                        else //state was changed by concrete class
                        {
                            if (this.inner != null) //we got something back but the state was changed
                            {
                                if (this.state == RunState.Initialized)
                                    return; //the implementing class set it ok, so we just ignore it
                                //something is odd, so we assume failure
                                this.inner = null;
                                this.state = RunState.FailedInitializing;
                            }
                            else //we got no service back, so we must be failed
                                this.state = RunState.FailedInitializing;
                        }
                    }
                }
            }
        }

        public void Pause()
        {
            if (this.inner!=null)
                this.inner.Pause();
        }

        public void Resume()
        {
            if (this.inner != null)
                this.inner.Resume();
        }

        public void Start()
        {
            if (this.inner != null)
                this.inner.Start();
        }

        public void Stop()
        {
            if (this.inner != null)
                this.inner.Stop();
        }
    }
}
