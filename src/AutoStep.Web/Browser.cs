using System;
using System.IO;
using AutoStep.Extensions.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoStep.Web
{
    public class Browser : IDisposable
    {        
        private readonly ILoadedExtensions extensionInfo; 

        private IWebDriver? driver;
        private ChromeDriverService chromeDriverService;

        public Browser(ILoadedExtensions extensionInfo)
        {
            this.extensionInfo = extensionInfo;
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

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        public IWebDriver Driver => driver!;

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
