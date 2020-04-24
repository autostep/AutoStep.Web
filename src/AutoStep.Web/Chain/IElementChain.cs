using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Provides access to the end of a chain of element operations.
    /// </summary>
    /// <remarks>
    /// Element chains are immutable.
    /// </remarks>
    public interface IElementChain
    {
        /// <summary>
        /// Gets the last node in the chain (which this chain object represents).
        /// </summary>
        public DeclarationNode? LeafNode { get; }

        /// <summary>
        /// Gets a value indicating whether there have been any chain operations prior to this one.
        /// </summary>
        public bool AnyPreviousNodes { get; }

        /// <summary>
        /// Gets the set of options for this chain.
        /// </summary>
        ElementChainOptions Options { get; }

        /// <summary>
        /// Adds a node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The async callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        IElementChain AddNode(string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback);

        /// <summary>
        /// Adds a non-modifying node to the chain (and returns a new chain).
        /// </summary>
        /// <param name="descriptor">A description of the node.</param>
        /// <param name="callback">The async callback to invoke.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        IElementChain AddNode(string descriptor, Func<IReadOnlyList<IWebElement>, IBrowser, CancellationToken, ValueTask> callback);
    }
}
