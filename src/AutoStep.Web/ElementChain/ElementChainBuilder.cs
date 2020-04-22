using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Execution.Contexts;
using AutoStep.Web.ElementChain;
using OpenQA.Selenium;

namespace AutoStep.Web.Queryable
{
    internal class ElementChainBuilder : IElementChain
    {
        private readonly ElementChainContext context;

        public ElementChainBuilder(ElementChainNode? node, MethodContext methodContext, ElementChainContext queryableContext)
        {
            LastNode = node;
            ActiveMethodContext = methodContext;
            context = queryableContext;
        }

        public ElementChainNode? LastNode { get; }

        public MethodContext ActiveMethodContext { get; }

        public IWebDriver WebDriver => context.Browser.Driver;

        public bool AnyPreviousStages => LastNode != null;

        public IElementChain AddStage(string descriptor, Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback)
        {
            return new ElementChainBuilder(new ElementChainNode(LastNode, callback, descriptor, ActiveMethodContext, modifiesSet: true), ActiveMethodContext, context);
        }

        public IElementChain AddStage(string descriptor, Func<IReadOnlyList<IWebElement>, IEnumerable<IWebElement>> callback)
        {
            return AddStage(descriptor, (webElements, cancelToken) =>
            {
                // Need to evaluate at each stage.
                return new ValueTask<IReadOnlyList<IWebElement>>(callback(webElements).ToList());
            });
        }

        public IElementChain AddStage(string descriptor, Action<IReadOnlyList<IWebElement>> callback)
        {
            return new ElementChainBuilder(new ElementChainNode(LastNode, (webElements, cancelToken) => 
            { 
                callback(webElements);
                return new ValueTask<IReadOnlyList<IWebElement>>(webElements);
            }, descriptor, ActiveMethodContext, modifiesSet: false), ActiveMethodContext, context);
        }

        public async ValueTask<IReadOnlyList<IWebElement>> EvaluateAsync(CancellationToken cancelToken)
        {
            var evaluator = new ElementChainEvaluator(context);

            if (LastNode is object)
            {
                return await evaluator.Evaluate(LastNode, cancelToken);
            }

            return Array.Empty<IWebElement>();
        }

        public string Describe()
        {
            return context.Describer.Describe(LastNode);
        }
    }
}
