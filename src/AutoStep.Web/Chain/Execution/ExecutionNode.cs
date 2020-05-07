using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Declaration;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// An execution node represents a single execution of a declaration node.
    /// </summary>
    public abstract class ExecutionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionNode"/> class.
        /// </summary>
        /// <param name="declaration">The declaration node that this node executes.</param>
        protected ExecutionNode(DeclarationNode declaration)
        {
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets or sets the next node in the execution chain.
        /// </summary>
        public ExecutionNode? Next { get; set; }

        /// <summary>
        /// Gets the last element in the chain.
        /// </summary>
        public ExecutionNode Last => Next?.Last ?? this;

        /// <summary>
        /// Gets or sets an error encountered while executing the node.
        /// </summary>
        public Exception? Error { get; set; }

        /// <summary>
        /// Gets or sets the child execution nodes. Each child node is executed individually, and is provided the same input as this node.
        /// </summary>
        public IEnumerable<ExecutionNode> ChildNodes { get; protected set; } = Enumerable.Empty<ExecutionNode>();

        /// <summary>
        /// Gets the declaration node underlying this execution node.
        /// </summary>
        public DeclarationNode Declaration { get; }

        /// <summary>
        /// Gets or sets the input elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? InputElements { get; set; }

        /// <summary>
        /// Gets or sets the output elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? OutputElements { get; set; }

        /// <summary>
        /// Gets the execution context for the node (if known).
        /// </summary>
        public TestExecutionContext? ExecutionContext => Declaration.ExecutionContext;

        /// <summary>
        /// Gets or sets the cached elements for this node; cached elements are stored against the declaration node.
        /// </summary>
        internal IReadOnlyList<IWebElement>? CachedElements
        {
            get => Declaration.CachedElements;
            set => Declaration.CachedElements = value;
        }

        /// <summary>
        /// Gets the node descriptor.
        /// </summary>
        public string Descriptor => Declaration.Descriptor;

        /// <summary>
        /// Gets a value indicating whether this node will modify the set.
        /// </summary>
        public bool ModifiesSet => Declaration.ModifiesSet;

        /// <summary>
        /// Gets a value indicating whether this node was run in a given execution.
        /// </summary>
        /// <remarks>
        /// Nodes may not be run if they are skipped due to caching, or because an error occurred in an earlier node.
        /// </remarks>
        public bool WasExecuted => InputElements is object;

        /// <summary>
        /// Called when this node is invoked, prior to any child nodes being processed.
        /// </summary>
        /// <param name="inputElements">The input elements to the node.</param>
        /// <param name="browser">The web browser instance.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Awaitable task.</returns>
        public virtual ValueTask EnterNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            return default;
        }

        /// <summary>
        /// Called after all child nodes have been executed successfully, used to capture the output of the node.
        /// </summary>
        /// <param name="inputElements">The input elements to the node.</param>
        /// <param name="browser">The web browser instance.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Awaitable task, containing the results.</returns>
        public virtual ValueTask<IReadOnlyList<IWebElement>> ExitNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            return new ValueTask<IReadOnlyList<IWebElement>>(inputElements);
        }
    }
}
