using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoStep.Web
{
    public class Browser : IDisposable
    {
        IWebDriver? driver;

        public Browser()
        {
        }

        public void Initialise()
        {
            driver = new ChromeDriver();
        }

        public IWebDriver Driver => driver!;

        public void Dispose()
        {
            driver!.Quit();
            driver = null;
        }
    }
}
