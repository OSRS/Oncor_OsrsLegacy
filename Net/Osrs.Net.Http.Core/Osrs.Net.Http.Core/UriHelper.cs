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

using System;
using System.Collections.Generic;
using System.Text;

namespace Osrs.Net.Http
{
    /// <summary>
    /// A helper class for constructing encoded Uris for use in headers and other Uris.
    /// </summary>
    public static class UriHelper
    {
        private const string ForwardSlash = "/";
        private const string Pound = "#";
        private const string QuestionMark = "?";
        private const string SchemeDelimiter = "://";

        /// <summary>
        /// Combines the given URI components into a string that is properly encoded for use in HTTP headers.
        /// </summary>
        /// <param name="pathBase">The first portion of the request path associated with application root.</param>
        /// <param name="path">The portion of the request path that identifies the requested resource.</param>
        /// <param name="query">The query, if any.</param>
        /// <param name="fragment">The fragment, if any.</param>
        /// <returns></returns>
        public static string BuildRelative(PathString pathBase = new PathString(), PathString path = new PathString(), QueryString query = new QueryString(), FragmentString fragment = new FragmentString())
        {
            string combinePath = (pathBase.HasValue || path.HasValue) ? (pathBase + path).ToString() : "/";
            return combinePath + query.ToString() + fragment.ToString();
        }

        /// <summary>
        /// Combines the given URI components into a string that is properly encoded for use in HTTP headers.
        /// Note that unicode in the HostString will be encoded as punycode.
        /// </summary>
        /// <param name="scheme">http, https, etc.</param>
        /// <param name="host">The host portion of the uri normally included in the Host header. This may include the port.</param>
        /// <param name="pathBase">The first portion of the request path associated with application root.</param>
        /// <param name="path">The portion of the request path that identifies the requested resource.</param>
        /// <param name="query">The query, if any.</param>
        /// <param name="fragment">The fragment, if any.</param>
        /// <returns></returns>
        public static string BuildAbsolute(string scheme, HostString host, PathString pathBase = new PathString(), PathString path = new PathString(), QueryString query = new QueryString(), FragmentString fragment = new FragmentString())
        {
            var combinedPath = (pathBase.HasValue || path.HasValue) ? (pathBase + path).ToString() : "/";

            var encodedHost = host.ToString();
            var encodedQuery = query.ToString();
            var encodedFragment = fragment.ToString();

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = scheme.Length + SchemeDelimiter.Length + encodedHost.Length
                + combinedPath.Length + encodedQuery.Length + encodedFragment.Length;

            return new StringBuilder(length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(encodedHost)
                .Append(combinedPath)
                .Append(encodedQuery)
                .Append(encodedFragment)
                .ToString();
        }

        /// <summary>
        /// Seperates the given absolute URI string into components. Assumes no PathBase.
        /// </summary>
        /// <param name="uri">A string representation of the uri.</param>
        /// <param name="scheme">http, https, etc.</param>
        /// <param name="host">The host portion of the uri normally included in the Host header. This may include the port.</param>
        /// <param name="path">The portion of the request path that identifies the requested resource.</param>
        /// <param name="query">The query, if any.</param>
        /// <param name="fragment">The fragment, if any.</param>
        public static void FromAbsolute(string uri, out string scheme, out HostString host, out PathString path, out QueryString query, out FragmentString fragment)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            // Satisfy the out parameters
            path = new PathString();
            query = new QueryString();
            fragment = new FragmentString();
            var startIndex = uri.IndexOf(SchemeDelimiter);

            if (startIndex < 0)
            {
                throw new FormatException("No scheme delimiter in uri.");
            }

            scheme = uri.Substring(0, startIndex);

            // PERF: Calculate the end of the scheme for next IndexOf
            startIndex += SchemeDelimiter.Length;

            var searchIndex = -1;
            var limit = uri.Length;

            if ((searchIndex = uri.IndexOf(Pound, startIndex)) >= 0 && searchIndex < limit)
            {
                fragment = FragmentString.FromUriComponent(uri.Substring(searchIndex));
                limit = searchIndex;
            }

            if ((searchIndex = uri.IndexOf(QuestionMark, startIndex)) >= 0 && searchIndex < limit)
            {
                query = QueryString.FromUriComponent(uri.Substring(searchIndex, limit - searchIndex));
                limit = searchIndex;
            }

            if ((searchIndex = uri.IndexOf(ForwardSlash, startIndex)) >= 0 && searchIndex < limit)
            {
                path = PathString.FromUriComponent(uri.Substring(searchIndex, limit - searchIndex));
                limit = searchIndex;
            }

            host = HostString.FromUriComponent(uri.Substring(startIndex, limit - startIndex));
        }

        /// <summary>
        /// Generates a string from the given absolute or relative Uri that is appropriately encoded for use in
        /// HTTP headers. Note that a unicode host name will be encoded as punycode.
        /// </summary>
        /// <param name="uri">The Uri to encode.</param>
        /// <returns></returns>
        public static string Encode(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return BuildAbsolute(
                    scheme: uri.Scheme,
                    host: HostString.FromUriComponent(uri),
                    pathBase: PathString.FromUriComponent(uri),
                    query: QueryString.FromUriComponent(uri),
                    fragment: FragmentString.FromUriComponent(uri));
            }
            else
            {
                return uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            }
        }

        /// <summary>
        /// Returns the combined components of the request URL in a fully escaped form suitable for use in HTTP headers
        /// and other HTTP operations.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns></returns>
        public static string GetEncodedUrl(this HttpRequest request)
        {
            return BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path, request.QueryString);
        }

        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form (except for the QueryString)
        /// suitable only for display. This format should not be used in HTTP headers or other HTTP operations.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns></returns>
        public static string GetDisplayUrl(this HttpRequest request)
        {
            var host = request.Host.Value;
            var pathBase = request.PathBase.Value;
            var path = request.Path.Value;
            var queryString = request.QueryString.Value;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(request.Scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }

        public static string GetMatchedBaseUrl(HttpContext context)
        {
            return GetMatchedBaseUrl(context.Request.GetEncodedUrl(), context.Addresses);
        }

        public static string GetMatchedBaseUrl(string requestEncodedAbsoluteUrl, IEnumerable<string> contextAddresses)
        {
            if (string.IsNullOrEmpty(requestEncodedAbsoluteUrl))
                return string.Empty;
            foreach (string cur in contextAddresses)
            {
                if (requestEncodedAbsoluteUrl.StartsWith(cur, StringComparison.OrdinalIgnoreCase))
                    return cur;
            }
            return string.Empty;
        }

        public static string GetRequestUrl(HttpContext context)
        {
            string url = context.Request.GetEncodedUrl();
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            string matched = GetMatchedBaseUrl(url, context.Addresses);
            if (string.IsNullOrEmpty(matched) || url.Length == matched.Length)
                return string.Empty;

            string result = url.Substring(matched.Length);
            if (result[0] == '/')
                return result;

            return '/' + result;
        }
    }
}
