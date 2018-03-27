using Newtonsoft.Json.Linq;
using Osrs.Net.Http;
using Osrs.Net.Http.Handlers;
using Osrs.Net.Http.Listener;
using Osrs.Net.Http.Routing;
using Osrs.Runtime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Osrs.Threading;

namespace TestingApp
{
    public class Shim : IHandlerMapper
    {
        private readonly IHandlerMapper inner;

        public Shim(IHandlerMapper inner)
        {
            this.inner = inner;
        }

        public void Handle(HttpContext context)
        {
            this.inner.Handle(context);
        }

        public void Handle(HttpContext context, CancellationToken cancel)
        {
            this.inner.Handle(context, cancel);
        }

        public bool Match(HttpContext ctx)
        {
            return this.inner.Match(ctx);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool running = false;
            JObject config = LoadConfig();

            if (config != null)
            {
                MapHandler fileHandler = ConfigureFileHandler(config);

                List<MapHandler> handlers = new List<MapHandler>();
                if (fileHandler != null)
                    handlers.Add(fileHandler);

                HttpListenerServer server = ConfigureWebServer(config, handlers);
                if (server != null)
                {
                    running = true;

                    server.Start();

                    Console.WriteLine("Listening, enter to stop");
                    Console.ReadLine();
                    server.Stop();
                    Console.WriteLine("Stopped");
                    server.Dispose();
                }
            }

            if (!running) //we failed along the way
                Console.WriteLine("Failed to configure and startup");

            Console.WriteLine("Enter to exit");
            Console.ReadLine();
        }


        static JObject LoadConfig()
        {
            string config = Path.Combine(AppContext.BaseDirectory, "ServerConfig.json"); //we're using this for our config file -- wouldn't do this for production
            if (File.Exists(config))
            {
                Newtonsoft.Json.JsonTextReader rdr = new Newtonsoft.Json.JsonTextReader(File.OpenText(config)); //read it in for use
                if (rdr.Read())
                {
                    JObject o = JToken.Load(rdr) as JObject;
                    rdr.Close();
                    return o;
                }
            }
            return null;
        }

        static UrlBaseMapHandler ConfigureFileHandler(JObject config)
        {
            if (config != null)
            {
                JObject hand = config["simpleFileHandler"] as JObject; //allows us to host static files
                if (hand != null)
                {
                    JArray urls;
                    JValue root = hand["rootDirectory"] as JValue;
                    if (root != null)
                    {
                        string rootDir = root.ToString();
                        if (!string.IsNullOrEmpty(rootDir))
                        {
                            urls = hand["defaultFiles"] as JArray;
                            if (urls != null)
                            {
                                List<string> defFiles = new List<string>();
                                foreach (JToken cur in urls)
                                {
                                    string tmp = cur.ToString();
                                    if (!string.IsNullOrEmpty(tmp))
                                        defFiles.Add(tmp);
                                }
                                if (defFiles.Count > 0)
                                {
                                    urls = hand["allowedExtensions"] as JArray;
                                    if (urls != null)
                                    {
                                        FileExtensions exts = new FileExtensions();
                                        foreach (JToken cur in urls)
                                        {
                                            string tmp = cur.ToString();
                                            if (!string.IsNullOrEmpty(tmp))
                                                exts.Add(tmp);
                                        }

                                        if (exts.Count > 0)
                                        {
                                            SimpleFileHandler handler = new SimpleFileHandler(rootDir, defFiles, MimeTypes.GetAllWellKnown(), exts, new FileExtensions()); //so we can also have static files
                                            return new UrlBaseMapHandler(handler, new string[] { "/" });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        static HttpListenerServer ConfigureWebServer(JObject config, IEnumerable<MapHandler> handlerMappers)
        {
            if (handlerMappers != null)
            {
                JArray urls = config["listenerUrls"] as JArray; //list of urls to listen on
                if (urls != null)
                {
                    List<string> urlList = new List<string>();
                    foreach (JToken cur in urls)
                    {
                        string tmp = cur.ToString();
                        if (!string.IsNullOrEmpty(tmp))
                            urlList.Add(tmp);
                    }
                    if (urlList.Count > 0)
                    {
                        //ok we can start this thing
                        ServerTaskPoolOptions options = new ServerTaskPoolOptions();

                        ServerRouting router = new ServerRouting();
                        foreach (MapHandler cur in handlerMappers)
                        {
                            if (cur != null)
                            {
                                router.Map.Add(cur);
                            }
                        }

                        if (router.Map.Count > 0)
                        {
                            HttpListenerServerListener listener = new HttpListenerServerListener(urlList, new Shim(router));
                            HttpListenerServer server = HttpListenerServer.Create(listener, options, false);

                            return server;
                        }
                    }
                }
            }
            return null;
        }
    }
}
