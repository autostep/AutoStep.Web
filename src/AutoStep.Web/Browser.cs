using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Configuration;
using AutoStep.Extensions.Abstractions;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoStep.Web
{
    /// <summary>
    /// Implements a browser. Temporary until multi-browser support comes along.
    /// </summary>
    public class Browser : IBrowser
    {
        private readonly ILoadedExtensions extensionInfo;
        private readonly IConfiguration config;
        private IWebDriver? driver;
        private ChromeDriverService chromeDriverService;

        public Browser(ILoadedExtensions extensionInfo, IConfiguration config)
        {
            this.extensionInfo = extensionInfo;
            this.config = config;
        }

        public IWebDriver Driver => driver!;

        public void Initialise()
        {
            // Try and get from loaded extensions.
            string driverDir;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                driverDir = extensionInfo.GetPackagePath("Selenium.WebDriver.ChromeDriver", "driver", "win32");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                driverDir = extensionInfo.GetPackagePath("Selenium.WebDriver.ChromeDriver", "driver", "mac64");
            }
            else
            {
                driverDir = extensionInfo.GetPackagePath("Selenium.WebDriver.ChromeDriver", "driver", "linux64");
            }

            chromeDriverService = ChromeDriverService.CreateDefaultService(driverDir);
            chromeDriverService.HideCommandPromptWindow = true;

            var chromeOptions = new ChromeOptions();

            if (config.GetRunValue("headless", false))
            {
                chromeOptions.AddArgument("--headless");
            }

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

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
            if (chromeDriverService is object)
            {
                chromeDriverService.Dispose();
                chromeDriverService = null!;
            }
        }
    }
}
