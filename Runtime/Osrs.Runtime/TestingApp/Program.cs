using Osrs.Net;
using Osrs.Runtime.Services;
using System;
using Osrs.Threading;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //TaskPoolTest();
            TestServer.ServerTest();

            Console.WriteLine("All done");
            Console.ReadLine();
        }

        static void TaskPoolTest()
        {
            TaskPoolOptions options = new TaskPoolOptions();
            options.Timeout = new Osrs.Threading.Timeout(5000); //5 seconds
            //TaskPool pool = new TaskPool(Greet, options);
            TaskPool pool = ScavengingTaskPoolRunner.CreateScavengingTaskPool(Greet, options);

            pool.Next().Run();
            while (Console.ReadLine() != "q")
            {
                Console.WriteLine("Input");
            }
        }

        static void Greet()
        {
            Console.WriteLine("Hello");
        }
    }

    public class TestServer : IServerListener<string>
    {
        public static void ServerTest()
        {
            //TestSingle();
            //Console.WriteLine();
            //Console.WriteLine("Enter to continue");
            //Console.WriteLine();
            //Console.ReadLine();

            TestDual();
            Console.WriteLine();
            Console.WriteLine("Enter to continue");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static void TestSingle()
        {
            Console.WriteLine("Testing single");
            ServerTaskPoolOptions opts = new ServerTaskPoolOptions();
            ServerTaskPool<string> pool = new SingleServerTaskPool<string>(new TestServer(), opts);

            Console.WriteLine("Starting");
            pool.Start();
            Console.WriteLine("Started");

            System.Threading.SpinWait w = new System.Threading.SpinWait();
            for (int i = 0; i < 10000; i++)
            {
                w.SpinOnce();
            }

            Console.WriteLine("Stopping");
            pool.Stop();
            Console.WriteLine("Stopped");

            Console.WriteLine("Single Loop Test");
            Console.WriteLine("Total Listened " + listens);
            Console.WriteLine("Total Handled " + handles);
            Console.WriteLine("Testing single End");
        }

        private static void TestDual()
        {
            Console.WriteLine("Testing double");
            ServerTaskPoolOptions opts = new ServerTaskPoolOptions();
            opts.MaxActiveWorkers = 8;

            ServerTaskPool<string> pool = new DualServerTaskPool<string>(new TestServer(), opts);

            Console.WriteLine("Starting");
            pool.Start();
            Console.WriteLine("Started");

            System.Threading.Thread.Sleep(50000); //50 seconds
            //System.Threading.SpinWait w = new System.Threading.SpinWait();
            //for (int i = 0; i < 1000; i++)
            //{
            //    w.SpinOnce();
            //}

            Console.WriteLine("Stopping");
            pool.Stop();
            Console.WriteLine("Stopped");

            Console.WriteLine("Double Loop Test");
            Console.WriteLine("Total Listened " + listens);
            Console.WriteLine("Total Handled " + handles);
            Console.WriteLine("Testing double End");
        }

        private int i = 0;
        private static long listens = 0;
        private static long handles = 0;
        private int threads = 0;
        private int hthreads = 0;

        public string GetContext()
        {
            return this.GetContext(default(CancellationToken));
        }

        public string GetContext(CancellationToken cancel)
        {
            int lts = System.Threading.Interlocked.Increment(ref threads);
            int x = System.Threading.Interlocked.Increment(ref i);
            System.Threading.Interlocked.Increment(ref listens);
            if (x > 10000)
            {
                Console.Write(" --RESETTING-- ");
                this.i = 0;
            }

            if (lts > 1000)
                Console.Write("LTs " + lts);
            System.Threading.Interlocked.Decrement(ref threads);
            //Console.Write(" C" + x.ToString());
            return x.ToString();
        }

        public void Handle(string context)
        {
            this.Handle(context, default(CancellationToken));
        }

        public void Handle(string context, CancellationToken cancel)
        {
            int hts = System.Threading.Interlocked.Increment(ref hthreads);
            System.Threading.Interlocked.Increment(ref handles);
            //Console.Write("H");
            if (hts > 1000)
                Console.Write("HTs " + hts);
            System.Threading.Interlocked.Decrement(ref hthreads);
        }
    }
}
