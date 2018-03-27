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
    public struct CancellationToken
    {
        private CancellationTokenSource source;

        private bool cancelInitiable;
        public bool CanInitiateCancel
        {
            get
            {
                return this.cancelInitiable;
            }
        }

        public bool IsCancellationRequested
        {
            get
            {
                return source != null && source.IsCancellationRequested;
            }
        }

        public bool CanBeCanceled
        {
            get
            {
                return source != null && source.CanBeCanceled;
            }
        }

        public bool CanRegisterCallback
        {
            get
            {
                return source != null && source.CanRegisterCallback;
            }
        }

        public void RegisterCallback(Action callback)
        {
            if (this.CanRegisterCallback)
                this.source.RegisterCallback(callback);
        }

        public void RequestCancel()
        {
            if (this.cancelInitiable && this.CanBeCanceled)
                this.source.Cancel();
        }

        internal CancellationToken(CancellationTokenSource source, bool cancelInitiable)
        {
            this.source = source;
            this.cancelInitiable = cancelInitiable;
        }

        private static CancellationToken None
        {
            get { return default(CancellationToken); }
        }
    }
}
