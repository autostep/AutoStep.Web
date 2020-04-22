using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Elements.Metadata;
using AutoStep.Execution.Contexts;
using OpenQA.Selenium;

namespace AutoStep.Web.ElementChain
{
    public interface IDescribable
    {
        MethodContext MethodContext { get; }

        string Descriptor { get; }

        bool ModifiesSet { get; }
    }

    public class ElementChainNode : IDescribable
    {
        public ElementChainNode(Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback, 
                                string descriptor,
                                MethodContext methodContext, 
                                bool modifiesSet)
        {
            Callback = callback;
            MethodContext = methodContext;
            ModifiesSet = modifiesSet;
            Descriptor = descriptor;
        }

        public ElementChainNode(
            ElementChainNode? elementsQueryableNode,
            Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback,
            string descriptor,
            MethodContext methodContext,
            bool modifiesSet)
            : this(callback, descriptor, methodContext, modifiesSet)
        {
            ChildNode = elementsQueryableNode;
        }

        /// <summary>
        /// Contains a reference to the previous node in the chain.
        /// When this value is null, this node is the first node in the chain.
        /// </summary>
        public ElementChainNode? ChildNode { get; }

        public bool ModifiesSet { get; }

        public string Descriptor { get; }

        /// <summary>
        /// Contains the set of nodes enumerated to this point (by this node).
        /// </summary>
        public IReadOnlyList<IWebElement>? NodeEnumeratedElements { get; set; }

        /// <summary>
        /// Contains the callback to invoke that executes this node.
        /// </summary>
        public Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> Callback { get; }

        /// <summary>
        /// Gets the method context for the calling method.
        /// </summary>
        public MethodContext MethodContext { get; }
    }
}
