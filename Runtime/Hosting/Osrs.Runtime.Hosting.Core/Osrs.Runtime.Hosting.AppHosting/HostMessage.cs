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

namespace Osrs.Runtime.Hosting.AppHosting
{
    public enum HostCommand
    {
        Unknown,
        HeartBeat,
        Init,
        Start,
        Stop,
        Pause,
        Resume,
        Shutdown
    }

    public enum HostDirection
    {
        Request,
        Response
    }

    public sealed class HostMessage
    {
        public HostCommand Command { get; set; }
        public string Message { get; set; }

        internal string Format()
        {
            string m = this.Message != null ? this.Message : string.Empty;
            if (this.Command == HostCommand.HeartBeat)
                return "heartbeat " + m;
            else if (this.Command == HostCommand.Init)
                return "init " + m;
            else if (this.Command == HostCommand.Start)
                return "start " + m;
            else if (this.Command == HostCommand.Stop)
                return "stop " + m;
            else if (this.Command == HostCommand.Pause)
                return "pause " + m;
            else if (this.Command == HostCommand.Resume)
                return "resume " + m;
            else if (this.Command == HostCommand.Shutdown)
                return "shutdown " + m;

            return m;
        }

        private static readonly char[] sep = new char[] { ' ' };
        internal static HostMessage Parse(string message)
        {
            HostMessage result = new HostMessage();
            if (!string.IsNullOrEmpty(message))
            {
                string[] words = message.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                if (words != null && words.Length > 0)
                {
                    string cmd = words[0];
                    if (cmd == "heartbeat")
                    {
                        result.Command = HostCommand.HeartBeat;
                        if (words.Length > 1)
                            result.Message = message.Substring(10);
                    }
                    else if (cmd == "init")
                    {
                        result.Command = HostCommand.Init;
                        if (words.Length > 1)
                            result.Message = message.Substring(5);
                    }
                    else if (cmd == "start")
                    {
                        result.Command = HostCommand.Start;
                        if (words.Length > 1)
                            result.Message = message.Substring(6);
                    }
                    else if (cmd == "stop")
                    {
                        result.Command = HostCommand.Stop;
                        if (words.Length > 1)
                            result.Message = message.Substring(5);
                    }
                    else if (cmd == "pause")
                    {
                        result.Command = HostCommand.Pause;
                        if (words.Length > 1)
                            result.Message = message.Substring(6);
                    }
                    else if (cmd == "resume")
                    {
                        result.Command = HostCommand.Resume;
                        if (words.Length > 1)
                            result.Message = message.Substring(7);
                    }
                    else if (cmd == "shutdown")
                    {
                        result.Command = HostCommand.Shutdown;
                        if (words.Length > 1)
                            result.Message = message.Substring(9);
                    }
                    else
                    {
                        result.Command = HostCommand.Unknown;
                        result.Message = message;
                    }
                }
            }

            return result;
        }
    }
}
