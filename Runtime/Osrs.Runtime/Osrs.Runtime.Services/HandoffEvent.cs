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
using System.Collections.Concurrent;

namespace Osrs.Runtime.Services
{
    /// <summary>
    /// An event handoff between threads running in an eventloop to form a pipeline using task pools.
    /// For example, a server has 2 core stages:
    ///     T GetContext();
    ///     void Handle(T context);
    /// The first stage is the listener which happens on one event loop timed by a task pool (often with a single thread).
    /// The second stage is the handler which happens in a different task pool with many worker threads.
    /// The goal is to dispatch the context from the listener to the handler via the event loop model.
    /// 
    /// In this case the T GetContext() method is the triggering event with T being the data handed off (enqueued).
    /// The Handoff event enqueues the context and runs the next stage from the queue
    /// </summary>
    internal sealed class HandoffEvent<T>
    {
        private readonly Func<T> listen;
        private readonly Action<T> handle;
        private readonly ConcurrentQueue<T> data = new ConcurrentQueue<T>();
        private readonly int maxQueueSize;

        internal CancellationToken Token;
        private volatile int currentQueueSize = 0;

        internal HandoffEvent(Func<T> listener, Action<T> handler, int maxQueueSize)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.Assert(maxQueueSize > 0, nameof(maxQueueSize));
            this.listen = listener;
            this.handle = handler;
            this.maxQueueSize = maxQueueSize;
        }

        public void Listen()
        {
            if (this.maxQueueSize > System.Threading.Interlocked.Increment(ref this.currentQueueSize))
                this.data.Enqueue(this.listen());
            else
                System.Threading.Interlocked.Decrement(ref this.currentQueueSize); //we won't contribute to bumping up
        }

        public void Handle()
        {
            try
            {
                T data;

                if (!this.data.TryDequeue(out data))
                {
                    System.Threading.SpinWait waiter = new System.Threading.SpinWait();
                    do
                    {
                        if (!this.Token.IsCancellationRequested)
                            waiter.SpinOnce(); //backs off on the wait rather than just tight spin
                        else
                            break; //we're cancelled
                    }
                    while (!this.data.TryDequeue(out data));
                }

                try
                {
                    if (!this.Token.IsCancellationRequested)
                        this.handle(data);
                }
                catch
                { }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref this.currentQueueSize); //can't forget to pull us out of the contention
                }
            }
            catch(Exception e) //we could in theory get to an bad place where we actually dequeued but didn't decrement since we caught
            {
                System.Threading.Interlocked.Exchange(ref this.currentQueueSize, this.data.Count); //this might be sketchy as well if the queue size changes during the call to Count
            }
        }
    }
}
