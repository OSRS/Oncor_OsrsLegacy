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
using System.Security.Cryptography;

namespace Osrs.Security.TokenService
{
    public class HashingTokenProvider : ITokenProvider
    {
        private static RandomNumberGenerator randor;
        private readonly uint defaultLength = 128;

        public HashingTokenProvider(uint defaultLength)
        {
            if (defaultLength > 0)
                this.defaultLength = defaultLength;
        }

        public byte[] CreateBytes()
        {
            return Convert.FromBase64String(this.CreateString(this.defaultLength));
        }

        public byte[] CreateBytes(uint length)
        {
            return Convert.FromBase64String(this.CreateString(length));
        }

        public string CreateString()
        {
            return this.CreateString(this.defaultLength);
        }

        public string CreateString(uint length)
        {
            if (randor == null)
                randor = RandomNumberGenerator.Create();
            byte[] data = new byte[length];
            randor.GetBytes(data);
            string tmp = Convert.ToBase64String(data);
            if (tmp.Length > length)
                return tmp.Substring(0, (int)length);
            return tmp;
        }
    }
}
