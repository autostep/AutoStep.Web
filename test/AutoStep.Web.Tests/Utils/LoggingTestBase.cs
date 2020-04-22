using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace AutoStep.Tests.Utils
{
    public class LoggingTestBase
    {
        public LoggingTestBase(ITestOutputHelper outputHelper)
        {
            TestOutput = outputHelper;
            LogFactory = TestLogFactory.Create(outputHelper);
        }

        protected ITestOutputHelper TestOutput { get; }

        protected ILoggerFactory LogFactory { get; }
    }
}
