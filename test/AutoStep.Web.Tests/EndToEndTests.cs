using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Events;
using AutoStep.Extensions;
using AutoStep.Extensions.Abstractions;
using AutoStep.Language;
using AutoStep.Projects;
using AutoStep.Projects.Files;
using Microsoft.Extensions.Configuration;
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

            var extension = new Extension(NullLoggerFactory.Instance);

            var configuration = new ConfigurationBuilder().Build();

            extension.AttachToProject(configuration, project);

            // Get additional interaction file.
            var fileSet = FileSet.Create(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), new[] { "content/*.asi" });
            project.MergeInteractionFileSet(fileSet);

            Assert.Empty((await project.Compiler.CompileAsync()).Messages);

            Assert.Empty(project.Compiler.Link().Messages);

            var testRun = project.CreateTestRun(configuration);

            extension.ExtendExecution(configuration, testRun);

            var result = new ResultHandler();

            testRun.Events.Add(result);

            await testRun.ExecuteAsync(new NullLoggerFactory(), CancellationToken.None, (cfg, s) => {

                s.RegisterInstance<ILoadedExtensions>(new FakeLoadedExtensions());

                extension.ConfigureExecutionServices(cfg, s);
            });

            Assert.Null(result.Failure);
        }
    }

    internal class FakeLoadedExtensions : ILoadedExtensions
    {
        public string ExtensionsRootDir => throw new NotImplementedException();

        public IEnumerable<IPackageMetadata> LoadedPackages => throw new NotImplementedException();

        public void Dispose()
        {
        }

        public string GetPackagePath(string packageId, params string[] directoryParts)
        {
            throw new NotImplementedException();
        }

        public bool IsPackageLoaded(string packageId)
        {
            return false;
        }
    }

    internal class ResultHandler : BaseEventHandler
    {
        public Exception Failure { get; set; }

        public override async ValueTask OnScenarioAsync(IServiceProvider scope, ScenarioContext ctxt, Func<IServiceProvider, ScenarioContext, CancellationToken, ValueTask> nextHandler, CancellationToken cancelToken)
        {
            await nextHandler(scope, ctxt, cancelToken);

            Failure = ctxt.FailException;
        }
    }
}
