using Osrs.Runtime;
using Osrs.Runtime.Hosting.AppHosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AppHostService
{
    internal sealed class vProxy
    {
        private bool initialized = false;
        private EventLog logger;
        private string fileName;
        private Process procs;
        private HostCommunication comms;

        public RunState State
        {
            get;
            private set;
        }

        public void Start(EventLog logger)
        {
            this.logger = logger;
            if (this.State == RunState.Created || this.State == RunState.FailedInitializing)
                this.Init();

            if (this.State == RunState.Initialized || this.State == RunState.Stopped)
            {
                this.State = RunState.Starting;
                this.logger.WriteEntry("Start: Called");
                if (File.Exists(this.fileName))
                {
                    if (this.procs == null) //new for this startup
                    {
                        try
                        {
                            Process p = this.CreateProcess(this.fileName);
                            if (p.Start())
                            {
                                HostCommunication comm = new HostCommunication(p.StandardOutput, p.StandardInput);
                                //we need to also init this thing
                                HostMessage response = this.SendMessage(p, comm, HostCommand.Init, null);
                                if (response != null && response.Command == HostCommand.Init)
                                {
                                    if (response.Message.StartsWith("success"))
                                    {
                                        this.logger.WriteEntry("Start: initialized");
                                        this.comms = comm;
                                        this.procs = p;
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

                    if (this.procs != null)
                    {
                        HostCommunication comm = this.comms;
                        HostMessage response = this.SendMessage(this.procs, comm, HostCommand.Start, null);
                        if (response != null && response.Command == HostCommand.Start)
                        {
                            if (response.Message.StartsWith("success"))
                                this.logger.WriteEntry("Start: started");
                            else
                                this.logger.WriteEntry("Start: failed starting", EventLogEntryType.Warning);
                        }
                    }
                    else
                    {
                        this.logger.WriteEntry("Start: no process", EventLogEntryType.Warning);
                        this.State = RunState.FailedStarting;
                        return;
                    }
                }
                else
                {
                    this.logger.WriteEntry("Start: failed to find", EventLogEntryType.Warning);
                    this.State = RunState.FailedStarting;
                    return;
                }

                //we've started
                this.logger.WriteEntry("Start: Succeeded, starting monitoring");
                Task t = new Task(this.MonitorServices);
                t.Start();
                this.State = RunState.Running;
            }
        }

        public void Stop()
        {
            if (this.State == RunState.Running || this.State == RunState.Paused)
            {
                this.State = RunState.Stopping;
                this.logger.WriteEntry("Stop: Called");
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                HostCommunication comm = comms;
                HostMessage response = this.SendMessage(this.procs, comm, HostCommand.Stop, null);
                if (response != null && response.Command == HostCommand.Stop)
                {
                    if (response.Message.StartsWith("success"))
                        this.logger.WriteEntry("Stop: started", EventLogEntryType.Information);
                    else
                        this.logger.WriteEntry("Stop: failed stopping", EventLogEntryType.Warning);
                }
                this.State = RunState.Stopped;
            }
        }

        public void Init()
        {
            if (this.State == RunState.Created || this.State == RunState.FailedInitializing)
            {
                this.State = RunState.Initializing;
                try
                {
                    this.fileName = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), "AppHost.exe");
                    if (!string.IsNullOrEmpty(this.fileName))
                    {
                        if (File.Exists(this.fileName))
                        {
                            this.logger.WriteEntry("Init: Succeeded", EventLogEntryType.Information);
                            this.State = RunState.Initialized;
                            return;
                        }
                    }
                }
                catch
                { }
                this.logger.WriteEntry("Init: Failed", EventLogEntryType.Warning);
                this.State = RunState.FailedInitializing;
            }
        }

        public void Shutdown()
        {
            if (this.State == RunState.Running || this.State == RunState.Stopped)
            {
                this.logger.WriteEntry("Shutdown: Called");
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                HostCommunication comm = comms;
                HostMessage response = this.SendMessage(this.procs, comm, HostCommand.Shutdown, null);
                if (response != null && response.Command == HostCommand.Shutdown)
                {
                    if (response.Message.StartsWith("success"))
                        this.logger.WriteEntry("Shutdown: started", EventLogEntryType.Information);
                    else
                        this.logger.WriteEntry("Shutdown: failed shutting down", EventLogEntryType.Warning);
                }
                this.State = RunState.Shutdown;
            }
        }

        public void Pause()
        {
            if (this.State == RunState.Running)
            {
                this.State = RunState.Stopping;
                this.logger.WriteEntry("Pause: Called");
                HostCommunication comm = this.comms;
                HostMessage response = this.SendMessage(this.procs, comm, HostCommand.Pause, null);
                if (response != null && response.Command == HostCommand.Pause)
                {
                    if (response.Message.StartsWith("success"))
                        this.logger.WriteEntry("Pause: started", EventLogEntryType.Information);
                    else
                        this.logger.WriteEntry("Pause: failed pausing", EventLogEntryType.Warning);
                }
                this.State = RunState.Paused;
            }
        }

        public void Resume()
        {
            if (this.State == RunState.Paused)
            {
                this.State = RunState.Starting;
                this.logger.WriteEntry("Resume: Called", EventLogEntryType.Information);
                this.running = false; //tells the monitor to stop
                while (this.monitoring)
                {
                    Thread.Sleep(1); //wait for the monitor to stop
                }
                HostCommunication comm = this.comms;
                HostMessage response = this.SendMessage(this.procs, comm, HostCommand.Resume, null);
                if (response != null && response.Command == HostCommand.Resume)
                {
                    if (response.Message.StartsWith("success"))
                        this.logger.WriteEntry("Resume: started", EventLogEntryType.Information);
                    else
                        this.logger.WriteEntry("Resume: failed resuming", EventLogEntryType.Information);
                }
                this.State = RunState.Running;
            }
        }

        private Process CreateProcess(string fileName)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = fileName;
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
                Process p = this.procs;
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
                                    this.comms = comm;
                                    this.procs = p;
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
                HostCommunication comm = this.comms;
                HostMessage response = this.SendMessage(this.procs, comm, HostCommand.HeartBeat, null);
                if (response != null && response.Command == HostCommand.HeartBeat)
                {
                    if (!response.Message.StartsWith(Osrs.Runtime.RunState.Running.ToString()))
                    {
                        if (response.Message.StartsWith(starting) || response.Message.StartsWith(stopping))
                            continue; //may need to deal with hangs later
                        if (response.Message.StartsWith(failed))
                        {
                            this.logger.WriteEntry("Monitor: problem " + response.Message, EventLogEntryType.Warning);
                            if (this.RestartProcess(this.fileName))
                                this.logger.WriteEntry("Monitor: successfully restarted", EventLogEntryType.Information);
                            else
                                this.logger.WriteEntry("Monitor: failed to restart", EventLogEntryType.Warning);
                        }
                        else
                            this.logger.WriteEntry("Monitor: problem (ignored)" + response.Message, EventLogEntryType.Warning);
                    }
                }
                if (!this.running) //get us out ASAP
                    break;
                for (int i = 0; i < 10; i++)
                {
                    if (this.running) //get us out ASAP
                        Thread.Sleep(500); //5 seconds in total
                }
            }
            this.monitoring = false;
        }

        public static vProxy Instance
        {
            get;
        } = new vProxy();

        private vProxy()
        { this.State = RunState.Created; }
    }
}
