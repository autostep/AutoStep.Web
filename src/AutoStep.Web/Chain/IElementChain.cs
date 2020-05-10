using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Web.Chain.Declaration;
using AutoStep.Web.Chain.Execution;
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

        /// <summary>
        /// Add a grouping node to the chain, which takes a reducer and a set of nested chains.
        /// </summary>
        /// <param name="descriptor">A descriptor for the group.</param>
        /// <param name="reducer">The reducer callback.</param>
        /// <param name="chainBuilders">The set of chain builders that define the group.</param>
        /// <returns>A new <see cref="IElementChain"/> representing the previous chain plus the new node.</returns>
        IElementChain AddGroupingNode(string descriptor, Func<IEnumerable<IReadOnlyList<IWebElement>>, IReadOnlyList<IWebElement>> reducer, params Func<IElementChain, IElementChain>[] chainBuilders);

        /// <summary>
        /// Create an <see cref="ExecutionNode"/> from this element chain, that represents the execution start point.
        /// </summary>
        /// <returns>An execution node. Will be null if the chain is empty.</returns>
        ExecutionNode? CreateExecutionEntryNode();
    }
}
