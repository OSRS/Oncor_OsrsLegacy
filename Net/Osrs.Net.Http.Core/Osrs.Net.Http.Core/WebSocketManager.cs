﻿//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
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

using System.Collections.Generic;

namespace Osrs.Net.Http
{
    /// <summary>
    /// Manages the establishment of WebSocket connections for a specific HTTP request. 
    /// </summary>
    public abstract class WebSocketManager
    {
        /// <summary>
        /// Gets a value indicating whether the request is a WebSocket establishment request.
        /// </summary>
        public abstract bool IsWebSocketRequest { get; }

        /// <summary>
        /// Gets the list of requested WebSocket sub-protocols.
        /// </summary>
        public abstract IList<string> WebSocketRequestedProtocols { get; }
    }
}
