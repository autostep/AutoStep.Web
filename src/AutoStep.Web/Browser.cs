﻿using System;
using System.IO;
using AutoStep.Extensions.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoStep.Web
{
    public class Browser : IDisposable
    {        
        private readonly ILoadedExtensions extensionInfo; 

        IWebDriver? driver;

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

            var chromeDriverService = ChromeDriverService.CreateDefaultService(driverDir);
            chromeDriverService.SuppressInitialDiagnosticInformation = true;

            var chromeOptions = new ChromeOptions();

            driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        public IWebDriver Driver => driver!;

        public void Dispose()
        {
            if (driver is object)
            {
                driver!.Quit();
                driver = null;
            }
        }
    }
}
