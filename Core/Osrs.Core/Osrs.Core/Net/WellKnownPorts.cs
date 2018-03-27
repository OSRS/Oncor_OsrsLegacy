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
namespace Osrs.Net
{
    public static class WellKnownPorts
    {
        /// <summary>
        /// FTP - data port.
        /// </summary>
        public static readonly int FTP_Data = 20;

        /// <summary>
        /// FTP - control (command) port.
        /// </summary>
        public static readonly int FTP_Control = 21;

        /// <summary>
        /// SMTP protocol.
        /// </summary>
        public static readonly int SMTP = 25;

        /// <summary>
        /// DNS protocol.
        /// </summary>
        public static readonly int DNS = 53;

        /// <summary>
        /// HTTP protocol.
        /// </summary>
        public static readonly int HTTP = 80;

        /// <summary>
        /// POP3 protocol.
        /// </summary>
        public static readonly int POP3 = 110;

        /// <summary>
        /// NNTP (Network News Transfer Protocol)  protocol.
        /// </summary>
        public static readonly int NNTP = 119;

        /// <summary>
        /// NTP (Network Time Protocol) protocol.
        /// </summary>
        public static readonly int NTP = 123;

        /// <summary>
        /// IMAP4 protocol.
        /// </summary>
        public static readonly int IMAP4 = 143;

        /// <summary>
        /// HTTPS protocol.
        /// </summary>
        public static readonly int HTTPS = 443;

        /// <summary>
        /// SMTP over SSL protocol.
        /// </summary>
        public static readonly int SMTP_SSL = 465;

        /// <summary>
        /// FTP over SSL protocol.
        /// </summary>
        public static readonly int FTP_Control_SSL = 990;

        /// <summary>
        /// IMAP4 over SSL protocol.
        /// </summary>
        public static readonly int IMAP4_SSL = 993;

        /// <summary>
        /// POP3 over SSL protocol.
        /// </summary>
        public static readonly int POP3_SSL = 995;
    }
}
