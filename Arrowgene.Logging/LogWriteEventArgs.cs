using System;

namespace Arrowgene.Logging
{
    public class LogWriteEventArgs : EventArgs
    {
        public LogWriteEventArgs(Log log)
        {
            Log = log;
        }

        public Log Log { get; }
    }
}