using System;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public interface IBrowser : IDisposable
    {
        public IWebDriver Driver { get; }

        void Initialise();

        ValueTask<bool> WaitForPageReady(CancellationToken cancellationToken);
    }
}
