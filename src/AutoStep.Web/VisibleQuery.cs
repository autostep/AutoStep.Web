using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public class VisibleQuery : IElementQueryStage
    {
        public ValueTask<IEnumerable<IWebElement>> Invoke(IWebDriver driver, IEnumerable<IWebElement> input)
        {
            return new ValueTask<IEnumerable<IWebElement>>(input.Where(x => x.Displayed));
        }
    }
}
