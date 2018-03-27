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

namespace Osrs.Net.Http
{
    internal static class ResourceStrings
    {
        internal const string Http = "http";
        internal const string Https = "https";

        internal const string Exception_UseMiddlewareIServiceProviderNotAvailable = "'{0}' is not available.";
        internal const string Exception_UseMiddlewareNoInvokeMethod = "No public '{0}' method found.";
        internal const string Exception_UseMiddlewareNonTaskReturnType = "'{0}' does not return an object of type '{1}'.";
        internal const string Exception_UseMiddlewareNoParameters = "The '{0}' method's first argument must be of type '{ 1}'.";
        internal const string Exception_UseMiddleMutlipleInvokes = "Multiple public '{0}' methods are available.";
        internal const string Exception_PathMustStartWithSlash = "The path in '{0}' must start with '/'.";
        internal const string Exception_InvokeMiddlewareNoService = "Unable to resolve service for type '{0}' while attempting to Invoke middleware '{1}'.";
        internal const string Exception_InvokeDoesNotSupportRefOrOutParams = "The '{0}' method must not have ref or out parameters.";
        internal const string Exception_PortMustBeGreaterThanZero = "The value must be greater than zero.";

        internal static string Format(string value, params string[] args)
        {
            return string.Format(value, args);
        }
    }
}
