using System;
using Xunit;

namespace Arrowgene.Logging.Test;

public class LogProviderTest
{
    private class TestNsLogger : TestLogger
    {
        public override void Configure(object loggerTypeConfig, object identityConfig)
        {
            Assert.Equal("Arrowgene.Logging.Test", identityConfig);
        }
    }

    [Fact]
    public void TestNsResolution()
    {
        LogProvider.ConfigureNamespace("Arrowgene.Logging.Test", "Arrowgene.Logging.Test");
        LogProvider.Logger<TestNsLogger>(this);
    }
}