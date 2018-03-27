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

namespace Osrs.Net.Mail
{
    public sealed class SmtpClientConfig
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string EmailFrom { get; set; }
        public string FromPassword { get; set; }
        public bool HtmlFormat { get; set; }
        public int RetryCount { get; set; }

        public SmtpClientConfig(int port, string host):this(port, host, null, null, 3)
        { }

        public SmtpClientConfig(int port, string host, string emailFrom, string fromPassword, int retries=3)
        {
            if (port>0 && port<65535 && !string.IsNullOrEmpty(host))
            {
                Port = port;
                Host = host;
                EmailFrom = emailFrom;
                FromPassword = fromPassword;
                RetryCount = retries;
                HtmlFormat = true;
                return;
            }

            throw new ArgumentException();
        }
    }
}
