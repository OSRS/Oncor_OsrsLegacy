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

using System.Collections.Generic;
using System.IO;
using Osrs.Net.Http.Cookies;
using Osrs.Net.Http.Headers;
using Osrs.Text;
using System.Collections.Specialized;

namespace Osrs.Net.Http.Listener
{
    public sealed class HttpListenerRequest : HttpRequest
    {
        private readonly System.Net.HttpListenerRequest inner;

        private bool unset = true;
        private Stream body;
        public override Stream Body
        {
            get
            {
                if (this.body==null && this.unset)
                {
                    if (this.inner.HasEntityBody)
                        this.body = this.inner.InputStream;
                    this.unset = false; //in any case, we tried to set it
                }
                return this.body;
            }

            set
            {
                this.body = value;
                this.unset = false; //so we don't override it later
            }
        }

        private long? contentLength;
        public override long? ContentLength
        {
            get
            {
                return this.contentLength;
            }

            set
            {
                this.contentLength = value;
            }
        }

        private string contentType;
        public override string ContentType
        {
            get
            {
                return this.contentType;
            }

            set
            {
                if (value != null)
                    this.contentType = value;
                else
                    this.contentType = string.Empty;
            }
        }

        private IRequestCookieCollection cookies;
        public override IRequestCookieCollection Cookies
        {
            get
            {
                if (this.cookies == null)
                    this.cookies = new HttpListenerRequestCookieCollection(this.inner);
                return this.cookies;
            }

            set
            {
                if (value!=null)
                    this.cookies = value;
            }
        }

        private bool loadHeaders = true;
        private readonly IHeaderDictionary headers = new HeaderDictionary();
        public override IHeaderDictionary Headers
        {
            get
            {
                if (this.loadHeaders)
                {
                    NameValueCollection coll = this.inner.Headers;
                    foreach(string dat in coll.Keys)
                    {
                        this.headers[dat] = new StringValues(coll[dat]);
                    }
                    this.loadHeaders = false;
                }
                return this.headers;
            }
        }

        public override HostString Host
        {
            get;
            set;
        }

        private readonly HttpListenerContext context;
        public override HttpContext HttpContext
        {
            get
            {
                return this.context;
            }
        }

        public override bool IsHttps
        {
            get
            {
                return this.inner.IsSecureConnection;
            }

            set
            {
                //do nothing
            }
        }

        public override string Method
        {
            get;
            set;
        }

        public override PathString Path
        {
            get;
            set;
        }

        public override PathString PathBase
        {
            get;
            set;
        }

        public override string Protocol
        {
            get;
            set;
        }

        public override IQueryCollection Query
        {
            get;
            set;
        }

        public override QueryString QueryString
        {
            get;
            set;
        }

        public override string Scheme
        {
            get;
            set;
        }

        internal HttpListenerRequest(HttpListenerContext context, System.Net.HttpListenerContext innerContext)
        {
            this.context = context;
            this.inner = innerContext.Request;

            this.Scheme = this.inner.Url.Scheme;
            this.Host = new HostString(this.inner.UserHostName);
            this.Method = this.inner.HttpMethod;
            int qIndex = this.inner.RawUrl.IndexOf('?');
            if (qIndex < 0)
                this.Path = PathString.FromUriComponent(this.inner.RawUrl);
            else
                this.Path = PathString.FromUriComponent(this.inner.RawUrl.Substring(0, qIndex));
            this.PathBase = string.Empty;
            this.Protocol = this.inner.Url.Scheme;
            this.Query = QueryHelpers.ToQueryCollection(inner.Url);
            this.QueryString = QueryString.FromUriComponent(inner.Url);
        }
    }
}
