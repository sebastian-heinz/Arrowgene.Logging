using System;

namespace Arrowgene.Logging
{
    public class Logger : ILogger
    {
        private string _zone;
        private string _identity;

        public Logger()
        {
        }

        public event EventHandler<LogWriteEventArgs> LogWrite;

        public virtual void Initialize(string identity, string zone, object configuration)
        {
            _identity = identity;
            _zone = zone;
        }

        public void Write(Log log)
        {
            OnLogWrite(log);
        }

        public void Write(LogLevel logLevel, string message, object tag)
        {
            Log log = new Log(logLevel, message, tag, _identity, _zone);
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

        private void OnLogWrite(Log log)
        {
            EventHandler<LogWriteEventArgs> logWrite = LogWrite;
            if (logWrite != null)
            {
                LogWriteEventArgs logWriteEventArgs = new LogWriteEventArgs(log);
                logWrite(this, logWriteEventArgs);
            }
        }
    }
}