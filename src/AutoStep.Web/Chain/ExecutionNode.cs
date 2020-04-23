using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Represents a node in a chain execution set.
    /// </summary>
    public class ExecutionNode : IDescribable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionNode"/> class.
        /// </summary>
        /// <param name="node">The declaration node that this execution node executes.</param>
        public ExecutionNode(DeclarationNode node)
        {
            Node = node;
        }

        /// <summary>
        /// Gets the declaration node.
        /// </summary>
        public DeclarationNode Node { get; }

        /// <summary>
        /// Gets or sets an error encountered while executing the node.
        /// </summary>
        public Exception? Error { get; set; }

        /// <summary>
        /// Gets the input elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? InputElements { get; private set; }

        /// <summary>
        /// Gets the output elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? OutputElements { get; private set; }

        /// <summary>
        /// Gets or sets the cached elements attached to the declaration node.
        /// </summary>
        public IReadOnlyList<IWebElement>? CachedElements
        {
            get => Node.CachedElements;
            set => Node.CachedElements = value;
        }

        /// <summary>
        /// Gets the execution context for the node (if known).
        /// </summary>
        public TestExecutionContext? ExecutionContext => Node.ExecutionContext;

        /// <inheritdoc/>
        public string Descriptor => Node.Descriptor;

        /// <summary>
        /// Gets a value indicating whether this node was executed.
        /// </summary>
        public bool WasExecuted => InputElements is object;

        /// <inheritdoc/>
        public bool ModifiesSet => Node.ModifiesSet;

        /// <summary>
        /// Invoke this execution node.
        /// </summary>
        /// <param name="inputElements">The input elements to the node.</param>
        /// <param name="browser">The web browser instance.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Awaitable result containing the output elements.</returns>
        public async ValueTask<IReadOnlyList<IWebElement>> Invoke(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken)
        {
            try
            {
                InputElements = inputElements;

                // Throw cancellation here. This ensures that the cancellation exception is captured against the execution node
                // and it looks like an exception inside the callback.
                cancellationToken.ThrowIfCancellationRequested();

                OutputElements = await Node.Callback(inputElements, browser, cancellationToken);
                Error = null;
                return OutputElements;
            }
            catch (Exception ex)
            {
                Error = ex;
                throw;
            }
        }
    }
}
