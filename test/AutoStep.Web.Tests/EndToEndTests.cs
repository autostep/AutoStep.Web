using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Events;
using AutoStep.Language;
using AutoStep.Projects;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AutoStep.Web.Tests
{
    public class EndToEndTests
    {
        [Fact]
        public async Task RunSimpleTest()
        {
            const string Test = @"
                
                Feature: My Feature

                    Scenario: My Scenario

                        Given I have navigated to 'http://google.com'
                          And I have entered '7 Layer' into the Search field
                          And I have clicked the 'Google Search' button

                        Then the '7 Layer - Google Search' page should be displayed
            ";

            var project = new Project();

            project.TryAddFile(new ProjectTestFile("/test", new StringContentSource(Test)));

            project.AddWebInteractions(new NullLoggerFactory());

            await project.Compiler.CompileAsync();

            project.Compiler.Link();

            var testRun = project.CreateTestRun();

            testRun.AddWebHandlers();

            var result = new ResultHandler();

            testRun.Events.Add(result);

            await testRun.ExecuteAsync(new NullLoggerFactory());

            Assert.Null(result.Failure);
        }
    }

    internal class ResultHandler : BaseEventHandler
    {
        public Exception Failure { get; set; }

        public override async ValueTask OnScenario(IServiceScope scope, ScenarioContext ctxt, Func<IServiceScope, ScenarioContext, ValueTask> nextHandler)
        {
            await nextHandler(scope, ctxt);

            Failure = ctxt.FailException;
        }
    }
}
