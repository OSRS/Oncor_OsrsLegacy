using Osrs.Runtime;
using Osrs.Runtime.Hosting;
using Osrs.Runtime.Logging;
using System.Threading;

namespace HostingCoreTestHost
{
    public sealed class Hosted1 : HostedServiceBase
    {
        private LogProviderBase logger;

        public Hosted1()
        {
            if (instance == null)
            {
                instance = this;
                this.logger = LogManager.Instance.GetProvider(typeof(Hosted1));
                this.State = RunState.Created;
            }
        }

        protected override void Run()
        {
            if (this.logger != null)
                this.logger.Log(0, "Starting 1");
            this.State = RunState.Running;

            while (this.State == RunState.Running)
            {
                if (this.logger != null)
                    this.logger.Log(0, "Hi from 1");
                Thread.Sleep(3);
            }
        }

        private static Hosted1 instance;
    }
}
