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
using System.Collections.Generic;
using System.Threading;

namespace Osrs.Threading
{
    /// <summary>
    /// Modeled strongly after the System.Threading.CancellationTokenSource, this is meant to exit a function rather than stop a thread.
    /// While both can be used this way, the Osrs implementation does not directly support being used by System.Threading.Tasks and therefore is a simpler bet for exiting a function.
    /// The general use case is in a server to abort a request and return to the listener loop early.
    /// 
    /// Each time through the loop should use a new cancellation source rather than reusing.  This is in fact required once a source has been cancelled (it cannot be "uncancelled").
    /// </summary>
    public sealed class CancellationTokenSource : IDisposable
    {
        private const int CANNOT_CANCEL = 0;
        private const int NOT_CANCELLED = 1;
        private const int REQUESTED = 2;
        private const int NOTIFIED = 3;

        private volatile int state = NOT_CANCELLED;

        public bool IsCancellationRequested
        {
            get { return this.state > NOT_CANCELLED; }
        }

        public bool IsCancellationCompleted
        {
            get { return this.state == NOTIFIED; }
        }

        internal bool CanBeCanceled
        {
            get { return this.state != CANNOT_CANCEL; }
        }

        private bool disposed = false;
        public bool IsDisposed
        {
            get { return this.disposed; }
        }

        public CancellationToken Token()
        {
            ThrowIfDisposed();
            return new CancellationToken(this, false);
        }

        public CancellationToken Token(bool canInitiateCancel)
        {
            ThrowIfDisposed();
            return new CancellationToken(this, canInitiateCancel);
        }

        public CancellationTokenSource() : this(true, true)
        { }

        public CancellationTokenSource(bool allowCallbacks) : this(allowCallbacks, true)
        { }

        public CancellationTokenSource(bool allowCallbacks, bool canBeCancelled)
        {
            if (!canBeCancelled)
                this.state = CANNOT_CANCEL;
            this.allowCallbacks = allowCallbacks;
        }

        private readonly bool allowCallbacks = false;
        public bool CanRegisterCallback
        {
            get
            {
                return this.allowCallbacks && !this.disposed && this.state<REQUESTED;
            }
        }

        private readonly List<Action> callbacks = new List<Action>();
        /// <summary>
        /// If permitted, adds a callback that is invoked if a cancellation is requested
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterCallback(Action callback)
        {
            if (this.allowCallbacks && callback != null && this.state < REQUESTED) //don't try to add if cancel is already requested
                this.callbacks.Add(callback); //slight chance of issue if requested happens in the interim cycles
        }

        public void Cancel()
        {
            this.Cancel(false);
        }

        public void Cancel(bool throwOnException)
        {
            if (NOT_CANCELLED == Interlocked.CompareExchange(ref this.state, REQUESTED, NOT_CANCELLED))
            {
                if (this.allowCallbacks && this.callbacks.Count > 0)
                {
                    foreach (Action cur in this.callbacks)
                    {
                        try
                        {
                            cur();
                        }
                        catch (Exception e)
                        {
                            if (throwOnException)
                            {
                                this.state = NOTIFIED; //since we know it's requested and we "own" it
                                throw e;
                            }
                        }
                    }
                }
                this.state = NOTIFIED; //since we know it's requested and we "own" it
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Dispose(bool disposing)
        {
            // There is nothing to do if disposing=false because the CancellationTokenSource holds no unmanaged resources.
            if (disposing && this.allowCallbacks)
            {
                this.disposed = true;
                try
                {
                    this.callbacks.Clear(); //all we want to do is clear out the callbacks in case they have unmanaged resources
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Throws an exception if the source has been disposed.
        /// </summary>
        internal void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException(null, "CancellationTokenSource_Disposed");
        }

        private static readonly CancellationTokenSource def = new CancellationTokenSource(false, true);
        internal static CancellationTokenSource Default
        {
            get { return def; }
        }
    }
}
