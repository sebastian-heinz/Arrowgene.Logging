using System;
using Xunit;

namespace Arrowgene.Logging.Test;

public class LogProviderTest
{
    private class TestNsLogger : TestLogger
    {
        public override void Initialize(string identity, string name, Action<Log> write, object loggerTypeTag,
            object identityTag)
        {
            Assert.Equal("Arrowgene.Logging.Test", identityTag);
        }
    }

    [Fact]
    public void TestNsResolution()
    {
        LogProvider.ConfigureNamespace("Arrowgene.Logging.Test", "Arrowgene.Logging.Test");
        LogProvider.Logger<TestNsLogger>(this);
    }
}