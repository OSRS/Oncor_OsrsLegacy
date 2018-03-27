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

using Osrs.Runtime;
using System;
using System.Collections.Generic;

namespace Osrs.Net.Http.Routing
{
    public abstract class MatchRule : IHandlerMatcher
    {
        public IHandlerMatcher NextMatcher
        {
            get;
            set;
        }

        protected internal abstract bool MatchImpl(HttpContext ctx);

        public bool Match(HttpContext ctx)
        {
            if (ctx!=null && this.MatchImpl(ctx))
            {
                if (this.NextMatcher!=null)
                    return this.NextMatcher.Match(ctx);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Implements a "starts with" local sub-path url matching rule.
    /// Match rule will extract the local path (everything after the matched listener url) and match against the provided paths.
    /// For example, a path of /foo/goo/moo/dog.food will match against rules:
    /// /, /foo, /foo/, /foo/goo/mo, etc.
    /// </summary>
    public class UrlBaseMatchRule : MatchRule
    {
        private readonly HashSet<string> partialUris = new HashSet<string>();

        public UrlBaseMatchRule(params string[] partialUris)
        {
            if (partialUris==null || partialUris.Length<1) //default
            {
                this.partialUris.Add("/");
            }
            else
            {
                foreach (string cur in partialUris)
                {
                    if (cur.StartsWith("/"))
                        this.partialUris.Add(cur);
                    else
                        this.partialUris.Add("/" + cur); //we have to anchor for matching against /
                }
            }
        }

        public UrlBaseMatchRule(IEnumerable<string> partialUris)
        {
            MethodContract.NotNull(partialUris, nameof(partialUris));

            foreach (string cur in partialUris)
            {
                if (cur.StartsWith("/"))
                    this.partialUris.Add(cur);
                else
                    this.partialUris.Add("/" + cur); //we have to anchor for matching against /
            }
        }

        protected internal override bool MatchImpl(HttpContext ctx)
        {
            string match = ctx.Request.GetEncodedUrl();

            if (!string.IsNullOrEmpty(match))
            {
                string part = null;
                foreach (string curHost in ctx.Addresses)
                {
                    if (match.StartsWith(curHost, StringComparison.OrdinalIgnoreCase))
                        part = curHost;
                }
                if (part != null)
                {
                    //subset the part from the listener
                    match = match.Substring(part.Length);
                    if (!match.StartsWith("/"))
                        match = "/" + match; //we have to anchor for matching against /

                    foreach (string cur in this.partialUris)
                    {
                        if (match.StartsWith(cur, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            return false;
        }
    }

    public class ProtocolMatchRule : MatchRule
    {
        private readonly string[] prots;

        public ProtocolMatchRule(params string[] protocols)
        {
            if (protocols != null)
            {
                HashSet<string> pro = new HashSet<string>();
                foreach (string cur in protocols)
                {
                    if (!string.IsNullOrEmpty(cur))
                        pro.Add(cur);
                }
                if (pro.Count < 1)
                    pro.Add("http");
                this.prots = new string[pro.Count];
                int i = 0;
                foreach (string cur in pro)
                {
                    this.prots[i] = cur;
                    i++;
                }
            }
            else
                this.prots = new string[] { "http" };
        }

        public ProtocolMatchRule(IEnumerable<string> protocols)
        {
            if (protocols != null)
            {
                HashSet<string> pro = new HashSet<string>();
                foreach (string cur in protocols)
                {
                    if (!string.IsNullOrEmpty(cur))
                        pro.Add(cur);
                }
                if (pro.Count < 1)
                    pro.Add("http");
                this.prots = new string[pro.Count];
                int i = 0;
                foreach(string cur in pro)
                {
                    this.prots[i] = cur;
                    i++;
                }
            }
            else
                this.prots = new string[] { "http" };
        }

        protected internal override bool MatchImpl(HttpContext ctx)
        {
            if (ctx!=null)
            {
                string req = ctx.Request.Protocol;
                foreach (string cur in this.prots)
                {
                    if (req.Equals(cur, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Implements a limited set of http verbs matching this rule.
    /// </summary>
    public class VerbRestrictingMatchRule : MatchRule
    {
        private readonly string[] legalVerbs;

        public VerbRestrictingMatchRule(IEnumerable<string> verbs)
        {
            if (verbs == null) //default to "simple verbs"
            {
                this.legalVerbs = new string[] { HttpVerbs.GET.ToString(), HttpVerbs.POST.ToString() };
            }
            else
            {
                List<string> tmp = new List<string>();
                foreach (string cur in verbs)
                    tmp.Add(cur);
                this.legalVerbs = tmp.ToArray();
            }
        }

        public VerbRestrictingMatchRule(params string[] verbs)
        {
            if (verbs == null || verbs.Length < 1) //default to "simple verbs"
            {
                this.legalVerbs = new string[] { HttpVerbs.GET.ToString(), HttpVerbs.POST.ToString() };
            }
            else
                this.legalVerbs = verbs;
        }

        protected internal override bool MatchImpl(HttpContext ctx)
        {
            string verb = ctx.Request.Method;
            foreach (string cur in this.legalVerbs)
            {
                if (cur.Equals(verb, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Implements a network filtering by using the IIPFilter interface.
    /// Note that this will not filter properly if the requests are proxied (e.g through a load balancer or reverse proxy that changes source IPs)
    /// </summary>
    public class IpNetworkRestrictingMatchRule : MatchRule
    {
        private readonly IIPFilter filter;

        public IpNetworkRestrictingMatchRule(IIPFilter ipFilter)
        {
            MethodContract.NotNull(ipFilter, nameof(ipFilter));
            this.filter = ipFilter;
        }

        protected internal override bool MatchImpl(HttpContext ctx)
        {
            return this.filter.Contains(ctx.Connection.RemoteIpAddress);
        }
    }

    /// <summary>
    /// Creates a wrapper type for linking matchers together for form a match chain.
    /// This should really only be used for wrapping matchers that did not extend the MatchLink type, otherwise this is redundant.
    /// </summary>
    public sealed class MatchLinkRule : MatchRule
    {
        private readonly IHandlerMatcher localMatcher;

        public MatchLinkRule(IHandlerMatcher localMatcher)
        {
            MethodContract.NotNull(localMatcher, nameof(localMatcher));
            this.localMatcher = localMatcher;
        }

        protected internal override bool MatchImpl(HttpContext ctx)
        {
            return this.localMatcher.Match(ctx);
        }
    }
}
