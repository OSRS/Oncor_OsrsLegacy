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

using Osrs.Runtime;
using System;
using System.Text;

namespace Osrs.Security.Authentication
{
    public sealed class PersistedCredential
    {
        private readonly Guid userId;
        public Guid UserId
        {
            get { return this.userId; }
        }

        public bool IsLocked
        { get; internal set; } = false;

        /// <summary>
        /// Returns whether this type of ICredential will expire.
        /// </summary>
        public bool Expires
        {
            get
            {
                return DateTime.MaxValue > this.ValidTo; //default is == maxvalue
            }
        }

        /// <summary>
        /// Returns the DateTime that this credential will expire if Expires returns true.
        /// </summary>
        public DateTime ValidTo
        { get; internal set; } = DateTime.MaxValue;

        /// <summary>
        /// Returns the DateTime that this credential was created
        /// </summary>
        public DateTime ValidFrom
        { get; internal set; } = DateTime.MinValue;

        private readonly byte[] rawData;
        public byte[] Bytes
        {
            get
            {
                if (this.rawData != null)
                    return this.rawData;
                return Encoding.UTF8.GetBytes(this.text);
            }
        }

        public bool HasNativeBytes
        {
            get { return this.rawData != null; }
        }

        public bool HasNativeText
        {
            get { return this.text != null; }
        }

        private readonly string text;
        public string Text
        {
            get
            {
                if (this.text != null)
                    return this.text;
                return Encoding.UTF8.GetString(this.rawData);
            }
        }

        private readonly byte[] rawToken;
        public byte[] BytesToken
        {
            get
            {
                if (this.rawToken != null)
                    return this.rawToken;
                if (this.textToken != null)
                    return Encoding.UTF8.GetBytes(this.textToken);
                return new byte[] { };
            }
        }

        public bool HasToken
        {
            get { return this.textToken != null || this.rawToken != null; }
        }

        public bool HasNativeBytesToken
        {
            get { return this.rawToken != null; }
        }

        public bool HasNativeTextToken
        {
            get { return this.textToken != null; }
        }

        private readonly string textToken;
        public string TextToken
        {
            get
            {
                if (this.textToken != null)
                    return this.textToken;
                if (this.rawToken != null)
                    return Encoding.UTF8.GetString(this.rawToken);
                return string.Empty;
            }
        }

        public static string ToBase64(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                return Convert.ToBase64String(data);
            }
            return string.Empty;
        }

        public static byte[] FromBase64(string data)
        {
            if (data != null && data.Length > 0)
            {
                return Convert.FromBase64String(data);
            }
            return new byte[] { };
        }

        internal PersistedCredential(Guid userId, string payload) : this(userId, payload, (string)null)
        { }

        internal PersistedCredential(Guid userId, byte[] payload) : this(userId, payload, (string)null)
        { }

        internal PersistedCredential(Guid userId, string payload, string token)
        {
            MethodContract.NotNull(payload, nameof(payload));
            this.userId = userId;
            this.text = payload;
            this.textToken = token;
        }

        internal PersistedCredential(Guid userId, string payload, byte[] token)
        {
            MethodContract.NotNull(payload, nameof(payload));
            this.userId = userId;
            this.text = payload;
            this.rawToken = token;
        }

        internal PersistedCredential(Guid userId, byte[] payload, string token)
        {
            MethodContract.NotNull(payload, nameof(payload));
            this.userId = userId;
            this.rawData = payload;
            this.textToken = token;
        }

        internal PersistedCredential(Guid userId, byte[] payload, byte[] token)
        {
            MethodContract.NotNull(payload, nameof(payload));
            this.userId = userId;
            this.rawData = payload;
            this.rawToken = token;
        }
    }
}
