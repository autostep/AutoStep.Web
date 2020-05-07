using System;
using System.Collections.Generic;
using System.Text;
using AutoStep.Configuration;
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

        [Given("I have navigated to {url}", Documentation = @"
            
            Navigates to the specified absolute URL in the browser.

            ```
            Given I have navigated to 'http://myapp.com'
            Given I have navigated to 'http://localhost:5000'
            ```
        ")]
        public void GivenIHaveNavigatedTo(string url)
        {
            browser.Driver.Navigate().GoToUrl(new Uri(url, UriKind.Absolute));
        }

        [Given("I have navigated to the {appId} application", Documentation = @"

            Navigates to the configured URL for the specified application.            

            The application URL can be set in your ``autostep.config.json`` file, under the ``apps`` item:

            ```json
            ""apps"": {
              ""appId"": {
                ""url"": ""http://localhost:5000""
              }
            }
            ```
        ")]
        public void GivenIHaveNavigatedToTheApplication(string appId)
        {
            browser.Driver.Navigate().GoToUrl(GetAppUri(appId));
        }

        [Given("I have navigated to {url} in the {appId} application", Documentation = @"

            Navigates to a relative URL within the specified application.            

            The application root URL can be set in your ``autostep.config.json`` file, under the ``apps`` item:

            ```json
            ""apps"": {
              ""appId"": {
                ""url"": ""http://localhost:5000""
              }
            }
            ```
        ")]
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
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException("appId cannot be empty.", nameof(appId));
            }

            // Locate the application.
            var appUrl = config.GetRunValue<string?>("apps:" + appId + ":url", null);

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
    }
}
