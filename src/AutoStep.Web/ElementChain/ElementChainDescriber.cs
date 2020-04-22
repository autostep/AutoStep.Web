using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AutoStep.Elements.Metadata;
using AutoStep.Execution.Contexts;
using OpenQA.Selenium;

namespace AutoStep.Web.ElementChain
{
    internal class ElementChainDescriber : IElementChainDescriber
    {
        private const int IndentSize = 4;
        private const int MaxElementDetails = 20;

        private static readonly string[] DetailAttributeNames =
        {
            "id",
            "class",
            "aria-label",
            "text"
        };

        private static readonly int MaxDetailNameLength = DetailAttributeNames.Max(s => s.Length);

        public string Describe(ElementChainNode? lastChainNode)
        {
            if (lastChainNode is null)
            {
                return "[empty]";
            }

            // To describe a chain, we build an execution set from this node, then walk back.            
            var executionSet = new LinkedList<ElementChainNode>();
            ElementChainNode? first = lastChainNode;

            do
            {
                var addedNode = executionSet.AddFirst(first);

                first = first!.ChildNode;

            } while (first is object);

            return Render(executionSet, (strBuilder, node) =>
            {
                strBuilder.Append("  ");
                strBuilder.AppendLine(node.Descriptor);
            });
        }

        public string DescribeExecution(LinkedList<ElementChainExecutionStage> executionChain, bool captureElementDetail)
        {
            bool encounteredError = false;

            return Render(executionChain, (strBuilder, node) =>
            {
                // Input Elements.
                AppendIndent(strBuilder, 1);
                strBuilder.AppendLine(node.Descriptor);

                if (node.WasExecuted)
                {
                    AppendIndent(strBuilder, 2);
                    strBuilder.AppendLine("Input:");

                    RenderElementCollection(strBuilder, node.InputElements ?? Array.Empty<IWebElement>(), captureElementDetail, 3);

                    if (node.Error is object)
                    {
                        // Exception occurred, include full details.
                        encounteredError = true;

                        // TODO: Richer information for AutoStep assertion exceptions.
                        AppendIndent(strBuilder, 2);
                        strBuilder.AppendLine("STAGE FAILED - EXCEPTION " + node.Error.GetType().Name);

                        AppendIndent(strBuilder, 3);
                        strBuilder.AppendLine(node.Error.Message);
                    }
                    else if (node.ModifiesSet)
                    {
                        AppendIndent(strBuilder, 2);
                        strBuilder.AppendLine("Stage Passed - Output:");
                        RenderElementCollection(strBuilder, node.OutputElements ?? Array.Empty<IWebElement>(), captureElementDetail, 3);
                    }
                    else
                    {
                        AppendIndent(strBuilder, 2);
                        strBuilder.AppendLine("Stage Passed");
                    }
                }
                else if(encounteredError)
                {
                    AppendIndent(strBuilder, 1);
                    strBuilder.AppendLine("not run - previous stage failed"); 
                }
                else
                {
                    AppendIndent(strBuilder, 1);
                    strBuilder.AppendLine("skipped - using cached results from previous evaluation");
                }
            });
        }

        private void AppendIndent(StringBuilder builder, int depth)
        {
            builder.Append(' ', IndentSize * depth);
        }

        private void RenderElementCollection(StringBuilder builder, IReadOnlyList<IWebElement> webElements, bool captureElementDetail, int indentDepth)
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

        private void TryRenderElementAttribute(StringBuilder builder, IWebElement element, string attrName, int indent, int namePadding)
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

        private string Render<TElement>(LinkedList<TElement> executionSet, Action<StringBuilder, TElement> elementCallback)
            where TElement : IDescribable
        {
            MethodContext? lastCallInfo = null;
            StringBuilder strBuilder = new StringBuilder();

            // Now walk the linked list.
            foreach (var node in executionSet)
            {
                if (lastCallInfo != node.MethodContext)
                {
                    if (lastCallInfo is object)
                    {
                        // Close the last descriptor, add call separator.
                        strBuilder.AppendLine("]");
                        strBuilder.Append(" -> ");
                    }

                    WriteMethodCall(strBuilder, node.MethodContext.MethodCall!, node.MethodContext.Arguments);

                    strBuilder.AppendLine(": [");

                    lastCallInfo = node.MethodContext;
                }

                // Write the node.
                elementCallback(strBuilder, node);
            }

            strBuilder.AppendLine("]");

            return strBuilder.ToString();
        }

        private void WriteMethodCall(StringBuilder strBuilder, IMethodCallInfo methodInfo, IReadOnlyList<object?> boundArguments)
        {
            strBuilder.Append(methodInfo.MethodName);
            strBuilder.Append('(');

            for (var idx = 0; idx < boundArguments.Count; idx++)
            {
                if(idx > 0)
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
