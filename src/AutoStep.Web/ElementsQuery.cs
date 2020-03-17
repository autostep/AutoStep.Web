using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    using ElementValueTask = ValueTask<IEnumerable<IWebElement>>;

    public interface IElementQueryStage
    {
        ElementValueTask Invoke(IWebDriver driver, IEnumerable<IWebElement> input);
    }

    public class SelectQuery : IElementQueryStage
    {
        private readonly string selector;

        public SelectQuery(string selector)
        {
            this.selector = selector;
        }

        public ElementValueTask Invoke(IWebDriver driver, IEnumerable<IWebElement> input)
        {
            var elements = driver.FindElements(By.CssSelector(selector));

            return new ElementValueTask(elements);
        }
    }

    public class WithTextQuery : IElementQueryStage
    {
        private readonly string text;

        public WithTextQuery(string text)
        {
            this.text = text;
        }

        public ElementValueTask Invoke(IWebDriver driver, IEnumerable<IWebElement> input)
        {
            return new ElementValueTask(input.Where(x => x.Text == text));
        }
    }

    public class ElementsQuery : IEnumerable<IWebElement>
    {
        private Browser browser;
        private LinkedList<IElementQueryStage> queryParts = new LinkedList<IElementQueryStage>();

        private IEnumerable<IWebElement>? cachedResults;
        private LinkedListNode<IElementQueryStage>? lastExecutedStage;

        public ElementsQuery(Browser browser)
        {
            this.browser = browser;
        }

        public void AddSelect(string selector)
        {
            AddStage(new SelectQuery(selector));
        }

        public void AddWithText(string text)
        {
            AddStage(new WithTextQuery(text));
        }

        public void AddWithAttribute(string attributeName, string attributeValue)
        {
            AddStage(new WithAttributeQuery(attributeName, attributeValue));
        }

        public void AddVisible()
        {
            AddStage(new VisibleQuery());
        }

        private void AddStage(IElementQueryStage stage)
        {
            queryParts.AddLast(stage);
        }

        private async ElementValueTask Evaluate()
        {
            if (lastExecutedStage is object && lastExecutedStage.Next is null && cachedResults is object)
            {
                return cachedResults!;
            }

            var currentElements = Enumerable.Empty<IWebElement>();
            var currentStage = lastExecutedStage?.Next ?? queryParts.First;
            var executedStage = currentStage;
            
            while (currentStage is object)
            {
                currentElements = await currentStage.Value.Invoke(browser.Driver, currentElements);

                executedStage = currentStage;
                currentStage = currentStage.Next;
            }

            cachedResults = currentElements;
            lastExecutedStage = executedStage;

            return currentElements;
        }

        public IEnumerator<IWebElement> GetEnumerator()
        {
            return Evaluate().GetAwaiter().GetResult().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Evaluate().GetAwaiter().GetResult().GetEnumerator();
        }

    }
}
