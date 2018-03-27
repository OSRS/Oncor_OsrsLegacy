using Osrs.Runtime.Configuration;
using System;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManager.Instance.Bootstrap();
            ConfigurationManager.Instance.Initialize();
            ConfigurationManager.Instance.Start();
            ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
            ConfigurationParameter param = config.Get(typeof(Program), "hostList");

            Console.WriteLine("Enter to exit");
            Console.ReadLine();
        }
    }
}
