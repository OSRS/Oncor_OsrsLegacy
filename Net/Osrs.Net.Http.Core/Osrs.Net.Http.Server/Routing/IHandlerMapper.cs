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

namespace Osrs.Net.Http.Routing
{
    /// <summary>
    /// Provides a context-based check to determine if a context matches a rule.
    /// The rule can be anything determinable from the context, but should not do anything to the context.
    /// </summary>
    public interface IHandlerMatcher
    {
        bool Match(HttpContext ctx);
    }

    /// <summary>
    /// The most common form of matcher that also contains a reference to the handler to handle the matched request.
    /// If the matcher matches the rule, the handler is directly called.  These are often nested to form sub-paths
    /// </summary>
    public interface IHandlerMapper : IHandlerMatcher, IHttpHandler
    {
    }
}
