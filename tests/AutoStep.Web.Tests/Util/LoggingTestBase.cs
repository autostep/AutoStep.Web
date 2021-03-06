﻿using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoStep.Web.Tests.Util
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
