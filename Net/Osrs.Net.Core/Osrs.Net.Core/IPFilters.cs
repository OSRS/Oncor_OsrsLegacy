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
using System.Collections.Generic;
using System.Net;

namespace Osrs.Net
{
    /// <summary>
    /// An IPFilter that by default allows all IPAddresses that don't match any filter.
    /// The blacklist contains a collection of IPFilters that each are explicitly disallowed, therefore the default behavior is to allow any non-matched addresses.
    /// </summary>
    public sealed class IPFilterBlackList : IIPFilter
    {
        private readonly List<IIPFilter> blacklist = new List<IIPFilter>();

        /// <summary>
        /// Add a new blacklisted matching filter.
        /// </summary>
        /// <param name="filter">The filter whose matching members should be blacklisted</param>
        public void Add(IIPFilter filter)
        {
            if (filter != null)
                this.blacklist.Add(filter);
        }

        public bool Contains(IPAddress pt)
        {
            foreach(IIPFilter cur in this.blacklist)
            {
                if (cur.Contains(pt))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// An IPFilter that by default denies all IPAddresses that don't match any filter.
    /// The whitelist contains a collection of IPFilters that each are explicitly allowed, therefore the default behavior is to disallow any non-matched addresses.
    /// </summary>
    public sealed class IPFilterWhiteList : IIPFilter
    {
        private readonly List<IIPFilter> whiteList = new List<IIPFilter>();

        /// <summary>
        /// Add a new whitelisted matching filter.
        /// </summary>
        /// <param name="filter">The filter whose matching members should be blacklisted</param>
        public void Add(IIPFilter filter)
        {
            if (filter != null)
                this.whiteList.Add(filter);
        }

        public bool Contains(IPAddress pt)
        {
            foreach (IIPFilter cur in this.whiteList)
            {
                if (cur.Contains(pt))
                    return true;
            }
            return false;
        }
    }

    public sealed class IPFilters : IIPFilter
    {
        private readonly IPFilterWhiteList whiteList = new IPFilterWhiteList();
        private readonly IPFilterBlackList blacklist = new IPFilterBlackList();

        public void Add(IIPFilter filter)
        {
            Add(filter, FilterType.WhiteList);
        }

        public void Add(IIPFilter filter, FilterType listingType)
        {
            if (filter != null)
            {
                if (listingType == FilterType.WhiteList)
                    this.whiteList.Add(filter);
                else
                    this.blacklist.Add(filter);
            }
        }

        //true is to whitelist, false is to blacklist
        private bool defaultAllow = true;
        /// <summary>
        /// Should the behavior allow ipaddresses that don't fail the blacklist rules, or only allow ipaddress that both pass whitelist and don't file blacklist.
        /// A default behavior of whitelist is to assume all addresses that don't fail blacklisting are whitelisted (ignores the whitelist overall).
        /// A default behavior of blacklist is to assume all addresses that don't pass the whitelist are blacklisted, as well as any ipaddresses that fail the blacklist are blacklisted
        /// </summary>
        public FilterType DefaultBehavior
        {
            get
            {
                if (this.defaultAllow)
                    return FilterType.WhiteList;
                else
                    return FilterType.BlackList;
            }
            set
            {
                this.defaultAllow = value == FilterType.WhiteList;
            }
        }

        public bool Contains(IPAddress pt)
        {
            if (this.defaultAllow) //we are whitelisting
            {
                return this.blacklist.Contains(pt);
            }
            else //we are blacklisting
            {
                if (this.whiteList.Contains(pt))
                {
                    return this.blacklist.Contains(pt);
                }
            }
            return false;
        }

        public IPFilters()
        { }
    }
}
