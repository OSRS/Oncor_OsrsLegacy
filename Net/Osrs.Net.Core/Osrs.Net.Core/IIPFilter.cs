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
using System.Net;

namespace Osrs.Net
{
    public enum FilterType
    {
        WhiteList,
        BlackList
    }

    /// <summary>
    /// An IP Filter checks to ensure a given IPAddress matches a filter rule.
    /// This is generally a whitelisting mechanism.
    /// </summary>
    public interface IIPFilter
    {
        /// <summary>
        /// Returns true if the address is matched in the filter.  This should translate as the address is "good" (whitelisted).
        /// Blacklisting should be accomplished within the filter implementation by inverting any restricted addresses as a whitelist.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool Contains(IPAddress address);
    }

    //patterns look like:   127.*.*.0-255 or 127:*:*:0-255
    public abstract class IPFilterBase : IIPFilter
    {
        protected static readonly char[] octetSeps = new char[] { '.', ':' }; //either are ok for V4 or V6 masking
        protected static readonly char[] rangeSeps = new char[] { '-' };

        public abstract bool Contains(IPAddress endpoint);

        protected IPFilterBase()
        { }

        public static IPFilterBase Create(string pattern)
        {
            try
            {
                return new IPFilterV6(pattern);
            }
            catch
            { }

            try
            {
                return new IPFilterV4(pattern);
            }
            catch
            { }

            return null;
        }

        internal sealed class OctetRange
        {
            private readonly ushort min;
            private readonly ushort max;

            internal OctetRange(bool isV4) //wildcard *
            {
                this.min = ushort.MinValue;
                if (isV4)
                    this.max = byte.MaxValue;
                else
                    this.max = ushort.MaxValue;
            }

            internal OctetRange(ushort value) //singular value
            {
                this.min = value;
                this.max = value;
            }

            internal OctetRange(ushort min, ushort max)
            {
                this.min = Math.Min(min, max);
                this.max = Math.Max(min, max);
            }

            internal bool Matches(ushort b)
            {
                return this.min <= b && b <= this.max;
            }
        }

        protected static ushort ParseHex(string octet)
        {
            //value is sum(16^i*hexDigit) for i=0..N - in reverse order
            ushort val = 0;
            ushort pos = 0;
            char cur;
            for (int i = octet.Length - 1; -1 < i; i--)
            {
                cur = octet[i];
                switch (cur)
                {
                    case '0':
                        continue;
                    case '1':
                        val += (ushort)Math.Pow(16, pos);
                        break;
                    case '2':
                        val += (ushort)(2 * (int)Math.Pow(16, pos));
                        break;
                    case '3':
                        val += (ushort)(3 * (int)Math.Pow(16, pos));
                        break;
                    case '4':
                        val += (ushort)(4 * (int)Math.Pow(16, pos));
                        break;
                    case '5':
                        val += (ushort)(5 * (int)Math.Pow(16, pos));
                        break;
                    case '6':
                        val += (ushort)(6 * (int)Math.Pow(16, pos));
                        break;
                    case '7':
                        val += (ushort)(7 * (int)Math.Pow(16, pos));
                        break;
                    case '8':
                        val += (ushort)(8 * (int)Math.Pow(16, pos));
                        break;
                    case '9':
                        val += (ushort)(9 * (int)Math.Pow(16, pos));
                        break;
                    case 'A':
                        val += (ushort)(10 * (int)Math.Pow(16, pos));
                        break;
                    case 'B':
                        val += (ushort)(11 * (int)Math.Pow(16, pos));
                        break;
                    case 'C':
                        val += (ushort)(12 * (int)Math.Pow(16, pos));
                        break;
                    case 'D':
                        val += (ushort)(13 * (int)Math.Pow(16, pos));
                        break;
                    case 'E':
                        val += (ushort)(14 * (int)Math.Pow(16, pos));
                        break;
                    case 'F':
                        val += (ushort)(15 * (int)Math.Pow(16, pos));
                        break;
                    default:
                        throw new ArgumentException("pattern:octet");
                }
                pos++;
            }
            return val;
        }
    }

    public sealed class IPFilterV4 : IPFilterBase
    {
        private OctetRange A0;
        private OctetRange A1;
        private OctetRange A2;
        private OctetRange A3;

        public IPFilterV4(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("pattern");

            string[] octets = pattern.Split(IPFilterBase.octetSeps, StringSplitOptions.RemoveEmptyEntries);
            if (octets == null || octets.Length != 4)
                throw new ArgumentException("pattern");

            this.A0 = GetRange(octets[0]);
            this.A1 = GetRange(octets[1]);
            this.A2 = GetRange(octets[2]);
            this.A3 = GetRange(octets[3]);
        }

        private static OctetRange GetRange(string octet)
        {
            string[] rangeWords = octet.Split(IPFilterBase.rangeSeps, StringSplitOptions.RemoveEmptyEntries);
            if (rangeWords == null || rangeWords.Length < 1 || rangeWords.Length > 2)
                throw new ArgumentException("pattern:octet");

            string word = rangeWords[0].Trim();
            if (rangeWords.Length == 1) //just a single value
            {
                if ("*".Equals(word))
                    return new OctetRange(true);//wildcard

                try
                {
                    return new OctetRange(byte.Parse(word));
                }
                catch
                {
                    throw new ArgumentException("pattern:octet");
                }
            }
            else
            {
                try
                {
                    byte O1 = byte.Parse(word);
                    word = rangeWords[1].Trim();
                    byte O2 = byte.Parse(word);
                    return new OctetRange(O1, O2);
                }
                catch
                {
                    throw new ArgumentException("pattern:octet");
                }
            }
        }

        public override bool Contains(IPAddress endpoint)
        {
            byte[] bts = endpoint.GetAddressBytes();
            if (bts == null || bts.Length != 4)
                return false;
            return this.A0.Matches(bts[0]) && this.A1.Matches(bts[1]) && this.A2.Matches(bts[2]) && this.A3.Matches(bts[3]);
        }
    }

    public sealed class IPFilterV6 : IPFilterBase
    {
        private OctetRange A0;
        private OctetRange A1;
        private OctetRange A2;
        private OctetRange A3;
        private OctetRange A4;
        private OctetRange A5;
        private OctetRange A6;
        private OctetRange A7;

        public IPFilterV6(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("pattern");

            string[] octets = pattern.Split(IPFilterBase.octetSeps, StringSplitOptions.RemoveEmptyEntries);
            if (octets == null || octets.Length != 8)
                throw new ArgumentException("pattern");

            this.A0 = GetRange(octets[0]);
            this.A1 = GetRange(octets[1]);
            this.A2 = GetRange(octets[2]);
            this.A3 = GetRange(octets[3]);
            this.A4 = GetRange(octets[4]);
            this.A5 = GetRange(octets[5]);
            this.A6 = GetRange(octets[6]);
            this.A7 = GetRange(octets[7]);
        }

        private static OctetRange GetRange(string octet)
        {
            string[] rangeWords = octet.Split(IPFilterBase.rangeSeps, StringSplitOptions.RemoveEmptyEntries);
            if (rangeWords == null || rangeWords.Length < 1 || rangeWords.Length > 2)
                throw new ArgumentException("pattern:octet");

            string word = rangeWords[0].Trim();
            if (rangeWords.Length == 1) //just a single value
            {
                if ("*".Equals(word))
                    return new OctetRange(false);//wildcard

                try
                {
                    return new OctetRange(IPFilterBase.ParseHex(word));
                }
                catch
                {
                    throw new ArgumentException("pattern:octet");
                }
            }
            else
            {
                ushort O1 = IPFilterBase.ParseHex(word);
                word = rangeWords[1].Trim();
                ushort O2 = IPFilterBase.ParseHex(word);
                return new OctetRange(O1, O2);
            }
        }

        public override bool Contains(IPAddress endpoint)
        {
            byte[] bts = endpoint.GetAddressBytes();
            if (bts == null || bts.Length != 4)
                return false;
            return this.A0.Matches(bts[0]) && this.A1.Matches(bts[1]) && this.A2.Matches(bts[2]) && this.A3.Matches(bts[3]) &&
                this.A4.Matches(bts[4]) && this.A5.Matches(bts[5]) && this.A6.Matches(bts[6]) && this.A7.Matches(bts[7]);
        }
    }
}
