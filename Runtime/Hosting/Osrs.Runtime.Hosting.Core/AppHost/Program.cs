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

using Osrs.Reflection;
using Osrs.Runtime;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Hosting;
using Osrs.Runtime.Hosting.AppHosting;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Logging.Providers;
using System;
using System.Collections.Generic;

namespace AppHost
{
    class Program
    {
        private static object syncRoot = new object();
        private static HostCommunication comms;
        private static RunState state = RunState.Created;

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            comms = new HostCommunication(Console.In, Console.Out);

            //basic REPL loop logic on main thread
            while (true)
            {
                HostMessage message = comms.Receive();
                if (message.Command == HostCommand.HeartBeat)
                {
                    message.Message = HostingManager.Instance.RealtimeState().ToString();
                    comms.Send(message); //just an echo back
                }
                else if (message.Command == HostCommand.Pause)
                {
                    Pause();
                }
                else if (message.Command == HostCommand.Resume)
                {
                    Resume();
                }
                else if (message.Command == HostCommand.Stop)
                {
                    Stop();
                }
                else if (message.Command == HostCommand.Start)
                {
                    Start();
                }
                else if (message.Command == HostCommand.Init)
                {
                    Init();
                }
                else if (message.Command == HostCommand.Shutdown)
                {
                    Shutdown();
                    Environment.Exit(0); //exits the app
                }
            }
        }

        static void Init()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Init;

                if (state == RunState.Created)
                {
                    IEnumerable<TypeNameReference> names = null;
                    m.Message = "";

                    LogProviderBase log = null;
                    try
                    {
                        ConfigurationManager.Instance.Bootstrap();
                        ConfigurationManager.Instance.Initialize();
                        ConfigurationManager.Instance.Start();
                        if (ConfigurationManager.Instance.State == RunState.Running)
                        {
                            LogManager.Instance.Bootstrap();
                            LogManager.Instance.Initialize();
                            LogManager.Instance.Start();
                            if (LogManager.Instance.State == RunState.Running)
                                log = LogManager.Instance.GetProvider(typeof(Program));
                            if (log == null)
                                log = new NullLogger(typeof(Program));

                            ConfigurationProviderBase prov = ConfigurationManager.Instance.GetProvider();
                            if (prov != null)
                            {
                                ConfigurationParameter param = prov.Get(typeof(Program), "hostList");
                                if (param != null)
                                {
                                    string[] values = param.Value as string[];
                                    if (values != null && values.Length > 0)
                                    {
                                        HashSet<TypeNameReference> tps = new HashSet<TypeNameReference>();
                                        foreach (string s in values)
                                        {
                                            TypeNameReference cur = TypeNameReference.Parse(s);
                                            if (cur != null)
                                                tps.Add(cur);
                                            else
                                                log.Log(5, "Failed to parse TypeName for: " + s);
                                        }
                                        if (tps.Count > 0)
                                            names = tps;
                                    }
                                    else
                                    {
                                        m.Message = "Failed to get configuration value";
                                        log.Log(1000, m.Message);
                                    }
                                }
                                else
                                {
                                    m.Message = "Failed to get configuration parameter: hostList";
                                    log.Log(1000, m.Message);
                                }
                            }
                            else
                            {
                                m.Message = "Failed to get configuration provider";
                                log.Log(1000, m.Message);
                            }
                        }
                        else
                        {
                            m.Message = "Failed to initialize using local file, quitting (" + AppContext.BaseDirectory + ")";
                            if (log != null)
                                log.Log(1000, m.Message);
                        }
                    }
                    catch
                    {
                        m.Message = "Failed to initialize using config, falling back to local file";
                        if (log != null)
                            log.Log(1000, m.Message);
                    }

                    if (names != null)
                    {
                        HostingManager.Instance.Initialize(names);
                        state = HostingManager.Instance.State;
                        if (state == RunState.Initialized)
                        {
                            m.Message = "success " + m.Message;
                            if (log != null)
                                log.Log(0, m.Message);
                        }
                        else
                        {
                            m.Message = "failed " + m.Message;
                            if (log != null)
                                log.Log(1000, m.Message);
                        }
                    }
                    else
                    {
                        state = HostingManager.Instance.State;
                        m.Message = "failed " + m.Message;
                        if (log != null)
                            log.Log(1000, m.Message);
                    }
                }
                else
                {
                    state = HostingManager.Instance.State;
                    m.Message = "ignored";
                }

                comms.Send(m);
            }
        }

        static void Start()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Start;
                if (state == RunState.Initialized || state == RunState.Stopped)
                {
                    state = RunState.Starting;
                    HostingManager.Instance.Start();
                    state = RunState.Running;
                    m.Message = "success";
                }
                else
                    m.Message = "ignored";

                comms.Send(m);
            }
        }

        static void Stop()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Stop;
                if (state == RunState.Running || state == RunState.Paused)
                {
                    state = RunState.Stopping;
                    HostingManager.Instance.Stop();
                    state = RunState.Stopped;
                    m.Message = "success";
                }
                else
                    m.Message = "ignored";

                comms.Send(m);
            }
        }

        static void Pause()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Pause;
                if (state == RunState.Running)
                {
                    state = RunState.Stopping;
                    HostingManager.Instance.Pause();
                    state = RunState.Paused;
                    m.Message = "success";
                }
                else
                    m.Message = "ignored";

                comms.Send(m);
            }
        }

        static void Resume()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Start;
                if (state == RunState.Paused)
                {
                    state = RunState.Starting;
                    HostingManager.Instance.Resume();
                    state = RunState.Running;
                    m.Message = "success";
                }
                else
                    m.Message = "ignored";

                comms.Send(m);
            }
        }

        static void Shutdown()
        {
            lock (syncRoot)
            {
                HostMessage m = new HostMessage();
                m.Command = HostCommand.Shutdown;
                state = RunState.Stopping;
                HostingManager.Instance.Shutdown();
                state = RunState.Shutdown;
                m.Message = "success";

                comms.Send(m);
            }
        }
    }
}
