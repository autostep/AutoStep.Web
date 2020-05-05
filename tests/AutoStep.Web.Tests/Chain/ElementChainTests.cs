using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoStep.Web.Chain;
using FluentAssertions;
using OpenQA.Selenium;
using Xunit;

namespace AutoStep.Web.Tests.Chain
{
    public class ElementChainTests
    {
        [Fact]
        public void AddNodeNoModifyReturnsDifferentElementChainInstance()
        {
            var chain = new ElementChain(new ElementChainOptions());

            var newChain = chain.AddNode("node", (el, br, cancel) => default(ValueTask));

            chain.Should().NotBeSameAs(newChain);
        }

        [Fact]
        public void AddNodeModifyReturnsDifferentElementChainInstance()
        {
            var chain = new ElementChain(new ElementChainOptions());

            var newChain = chain.AddNode("node", (el, br, cancel) => default(ValueTask<IReadOnlyList<IWebElement>>));

            chain.Should().NotBeSameAs(newChain);
        }

        [Fact]
        public void AddNodeWithModifyCreatesLeafNodeWithMetadata()
        {
            IElementChain chain = new ElementChain(new ElementChainOptions());

            chain = chain.AddNode("node", (el, br, cancel) => default(ValueTask<IReadOnlyList<IWebElement>>));

            chain.LeafNode.Should().NotBeNull();
            chain.LeafNode!.ModifiesSet.Should().BeTrue();
            chain.LeafNode!.Descriptor.Should().Be("node");
        }

        [Fact]
        public void AddNodeNoModifyCreatesLeafNodeWithMetadata()
        {
            IElementChain chain = new ElementChain(new ElementChainOptions());

            chain = chain.AddNode("node", (el, br, cancel) => default(ValueTask));

            chain.LeafNode.Should().NotBeNull();
            chain.LeafNode!.ModifiesSet.Should().BeFalse();
            chain.LeafNode!.Descriptor.Should().Be("node");
        }

        [Fact]
        public async Task AddNodeNoModifyWrapsProvidedCallback()
        {
            IElementChain chain = new ElementChain(new ElementChainOptions());

            var invoked = false;

            chain = chain.AddNode("node", async (el, br, cancel) => { await Task.Delay(1); invoked = true; });

            var set = new IWebElement[1];

            chain.LeafNode.Should().NotBeNull();
            var result = await chain.LeafNode!.Callback(set, null!, default);

            invoked.Should().BeTrue();
            result.Should().BeSameAs(set);
        }
    }
}
