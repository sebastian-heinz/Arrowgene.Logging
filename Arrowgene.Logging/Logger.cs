using System;

namespace Arrowgene.Logging
{
    public class Logger : ILogger
    {
        private string _name;
        private string _identity;
        private Action<Log> _write;

        public virtual void Initialize(string identity,
            string name,
            Action<Log> write,
            object loggerTypeTag,
            object identityTag
        )
        {
            _identity = identity;
            _name = name;
            _write = write;
        }

        public void Write(Log log)
        {
            _write(log);
        }

        public void Write(LogLevel logLevel, string message, object tag)
        {
            Log log = new Log(logLevel, message, tag, _identity, _name);
            Write(log);
        }

        public void Trace(string message)
        {
            Write(LogLevel.Trace, message, null);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, message, null);
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message, null);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message, null);
        }

        public void Exception(Exception exception)
        {
            if (exception == null)
            {
                Write(LogLevel.Error, "Exception was null.", null);
                return;
            }

            Write(LogLevel.Error, exception.ToString(), exception);
        }
    }
}