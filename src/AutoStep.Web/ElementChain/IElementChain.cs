﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web.ElementChain
{
    public interface IElementChain
    {
        public IWebDriver WebDriver { get; }

        public bool AnyPreviousStages { get; }

        public ElementChainNode? LastNode { get; }

        IElementChain AddStage(
            Func<IReadOnlyList<IWebElement>, CancellationToken, ValueTask<IReadOnlyList<IWebElement>>> callback);

        IElementChain AddStage(
            Action<IReadOnlyList<IWebElement>> callback);

        IElementChain AddStage(
            Func<IReadOnlyList<IWebElement>, IEnumerable<IWebElement>> callback);

        ValueTask<IReadOnlyList<IWebElement>> EvaluateAsync(CancellationToken cancelToken);
    }
}