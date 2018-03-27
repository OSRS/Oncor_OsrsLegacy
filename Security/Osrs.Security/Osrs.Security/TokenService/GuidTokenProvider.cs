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

namespace Osrs.Security.TokenService
{
    public class GuidTokenProvider : ITokenProvider
    {
        private readonly uint defaultLength = 128;

        public GuidTokenProvider(uint defaultLength)
        {
            if (defaultLength > 0)
                this.defaultLength = defaultLength;
        }

        public string CreateString()
        {
            return this.CreateString(this.defaultLength);
        }

        public string CreateString(uint length)
        {
            string result = Convert.ToBase64String(CreateBytes(length));
            if (result.Length > length)
                return result.Substring(0, (int)length);
            return result;
        }

        public byte[] CreateBytes()
        {
            return this.CreateBytes(this.defaultLength);
        }

        public byte[] CreateBytes(uint length)
        {
            if (length == 0)
                length = defaultLength;

            byte[] result = new byte[length];
            int curIndex = 0;
            byte[] guid = Guid.NewGuid().ToByteArray();

            for (int i = 0; i < result.Length; i++)
            {
                if (curIndex < guid.Length)
                    result[i] = guid[curIndex];
                else
                {
                    curIndex = 0;
                    guid = Guid.NewGuid().ToByteArray();
                }
            }

            return result;
        }
    }
}
