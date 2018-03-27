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
using System.Collections.Generic;

namespace Osrs.Net.Http.Listener
{
    public sealed class HttpListenerContext : HttpContext
    {
        private readonly System.Net.HttpListenerContext inner;
        private CancellationToken cancelled;

        private HttpConnectionInfo conn;
        public override HttpConnectionInfo Connection
        {
            get
            {
                if (this.conn==null)
                {
                    this.conn = new HttpListenerConnectionInfo(this.inner);
                }
                return this.conn;
            }
        }

        private HttpRequest req;
        public override HttpRequest Request
        {
            get
            {
                if (this.req==null)
                    this.req = new HttpListenerRequest(this, this.inner);
                return this.req;
            }
        }

        public override CancellationToken RequestAborted
        {
            get
            {
                return this.cancelled;
            }

            set
            {
                this.cancelled = value;
            }
        }

        private HttpResponse res;
        public override HttpResponse Response
        {
            get
            {
                if (this.res == null)
                    this.res = new HttpListenerResponse(this, this.inner);
                return this.res;
            }
        }

        public override WebSocketManager WebSockets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private readonly ICollection<string> addresses;
        public override ICollection<string> Addresses
        {
            get
            {
                return this.addresses;
            }
        }

        public override void Abort()
        {
            if (this.cancelled.CanInitiateCancel)
                this.cancelled.RequestCancel();
        }

        internal HttpListenerContext(System.Net.HttpListenerContext ctx, CancellationToken cancelled, ICollection<string> addresses)
        {
            this.inner = ctx;
            this.cancelled = cancelled;
            this.addresses = addresses;
        }
    }
}
