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

using Osrs.Runtime.Services;
using Osrs.Net.Http.Server;
using Osrs.Runtime;
using System;

namespace Osrs.Net.Http.Listener
{
    public sealed class HttpListenerServer : HttpServer, IDisposable
    {
        private readonly HttpListenerServerListener listener;

        public override void Start()
        {
            this.listener.Start();
            base.Start();
        }

        public override void Stop()
        {
            this.listener.Stop(); //interrupts the active getcontext()
            base.Stop();
        }

        public static HttpListenerServer Create(HttpListenerServerListener listener, ServerTaskPoolOptions options, bool useSinglePool)
        {
            MethodContract.NotNull(listener, nameof(listener));
            MethodContract.NotNull(options, nameof(options));

            if (useSinglePool)
            {
                return new HttpListenerServer(new SingleServerTaskPool<HttpContext>(listener, options), listener);
            }
            else
            {
                return new HttpListenerServer(new DualServerTaskPool<HttpContext>(listener, options), listener);
            }
        }

        private HttpListenerServer(ServerTaskPool<HttpContext> pool, HttpListenerServerListener listener) : base(pool)
        {
            this.listener = listener;
        }

        public void Dispose()
        {
            try
            {
                if (this.listener != null)
                {
                    this.listener.Dispose();
                }
            }
            catch
            { }
        }
    }
}
