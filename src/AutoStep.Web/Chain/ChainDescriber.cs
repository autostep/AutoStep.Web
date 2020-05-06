using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AutoStep.Elements.Metadata;
using AutoStep.Execution.Contexts;
using AutoStep.Web.Chain.Declaration;
using AutoStep.Web.Chain.Execution;
using OpenQA.Selenium;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Provides the facility to describe element chains.
    /// </summary>
    internal class ChainDescriber : IChainDescriber
    {
        private const int IndentSize = 4;
        private const int MaxElementDetails = 20;

        private static readonly string[] DetailAttributeNames =
        {
            "id",
            "class",
            "aria-label",
            "text",
        };

        private static readonly int MaxDetailNameLength = DetailAttributeNames.Max(s => s.Length);

        /// <inheritdoc/>
        public string Describe(IElementChain? chain)
        {
            if (chain is null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            if (chain.LeafNode is null)
            {
                return "[empty]";
            }

            // To describe a chain, we build an execution set from this node, then walk back.
            var executionSet = new LinkedList<DeclarationNode>();
            DeclarationNode? first = chain.LeafNode;

            do
            {
                executionSet.AddFirst(first);

                first = first!.PreviousNode;
            }
            while (first is object);

            TestExecutionContext? lastExecutionContext = null;
            var strBuilder = new StringBuilder();

            // Now walk the linked list.
            foreach (var node in executionSet)
            {
                if (lastExecutionContext != node.ExecutionContext)
                {
                    RenderStartExecutionContext(strBuilder, lastExecutionContext, node.ExecutionContext);

                    strBuilder.AppendLine(" : [");

                    lastExecutionContext = node.ExecutionContext;
                }

                // Write the node.
                strBuilder.Append("  ");
                strBuilder.AppendLine(node.Descriptor);
            }

            if (lastExecutionContext is object)
            {
                strBuilder.AppendLine("]");
            }

            return strBuilder.ToString();
        }

        /// <inheritdoc/>
        public string DescribeExecution(ExecutionNode entryPoint, bool captureElementDetail)
        {
            TestExecutionContext? lastExecutionContext = null;
            var strBuilder = new StringBuilder();
            ExecutionNode? current = entryPoint;
            bool encounteredError = false;

            // Now walk the linked list.
            while (current is object)
            {
                if (lastExecutionContext != current.ExecutionContext)
                {
                    RenderStartExecutionContext(strBuilder, lastExecutionContext, current.ExecutionContext);

                    strBuilder.AppendLine(" : [");

                    lastExecutionContext = current.ExecutionContext;
                }

                RenderNode(strBuilder, current, 0, captureElementDetail, ref encounteredError);

                current = current.Next;
            }

            if (lastExecutionContext is object)
            {
                strBuilder.AppendLine("]");
            }

            return strBuilder.ToString();
        }

        private static void RenderStartExecutionContext(StringBuilder builder, TestExecutionContext? lastContext, TestExecutionContext? newContext)
        {
            if (lastContext is object)
            {
                // Close the last descriptor, add call separator.
                builder.AppendLine("]");
                builder.Append(" -> ");
            }

            if (newContext is MethodContext methodContext)
            {
                WriteMethodCall(builder, methodContext.MethodCall!, methodContext.Arguments);
            }
            else if (newContext is StepContext stepContext)
            {
                WriteStep(builder, stepContext);
            }
            else
            {
                builder.Append("nodes");
            }
        }

        private static void RenderNode(StringBuilder builder, ExecutionNode node, int indentDepth, bool captureElementDetail, ref bool encounteredError)
        {
            // Write the descriptor first.
            AppendIndent(builder, indentDepth);
            builder.AppendLine(node.Descriptor);

            // If the node was executed, indicate the input and output elements.
            if (node.WasExecuted)
            {
                // Render the input elements detail.
                AppendIndent(builder, indentDepth + 2);
                builder.AppendLine("Input:");
                RenderElementCollection(builder, node.InputElements ?? Array.Empty<IWebElement>(), captureElementDetail, indentDepth + 3);

                if (node.Children.Any())
                {
                    AppendIndent(builder, indentDepth + 2);
                    builder.AppendLine("Children: [ ");

                    foreach (var child in node.Children)
                    {
                        AppendIndent(builder, indentDepth + 3);
                        builder.AppendLine("Nested Chain:");
                        ExecutionNode? current = child;

                        // Now walk the linked list.
                        while (current is object)
                        {
                            RenderNode(builder, current, indentDepth + 4, captureElementDetail, ref encounteredError);
                            current = current.Next;
                        }
                    }

                    AppendIndent(builder, indentDepth + 2);
                    builder.AppendLine("]");
                }

                if (node.Error is object)
                {
                    // Exception occurred, include full details.
                    encounteredError = true;

                    // TODO: Richer information for AutoStep assertion exceptions?
                    AppendIndent(builder, indentDepth + 2);
                    builder.AppendLine("NODE FAILED - EXCEPTION " + node.Error.GetType().Name);

                    AppendIndent(builder, indentDepth + 3);
                    builder.AppendLine(node.Error.Message);
                }
                else if (node.ModifiesSet)
                {
                    // Only render the output elements detail if the node was a modification one.
                    AppendIndent(builder, indentDepth + 2);
                    builder.AppendLine("Node Passed - Output:");
                    RenderElementCollection(builder, node.OutputElements ?? Array.Empty<IWebElement>(), captureElementDetail, indentDepth + 3);
                }
                else
                {
                    // No output, don't show it.
                    AppendIndent(builder, indentDepth + 2);
                    builder.AppendLine("Node Passed");
                }
            }
            else if (encounteredError)
            {
                AppendIndent(builder, indentDepth + 1);
                builder.AppendLine("not run - previous node failed");
            }
            else
            {
                AppendIndent(builder, indentDepth + 1);
                builder.AppendLine("skipped - using cached results from previous evaluation");
            }
        }

        private static void AppendIndent(StringBuilder builder, int depth)
        {
            builder.Append(' ', IndentSize * depth);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Need to ensure that any unexpected exception when accessing element properties can be handled.")]
        private static void RenderElementCollection(StringBuilder builder, IReadOnlyList<IWebElement> webElements, bool captureElementDetail, int indentDepth)
        {
            AppendIndent(builder, indentDepth);

            if (webElements.Count == 0)
            {
                builder.AppendLine("No elements.");
            }
            else if (webElements.Count == 1)
            {
                builder.AppendLine("1 element.");
            }
            else
            {
                builder.AppendLine($"{webElements.Count} elements.");
            }

            if (captureElementDetail)
            {
                var detailCount = Math.Min(webElements.Count, MaxElementDetails);

                for (var idx = 0; idx < detailCount; idx++)
                {
                    var element = webElements[idx];

                    try
                    {
                        var tagName = element.TagName;

                        // This should throw if the element is stale.
                        if (element.Displayed)
                        {
                            var location = element.Location;

                            AppendIndent(builder, indentDepth);

                            builder.AppendLine($"[{idx}] - <{element.TagName} /> at X {location.X}, Y {location.Y}");
                        }
                        else
                        {
                            AppendIndent(builder, indentDepth);

                            builder.AppendLine($"[{idx}] - <{element.TagName} /> - not displayed");
                        }

                        foreach (var attr in DetailAttributeNames)
                        {
                            TryRenderElementAttribute(builder, element, attr, indentDepth + 1, MaxDetailNameLength);
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        AppendIndent(builder, indentDepth);
                        builder.AppendLine($"[{idx}] - element stale - no detail available");
                    }
                    catch (Exception ex)
                    {
                        AppendIndent(builder, indentDepth);
                        builder.AppendLine($"[{idx}] - error reading element info - " + ex.Message);
                    }
                }
            }
        }

        private static void TryRenderElementAttribute(StringBuilder builder, IWebElement element, string attrName, int indent, int namePadding)
        {
            var propValue = element.GetAttribute(attrName);

            if (!string.IsNullOrWhiteSpace(propValue))
            {
                AppendIndent(builder, indent);

                builder.Append(attrName + ": ");

                namePadding -= attrName.Length;

                if (namePadding > 0)
                {
                    // Remaining width is padding.
                    builder.Append(' ', namePadding);
                }

                builder.AppendLine(propValue);
            }
        }

        private static void WriteStep(StringBuilder strBuilder, StepContext stepContext)
        {
            strBuilder.Append(stepContext.Step.Type);
            strBuilder.Append(stepContext.Step.Text);
        }

        private static void WriteMethodCall(StringBuilder strBuilder, IMethodCallInfo methodInfo, IReadOnlyList<object?> boundArguments)
        {
            strBuilder.Append(methodInfo.MethodName);
            strBuilder.Append('(');

            for (var idx = 0; idx < boundArguments.Count; idx++)
            {
                if (idx > 0)
                {
                    strBuilder.Append(", ");
                }

                var arg = boundArguments[idx];

                if (arg is null)
                {
                    strBuilder.Append("null");
                }
                else if (arg is string argStr)
                {
                    strBuilder.Append('\'');
                    strBuilder.Append(argStr);
                    strBuilder.Append('\'');
                }
                else
                {
                    strBuilder.Append(arg.ToString());
                }
            }

            strBuilder.Append(')');
        }
    }
}
