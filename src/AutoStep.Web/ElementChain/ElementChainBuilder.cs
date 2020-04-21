using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Elements.Metadata;
using AutoStep.Web.ElementChain;
using OpenQA.Selenium;

namespace AutoStep.Web.Queryable
{
    internal class ElementChainBuilder : IElementChain
    {
        private readonly ElementChainContext context;

        public ElementChainBuilder(ElementChainNode? node, IMethodCallInfo methodInfo, ElementChainContext queryableContext)
        {
            LastNode = node;
            ActiveMethodCall = methodInfo;
            context = queryableContext;
        }

        public ElementChainNode? LastNode { get; }

        public IMethodCallInfo ActiveMethodCall { get; }

        public IWebDriver WebDriver => context.Browser.Driver;

        public bool AnyPreviousStages => LastNode != null;

        public IElementChain AddStage(
            Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback)
        {
            return new ElementChainBuilder(new ElementChainNode(LastNode, callback, ActiveMethodCall), ActiveMethodCall, context);
        }

        public IElementChain AddStage(Func<IReadOnlyList<IWebElement>, IEnumerable<IWebElement>> callback)
        {
            return AddStage((webElements, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements).ToList());
            });
        }

        public IElementChain AddStage(Action<IReadOnlyList<IWebElement>> callback)
        {
            return AddStage(el =>
            {
                callback(el);
                return el;
            });
        }

        public async ValueTask<IReadOnlyList<IWebElement>> EvaluateAsync(CancellationToken cancelToken)
        {
            var evaluator = new ElementChainEvaluator(context);

            if (LastNode is object)
            {
                await evaluator.Evaluate(LastNode, cancelToken);
            }

            return new Collection<IWebElement>();
        }
    }
}
