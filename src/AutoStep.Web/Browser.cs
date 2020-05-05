using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Extensions.Abstractions;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoStep.Web
{
    public interface IBrowser : IDisposable
    {
        public IWebDriver Driver { get; }

        void Initialise();

        ValueTask<bool> WaitForPageReady(CancellationToken cancellationToken);
    }

    public class Browser : IBrowser
    {
        private readonly ILoadedExtensions extensionInfo;
        private readonly IConfigurationRoot config;
        private IWebDriver? driver;
        private ChromeDriverService chromeDriverService;

        public Browser(ILoadedExtensions extensionInfo, IConfigurationRoot config)
        {
            this.extensionInfo = extensionInfo;
            this.config = config;
        }

        public void Initialise()
        {
            // Driver directory should be the same as this package.
            // Try and get from loaded extensions.

            string driverDir;

            if (extensionInfo.IsPackageLoaded("Selenium.Chrome.WebDriver"))
            {
                driverDir = extensionInfo.GetPackagePath("Selenium.Chrome.WebDriver", "driver");
            }
            else
            {
                // Not loaded as an extension, just use the assembly folder.
                driverDir = Path.GetDirectoryName(typeof(Browser).Assembly.Location);
            }

            chromeDriverService = ChromeDriverService.CreateDefaultService(driverDir);
            chromeDriverService.HideCommandPromptWindow = true;

            var chromeOptions = new ChromeOptions();

            if (config.GetValue("headless", false))
            {
                chromeOptions.AddArgument("--headless");
            }

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        public IWebDriver Driver => driver!;

        public ValueTask<bool> WaitForPageReady(CancellationToken cancellationToken)
        {
            // Page is now ready.
            return new ValueTask<bool>(true);
        }

        public void Dispose()
        {
            if (driver is object)
            {
                driver!.Quit();
                driver.Dispose();
                driver = null;
            }

            // Dispose of the chrome driver as well.
            if(chromeDriverService is object)
            {
                chromeDriverService.Dispose();
                chromeDriverService = null;
            }
        }
    }
}
