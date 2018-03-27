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

using Osrs.Net;
using Osrs.Threading;
using System;
using System.Threading.Tasks;

namespace Osrs.Runtime.Services
{
    internal sealed class ServerRunner<T>
    {
        internal readonly HandoffEvent<T> Eventer;
        private readonly Action handle;

        internal ServerRunner(Func<T> listener, Action<T> handler, int maxQueueSize)
        {
            this.Eventer = new HandoffEvent<T>(listener, handler, maxQueueSize);
            this.handle = this.Eventer.Handle;
        }

        public void Run()
        {
            this.Eventer.Listen();
            Task.Run(this.handle);
        }
    }

    public abstract class ServerTaskPool<T> : ISimpleService
    {
        public abstract RunState State
        {
            get;
        }

        public abstract void Start();
        public abstract void Stop();
    }

    public sealed class SingleServerTaskPool<T> : ServerTaskPool<T>
    {
        private TaskPool pool;
        private ServerRunner<T> runner;

        private RunState state = RunState.Created;
        public override RunState State
        {
            get { return this.state; }
        }

        public SingleServerTaskPool(IServerListener<T> serverLogic, ServerTaskPoolOptions options) : this(serverLogic, serverLogic, options)
        { }

        public SingleServerTaskPool(IListener<T> listener, IHandler<T> handler, ServerTaskPoolOptions options)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.NotNull(options, nameof(options));

            Setup((Func<T>)listener.GetContext, (Action<T>)handler.Handle, options);
        }

        public SingleServerTaskPool(Func<T> listener, Action<T> handler, ServerTaskPoolOptions options)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.NotNull(options, nameof(options));

            Setup(listener, handler, options);
        }

        public override void Start()
        {
            lock (this.pool)
            {
                if (this.state == RunState.Created || this.state == RunState.Stopped)
                {
                    this.state = RunState.Starting;
                    try
                    {
                        this.runner.Eventer.Token = this.pool.CurrentToken;
                        TaskPoolRunner r = null;
                        do {
                            r = this.pool.Next();
                            if (r!=null)
                                r.Run(); //scavenging pool will make this a never-ending loop across all threads
                        }
                        while (r==null);
                        this.state = RunState.Running;
                    }
                    catch
                    {
                        this.state = RunState.FailedStarting;
                    }
                }
            }
        }

        public override void Stop()
        {
            lock(this.pool)
            {
                if (this.state == RunState.Running)
                {
                    this.state = RunState.Stopping;
                    try
                    {
                        this.pool.Reset(); //pool will cancel all running requests
                        this.state = RunState.Stopped;
                    }
                    catch
                    {
                        this.state = RunState.FailedStopping;
                    }
                }
            }
        }

        private void Setup(Func<T> listener, Action<T> handler, ServerTaskPoolOptions options)
        {
            TaskPoolOptions poolOpts = new TaskPoolOptions();
            poolOpts.MaxActiveWorkers = options.MaxActiveWorkers;
            poolOpts.MaxWaitingRequests = options.MaxWaitingAccepts;
            poolOpts.Timeout = options.Timeout;
            this.runner = new ServerRunner<T>(listener, handler, options.MaxWaitingRequests);
            this.pool = ScavengingTaskPoolRunner.CreateScavengingTaskPool(runner.Run, poolOpts);
        }
    }

    public sealed class DualServerTaskPool<T> : ServerTaskPool<T>
    {
        private TaskPool listenerPool;
        private TaskPool handlerPool;
        private Action handle;
        private int handlerThreads;
        private CancellationToken token;
        private HandoffEvent<T> eventer;

        private RunState state = RunState.Created;
        public override RunState State
        {
            get { return this.state; }
        }

        public DualServerTaskPool(IServerListener<T> serverLogic, ServerTaskPoolOptions options) : this(serverLogic, serverLogic, options)
        { }

        public DualServerTaskPool(IListener<T> listener, IHandler<T> handler, ServerTaskPoolOptions options)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.NotNull(options, nameof(options));

            Setup((Func<T>)listener.GetContext, (Action<T>)handler.Handle, options);
        }

        public DualServerTaskPool(Func<T> listener, Action<T> handler, ServerTaskPoolOptions options)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.NotNull(options, nameof(options));

            Setup(listener, handler, options);
        }

        private void Forever()
        {
            while(true)
            {
                if (!this.token.IsCancellationRequested)
                    this.handle();
                else
                    return; //exit
            }
        }

        public override void Start()
        {
            lock (this.listenerPool)
            {
                if (this.state == RunState.Created || this.state == RunState.Stopped)
                {
                    this.state = RunState.Starting;
                    try
                    {
                        this.token = this.handlerPool.CurrentToken;
                        this.eventer.Token = this.handlerPool.CurrentToken;
                        this.listenerPool.Next().Run(); //scavenging pool will make this a never-ending loop across all threads
                        
                        //spin up all the handlers
                        TaskPoolRunner r = null;
                        for (int i=0;i<this.handlerThreads;i++)
                        {
                            do
                            {
                                r = this.handlerPool.Next();
                                if (r != null)
                                    r.Run(); //scavenging pool will make this a never-ending loop across all threads
                            }
                            while (r == null);
                            r = null;
                        }

                        this.state = RunState.Running;
                    }
                    catch
                    {
                        this.state = RunState.FailedStarting;
                    }
                }
            }
        }

        public override void Stop()
        {
            lock (this.listenerPool)
            {
                if (this.state == RunState.Running)
                {
                    this.state = RunState.Stopping;
                    try
                    {
                        this.listenerPool.Reset(); //pool will cancel all running requests
                        this.handlerPool.Reset();
                        this.state = RunState.Stopped;
                    }
                    catch
                    {
                        this.state = RunState.FailedStopping;
                    }
                }
            }
        }

        private void Setup(Func<T> listener, Action<T> handler, ServerTaskPoolOptions options)
        {
            TaskPoolOptions poolOpts = new TaskPoolOptions();
            poolOpts.MaxActiveWorkers = options.MaxActiveListenerWorkers;
            poolOpts.MaxWaitingRequests = options.MaxWaitingAccepts;
            poolOpts.Timeout = options.Timeout;

            this.eventer = new HandoffEvent<T>(listener, handler, options.MaxWaitingRequests);
            this.handle = eventer.Handle;
            this.listenerPool = ScavengingTaskPoolRunner.CreateScavengingTaskPool(eventer.Listen, poolOpts);

            this.handlerThreads = options.MaxActiveHandlerWorkers;
            poolOpts = new TaskPoolOptions();
            poolOpts.MaxActiveWorkers = this.handlerThreads;
            poolOpts.MaxWaitingRequests = options.MaxWaitingRequests;
            poolOpts.Timeout = new Timeout(0);
            this.handlerPool = new TaskPool(this.Forever, poolOpts);
        }
    }
}
