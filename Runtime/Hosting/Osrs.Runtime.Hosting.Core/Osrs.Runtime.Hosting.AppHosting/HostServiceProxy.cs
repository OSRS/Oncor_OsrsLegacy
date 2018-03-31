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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Logging.Providers;

namespace Osrs.Runtime.Hosting.AppHosting
{
    public sealed class HostServiceProxy : IHostedService
    {
        private LogProviderBase logger;
        private string fileName;
        private HashSet<string> processNames = new HashSet<string>();
        private Dictionary<string, Process> procs = new Dictionary<string, Process>();
        private Dictionary<string, HostCommunication> comms = new Dictionary<string, HostCommunication>();
        
        public RunState State
        {
            get;
            private set;
        }

        public void Start()
        {
            if (this.State == RunState.Initialized || this.State == RunState.Stopped)
            {
                this.State = RunState.Starting;
                if (ConfigurationManager.Instance.State == RunState.Running)
                {
                    if (LogManager.Instance.State == RunState.Running)
                    {
                        this.logger.Log(0, "Start: Called");
                        ServiceListFile fil = ServiceListFile.Open(this.fileName);
                        if (fil != null)
                        {
                            string tmpName;
                            foreach (string name in fil)
                            {
                                tmpName = Path.Combine(name, "AppHost.exe");
                                if (File.Exists(tmpName))
                                {
                                    if (!processNames.Contains(tmpName)) //new for this startup
                                    {
                                        try
                                        {
                                            Process p = this.CreateProcess(tmpName);
                                            if (p.Start())
                                            {
                                                HostCommunication comm = new HostCommunication(p.StandardOutput, p.StandardInput);
                                                //we need to also init this thing
                                                HostMessage response = this.SendMessage(p, comm, HostCommand.Init, null);
                                                if (response != null && response.Command == HostCommand.Init)
                                                {
                                                    if (response.Message.StartsWith("success"))
                                                    {
                                                        this.logger.Log(0, "Start: initialized " + tmpName);
                                                        this.comms[tmpName] = comm;
                                                        this.procs[tmpName] = p;
                                                        this.processNames.Add(tmpName);
                                                    }
                                                }
                                                else
                                                    p.Kill();
                                            }
                                            else
                                                p.Kill();
                                        }
                                        catch
                                        { }
                                    }

                                    if (processNames.Contains(tmpName))
                                    {
                                        HostCommunication comm = this.comms[tmpName];
                                        HostMessage response = this.SendMessage(this.procs[tmpName], comm, HostCommand.Start, null);
                                        if (response != null && response.Command == HostCommand.Start)
                                        {
                                            if (response.Message.StartsWith("success"))
                                                this.logger.Log(0, "Start: started " + tmpName);
                                            else
                                                this.logger.Log(100, "Start: failed starting " + tmpName);
                                        }
                                    }
                                    else
                                        this.logger.Log(0, "Start: no process " + tmpName);
                                }
                                else
                                    this.logger.Log(0, "Start: failed to find " + tmpName);
                            }
                            //we've started what we can, now we start monitoring
                            this.logger.Log(0, "Start: Succeeded, starting monitoring");
                            Task t = new Task(this.MonitorServices);
                            t.Start();
                            this.State = RunState.Running;
                        }
                        else
                        {
                            this.State = RunState.FailedStarting;
                            this.logger.Log(0, "Start: Failed: no processFiles");
                        }
                    }
                    else
                    {
                        this.State = RunState.FailedStarting;
                        this.logger.Log(0, "Start: Failed: Logging didn't start");
                    }
                }
            }
        }

        public void Stop()
        {
            if (this.State == RunState.Running || this.State == RunState.Paused)
            {
                this.State = RunState.Stopping;
                this.logger.Log(0, "Stop: Called");
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                foreach (KeyValuePair<string, HostCommunication> name in this.comms)
                {
                    HostCommunication comm = name.Value;
                    HostMessage response = this.SendMessage(this.procs[name.Key], comm, HostCommand.Stop, null);
                    if (response != null && response.Command == HostCommand.Stop)
                    {
                        if (response.Message.StartsWith("success"))
                            this.logger.Log(0, "Stop: started " + name.Key);
                        else
                            this.logger.Log(100, "Stop: failed stopping " + name.Key);
                    }
                }
                this.State = RunState.Stopped;
            }
        }

        public void Initialize()
        {
            if (this.State == RunState.Created || this.State == RunState.FailedInitializing)
            {
                this.State = RunState.Initializing;
                if (ConfigurationManager.Instance.State == RunState.Created)
                {
                    ConfigurationManager.Instance.Bootstrap();
                    ConfigurationManager.Instance.Initialize();
                }
                if (ConfigurationManager.Instance.State == RunState.Initialized)
                    ConfigurationManager.Instance.Start();

                if (ConfigurationManager.Instance.State == Osrs.Runtime.RunState.Running)
                {
                    if (LogManager.Instance.State == RunState.Created)
                    {
                        LogManager.Instance.Bootstrap();
                        LogManager.Instance.Initialize();
                    }
                    if (LogManager.Instance.State == RunState.Initialized)
                        LogManager.Instance.Start();

                    if (LogManager.Instance.State == Osrs.Runtime.RunState.Running)
                    {
                        logger = LogManager.Instance.GetProvider(typeof(HostServiceProxy));
                        if (logger == null)
                            logger = new NullLogger(typeof(HostServiceProxy));

                        ConfigurationProviderBase cfg = ConfigurationManager.Instance.GetProvider();
                        if (cfg != null)
                        {
                            ConfigurationParameter parm = cfg.Get(typeof(HostServiceProxy), "appListFileName");
                            if (parm != null)
                            {
                                try
                                {
                                    this.fileName = (string)parm.Value;
                                    if (!string.IsNullOrEmpty(this.fileName))
                                    {
                                        if (File.Exists(this.fileName))
                                        {
                                            this.logger.Log(0, "Init(Constructor): Succeeded");
                                            this.State = RunState.Initialized;
                                            return;
                                        }
                                    }
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
            }
        }

        public void Shutdown()
        {
            if (this.State == RunState.Running || this.State == RunState.Stopped)
            {
                this.logger.Log(0, "Shutdown: Called");
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                foreach (KeyValuePair<string, HostCommunication> name in this.comms)
                {
                    HostCommunication comm = name.Value;
                    HostMessage response = this.SendMessage(this.procs[name.Key], comm, HostCommand.Shutdown, null);
                    if (response != null && response.Command == HostCommand.Shutdown)
                    {
                        if (response.Message.StartsWith("success"))
                            this.logger.Log(0, "Shutdown: started " + name.Key);
                        else
                            this.logger.Log(100, "Shutdown: failed shutting down " + name.Key);
                    }
                }
                this.State = RunState.Shutdown;
            }
        }

        public void Pause()
        {
            if (this.State == RunState.Running)
            {
                this.State = RunState.Stopping;
                this.logger.Log(0, "Pause: Called");
                foreach (KeyValuePair<string, HostCommunication> name in this.comms)
                {
                    HostCommunication comm = name.Value;
                    HostMessage response = this.SendMessage(this.procs[name.Key], comm, HostCommand.Pause, null);
                    if (response != null && response.Command == HostCommand.Pause)
                    {
                        if (response.Message.StartsWith("success"))
                            this.logger.Log(0, "Pause: started " + name.Key);
                        else
                            this.logger.Log(100, "Pause: failed pausing " + name.Key);
                    }
                }
                this.State = RunState.Paused;
            }
        }

        public void Resume()
        {
            if (this.State == RunState.Paused)
            {
                this.State = RunState.Starting;
                this.logger.Log(0, "Resume: Called");
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                foreach (KeyValuePair<string, HostCommunication> name in this.comms)
                {
                    HostCommunication comm = name.Value;
                    HostMessage response = this.SendMessage(this.procs[name.Key], comm, HostCommand.Resume, null);
                    if (response != null && response.Command == HostCommand.Resume)
                    {
                        if (response.Message.StartsWith("success"))
                            this.logger.Log(0, "Resume: started " + name.Key);
                        else
                            this.logger.Log(100, "Resume: failed resuming " + name.Key);
                    }
                }
                this.State = RunState.Running;
            }
        }

        private Process CreateProcess(string fileName)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = fileName;
            StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = false;
            StartInfo.RedirectStandardInput = true;
            StartInfo.RedirectStandardOutput = true;
            Process p = new Process();
            p.StartInfo = StartInfo;
            return p;
        }

        private bool RestartProcess(string fileName)
        {
            try
            {
                Process p = this.procs[fileName];
                if (p != null && !p.HasExited)
                    p.Kill();

                p = CreateProcess(fileName);
                if (p.Start())
                {
                    HostCommunication comm = new HostCommunication(p.StandardOutput, p.StandardInput);
                    //we need to also init this thing
                    HostMessage response = this.SendMessage(p, comm, HostCommand.Init, null);
                    if (response != null && response.Command == HostCommand.Init)
                    {
                        if (response.Message.StartsWith("success"))
                        {
                            response = this.SendMessage(p, comm, HostCommand.Start, null);
                            if (response != null && response.Command == HostCommand.Start)
                            {
                                if (response.Message.StartsWith("success"))
                                {
                                    this.comms[fileName] = comm;
                                    this.procs[fileName] = p;
                                    return true;
                                }
                                else
                                    p.Kill();
                            }
                            else
                                p.Kill();
                        }
                        else
                            p.Kill();
                    }
                    else
                        p.Kill();
                }
            }
            catch
            { }
            return false;
        }

        private HostMessage SendMessage(Process p, HostCommunication comm, HostCommand cmd, string msg)
        {
            try
            {
                if (!p.HasExited)
                {
                    HostMessage mess = new HostMessage();
                    mess.Command = cmd;
                    if (msg != null)
                        mess.Message = msg;
                    return comm.SendReceive(mess);
                }
            }
            catch
            { }
            return new HostMessage();
        }

        private bool monitoring = false;
        private bool running = false;
        private void MonitorServices()
        {
            this.monitoring = true;
            this.running = true;
            string success = Osrs.Runtime.RunState.Running.ToString();
            string failed = Osrs.Runtime.RunState.FailedRunning.ToString();
            string starting = Osrs.Runtime.RunState.Starting.ToString();
            string stopping = Osrs.Runtime.RunState.Stopping.ToString();
            while (this.running)
            {
                //make a copy of the list so we're sure we can mutate the lists for restart
                foreach (string nam in this.processNames)
                {
                    HostCommunication comm = this.comms[nam];
                    HostMessage response = this.SendMessage(this.procs[nam], comm, HostCommand.HeartBeat, null);
                    if (response != null && response.Command == HostCommand.HeartBeat)
                    {
                        if (!response.Message.StartsWith(Osrs.Runtime.RunState.Running.ToString()))
                        {
                            if (response.Message.StartsWith(starting) || response.Message.StartsWith(stopping))
                                continue; //may need to deal with hangs later
                            if (response.Message.StartsWith(failed))
                            {
                                this.logger.Log(0, "Monitor: problem " + response.Message + ": " + nam);
                                if (this.RestartProcess(nam))
                                    this.logger.Log(0, "Monitor: successfully restarted: " + nam);
                                else
                                    this.logger.Log(0, "Monitor: failed to restart: " + nam);
                            }
                            else
                                this.logger.Log(0, "Monitor: problem (ignored)" + response.Message + ": " + nam);
                        }
                    }
                    if (!this.running) //get us out ASAP
                        break;
                }
                for (int i = 0; i < 10; i++)
                {
                    if (this.running) //get us out ASAP
                        Thread.Sleep(500); //5 seconds in total
                }
            }
            this.monitoring = false;
        }

        public static HostServiceProxy Instance
        {
            get;
            private set;
        }

        private static readonly SingletonHelper<HostServiceProxy> helper = new SingletonHelper<HostServiceProxy>();
        public HostServiceProxy()
        {
            helper.Construct(this);
            Instance = this;
            this.State = RunState.Created;
        }
    }
}
