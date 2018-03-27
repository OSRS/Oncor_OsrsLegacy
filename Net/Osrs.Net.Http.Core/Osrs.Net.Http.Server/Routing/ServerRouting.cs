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
using Osrs.Threading;

namespace Osrs.Net.Http.Routing
{
    /// <summary>
    /// A singular dispatching mechanism to enable a collection of handlers to masquarade as a single handler.
    /// This serves as a mapper directly in that it will determine if any of the handlers are mapped, but will also "pick" the right one to dispatch to for handling.
    /// </summary>
    public class ServerRouting : IHandlerMapper
    {
        private readonly List<IHandlerMapper> map = new List<IHandlerMapper>();

        public ICollection<IHandlerMapper> Map
        {
            get { return this.map; }
        }

        public ServerRouting()
        { }

        public void Handle(HttpContext context)
        {
            Handle(context, default(CancellationToken));
        }

        public void Handle(HttpContext context, CancellationToken cancel)
        {
            foreach (IHandlerMapper cur in this.map)
            {
                if (cur != null && cur.Match(context))
                {
                    cur.Handle(context, cancel);
                    return;
                }
            }
            //rarely happens, but we didn't catch
            context.Response.StatusCode = HttpStatusCodes.Status404NotFound;
        }

        public bool Match(HttpContext ctx)
        {
            if (ctx != null)
            {
                foreach (IHandlerMapper cur in this.Map)
                {
                    if (cur != null && cur.Match(ctx))
                        return true;
                }
            }
            return false;
        }
    }
}
