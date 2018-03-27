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

namespace Osrs.Threading
{
    /// <summary>
    /// A representation of a timeout period in milliseconds
    /// </summary>
    public sealed class Timeout
    {
        private readonly int timeout;

        public int Millis
        {
            get { return this.timeout; }
        }

        public bool IsInfinite
        {
            get { return this.timeout < 1; }
        }

        public Timeout(int millis)
        {
            if (millis > 0)
                this.timeout = millis;
            else
                this.timeout = 0;
        }

        public TimeoutCancellation Create(CancellationTokenSource toCancel)
        {
            if (!this.IsInfinite && toCancel!=null && !toCancel.IsDisposed)
                return new TimeoutCancellation(this, toCancel);
            return null;
        }
    }

    /// <summary>
    /// A timeout trigger for automatically cancelling a long-running method call.
    /// Note that this is using the Osrs cancellation token and is therefore expecting the function to be cancelled to periodically check if cancellation is requested.
    /// </summary>
    public sealed class TimeoutCancellation : IDisposable
    {
        private System.Threading.Timer ticktock;
        private readonly CancellationTokenSource toCancel;

        public bool IsActive
        {
            get
            {
                return this.ticktock!=null && this.toCancel!=null && !this.toCancel.IsDisposed && !this.toCancel.IsCancellationRequested;
            }
        }

        public TimeoutCancellation(Timeout time, CancellationTokenSource toCancel)
        {
            if (time != null && toCancel != null && !time.IsInfinite && toCancel.CanBeCanceled)
            {
                this.ticktock = new System.Threading.Timer(this.Fire, null, time.Millis, System.Threading.Timeout.Infinite);
                this.toCancel = toCancel;
            }
        }

        private void Fire(object nullState)
        {
            try
            {
                this.toCancel.Cancel(false);
            }
            catch
            { }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Dispose(bool disposing)
        {
            if (disposing && this.ticktock!=null)
            {
                this.ticktock.Dispose();
                this.ticktock = null;
            }
        }
    }
}
