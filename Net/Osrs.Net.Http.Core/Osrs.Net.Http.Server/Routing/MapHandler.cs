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
using Osrs.Threading;
using Osrs.Runtime;
using System.Collections.Generic;

namespace Osrs.Net.Http.Routing
{
    public abstract class MapHandler : IHandlerMapper
    {
        private readonly IHttpHandler handler;
        private readonly IHandlerMapper map;

        protected MapHandler(IHttpHandler handler)
        {
            MethodContract.NotNull(handler, nameof(handler));
            this.handler = handler;
            this.map = this.handler as IHandlerMapper; //so we can cascade mapping
        }

        public void Handle(HttpContext context)
        {
            Handle(context, default(CancellationToken));
        }

        public void Handle(HttpContext context, CancellationToken cancel)
        {
            this.handler.Handle(context, cancel);
        }

        public bool Match(HttpContext ctx)
        {
            if (ctx!=null && this.Matches(ctx))
            {
                if (this.map != null)
                    return this.map.Match(ctx);
                return true;
            }
            return false;
        }

        public abstract bool Matches(HttpContext ctx);
    }

    public class MatchRuleMapHandler : MapHandler
    {
        private readonly IHandlerMatcher matchRule;

        public MatchRuleMapHandler(IHttpHandler handler, IHandlerMatcher matchRule) : base(handler)
        {
            MethodContract.NotNull(handler, nameof(handler));
            MethodContract.NotNull(matchRule, nameof(matchRule));
            this.matchRule = matchRule;
        }

        public override bool Matches(HttpContext ctx)
        {
            return this.matchRule.Match(ctx);
        }
    }

    public class UrlBaseMapHandler : MapHandler
    {
        private readonly MatchRule match;

        public UrlBaseMapHandler(IHttpHandler handler, params string[] partialUris) : base(handler)
        {
            this.match = new UrlBaseMatchRule(partialUris);
        }

        public UrlBaseMapHandler(IHttpHandler handler, IEnumerable<string> partialUris) : base(handler)
        {
            this.match = new UrlBaseMatchRule(partialUris);
        }

        public override bool Matches(HttpContext ctx)
        {
            return this.match.Match(ctx);
        }
    }

    public class VerbRestrictingMapHandler : MapHandler
    {
        private readonly MatchRule match;

        public VerbRestrictingMapHandler(IHttpHandler handler, IEnumerable<string> verbs) : base(handler)
        {
            this.match = new VerbRestrictingMatchRule(verbs);
        }

        public VerbRestrictingMapHandler(IHttpHandler handler, params string[] verbs ) : base(handler)
        {
            this.match = new VerbRestrictingMatchRule(verbs);
        }

        public override bool Matches(HttpContext ctx)
        {
            return this.match.Match(ctx);
        }
    }

    public class IPNetworkRestrictingMapHanlder : MapHandler
    {
        private readonly MatchRule match;

        public IPNetworkRestrictingMapHanlder(IHttpHandler handler, IIPFilter ipFilter) : base(handler)
        {
            MethodContract.NotNull(ipFilter, nameof(ipFilter));
            this.match = new IpNetworkRestrictingMatchRule(ipFilter);
        }

        public override bool Matches(HttpContext ctx)
        {
            return this.match.Match(ctx);
        }
    }
}
