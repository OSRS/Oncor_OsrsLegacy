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
using System.Threading;
using System.Threading.Tasks;
using Osrs.Reflection;
using Osrs.Runtime;
using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;
using Osrs.Runtime.Logging.Providers;

namespace Osrs.Runtime.Hosting
{
    public sealed class HostingManager
    {
        private object syncRoot = new object();
        private List<IHostedService> hostedServices = new List<IHostedService>();

        public RunState State
        {
            get;
            private set;
        } = RunState.Created;

        private LogProviderBase logger;
        private void GetLogger()
        {
            this.logger = LogManager.Instance.GetProvider(typeof(HostingManager));
            if (this.logger == null)
                this.logger = new NullLogger(typeof(HostingManager));
            if (LogManager.Instance.State != RunState.Running)
                LogManager.Instance.Start(); //try to enable logging while we shut down
        }

        public void Initialize(IEnumerable<TypeNameReference> typeList)
        {
            lock(this.syncRoot)
            {
                if (this.State == RunState.Created)
                {
                    if (ConfigurationManager.Instance.State == RunState.Created)
                    {
                        ConfigurationManager.Instance.Bootstrap();
                        ConfigurationManager.Instance.Initialize();
                    }
                    if (ConfigurationManager.Instance.State == RunState.Initialized)
                        ConfigurationManager.Instance.Start();
                    if (LogManager.Instance.State == RunState.Created)
                    {
                        LogManager.Instance.Bootstrap();
                        LogManager.Instance.Initialize();
                    }
                    if (LogManager.Instance.State == RunState.Initialized)
                        LogManager.Instance.Start(); //make sure we have something to log to
                    this.logger = null;
                    
                    GetLogger();
                    this.logger.Log(0, "Initialize: Called");
                    //at this point, the logging and configuration services are running

                    if (typeList == null)
                    {
                        this.logger.Log(0, "Initialize: Null typeList, exiting");
                        return;
                    }

                    if (ConfigurationManager.Instance.State != RunState.Running)
                        this.logger.Log(100, "Initialize: ConfigurationManager failed to initialize");
                    if (LogManager.Instance.State != RunState.Running)
                        this.logger.Log(100, "Initialize: LogManager failed to initialize");

                    foreach (TypeNameReference cur in typeList)
                    {
                        try
                        {
                            IHostedService b = ReflectionUtils.Get<IHostedService>(cur);
                            if (b != null)
                            {
                                this.hostedServices.Add(b);
                                b.Initialize();
                                if (b.State != RunState.Initialized)
                                    this.logger.Log(5, "Initialize: Init failed for " + NameReflectionUtils.GetName(b).ToString());
                            }
                            else
                                this.logger.Log(5, "Initialize: Load failed for " + cur);
                        }
                        catch
                        { }
                    }

                    this.logger.Log(0, "Initialize: Succeeded");
                    this.State = RunState.Initialized;
                }
                else
                    this.logger.Log(0, "Initialize: Called from improper state: "+this.State.ToString());
            }
        }

        public void Start()
        {
            lock(this.syncRoot)
            {
                GetLogger();
                this.logger.Log(0, "Start: Called");
                if (this.State == RunState.Initialized || this.State == RunState.Stopped)
                {
                    this.State = RunState.Starting;
                    if (ConfigurationManager.Instance.State != RunState.Running)
                    {
                        ConfigurationManager.Instance.Start();
                        if (ConfigurationManager.Instance.State != RunState.Running)
                            this.logger.Log(0, "Start: Failed to start " + NameReflectionUtils.GetName(ConfigurationManager.Instance).ToString());
                    }
                    if (LogManager.Instance.State != RunState.Running)
                    {
                        LogManager.Instance.Start();
                        if (LogManager.Instance.State != RunState.Running)
                            this.logger.Log(0, "Start: Failed to start " + NameReflectionUtils.GetName(LogManager.Instance).ToString());
                    }

                    foreach (IHostedService cur in this.hostedServices)
                    {
                        cur.Start();
                        if (cur.State!= RunState.Running)
                            this.logger.Log(0, "Start: Failed to start " + NameReflectionUtils.GetName(cur).ToString());

                    }
                    this.logger.Log(0, "Start: Succeeded, starting monitoring");
                    this.State = RunState.Running;

                    Task t = new Task(this.MonitorServices);
                    t.Start();
                }
                else
                    this.logger.Log(0, "Start: Called from improper state: "+this.State.ToString());
            }
        }

        public void Stop()
        {
            lock(this.syncRoot)
            {
                GetLogger();
                this.logger.Log(0, "Stop: Called");
                if (this.State == RunState.Running || this.State == RunState.Paused)
                {
                    this.State = RunState.Stopping;
                    while (this.monitoring)
                    {
                        Thread.Sleep(5); //just try to let it finish a round of restarts
                    }
                    foreach (IHostedService cur in this.hostedServices)
                    {
                        cur.Stop();
                        if (cur.State!= RunState.Stopped)
                            this.logger.Log(0, "Stop: Failed to stop " + NameReflectionUtils.GetName(cur).ToString());
                    }

                    ConfigurationManager.Instance.Stop();
                    if (ConfigurationManager.Instance.State != RunState.Stopped)
                        this.logger.Log(0, "Stop: Failed to stop " + NameReflectionUtils.GetName(ConfigurationManager.Instance).ToString());

                    this.logger.Log(0, "Stop: Succeeded"); //try to make sure we can log this event before we kill the logger
                    LogManager.Instance.Stop();
                    if (LogManager.Instance.State != RunState.Stopped)
                        this.logger.Log(0, "Stop: Failed to stop " + NameReflectionUtils.GetName(LogManager.Instance).ToString()); //ha! may or may not actually log

                    this.State = RunState.Stopped;
                }
                else
                    this.logger.Log(0, "Stop: Called from improper state: " + this.State.ToString());
            }
        }

        public void Pause() //leave the monitor running and never stop the logger -- just in case
        {
            lock(this.syncRoot)
            {
                this.logger.Log(0, "Pause: Called");
                if (this.State == RunState.Running)
                {
                    this.State = RunState.Stopping;
                    foreach (IHostedService cur in this.hostedServices)
                    {
                        cur.Pause();
                        if (cur.State != RunState.Paused)
                            this.logger.Log(0, "Pause: Failed to pause " + NameReflectionUtils.GetName(cur).ToString());
                    }
                    this.logger.Log(0, "Pause: Succeeded");
                    this.State = RunState.Paused;
                }
                else
                    this.logger.Log(0, "Pause: Called from improper state: " + this.State.ToString());
            }
        }

        public void Resume() //we left the monitor running, so we may not need to restart it
        {
            lock(this.syncRoot)
            {
                if (LogManager.Instance.State!= RunState.Running)
                    LogManager.Instance.Start();
                this.logger.Log(0, "Resume: Called");
                if (this.State == RunState.Paused)
                {
                    this.State = RunState.Starting;
                    foreach (IHostedService cur in this.hostedServices)
                    {
                        cur.Resume();
                        if (cur.State != RunState.Running)
                            this.logger.Log(0, "Resume: Failed to resume " + NameReflectionUtils.GetName(cur).ToString());
                    }
                    this.logger.Log(0, "Resume: Succeeded, restarting monitoring");
                    this.State = RunState.Running;
                    if (!this.monitoring)
                    {
                        Task t = new Task(this.MonitorServices);
                        t.Start();
                    }
                }
                else
                    this.logger.Log(0, "Resume: Called from improper state: " + this.State.ToString());
            }
        }

        public void Shutdown()
        {
            lock (this.syncRoot)
            {
                GetLogger();

                this.logger.Log(0, "Shutdown: Called");
                this.State = RunState.Stopping;
                while (this.monitoring)
                {
                    Thread.Sleep(5); //just try to let it finish a round of restarts
                }
                foreach (IHostedService cur in this.hostedServices)
                {
                    cur.Shutdown();
                    if (cur.State != RunState.Shutdown)
                        this.logger.Log(0, "Shutdown: Failed to shutdown " + NameReflectionUtils.GetName(cur).ToString());
                }
                LogManager.Instance.Shutdown();
                if (LogManager.Instance.State != RunState.Shutdown)
                    this.logger.Log(0, "Shutdown: Failed to shutdown " + NameReflectionUtils.GetName(LogManager.Instance).ToString());
                ConfigurationManager.Instance.Shutdown();
                if (ConfigurationManager.Instance.State != RunState.Shutdown)
                    this.logger.Log(0, "Shutdown: Failed to shutdown " + NameReflectionUtils.GetName(ConfigurationManager.Instance).ToString());

                this.logger.Log(0, "Shutdown: Succeeded");
                this.State = RunState.Shutdown;
            }
        }

        private bool monitoring = false;
        private void MonitorServices()
        {
            this.monitoring = true;
            while(this.State == RunState.Running)
            {
                foreach (IHostedService cur in this.hostedServices)
                {
                    if (cur.State == RunState.FailedRunning) //only try to restart failed services
                    {
                        try
                        {
                            this.logger.Log(1000, "MonitorServices: Discovered failed service, attempting restart: " + NameReflectionUtils.GetName(cur).ToString());
                            cur.Start();
                            if (cur.State == RunState.FailedStarting)
                                this.logger.Log(1000, "MonitorServices: restart failed: " + NameReflectionUtils.GetName(cur).ToString());
                        }
                        catch 
                        { } //suck it up and keep going
                    }
                }
                Thread.Sleep(5000); //5 seconds
            }
            this.monitoring = false;
        }

        /// <summary>
        /// Checks the current state for all running services and returns worst-case service
        /// </summary>
        /// <returns></returns>
        public RunState RealtimeState()
        {
            RunState cur = this.State;
            if (cur == RunState.Shutdown || cur == RunState.FailedRunning || cur == RunState.Created)
                return cur; //all are worst cases
            if (cur == RunState.Starting || cur == RunState.Stopping)
                return cur;

            foreach(IHostedService svc in this.hostedServices)
            {
                RunState svcState = svc.State;
                if (cur!=svcState)
                {
                    if (svcState == RunState.Shutdown || svcState == RunState.FailedRunning)
                        return svcState; //worst case, so fail-fast
                    else if (cur == RunState.Running)
                    {
                        if (svcState == RunState.Stopped || svcState == RunState.Paused)
                            cur = svcState;
                    }
                    else if (cur == RunState.Paused)
                    {
                        if (svcState == RunState.Stopped)
                            cur = svcState;
                    }
                }
            }
            return cur;
        }

        private HostingManager()
        { this.logger = new NullLogger(typeof(HostingManager)); }

        private static HostingManager instance;
        public static HostingManager Instance
        {
            get { return instance; }
        }

        static HostingManager() //static initializer
        {
            instance = new HostingManager();
        }
    }
}
