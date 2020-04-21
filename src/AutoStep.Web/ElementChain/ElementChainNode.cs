using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Elements.Metadata;
using OpenQA.Selenium;

namespace AutoStep.Web.ElementChain
{
    public class ElementChainNode
    {
        public ElementChainNode(Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback, IMethodCallInfo methodCall)
        {
            Callback = callback;
            MethodMetadata = methodCall;
        }

        public ElementChainNode(
            ElementChainNode? elementsQueryableNode,
            Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            IMethodCallInfo methodCall)
            : this(callback, methodCall)
        {
            ChildNode = elementsQueryableNode;
        }

        /// <summary>
        /// Contains a reference to the previous node in the chain.
        /// When this value is null, this node is the first node in the chain.
        /// </summary>
        public ElementChainNode? ChildNode { get; }

        public Action<IReadOnlyList<IWebElement>, StringBuilder> DescriptionBuilder { get; }

        /// <summary>
        /// Contains the set of nodes enumerated to this point (by this node).
        /// </summary>
        public IReadOnlyList<IWebElement>? NodeEnumeratedElements { get; set; }

        /// <summary>
        /// Contains the callback to invoke that executes this node.
        /// </summary>
        public Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> Callback { get; }

        public IMethodCallInfo MethodMetadata { get; }
    }
}
