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

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Osrs.Threading;
using System.Collections.Generic;
using Osrs.Runtime;
using Osrs.Net.Http.Headers;
using Osrs.Text;
using System.Globalization;

namespace Osrs.Net.Http.Handlers
{
    public class CorsOptions
    {
        private static CorsOptions AllowAll { get { return new CorsOptions(); } }

        private readonly HashSet<HttpVerbs> verbs = new HashSet<HttpVerbs>();
        /// <summary>
        /// Gets the verbs (methods) that are supported by the resource.
        /// </summary>
        public ICollection<HttpVerbs> Verbs
        {
            get { return this.verbs; }
        }

        /// <summary>
        /// Gets a value indicating if all verbs (methods) are allowed.
        /// </summary>
        public bool AllowAnyVerb
        {
            get { return this.verbs.Count < 1 || this.verbs.Count == 1 && this.verbs.Contains(HttpVerbs.Unknown); }
        }

        private readonly HashSet<string> origins = new HashSet<string>();
        /// <summary>
        /// Gets the origins that are allowed to access the resource.
        /// </summary>
        public ICollection<string> Origins
        {
            get { return this.origins; }
        }

        /// <summary>
        /// Gets a value indicating if all origins are allowed.
        /// </summary>
        public bool AllowAnyOrigin
        {
            get { return this.origins.Count == 0; }
        }

        private readonly HashSet<string> requestHeaders = new HashSet<string>();
        /// <summary>
        /// Gets the headers that are supported by the resource.
        /// </summary>
        public ICollection<string> RequestHeaders
        {
            get { return this.requestHeaders; }
        }

        /// <summary>
        /// Gets a value indicating if all headers are allowed.
        /// </summary>
        public bool AllowAnyRequestHeader
        {
            get { return this.requestHeaders.Count == 0; }
        }

        private readonly HashSet<string> responseHeaders = new HashSet<string>();
        /// <summary>
        /// Gets the headers that the resource might use and can be exposed.
        /// </summary>
        public ICollection<string> ResponseHeaders
        {
            get { return this.responseHeaders; }
        }

        public bool AllowAnyResponseHeader
        {
            get { return this.responseHeaders.Count == 0; }
        }

        private TimeSpan? preflightMaxAge;

        /// <summary>
        /// Gets or sets the <see cref="TimeSpan"/> for which the results of a preflight request can be cached.
        /// </summary>
        public TimeSpan? PreflightMaxAge
        {
            get
            {
                return preflightMaxAge;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                preflightMaxAge = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the resource supports user credentials in the request.
        /// </summary>
        public bool SupportsCredentials { get; set; }

        public CorsOptions()
        { }
    }

    public class CorsHandler : HttpHandlerBase, IPassThroughHandler
    {
        private readonly CorsOptions options;
        private readonly bool hasNonSimpleVerbs = false;

        public IHttpHandler Next
        {
            get;
            set;
        }

        public CorsHandler(IHttpHandler next, CorsOptions options)
        {
            MethodContract.NotNull(next, nameof(next));
            MethodContract.NotNull(options, nameof(options));

            //lock it down so it can't change while in use
            this.options = new CorsOptions();
            foreach (string cur in options.Origins)
                this.options.Origins.Add(cur);

            foreach (string cur in options.RequestHeaders)
                this.options.RequestHeaders.Add(cur);

            foreach (string cur in options.ResponseHeaders)
                this.options.ResponseHeaders.Add(cur);

            foreach (HttpVerbs cur in options.Verbs)
            {
                foreach (string v in CorsConstants.SimpleMethods) //short list, so not horrible esp. since we do this once per corshandler instance
                {
                    this.hasNonSimpleVerbs = this.hasNonSimpleVerbs || v.Equals(cur.ToString());
                }
                this.options.Verbs.Add(cur);
            }

            this.options.PreflightMaxAge = options.PreflightMaxAge;
            this.options.SupportsCredentials = options.SupportsCredentials;
            this.Next = next;
        }

        public override void Handle(HttpContext context, CancellationToken cancel)
        {
            HttpRequest request = context.Request;
            IHeaderDictionary headers = request.Headers;
            if (headers.ContainsKey(CorsConstants.Origin))
            {
                HttpResponse response = context.Response;
                IHeaderDictionary responseHeaders = response.Headers;

                string origin = headers[CorsConstants.Origin];

                List<string> writeHeaders;

                if (string.Equals(request.Method, CorsConstants.PreflightHttpMethod, StringComparison.OrdinalIgnoreCase) && !headers.IsNullOrEmpty(CorsConstants.AccessControlRequestMethod))
                {
                    //preflight
                    if (!(StringValues.IsNullOrEmpty(origin) || !this.options.AllowAnyOrigin && !this.options.Origins.Contains(origin)))
                    {
                        string accessControlRequestMethod = headers[CorsConstants.AccessControlRequestMethod];
                        if (!this.options.AllowAnyVerb)
                        {
                            bool found = false;
                            foreach(HttpVerbs curVerb in this.options.Verbs)
                            {
                                if (curVerb.ToString().Equals(accessControlRequestMethod, StringComparison.OrdinalIgnoreCase))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            {
                                string[] accessControlRequestHeaders = headers.GetCommaSeparatedValues(CorsConstants.AccessControlRequestHeaders);
                                if (!(!this.options.AllowAnyRequestHeader && accessControlRequestHeaders != null && !AllRequestHeadersGood(accessControlRequestHeaders)))
                                {
                                    AddOrigin(origin, response);
                                    if (!IsSimpleVerb(accessControlRequestMethod)) //we know there's only one that was in the request
                                        responseHeaders.AppendCommaSeparatedValues(CorsConstants.AccessControlAllowMethods, accessControlRequestMethod);

                                    writeHeaders = GetNonSimpleRequestHeaders(accessControlRequestHeaders);
                                    if (writeHeaders.Count > 0)
                                    {
                                        responseHeaders.AppendCommaSeparatedValues(CorsConstants.AccessControlAllowHeaders, writeHeaders.ToArray());
                                    }

                                    if (this.options.PreflightMaxAge.HasValue)
                                        responseHeaders[CorsConstants.AccessControlMaxAge] = this.options.PreflightMaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                    context.Response.StatusCode = HttpStatusCodes.Status204NoContent;
                    return; //this was a preflight, so we don't forward to the actual handler and just send back the CORS info
                }
                else
                {
                    //real deal
                    if (!(StringValues.IsNullOrEmpty(origin) || !options.AllowAnyOrigin && !options.Origins.Contains(origin)))
                    {
                        AddOrigin(origin, response);
                        writeHeaders = GetNonSimpleResponseHeaders(this.options.ResponseHeaders);
                        if (writeHeaders.Count>0)
                        {
                            responseHeaders.AppendCommaSeparatedValues(CorsConstants.AccessControlExposeHeaders, writeHeaders.ToArray());
                        }
                    }
                }
            }

            //after doing the CORS thing, and it's not a preflight, finish by letting the real work happen
            if (this.Next!=null)
                this.Next.Handle(context);
        }

        private List<string> GetNonSimpleRequestHeaders(string[] headers)
        {
            List<string> tmp = new List<string>();
            bool notFound;
            foreach(string cur in headers)
            {
                notFound = true;
                foreach(string curHeader in CorsConstants.SimpleRequestHeaders)
                {
                    if (curHeader.Equals(cur, StringComparison.OrdinalIgnoreCase))
                    {
                        notFound = false;
                        break;
                    }
                }
                if (notFound)
                    tmp.Add(cur);
            }
            return tmp;
        }

        private List<string> GetNonSimpleResponseHeaders(ICollection<string> headers)
        {
            List<string> tmp = new List<string>();
            bool notFound;
            foreach (string cur in headers)
            {
                notFound = true;
                foreach (string curHeader in CorsConstants.SimpleResponseHeaders)
                {
                    if (curHeader.Equals(cur, StringComparison.OrdinalIgnoreCase))
                    {
                        notFound = false;
                        break;
                    }
                }
                if (notFound)
                    tmp.Add(cur);
            }
            return tmp;
        }

        private bool IsSimpleVerb(string verb)
        {
            foreach (string v in CorsConstants.SimpleMethods) //short list, so not horrible esp. since we do this once per corshandler instance
            {
                if (v.Equals(verb, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private bool AllRequestHeadersGood(string[] headers)
        {
            foreach(string cur in headers)
            {
                bool notFound = true;
                foreach(string curSimple in CorsConstants.SimpleRequestHeaders)
                {
                    if (curSimple.Equals(cur, StringComparison.OrdinalIgnoreCase))
                    {
                        notFound = false;
                        break;
                    }
                }
                if (notFound) //not simple if this is still true
                {
                    foreach (string curPolicy in this.options.RequestHeaders)
                    {
                        if (curPolicy.Equals(cur, StringComparison.OrdinalIgnoreCase))
                        {
                            notFound = false;
                            break;
                        }
                    }
                    if (notFound)
                        return false; //not simple and not in list
                }
            }

            return true;
        }

        private void AddOrigin(string origin, HttpResponse response)
        {
            if (this.options.AllowAnyOrigin)
            {
                if (this.options.SupportsCredentials)
                {
                    response.Headers[CorsConstants.AccessControlAllowOrigin] = origin;
                    response.Headers[CorsConstants.VaryByOrigin] = CorsConstants.Origin;
                }
                else
                    response.Headers[CorsConstants.AccessControlAllowOrigin] = CorsConstants.AnyOrigin;
            }
            else if (this.options.Origins.Contains(origin))
                response.Headers[CorsConstants.AccessControlAllowOrigin] = origin;

            if (this.options.SupportsCredentials)
                response.Headers[CorsConstants.AccessControlAllowCredentials] = "true";
        }
    }
}
