using AutoStep.Execution;
using AutoStep.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoStep.ExtensionTesting
{
    public class EntryPoint : BaseExtensionEntryPoint
    {
        public EntryPoint(ILoggerFactory logFactory) : base(logFactory)
        {
        }

        public override void ExtendExecution(IConfiguration projectConfig, TestRun testRun)
        {
            // Add an event handler for expecting errors.
            testRun.Events.Add(new ErrorEventHandler());
        }
    }
}
