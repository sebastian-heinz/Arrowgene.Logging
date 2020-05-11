using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Arrowgene.Logging
{
    public static class LogProvider
    {
        private static readonly BlockingCollection<Log> Events;
        private static readonly Dictionary<string, ILogger> Loggers;
        private static readonly Dictionary<string, object> Configurations;
        private static readonly object Lock;

        private static CancellationTokenSource CancellationTokenSource;
        private static Thread Thread;

        static LogProvider()
        {
            Events = new BlockingCollection<Log>();
            Loggers = new Dictionary<string, ILogger>();
            Configurations = new Dictionary<string, object>();
            Lock = new object();
        }

        /// <summary>
        /// Notifies about any logging event from every ILogger instance
        /// </summary>
        public static event EventHandler<LogWriteEventArgs> OnLogWrite;

        public static void Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            Thread = new Thread(WriteThread);
            Thread.Name = "LogWriteThread";
            Thread.Start();
        }

        public static void Stop()
        {
            CancellationTokenSource.Cancel();
            Thread.Join();
        }

        public static ILogger Logger(object instance)
        {
            return Logger<Logger>(instance);
        }

        public static ILogger Logger(Type type)
        {
            return Logger<Logger>(type);
        }

        public static T Logger<T>(object instance) where T : ILogger, new()
        {
            return Logger<T>(instance.GetType());
        }

        public static T Logger<T>(Type type) where T : ILogger, new()
        {
            return Logger<T>(type.FullName, type.Name);
        }

        /// <summary>
        /// Returns a logger that matches the identity or creates a new one.
        /// </summary>
        /// <param name="identity">Unique token to identify a logger</param>
        /// <param name="name"></param>
        /// <typeparam name="T">Type of the logger instance</typeparam>
        /// <returns>New instance of T or existing ILogger if the identity already exists</returns>
        /// <exception cref="T:System.Exception"><paramref name="identity">identity</paramref> does not match T.</exception>
        public static T Logger<T>(string identity, string name = null) where T : ILogger, new()
        {
            ILogger logger;
            lock (Lock)
            {
                if (!Loggers.TryGetValue(identity, out logger))
                {
                    object configuration = null;
                    string typeName = typeof(T).FullName;
                    if (typeName != null && Configurations.ContainsKey(typeName))
                    {
                        configuration = Configurations[typeName];
                    }

                    logger = new T();
                    logger.Initialize(identity, name, Write, configuration);
                    Loggers.Add(identity, logger);
                }
            }

            if (logger is T concreteLogger)
            {
                return concreteLogger;
            }

            string realType = logger == null ? "null" : logger.GetType().ToString();
            throw new Exception($"Logger identity: {identity} is not type of {typeof(T)} but {realType}");
        }

        /// <summary>
        /// Provide a configuration object that will be passed to every <see cref="ILogger"/> instance
        /// by calling <see cref="ILogger.Initialize(string,string,System.Action{Arrowgene.Logging.Log}(Arrowgene.Logging.Log),object)"/> on it.
        /// </summary>
        public static void Configure<T>(object configuration) where T : ILogger, new()
        {
            Configure(typeof(T), configuration);
        }

        public static void Configure(Type type, object configuration)
        {
            Configure(type.FullName, configuration);
        }

        /// <summary>
        /// Provide a configuration object that will be passed to every <see cref="ILogger"/> instance
        /// by calling <see cref="ILogger.Initialize(string,string,System.Action{Arrowgene.Logging.Log}(Arrowgene.Logging.Log),object)"/> on it.
        /// </summary>
        public static void Configure(string identity, object configuration)
        {
            Configurations.Add(identity, configuration);
        }

        public static void Write(Log log)
        {
            Events.Add(log);
        }

        private static void WriteThread()
        {
            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                Log log;
                try
                {
                    log = Events.Take(CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                EventHandler<LogWriteEventArgs> onLogWrite = OnLogWrite;
                if (onLogWrite != null)
                {
                    LogWriteEventArgs logWriteEventArgs = new LogWriteEventArgs(log);
                    onLogWrite(null, logWriteEventArgs);
                }
            }
        }
    }
}