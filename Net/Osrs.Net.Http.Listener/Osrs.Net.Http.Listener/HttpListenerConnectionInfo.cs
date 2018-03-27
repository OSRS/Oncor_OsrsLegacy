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

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Osrs.Net.Http.Listener
{
    public class HttpListenerConnectionInfo : HttpConnectionInfo
    {
        private readonly System.Net.HttpListenerContext inner;

        private X509Certificate2 clientCert;
        private int fetchedCert = 0;
        public override X509Certificate2 ClientCertificate
        {
            get
            {
                if (0==this.fetchedCert)
                {
                    if (1 == Interlocked.Increment(ref this.fetchedCert))
                        this.clientCert = this.inner.Request.GetClientCertificate();
                    else
                        Interlocked.Decrement(ref this.fetchedCert); //this wasn't the winner in a multiple call situation -- the return might still be null though
                }
                return this.clientCert;
            }

            set
            {
                this.clientCert = value;
            }
        }

        private IPAddress localIp;
        public override IPAddress LocalIpAddress
        {
            get
            {
                return this.localIp;
            }

            set
            {
                this.localIp = value;
            }
        }

        private int localPort;
        public override int LocalPort
        {
            get
            {
                return this.localPort;
            }

            set
            {
                this.localPort = value;
            }
        }

        private IPAddress remoteIp;
        public override IPAddress RemoteIpAddress
        {
            get
            {
                return this.remoteIp;
            }

            set
            {
                this.remoteIp = value;
            }
        }

        private int remotePort;
        public override int RemotePort
        {
            get
            {
                return this.remotePort;
            }

            set
            {
                this.remotePort = value;
            }
        }

        internal HttpListenerConnectionInfo(System.Net.HttpListenerContext ctx)
        {
            this.inner = ctx;
            this.localIp = this.inner.Request.LocalEndPoint.Address;
            this.localPort = this.inner.Request.LocalEndPoint.Port;
            this.remoteIp = this.inner.Request.RemoteEndPoint.Address;
            this.remotePort = this.inner.Request.RemoteEndPoint.Port;
        }
    }
}
