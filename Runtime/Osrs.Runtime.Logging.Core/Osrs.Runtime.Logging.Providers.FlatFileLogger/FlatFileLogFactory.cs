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

using Osrs.Runtime.Configuration;
using System;
using System.IO;

namespace Osrs.Runtime.Logging.Providers
{
    public sealed class FlatFileLogFactory : LogProviderFactory
    {
        internal static int count = 0;
        internal static object syncRoot = new object(); //yes one for all
        internal static StreamWriter writer;
        internal static LogProviderBase log; //permanent logger

        /// <summary>
        /// For this to be used, it must be set prior to Initialize being called.
        /// Note that the LogManager is responsible for calling Initialize, and it should not be called directly.
        /// </summary>
        public static string Configuration
        {
            get;
            set;
        }

        protected override LogProviderBase GetLogger(Type t)
        {
            return new FlatFileLogger(t);
        }

        protected override void Shutdown()
        {
            lock (syncRoot)
            {
                if (writer != null)
                {
                    try
                    {
                        if (log == null || log is NullLogger)
                            log = LogManager.Instance.GetProvider(typeof(FlatFileLogFactory));
                        log.Log(0, "Shutdown: Flushing and closing log");
                        writer.Flush();
                        writer.Dispose();
                        try
                        {
                            writer.Dispose();
                        }
                        catch
                        { }
                        writer = null;
                    }
                    catch
                    {
                        log.Log(1000, "Shutdown: Failed flushing and closing log");
                    }
                }
            }
        }

        /// <summary>
        /// Tries to intialize by configuration, if no configuartion parameter is found, tries to use local static configuration property.
        /// If that is not found, initialization fails.
        /// </summary>
        /// <returns></returns>
        protected override bool Initialize()
        {
            lock (syncRoot)
            {
                LogProviderBase logger = LogManager.Instance.GetProvider(typeof(FlatFileLogFactory));
                if (logger == null)
                    logger = new NullLogger(typeof(FlatFileLogFactory));

                ConfigurationProviderBase config = ConfigurationManager.Instance.GetProvider();
                if (config != null)
                {
                    ConfigurationParameter param = config.Get(typeof(FlatFileLogFactory), "fileName");
                    if (param != null)
                    {
                        try
                        {
                            string fileName = (string)param.Value;
                            if (Init(fileName))
                            {
                                logger = null; //make sure to clear it out
                                return true;
                            }
                            else
                            {
                                logger.Log(1000, "InitImpl: Failed to get or create log file: " + fileName);
                            }
                        }
                        catch
                        {
                            logger.Log(1000, "InitImpl: Failed to get configuration param");
                        }
                    }
                    else
                    {
                        if (Init(Configuration))
                        {
                            logger = null; //make sure to clear it out
                            return true;
                        }

                        logger.Log(1000, "InitImpl: Failed to get config parameter: fileName");
                    }
                }
                else
                {
                    logger.Log(1000, "InitImpl: Failed to get config provider");
                    if (Init(Configuration))
                    {
                        logger = null; //make sure to clear it out
                        return true;
                    }
                }
                logger = null; //make sure to clear it out
                return false;
            }
        }

        private bool Init(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    writer = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read));
                    writer.AutoFlush = true;

                    if (writer != null && writer.BaseStream.CanWrite)
                        return true;
                }
                catch
                { }
            }
            return false;
        }
    }
}
