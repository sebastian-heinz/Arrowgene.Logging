﻿using System;
using System.Collections.Generic;

namespace Arrowgene.Logging
{
    public sealed class LogProvider
    {
        private static readonly LogProvider _instance = new LogProvider();

        static LogProvider()
        {
        }

        public static LogProvider Instance => _instance;

        public static ILogger Logger(object instance)
        {
            return Instance.GetLogger<Logger>(instance);
        }

        public static ILogger Logger(Type type)
        {
            return Instance.GetLogger<Logger>(type);
        }

        public static T Logger<T>(object instance) where T : ILogger, new()
        {
            return Instance.GetLogger<T>(instance);
        }

        public static T Logger<T>(Type type) where T : ILogger, new()
        {
            return Instance.GetLogger<T>(type);
        }

        public static T Logger<T>(string identity, string zone = null) where T : ILogger, new()
        {
            return Instance.GetLogger<T>(identity, zone);
        }

        public static void Configure<T>(object configuration) where T : ILogger, new()
        {
            Instance.SetConfigure(typeof(T), configuration);
        }

        public static void Configure(Type type, object configuration)
        {
            Instance.SetConfigure(type.FullName, configuration);
        }

        /// <summary>
        /// Provide a configuration object that will be passed to every <see cref="ILogger"/> instance
        /// by calling <see cref="ILogger.Initialize(string, string, object)"/> on it.
        /// </summary>
        public static void Configure(string identity, object configuration)
        {
            Instance.SetConfigure(identity, configuration);
        }

        private readonly Dictionary<string, ILogger> _loggers;
        private readonly Dictionary<string, object> _configurations;
        private readonly object _lock;

        private LogProvider()
        {
            _loggers = new Dictionary<string, ILogger>();
            _configurations = new Dictionary<string, object>();
            _lock = new object();
        }

        /// <summary>
        /// Notifies about any logging event from every ILogger instance
        /// </summary>
        public static event EventHandler<LogWriteEventArgs> GlobalLogWrite;

        public void SetConfigure<T>(object configuration) where T : ILogger, new()
        {
            SetConfigure(typeof(T), configuration);
        }

        public void SetConfigure(Type type, object configuration)
        {
            SetConfigure(type.FullName, configuration);
        }

        /// <summary>
        /// Provide a configuration object that will be passed to every <see cref="ILogger"/> instance
        /// by calling <see cref="ILogger.Initialize(string, string, object)"/> on it.
        /// </summary>
        public void SetConfigure(string identity, object configuration)
        {
            _configurations.Add(identity, configuration);
        }

        public ILogger GetLogger(object instance)
        {
            return GetLogger<Logger>(instance);
        }

        public ILogger GetLogger(Type type)
        {
            return GetLogger<Logger>(type);
        }

        public T GetLogger<T>(object instance) where T : ILogger, new()
        {
            return GetLogger<T>(instance.GetType());
        }

        public T GetLogger<T>(Type type) where T : ILogger, new()
        {
            return GetLogger<T>(type.FullName, type.Name);
        }

        /// <summary>
        /// Returns a logger that matches the identity or creates a new one.
        /// </summary>
        /// <param name="identity">Unique token to identify a logger</param>
        /// <param name="name">Name to identify the log origin</param>
        /// <typeparam name="T">Type of the logger instance</typeparam>
        /// <returns>New instance of T or existing ILogger if the identity already exists</returns>
        /// <exception cref="T:System.Exception"><paramref name="identity">identity</paramref> does not match T.</exception>
        public T GetLogger<T>(string identity, string name = null) where T : ILogger, new()
        {
            ILogger logger;
            lock (_lock)
            {
                if (!_loggers.TryGetValue(identity, out logger))
                {
                    object configuration = null;
                    string typeName = typeof(T).FullName;
                    if (typeName != null && _configurations.ContainsKey(typeName))
                    {
                        configuration = _configurations[typeName];
                    }

                    logger = new T();
                    logger.Initialize(identity, name, configuration);
                    logger.LogWrite += LoggerOnLogWrite;
                    _loggers.Add(identity, logger);
                }
            }

            if (logger is T concreteLogger)
            {
                return concreteLogger;
            }

            string realType = logger == null ? "null" : logger.GetType().ToString();
            throw new Exception($"Logger identity: {identity} is not type of {typeof(T)} but {realType}");
        }

        private void LoggerOnLogWrite(object sender, LogWriteEventArgs writeEventArgs)
        {
            EventHandler<LogWriteEventArgs> globalLogWrite = GlobalLogWrite;
            if (globalLogWrite != null)
            {
                globalLogWrite(sender, writeEventArgs);
            }
        }
    }
}