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

using System.Net;

namespace Osrs.Net
{
    public class IPNetwork : IIPFilter
    {
        public IPNetwork(IPAddress prefix, int prefixLength)
        {
            Prefix = prefix;
            PrefixLength = prefixLength;
            PrefixBytes = Prefix.GetAddressBytes();
            Mask = CreateMask();
        }

        public IPAddress Prefix { get; }

        private byte[] PrefixBytes { get; }

        /// <summary>
        /// The CIDR notation of the subnet mask 
        /// </summary>
        public int PrefixLength { get; }

        private byte[] Mask { get; }

        public bool Contains(IPAddress address)
        {
            if (Prefix.AddressFamily != address.AddressFamily)
            {
                return false;
            }

            var addressBytes = address.GetAddressBytes();
            for (int i = 0; i < PrefixBytes.Length && Mask[i] != 0; i++)
            {
                if (PrefixBytes[i] != (addressBytes[i] & Mask[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] CreateMask()
        {
            var mask = new byte[PrefixBytes.Length];
            int remainingBits = PrefixLength;
            int i = 0;
            while (remainingBits >= 8)
            {
                mask[i] = 0xFF;
                i++;
                remainingBits -= 8;
            }
            if (remainingBits > 0)
            {
                mask[i] = (byte)(0xFF << (8 - remainingBits));
            }

            return mask;
        }
    }
}
