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
using Osrs.Net.Http.Server;
using Osrs.Threading;
using Osrs.Runtime;

namespace Osrs.Net.Http.Listener
{
    public sealed class HttpListenerServerListener : HttpServerListener, IDisposable
    {
        private readonly List<string> urls = new List<string>();
        private readonly System.Net.HttpListener listener;
        private readonly IHttpHandler handler;

        public HttpListenerServerListener(IEnumerable<string> urls, IHttpHandler handler):base()
        {
            MethodContract.NotNull(urls, nameof(urls));
            MethodContract.NotNull(handler, nameof(handler));
            foreach(string url in urls)
            {
                Uri x;
                MethodContract.Assert(Uri.TryCreate(url, UriKind.Absolute, out x), string.Format("{0} not a valid uri", nameof(urls)));
                MethodContract.Assert(x.Scheme == Uri.UriSchemeHttp || x.Scheme == Uri.UriSchemeHttps, string.Format("{0} not a valid http/https uri", nameof(urls)));
                if (url.EndsWith("/"))
                    this.urls.Add(url);
                else
                    this.urls.Add(url + '/');
            }

            this.handler = handler;
            this.listener = new System.Net.HttpListener();
            this.listener.AuthenticationSchemes = System.Net.AuthenticationSchemes.Anonymous;
            foreach (string uri in this.urls)
            {
                this.listener.Prefixes.Add(uri);
            }
        }

        public void Start()
        {
            this.listener.Start();
        }

        public void Stop()
        {
            this.listener.Stop();
        }

        public override HttpContext GetContext(CancellationToken cancel)
        {
            System.Net.HttpListenerContext ctx = this.listener.GetContext();
            return new HttpListenerContext(ctx, cancel, this.urls);
        }

        public override void Handle(HttpContext context, CancellationToken cancel)
        {
            HttpListenerResponse rsp;
            try
            {
                this.handler.Handle(context, cancel);
            }
            catch(Exception e)
            {
                try
                {
                    context.Response.StatusCode = HttpStatusCodes.Status500InternalServerError;
                    rsp = context.Response as HttpListenerResponse;
                    rsp.Close();
                }
                catch
                { } //oh well, we tried - but get back to work

                return;
            }

            rsp = context.Response as HttpListenerResponse;
            rsp.Close();
        }

        public void Dispose()
        {
            try
            {
                if (this.listener != null)
                {
                    this.listener.Close();
                }
            }
            catch
            { }
        }
    }
}
