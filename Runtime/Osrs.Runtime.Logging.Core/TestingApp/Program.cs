using Osrs.Runtime.Configuration;
using Osrs.Runtime.Logging;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManager.Instance.Bootstrap();
            ConfigurationManager.Instance.Initialize();
            ConfigurationManager.Instance.Start();

            LogManager.Instance.Bootstrap();
            LogManager.Instance.Initialize();
            LogManager.Instance.Start();

            if (LogManager.Instance.State == Osrs.Runtime.RunState.Running)
            {
                LogProviderBase prov = LogManager.Instance.GetProvider(typeof(Program));
                if (prov!=null)
                {
                    prov.Log("Hi there", LogLevel.Info);
                }
            }
        }
    }
}
