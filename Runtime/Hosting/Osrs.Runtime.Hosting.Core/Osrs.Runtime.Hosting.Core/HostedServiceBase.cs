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

using System.Threading;
using System.Threading.Tasks;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Logging.Providers;

namespace Osrs.Runtime.Hosting
{
    public interface IHostedService : IService, ISimpleService, IStatefulExecution, IShutdown, IInitializable
    {}

    public abstract class HostedServiceBase : ServiceBase, IHostedService
    {
        private Task runningTask;

        protected sealed override void StartImpl()
        {
            LogProviderBase logger = LogManager.Instance.GetProvider(this.GetType());
            if (logger == null)
                logger = new NullLogger(this.GetType());
            logger.Log(0, "Start: Called");
            if (this.State == RunState.Starting) //set by base class
            {
                this.runningTask = new Task(this.Run);
                this.runningTask.Start();
                while(this.runningTask.Status == TaskStatus.WaitingForActivation || this.runningTask.Status == TaskStatus.WaitingToRun)
                {
                    Thread.Sleep(0); //wait to allow the start to actually get started
                }
                if (this.State == RunState.Starting)
                    this.State = RunState.Running;
                else if (this.runningTask.Status == TaskStatus.Faulted)
                {
                    this.State = RunState.FailedStarting;
                    logger.Log(0, "Start: Failed on startup, task thread faulted");
                }
            }
            else
                logger.Log(0, "Start: Called from improper state: " + this.State.ToString());
        }

        protected abstract void Run();

        protected sealed override void StopImpl()
        {
            LogProviderBase logger = LogManager.Instance.GetProvider(this.GetType());
            if (logger == null)
                logger = new NullLogger(this.GetType());
            logger.Log(0, "Stop: Called");
            if (this.State == RunState.Stopping)
            {
                logger.Log(0, "Stop: stopping");
                if (this.runningTask != null)
                {
                    int i = 0;
                    while (this.runningTask.Status == TaskStatus.Running)
                    {
                        if ((i % 10) == 0)
                            logger.Log(0, "Stop: waiting for thread to stop "+i.ToString());
                        Thread.Sleep(5); //this is an issue if the task isn't well behaved
                        i++;
                    }
                    if (this.runningTask.Status == TaskStatus.Canceled || this.runningTask.Status == TaskStatus.RanToCompletion)
                        this.runningTask = null;
                    else
                        logger.Log(0, "Stop: thread stopped into an unexpected state " + this.runningTask.Status.ToString());
                }
                if (this.State == RunState.Stopping)
                {
                    this.State = RunState.Stopped;
                    logger.Log(0, "Stop: succeeded");
                }
                else
                    logger.Log(0, "Stop: unexpected final state "+this.State.ToString());
            }
            else
                logger.Log(0, "Stop: Called from improper state: " + this.State.ToString());
        }

        public virtual void Initialize()
        {
            if (RuntimeUtils.Initializable(this.State))
                this.State = RunState.Initialized;
        }

        protected HostedServiceBase() : base()
        { }
    }
}
