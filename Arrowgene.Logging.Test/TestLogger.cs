using System;

namespace Arrowgene.Logging.Test;

public class TestLogger : ILogger
{
    public virtual void Initialize(string identity, string name, Action<Log> write, object loggerTypeTag, object identityTag)
    {
    }

    public virtual void Write(LogLevel logLevel, string message, object tag)
    {
    }

    public virtual void Trace(string message)
    {
    }

    public virtual void Info(string message)
    {
    }

    public virtual void Debug(string message)
    {
    }

    public virtual void Error(string message)
    {
    }

    public virtual void Exception(Exception exception)
    {
    }
}