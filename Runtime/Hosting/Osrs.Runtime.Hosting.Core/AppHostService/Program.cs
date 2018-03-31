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

using System.ComponentModel;
using System.ServiceProcess;
using Osrs.Runtime.Hosting.AppHosting;

namespace AppHostService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HostingService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }

    public sealed class HostingService : ServiceBase
    {
        private System.ComponentModel.Container component = new System.ComponentModel.Container();
        private vProxy proxy = vProxy.Instance; //HostServiceProxy.Instance;

        public HostingService()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            proxy.Start(this.EventLog);
            base.OnStart(args);
            if (proxy.State != Osrs.Runtime.RunState.Running)
                this.EventLog.WriteEntry("Start Failed " + proxy.State);
        }

        protected override void OnStop()
        {
            proxy.Stop();

            base.OnStop();
        }

        protected override void OnShutdown()
        {
            proxy.Shutdown();

            base.OnShutdown();
        }

        protected override void OnContinue()
        {
            proxy.Resume();
            base.OnContinue();
        }

        protected override void OnPause()
        {
            proxy.Pause();
            base.OnPause();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.proxy != null)
                    {
                        this.proxy.Shutdown();
                        this.proxy = null;
                    }
                }
                catch
                { }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            component = new System.ComponentModel.Container();
            this.ServiceName = "AppHostingService";
            this.AutoLog = true;
        }
    }

    [RunInstaller(true)]
    public sealed class Installer : System.Configuration.Install.Installer
    {
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            this.serviceInstaller.DisplayName = "AppHostingService";
            this.serviceInstaller.ServiceName = "AppHostingService";
            this.serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[]
                {
                    this.serviceProcessInstaller,
                    this.serviceInstaller
                }
            );
        }

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        public Installer()
        {
            InitializeComponent();
        }
    }
}
