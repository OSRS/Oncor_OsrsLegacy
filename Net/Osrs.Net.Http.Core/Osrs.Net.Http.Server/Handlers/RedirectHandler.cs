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
using Osrs.Net.Http.Headers;
using System.Collections.Generic;
using Osrs.Runtime;
using System.Text;

namespace Osrs.Net.Http.Handlers
{
    public abstract class HttpHandlerBase : IHttpHandler
    {
        public void Handle(HttpContext context)
        {
            this.Handle(context, default(CancellationToken));
        }

        public abstract void Handle(HttpContext context, CancellationToken cancel);
    }

    public class RedirectToHttpsHandler : HttpHandlerBase
    {
        private readonly bool permanent;

        public RedirectToHttpsHandler(bool permanent)
        {
            this.permanent = permanent;
        }

        public override void Handle(HttpContext context, CancellationToken cancel)
        {
            HttpRequest re = context.Request;
            if (permanent)
                context.Response.StatusCode = HttpStatusCodes.Status301MovedPermanently;
            else
                context.Response.StatusCode = HttpStatusCodes.Status302Found;

            context.Response.Headers[HeaderNames.Location] = new StringBuilder().Append("https://").Append(re.Host).Append(re.PathBase).Append(re.Path).Append(re.QueryString).ToString();
        }
    }

    public class RedirectHandler : HttpHandlerBase
    {
        public delegate bool PatternMatch(string url);
        public delegate string Rewrite(string url);
        public readonly bool permanent;
        private readonly List<KeyValuePair<PatternMatch, Rewrite>> rules = new List<KeyValuePair<PatternMatch, Rewrite>>();

        public RedirectHandler() : this(false)
        { }

        public RedirectHandler(bool permanent)
        {
            this.permanent = permanent;
        }

        public void Add(PatternMatch matcher, Rewrite rewriter)
        {
            MethodContract.NotNull(matcher, nameof(matcher));
            MethodContract.NotNull(rewriter, nameof(rewriter));
            this.rules.Add(new KeyValuePair<PatternMatch, Rewrite>(matcher, rewriter));
        }

        public override void Handle(HttpContext context, CancellationToken cancel)
        {
            string uri = context.Request.GetEncodedUrl();
            foreach(KeyValuePair<PatternMatch, Rewrite> cur in this.rules)
            {
                if (cur.Key(uri))
                {
                    Redirect(context.Response, cur.Value(uri), this.permanent);
                    return;
                }
            }
            //oops, not found so we need to respond properly
            context.Response.StatusCode = HttpStatusCodes.Status404NotFound;
        }

        private void Redirect(HttpResponse response, string location, bool permanent)
        {
            if (permanent)
                response.StatusCode = HttpStatusCodes.Status301MovedPermanently;
            else
                response.StatusCode = HttpStatusCodes.Status302Found;

            response.Headers[HeaderNames.Location] = location;
        }
    }
}
