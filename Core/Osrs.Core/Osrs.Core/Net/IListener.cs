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

namespace Osrs.Net
{
    /// <summary>
    /// For network servers, a listener listens for a request / connection and returns a context object representing the pending request.
    /// A handler would then be assigned to respond to the request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IListener<T>
    {
        T GetContext();
        T GetContext(CancellationToken cancel);
    }

    /// <summary>
    /// For network servers, a handler processes a received request / connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandler<T>
    {
        void Handle(T context);
        void Handle(T context, CancellationToken cancel);
    }

    /// <summary>
    /// A server listener has a "built-in" handler that handles all incoming requests directly.
    /// In most implementations, the handler is dispatched on a different thread to keep the listener from blocking.
    /// The handler is generally a dispatching handler that abstracts a collection of "inner" handlers.  The "best match" handler is then dispatched to do the work.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServerListener<T> : IListener<T>, IHandler<T>
    { }
}
