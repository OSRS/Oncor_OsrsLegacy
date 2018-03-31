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
using System.IO;

namespace Osrs.Runtime.Hosting.AppHosting
{
    public sealed class HostCommunication
    {
        private readonly TextReader inStream;
        private readonly TextWriter outStream;

        public HostCommunication(TextReader inStream, TextWriter outStream)
        {
            if (inStream == null || outStream == null)
                throw new ArgumentNullException();
            this.inStream = inStream;
            this.outStream = outStream;
        }

        public void Send(HostMessage message)
        {
            if (message == null)
                return;
            this.outStream.WriteLine(message.Format());
        }

        public HostMessage SendReceive(HostMessage message)
        {
            if (message == null)
                return null;
            this.Send(message);
            return this.Receive();
        }

        public HostMessage Receive()
        {
            return HostMessage.Parse(this.inStream.ReadLine());
        }
    }
}
