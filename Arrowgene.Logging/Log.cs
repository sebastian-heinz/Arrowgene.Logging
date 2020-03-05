using System;

namespace Arrowgene.Logging
{
    public class Log
    {
        public Log(LogLevel logLevel, string text, object tag = null, string loggerIdentity = null, string loggerName = null)
        {
            Text = text;
            LogLevel = logLevel;
            DateTime = DateTime.Now;
            LoggerIdentity = loggerIdentity;
            LoggerName = loggerName ?? "";
            Tag = tag;
        }
        
        public string LoggerIdentity { get; }

        public string LoggerName { get; }

        public string Text { get; }

        public LogLevel LogLevel { get; }

        public DateTime DateTime { get; }
        
        public object Tag { get; }

        public T GetTag<T>()
        {
            return Tag is T ? (T) Tag : default(T);
        }

        public override string ToString()
        {
            string log = "{0:yyyy-MM-dd HH:mm:ss} - {1}";
            if (string.IsNullOrEmpty(LoggerName))
            {
                log += "{2}:";
            }
            else
            {
                log += " - {2}:";
            }
            log += " {3}";
            return string.Format(log, DateTime, LogLevel, LoggerName, Text);
        }
    }
}