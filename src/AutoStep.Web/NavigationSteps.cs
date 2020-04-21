using System;
using System.Collections.Generic;
using System.Text;

namespace AutoStep.Web
{
    [Steps]
    public class NavigationSteps
    {
        private readonly IBrowser browser;

        public NavigationSteps(IBrowser browser)
        {
            this.browser = browser;
        }

        [Given("I have navigated to {url}")]
        public void GivenIHaveNavigatedTo(string url)
        {
            browser.Driver.Navigate().GoToUrl(url);
        }
    }
}
