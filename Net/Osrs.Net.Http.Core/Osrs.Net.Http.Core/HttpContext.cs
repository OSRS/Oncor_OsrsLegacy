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

using Osrs.Threading;
using System.Collections.Generic;

namespace Osrs.Net.Http
{
    /// <summary>
    /// Encapsulates all HTTP-specific information about an individual HTTP request.
    /// </summary>
    public abstract class HttpContext
    {
        /// <summary>
        /// Gets the list of address strings the server was listening on for this context.
        /// The expectation is the handler can match one of the listener addresses to the context for partial path resolution.
        /// </summary>
        public abstract ICollection<string> Addresses { get; }

        /// <summary>
        /// Gets the <see cref="HttpRequest"/> object for this request.
        /// </summary>
        public abstract HttpRequest Request { get; }

        /// <summary>
        /// Gets the <see cref="HttpResponse"/> object for this request.
        /// </summary>
        public abstract HttpResponse Response { get; }

        /// <summary>
        /// Gets information about the underlying connection for this request.
        /// </summary>
        public abstract HttpConnectionInfo Connection { get; }

        /// <summary>
        /// Gets an object that manages the establishment of WebSocket connections for this request.
        /// </summary>
        public abstract WebSocketManager WebSockets { get; }

        /// <summary>
        /// Notifies when the connection underlying this request is aborted and thus request operations should be
        /// cancelled.
        /// </summary>
        public abstract CancellationToken RequestAborted { get; set; }

        /// <summary>
        /// Aborts the connection underlying this request.
        /// </summary>
        public abstract void Abort();
    }
}
