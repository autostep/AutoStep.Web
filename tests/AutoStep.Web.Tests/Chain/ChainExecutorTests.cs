using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain;
using AutoStep.Web.Chain.Execution;
using AutoStep.Web.Tests.Util;
using Castle.DynamicProxy.Generators.Emitters;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace AutoStep.Web.Tests.Chain
{
    public class ChainExecutorTests : LoggingTestBase
    {
        public ChainExecutorTests(ITestOutputHelper outputHelper) 
            : base(outputHelper)
        {
        }

        [Fact]
        public async Task EmptyChainReturnsEmptyElementSet()
        {
            var mockBrowser = new Mock<IBrowser>();
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var chain = new ElementChain(new ElementChainOptions());

            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task ChainExecutesInOrder()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var executed = new List<string>();

            IElementChain chain = new ElementChain(new ElementChainOptions());
            chain = chain.AddNode("node1", el => executed.Add("node1"));
            chain = chain.AddNode("node2", el => executed.Add("node2"));
            chain = chain.AddNode("node3", el => executed.Add("node3"));

            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(0);

            executed.Should().Equal("node1", "node2", "node3");
        }

        [Fact]
        public async Task ChainExecutesGroupNode()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var executed = new List<string>();

            IElementChain chain = new ElementChain(new ElementChainOptions());
            chain = chain.AddGroupingNode("group", resultElements =>
            {
                foreach (var item in resultElements)
                {
                    executed.Add("reducer-" + item.Count);
                }

                return Array.Empty<IWebElement>();
            }, 
            nested => nested.AddNode("n1", el => { executed.Add("nested1"); return new IWebElement[1]; }),
            // This branch will only output the last element set (of three).
            nested => nested.AddNode("n2", el => { executed.Add("nested2"); return new IWebElement[2]; })
                            .AddNode("n3", el => { executed.Add("nested3"); return new IWebElement[3]; }),
            nested => nested.AddNode("n4", el => { executed.Add("nested4"); return new IWebElement[4]; }));

            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(0);

            executed.Should().Equal("nested1", "nested2", "nested3", "nested4", 
                                    // Three branches, three reducer sets.
                                    "reducer-1", "reducer-3", "reducer-4");
        }

        [Fact]
        public async Task ChainExecutesNestedGroupNode()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var executed = new List<string>();

            IElementChain chain = new ElementChain(new ElementChainOptions());
            chain = chain.AddGroupingNode("group", resultElements =>
            {
                executed.Add("reducerA");
             
                return Array.Empty<IWebElement>();
            },
            nested => nested.AddNode("n1", el => executed.Add("nested1"))
                            .AddGroupingNode("group2", resultElements =>
                            {
                                executed.Add("reducerB");

                                return Array.Empty<IWebElement>();
                            },
                            nested => nested.AddNode("n2", el => executed.Add("nested2"))));

            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(0);

            executed.Should().Equal("nested1", "nested2", "reducerB", "reducerA");
        }


        [Fact]
        public async Task ChainRetries()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var count = 0;

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1 });
            chain = chain.AddNode("node1", el => count++);
            chain = chain.AddNode("node2", el => count++);
            chain = chain.AddNode("node3", el => 
            {
                count++;
                if (count < 4) throw new InvalidOperationException();
            });

            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(0);

            // Should have run twice through.
            count.Should().Be(6);
        }

        [Fact]
        public void ChainThrowsExceptionIfAllRetriesFail()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var count = 0;

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1, TotalRetryTimeoutMs = 200 });
            chain = chain.AddNode("node1", el => count++);
            chain = chain.AddNode("node2", el => count++);
            chain = chain.AddNode("node3", el =>
            {
                count++;
                throw new InvalidOperationException();
            });

            executor.Awaiting(e => e.ExecuteAsync(chain, CancellationToken.None)).Should().Throw<InvalidOperationException>();

            // Should have run twice through.
            count.Should().BeGreaterThan(6);
        }

        [Fact]
        public async Task ChainSecondPassStartsFromLastNodeWithCache()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var mockElement = new Mock<IWebElement>().Object;
            var elements = new IWebElement[] { mockElement };

            var executed = new List<string>();

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1 });
            chain = chain.AddNode("node1", el => executed.Add("node1"));
            // Second node has elements.
            chain = chain.AddNode("node2", el => { executed.Add("node2"); return elements; });
            
            // Invoke for node 1 and 2.
            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].Should().BeSameAs(mockElement);

            chain = chain.AddNode("node3", el => executed.Add("node3"));

            // Invoke again now that node 3 is added.
            result = await executor.ExecuteAsync(chain, CancellationToken.None);

            executed.Should().Equal("node1", "node2", "node3");

            result.Should().HaveCount(1);
            result[0].Should().BeSameAs(mockElement);
        }

        [Fact]
        public async Task ChainSecondPassIgnoresCachedElementsOnSubsequentRetry()
        {
            var mockBrowser = new Mock<IBrowser>();
            mockBrowser.Setup(x => x.WaitForPageReady(CancellationToken.None)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var mockElement = new Mock<IWebElement>().Object;
            var elements = new IWebElement[] { mockElement };

            var executed = new List<string>();

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1 });
            chain = chain.AddNode("node1", el => executed.Add("node1"));
            // Second node has elements.
            chain = chain.AddNode("node2", el => { executed.Add("node2"); return elements; });

            // Invoke for node 1 and 2.
            var result = await executor.ExecuteAsync(chain, CancellationToken.None);

            var runOnce = false;

            // Add node that will succeed on the second attempt.
            chain = chain.AddNode("node3", el =>
            { 
                executed.Add("node3");

                if (!runOnce)
                {
                    runOnce = true;
                    throw new InvalidOperationException();
                }
            });

            // Reset the executed list.
            executed.Clear();

            // Invoke again now that node 3 is added.
            result = await executor.ExecuteAsync(chain, CancellationToken.None);

            // First attempt should skip 1 and 2; second attempt should run all of them.   
            executed.Should().Equal("node3", "node1", "node2", "node3");

            result.Should().HaveCount(1);
            result[0].Should().BeSameAs(mockElement);
        }

        [Fact]
        public void ChainCancellableDuringRetryWait()
        {
            var mockBrowser = new Mock<IBrowser>();
            var cancelSource = new CancellationTokenSource();

            mockBrowser.Setup(x => x.WaitForPageReady(cancelSource.Token)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var count = 0;

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1000 });
            chain = chain.AddNode("node1", el => count++);
            chain = chain.AddNode("node2", el => count++);
            chain = chain.AddNode("node3", el =>
            {
                count++;
                cancelSource.Cancel();
                throw new InvalidOperationException();
            });

            // The full retry period should never be used, because we are cancelling part way through.
            Func<Task> measurable = () =>  executor.Awaiting(e => e.ExecuteAsync(chain, cancelSource.Token))
                                                   .Should().ThrowAsync<TaskCanceledException>();            

            measurable.Should().CompleteWithin(TimeSpan.FromMilliseconds(50));

            // Should have run once.
            count.Should().Be(3);
        }

        [Fact]
        public void ChainCancellableDuringExecution()
        {
            var mockBrowser = new Mock<IBrowser>();
            var cancelSource = new CancellationTokenSource();

            mockBrowser.Setup(x => x.WaitForPageReady(cancelSource.Token)).Returns(new ValueTask<bool>(true));
            var logger = LogFactory.CreateLogger<ChainExecutor>();
            var describer = new ChainDescriber();

            var executor = new ChainExecutor(mockBrowser.Object, logger, describer);

            var count = 0;

            IElementChain chain = new ElementChain(new ElementChainOptions { RetryDelayMs = 1 });
            chain = chain.AddNode("node1", el => count++);
            chain = chain.AddNode("node2", el => { count++; cancelSource.Cancel(); });
            chain = chain.AddNode("node3", el => count++);

            // The full retry period should never be used, because we are cancelling part way through.
            executor.Awaiting(e => e.ExecuteAsync(chain, cancelSource.Token)).Should().Throw<OperationCanceledException>();

            // Should have run partially.
            count.Should().Be(2);
        }
    }
}
