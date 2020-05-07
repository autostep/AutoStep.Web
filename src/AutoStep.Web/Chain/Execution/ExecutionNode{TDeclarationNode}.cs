using AutoStep.Web.Chain.Declaration;

namespace AutoStep.Web.Chain.Execution
{
    /// <summary>
    /// Provides a typed form of an Execution Node, to make it easier for derived classes to access the expected node type.
    /// </summary>
    /// <typeparam name="TDeclarationNode">The declaration node type.</typeparam>
    public abstract class ExecutionNode<TDeclarationNode> : ExecutionNode
        where TDeclarationNode : DeclarationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionNode{TDeclarationNode}"/> class.
        /// </summary>
        /// <param name="node">The declaration node that this execution node executes.</param>
        protected ExecutionNode(TDeclarationNode node)
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
