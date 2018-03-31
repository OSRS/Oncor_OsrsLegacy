using Osrs.Runtime;
using Osrs.Runtime.Hosting;
using Osrs.Runtime.Logging;
using System.Threading;

namespace HostingCoreTestHost
{
    public sealed class Hosted2 : HostedServiceBase
    {
        private LogProviderBase logger;

        public Hosted2()
        {
            if (instance == null)
            {
                instance = this;
                this.logger = LogManager.Instance.GetProvider(typeof(Hosted2));
                this.State = RunState.Created;
            }
        }

        protected override void Run()
        {
            if (this.logger != null)
                this.logger.Log(0, "Starting 2");
            this.State = RunState.Running;

            while (this.State == RunState.Running)
            {
                if (this.logger != null)
                    this.logger.Log(0, "Hi from 2");
                Thread.Sleep(3);
            }
        }

        private static Hosted2 instance;
    }
}
