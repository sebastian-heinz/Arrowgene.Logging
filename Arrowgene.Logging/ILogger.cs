using System;

namespace Arrowgene.Logging
{
    public interface ILogger
    {
        void Initialize(string identity, string name, Action<Log> write);
        void Configure(object loggerTypeConfig, object identityConfig);
        void Write(LogLevel logLevel, string message, object tag);
        void Trace(string message);
        void Info(string message);
        void Debug(string message);
        void Error(string message);
        void Exception(Exception exception);
    }
}