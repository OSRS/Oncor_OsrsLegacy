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
using System.Net.Mail;
using System.Net;

namespace Osrs.Net.Mail
{
    public sealed class SmtpClient
    {
        private SmtpClientConfig config = null;
        public SmtpClient(SmtpClientConfig config)
        {
            if (config != null)
            {
                this.config = config;
            }
            else
                throw new ArgumentException();
        }

        public bool Send(string emailTo, string subject, string body)
        {
            return Send(config.EmailFrom, emailTo, subject, body);
        }

        public bool Send(string emailFrom,  string emailTo, string subject, string body)
        {
            bool success = false;

            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(config.Host, config.Port);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(config.EmailFrom,config.FromPassword);

                MailMessage message = new MailMessage(emailFrom, emailTo);
                message.Subject = subject;
                message.Body = body;

                int retries = config.RetryCount;

                while (retries > 0 && success == false)
                {
                    try
                    {
                        client.Send(message);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        success = false;
                        retries--;
                    }
                }

                message.Dispose();
                client.Dispose();
            }
            catch (Exception e)
            {
            }

            return success;
        }
    }
}
