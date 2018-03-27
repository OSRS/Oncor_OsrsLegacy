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
using System.Threading.Tasks;

namespace Osrs.Runtime.Services
{
    /// <summary>
    /// A broker for a task that runs a standard method.
    /// In general, a TaskPool is like a Threadpool with tasks waiting to be selected to work
    /// The asynchronous part is the Run() method, which is non-blocking
    /// </summary>
    public class TaskPool
    {
        private readonly TaskPoolOptions options;
        private readonly TaskPoolRunner runner;

        private CancellationTokenSource tokenSource;
        public CancellationToken CurrentToken
        {
            get { return this.tokenSource.Token(false); }
        }

        private volatile int activeWorkers = 0;
        private volatile int activeWaiters = 0; //how many calls to Next() are waiting

        public TaskPool(Action toRun, TaskPoolOptions options)
        {
            MethodContract.NotNull(toRun, nameof(toRun));
            this.options = new TaskPoolOptions();
            if (options != null)
            {
                this.options.MaxWaitingRequests = options.MaxWaitingRequests;
                this.options.MaxActiveWorkers = options.MaxActiveWorkers;
                this.options.Timeout = options.Timeout;
            }
            this.activeWorkers = 0;
            this.runner = new TaskPoolRunner(this, toRun, this.options.Timeout);
            this.Reset();
        }

        internal void ReturnToQueue() //attempt to return a runner
        {
            System.Threading.Interlocked.Decrement(ref this.activeWorkers);
        }

        public TaskPoolRunner Next()
        {
            if (this.options.MaxWaitingRequests > System.Threading.Interlocked.Increment(ref this.activeWaiters))
            {
                if (!this.tokenSource.IsCancellationRequested)
                {
                    System.Threading.SpinWait waiter = new System.Threading.SpinWait();
                    while (this.options.MaxActiveWorkers < System.Threading.Interlocked.Increment(ref this.activeWorkers))
                    {
                        System.Threading.Interlocked.Decrement(ref this.activeWorkers); //nope, not yet
                        if (this.tokenSource.IsCancellationRequested)
                        {
                            System.Threading.Interlocked.Decrement(ref this.activeWaiters); //we're leaving, so be nice and stop waiting
                            return null;
                        }
                        waiter.SpinOnce();
                    }

                    //Note that the while loop incremented the activeWorkers for us, how nice

                    System.Threading.Interlocked.Decrement(ref this.activeWaiters); //we're now a worker
                    return this.runner; //done -- we still have to decrement active workers in the runner when finished
                }
                //System.Threading.Interlocked.Decrement(ref this.activeWaiters); //we're leaving, so be nice and stop waiting
            }
            System.Threading.Interlocked.Decrement(ref this.activeWaiters);
            return null; //can't wait because we have too many already waiting
        }

        public void Reset()
        {
            lock (this.runner)
            {
                this.activeWaiters = int.MaxValue; //guaranteed to make requests drop on the floor
                if (this.activeWorkers > 0)
                {
                    if (!this.tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            this.tokenSource.Cancel();
                        }
                        catch
                        { }
                    }
                    System.Threading.SpinWait waiter = new System.Threading.SpinWait();
                    while (this.activeWorkers > 0)
                        waiter.SpinOnce(); //this is the possible gotcha - we have to be sure all tasks play nicely
                }

                this.activeWaiters = 0;
                this.tokenSource = new CancellationTokenSource(true, true);
                this.tokenSource.RegisterCallback(this.runner.Cancel);
                this.runner.NativeToken = new System.Threading.CancellationTokenSource();
                this.runner.Token = this.tokenSource.Token();
            }
        }
    }

    /// <summary>
    /// Note that there's only 1 runner per task pool, so the tokens are shared
    /// </summary>
    public sealed class TaskPoolRunner
    {
        private readonly Action run;
        private readonly Action exec;
        private readonly TaskPool pool;
        private readonly Timeout timeout;

        internal System.Threading.CancellationTokenSource NativeToken;

        internal CancellationToken Token;

        internal TaskPoolRunner(TaskPool pool, Action run, Timeout timeout)
        {
            this.pool = pool;
            this.run = run;
            this.exec = (Action)this.Exec; //just to do the cast once and save a cycle or 2 maybe
            this.timeout = timeout;
        }

        private void Exec()
        {
            try
            {
                if (!this.Token.IsCancellationRequested)
                    this.run(); //do the work
            }
            catch
            { }
            finally
            {
                this.pool.ReturnToQueue(); //make available again
            }
        }

        internal void Cancel()
        {
            try
            {
                if(!this.NativeToken.IsCancellationRequested)
                    this.NativeToken.CancelAfter(5000); //allow ample time for a graceful shutdown
            }
            catch
            { }
        }

        public void Run(Timeout timeout)
        {
            if (!this.Token.IsCancellationRequested)
            {
                if (timeout == null || timeout.IsInfinite)
                    Task.Run((Action)this.Exec);
                else //TODO -- add timeout
                {
                    Task.Run(this.exec, NativeToken.Token);
                }
            }
        }

        public void Run()
        {
            this.Run(this.timeout);
        }
    }
}
