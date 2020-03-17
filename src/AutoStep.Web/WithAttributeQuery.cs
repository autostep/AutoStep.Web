using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    internal class WithAttributeQuery : IElementQueryStage
    {
        private readonly string attributeName;
        private readonly string attributeValue;

        public WithAttributeQuery(string attributeName, string attributeValue)
        {
            this.attributeName = attributeName;
            this.attributeValue = attributeValue;
        }

        public ValueTask<IEnumerable<IWebElement>> Invoke(IWebDriver driver, IEnumerable<IWebElement> input)
        {
            return new ValueTask<IEnumerable<IWebElement>>(input.Where(x => x.GetAttribute(attributeName) == attributeValue));
        }
    }
}
