using System;
using System.Collections.Generic;
using System.Text;
using AutoStep.Execution.Contexts;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Indicates that an item can be described by a <see cref="IChainDescriber"/>.
    /// </summary>
    public interface IDescribable
    {
        /// <summary>
        /// Gets an optional execution context for the caller (used to improve descriptive output).
        /// </summary>
        TestExecutionContext? ExecutionContext { get; }

        /// <summary>
        /// Gets a description of the node.
        /// </summary>
        string Descriptor { get; }

        /// <summary>
        /// Gets a value indicating whether this node can modify the set of elements.
        /// </summary>
        bool ModifiesSet { get; }
    }
}
