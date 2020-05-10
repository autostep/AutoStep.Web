using System;
using AutoStep.Configuration;
using Microsoft.Extensions.Configuration;

namespace AutoStep.Web
{
    /// <summary>
    /// Provides browser navigation steps.
    /// </summary>
    [Steps]
    public class NavigationSteps
    {
        private readonly IBrowser browser;
        private readonly IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationSteps"/> class.
        /// </summary>
        /// <param name="browser">The browser instance.</param>
        /// <param name="config">The current configuration.</param>
        public NavigationSteps(IBrowser browser, IConfiguration config)
        {
            this.browser = browser;
            this.config = config;
        }

        #pragma warning disable SA1600 // Elements documentation.
        #pragma warning disable CS1591 // Step docs comes from an attribute; don't want to duplicate info.

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
                throw new ArgumentException(Messages.ProvidedUrlNotValid.FormatWith(relativeUrl));
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
                throw new ArgumentException(Messages.ParameterCannotBeEmpty, nameof(appId));
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
