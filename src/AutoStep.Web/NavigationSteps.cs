using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AutoStep.Web
{
    [Steps]
    public class NavigationSteps
    {
        private readonly IBrowser browser;
        private readonly IConfiguration config;

        public NavigationSteps(IBrowser browser, IConfiguration config)
        {
            this.browser = browser;
            this.config = config;
        }

        [Given("I have navigated to {url}")]
        public void GivenIHaveNavigatedTo(string url)
        {
            browser.Driver.Navigate().GoToUrl(url);
        }

        [Given("I have navigated to the {appId} application")]
        public void GivenIHaveNavigatedToTheApplication(string appId)
        {
            browser.Driver.Navigate().GoToUrl(GetAppUri(appId));
        }

        [Given("I have navigated to {url} in the {appId} application")]
        public void GivenIHaveNavigatedToTheApplication(string relativeUrl, string appId)
        {
            var appUri = GetAppUri(appId);

            if (!Uri.TryCreate(appUri, relativeUrl, out var fullUri))
            {
                throw new ArgumentException("Provided url is not valid.");
            }

            browser.Driver.Navigate().GoToUrl(fullUri);
        }

        [Given("I have navigated back")]
        public void GivenIHaveNavigatedBack()
        {
            browser.Driver.Navigate().Back();
        }

        [Given("I have navigated forward")]
        public void GivenIHaveNavigatedForward()
        {
            browser.Driver.Navigate().Forward();
        }

        private Uri GetAppUri(string appId)
        {
            // Locate the application.
            var applicationConfig = config.GetSection("apps:" + appId);

            if (applicationConfig.Exists())
            {
                var appUrl = applicationConfig?.GetValue<string?>("url", null);

                if (string.IsNullOrWhiteSpace(appUrl))
                {
                    throw new ProjectConfigurationException($"url for the {appId} application cannot be blank.");
                }

                if (Uri.TryCreate(appUrl, UriKind.Absolute, out var uri))
                {
                   return new Uri(appUrl);
                }
                else
                {
                    throw new ProjectConfigurationException($"url for the {appId} application is not a valid absolute URL.");
                }
            }
            else
            {
                throw new ProjectConfigurationException($"Configuration for the {appId} application does not exist.");
            }
        }
    }
}
