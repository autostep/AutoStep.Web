using System;
using System.Collections.Generic;
using System.Text;

namespace AutoStep.Web.ElementChain
{
    internal interface IElementChainDescriber
    {
        string Describe(ElementChainNode? lastChainNode);

        string DescribeExecution(LinkedList<ElementChainExecutionStage> executionChain, bool captureElementDetail);
    }
}
