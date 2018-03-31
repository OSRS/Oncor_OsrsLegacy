using Osrs.Runtime.Hosting.AppHosting;
using System;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            HostServiceProxy proxy = HostServiceProxy.Instance;

            proxy.Start();
            Console.WriteLine("State: " + proxy.State.ToString() + " Enter to continue");
            Console.ReadLine();
            proxy.Stop();
            Console.WriteLine("State: " + proxy.State.ToString() + " Enter to continue");
            Console.ReadLine();
            proxy.Shutdown();
            Console.WriteLine("State: " + proxy.State.ToString() + " Enter to exit");
            Console.ReadLine();
        }
    }
}
