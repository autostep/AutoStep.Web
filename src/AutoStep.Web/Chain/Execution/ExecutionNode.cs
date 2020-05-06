using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Declaration;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain.Execution
{

    public abstract class ExecutionNode
    {
        protected ExecutionNode(DeclarationNode declaration)
        {
            this.Declaration = declaration;
        }

        public ExecutionNode? Previous { get; set; }

        public ExecutionNode? Next { get; set; }

        /// <summary>
        /// Gets or sets an error encountered while executing the node.
        /// </summary>
        public Exception? Error { get; set; }

        /// <summary>
        /// Gets access to the last element in the chain.
        /// </summary>
        public ExecutionNode Last => Next?.Last ?? this;

        public DeclarationNode Declaration { get; }

        /// <summary>
        /// Gets the input elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? InputElements { get; protected set; }

        /// <summary>
        /// Gets the output elements for this node.
        /// </summary>
        public IReadOnlyList<IWebElement>? OutputElements { get; protected set; }

        /// <summary>
        /// Gets the execution context for the node (if known).
        /// </summary>
        public TestExecutionContext? ExecutionContext => Declaration.ExecutionContext;

        public IReadOnlyList<IWebElement>? CachedElements
        {
            get => Declaration.CachedElements;
            set => Declaration.CachedElements = value;
        }

        public string Descriptor => Declaration.Descriptor;

        public bool ModifiesSet => Declaration.ModifiesSet;

        public bool WasExecuted => InputElements is object;

        public IEnumerable<ExecutionNode> Children { get; protected set; } = Enumerable.Empty<ExecutionNode>();

        /// <summary>
        /// Invoke this execution node.
        /// </summary>
        /// <param name="inputElements">The input elements to the node.</param>
        /// <param name="browser">The web browser instance.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Awaitable result containing the output elements.</returns>
        public abstract ValueTask EnterNode(IReadOnlyList<IWebElement> inputElements, IBrowser browser, CancellationToken cancellationToken);

        public abstract ValueTask<IReadOnlyList<IWebElement>> ExitNode(IBrowser browser, CancellationToken cancelToken);
    }

    /// <summary>
    /// Represents a node in a chain execution set.
    /// </summary>
    public abstract class ExecutionNode<TDeclarationNode> : ExecutionNode
        where TDeclarationNode : DeclarationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionNode{TDeclarationNode}"/> class.
        /// </summary>
        /// <param name="node">The declaration node that this execution node executes.</param>
        public ExecutionNode(TDeclarationNode node)
            : base(node)
        {
            Node = node;
        }

        /// <summary>
        /// Gets the declaration node.
        /// </summary>
        public TDeclarationNode Node { get; }
    }
}
