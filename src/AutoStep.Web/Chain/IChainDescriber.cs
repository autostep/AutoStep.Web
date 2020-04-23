using System.Collections.Generic;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Defines a service that can generate strings describing Element Chains, based on an element chain declaration or execution chain.
    /// </summary>
    public interface IChainDescriber
    {
        /// <summary>
        /// Describe the element chain. Generates a list of methods/steps and their associated nodes in the chain.
        /// </summary>
        /// <param name="chain">The chain to describe.</param>
        /// <returns>A description string.</returns>
        string Describe(IElementChain? chain);

        /// <summary>
        /// Describe an executed element chain. Generates a list of methods/steps, their associated nodes and, if indicated with <paramref name="captureElementDetail"/>, details
        /// of the input and output web elements at each stage.
        /// </summary>
        /// <param name="executionChain">The execution chain.</param>
        /// <param name="captureElementDetail">Indicates whether or not to capture element detail. Setting this to true makes this method long-running.</param>
        /// <returns>A description string.</returns>
        string DescribeExecution(LinkedList<ExecutionNode> executionChain, bool captureElementDetail);
    }
}
