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

using System.IO;
using Osrs.Net.Http.Cookies;
using Osrs.Net.Http.Headers;
using Osrs.Runtime.ObjectPooling;

namespace Osrs.Net.Http.Listener
{
    public sealed class HttpListenerResponse : HttpResponse
    {
        private static DefaultObjectPoolProvider pool = new DefaultObjectPoolProvider();
        private readonly System.Net.HttpListenerResponse inner;

        private Stream body;
        public override Stream Body
        {
            get
            {
                return this.body;
            }

            set
            {
                this.body = value;
            }
        }

        public override long? ContentLength
        {
            get
            {
                return this.inner.ContentLength64;
            }
            set
            {
                if (value.HasValue)
                    this.inner.ContentLength64 = value.Value;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.inner.ContentType;
            }

            set
            {
                this.inner.ContentType = value;
            }
        }

        private readonly ResponseCookies cookies;
        public override IResponseCookies Cookies
        {
            get
            {
                return this.cookies;
            }
        }

        private readonly IHeaderDictionary headers;
        public override IHeaderDictionary Headers
        {
            get
            {
                return this.headers;
            }
        }

        private readonly HttpContext context;
        public override HttpContext HttpContext
        {
            get
            {
                return this.context;
            }
        }

        public override int StatusCode
        {
            get
            {
                return this.inner.StatusCode;
            }

            set
            {
                this.inner.StatusCode = value;
            }
        }

        internal HttpListenerResponse(HttpListenerContext context, System.Net.HttpListenerContext inner)
        {
            this.context = context;
            this.inner = inner.Response;
            this.Body = this.inner.OutputStream;
            this.headers = new ResponseHeaderWrapper(inner);
            this.cookies = new ResponseCookies(this.headers, pool.CreateStringBuilderPool());
        }

        internal void Close()
        {
            this.inner.Close();
        }
    }
}
